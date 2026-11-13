using Ktpt.MusicSchool.Data;
using Ktpt.MusicSchool.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Services;

public static class OrderService
{
    public static async Task<int> CreateOrderAsync(int variantId, int quantity, int clientId, decimal shownPrice, DateOnly lessonDate)
    {
        if (Session.CurrentUser == null)
            throw new AppException("Гость не может оформить заказ. Войдите в систему.");

        if (!Session.IsStaff && clientId != Session.CurrentUser.UserId)
            throw new AppException("Можно оформить заказ только на себя.");

        if (quantity <= 0)
            throw new AppException("Количество мест должно быть больше нуля.");

        using var db = Db.Create();
        using var transaction = await db.Database.BeginTransactionAsync();

        LessonVariant? variant = await db.LessonVariants
            .Include(v => v.Lesson)
            .AsNoTracking()
            .SingleOrDefaultAsync(v => v.VariantId == variantId);

        if (variant == null)
            throw new AppException("Поток не найден. Обновите карточку.");

        if (Session.RoleName == "Преподаватель" && variant.TeacherId != Session.CurrentUser.UserId)
            throw new AppException("Вы можете записывать учеников только на свои занятия!");

        decimal price = await CatalogService.GetPriceAsync(db, variant.Lesson);
        if (price != shownPrice)
            throw new AppException("Цена изменилась. Нажмите «Обновить карточку».");

        int changed = await db.LessonVariants
            .Where(v => v.VariantId == variantId && v.AvailableSeats >= quantity)
            .ExecuteUpdateAsync(s => s.SetProperty(v => v.AvailableSeats, v => v.AvailableSeats - quantity));

        if (changed == 0)
            throw new AppException("Недостаточно свободных мест. Обновите карточку.");

        var order = new Order
        {
            OrderDate = DateOnly.FromDateTime(DateTime.Today),
            ClientId = clientId
        };
        order.OrderItems.Add(new OrderItem
        {
            VariantId = variantId,
            Quantity = quantity,
            UnitPrice = price,
            LessonDate = lessonDate
        });

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return order.OrderId;
    }

    public static async Task<List<Order>> GetOrdersAsync()
    {
        if (Session.IsGuest)
            throw new AppException("Гостю заказы недоступны.");

        using var db = Db.Create();
        var query = db.Orders.Include(o => o.Client).AsQueryable();

        if (Session.IsAdmin) { }
        else if (Session.RoleName == "Преподаватель")
            query = query.Where(o => o.OrderItems.Any(i => i.Variant.TeacherId == Session.CurrentUser!.UserId));
        else
            query = query.Where(o => o.ClientId == Session.CurrentUser!.UserId);

        return await query
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.OrderId)
            .AsNoTracking()
            .ToListAsync();
    }

    public static async Task<Order?> GetOrderAsync(int orderId)
    {
        if (Session.IsGuest)
            throw new AppException("Гостю заказы недоступны.");

        using var db = Db.Create();
        var query = db.Orders
            .Include(o => o.Client)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Variant)
                    .ThenInclude(v => v.Lesson)
            .AsQueryable();

        if (Session.IsAdmin) { }
        else if (Session.RoleName == "Преподаватель")
            query = query.Where(o => o.OrderItems.Any(i => i.Variant.TeacherId == Session.CurrentUser!.UserId));
        else
            query = query.Where(o => o.ClientId == Session.CurrentUser!.UserId);

        return await query.AsNoTracking().SingleOrDefaultAsync(o => o.OrderId == orderId);
    }

    public static async Task DeleteAsync(int orderId, int? itemId)
    {
        if (itemId == null) Session.RequireStaff();
        else Session.RequireAdmin();

        using var db = Db.Create();
        using var transaction = await db.Database.BeginTransactionAsync();

        List<OrderItem> items = await db.OrderItems
            .Where(i => i.OrderId == orderId && (itemId == null || i.OrderItemId == itemId))
            .ToListAsync();

        if (items.Count == 0) throw new AppException("Данные уже удалены.");

        foreach (OrderItem item in items)
        {
            await db.LessonVariants
                .Where(v => v.VariantId == item.VariantId)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.AvailableSeats, v => v.AvailableSeats + item.Quantity));
        }

        db.OrderItems.RemoveRange(items);
        await db.SaveChangesAsync();

        bool hasItems = await db.OrderItems.AnyAsync(i => i.OrderId == orderId);
        if (!hasItems) await db.Orders.Where(o => o.OrderId == orderId).ExecuteDeleteAsync();

        await transaction.CommitAsync();
    }
}