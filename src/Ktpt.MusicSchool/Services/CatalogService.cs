using Ktpt.MusicSchool.Data;
using Ktpt.MusicSchool.Models;
using Ktpt.MusicSchool.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Services;

public static class CatalogService
{
    public static async Task<decimal> GetPriceAsync(MusicSchoolContext db, Lesson lesson)
    {
        return PriceRules.Price(lesson.BasePrice);
    }

    public static async Task<List<CourseCard>> LoadCardsAsync()
    {
        using var db = Db.Create();
        List<Lesson> lessons = await db.Lessons
            .Include(l => l.Category)
            .Include(l => l.LessonVariants)
            .AsNoTracking()
            .ToListAsync();

        var cards = new List<CourseCard>();
        foreach (Lesson l in lessons)
        {
            int seats = l.LessonVariants.Sum(v => v.AvailableSeats);
            decimal price = await GetPriceAsync(db, l);

            cards.Add(new CourseCard
            {
                Id = l.LessonId,
                Name = l.Title,
                Code = l.Category.CategoryName,
                Category = l.Category.CategoryName,
                Content = l.Description ?? "Описание не указано",
                Seats = seats,
                SeatsText = "Мест: " + PriceRules.Availability(seats),
                Price = price,
                PriceText = $"{price:N2} руб.",
                Background = UiSupport.Brush(PriceRules.LowStock(seats) ? "LowStockBrush" : "NormalBrush"),
                Image = UiSupport.Picture(l.ImagePath)
            });
        }
        return cards;
    }
}