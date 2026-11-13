using Avalonia.Controls;
using Avalonia.Interactivity;
using Ktpt.MusicSchool.Data;
using Ktpt.MusicSchool.Models;
using Ktpt.MusicSchool.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Views;

public partial class CourseView : UserControl
{
    public int CourseId { get; set; }
    private decimal shownPrice;

    public CourseView()
    {
        InitializeComponent();
        DateBox.SelectedDate = DateTime.Today;
        Loaded += async (s, e) => await LoadCourseAsync();
    }

    private async Task LoadCourseAsync()
    {
        try
        {
            SaveButton.IsEnabled = false;
            using var db = Db.Create();

            Lesson? lesson = await db.Lessons
                .Include(l => l.LessonVariants)
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.LessonId == CourseId);

            if (lesson == null) throw new AppException("Занятие не найдено.");

            NameText.Text = lesson.Title;
            DescriptionText.Text = lesson.Description;
            CourseImage.Source = UiSupport.Picture(lesson.ImagePath);

            shownPrice = await CatalogService.GetPriceAsync(db, lesson);
            PriceText.Text = $"Цена одного занятия: {shownPrice:N2} руб.";

            var variants = lesson.LessonVariants
                .Where(v => v.AvailableSeats > 0)
                .OrderBy(v => v.VariantName)
                .ToList();

            VariantBox.ItemsSource = variants;
            VariantBox.SelectedIndex = variants.Count > 0 ? 0 : -1;

            ClientPanel.IsVisible = Session.IsStaff;
            if (Session.IsStaff)
            {
                var users = await Session.GetUsersAsync();
                ClientBox.ItemsSource = users.Where(u => u.RoleId == 3).ToList();
                if (ClientBox.ItemCount > 0) ClientBox.SelectedIndex = 0;
            }

            SaveButton.IsEnabled = variants.Count > 0;
            if (variants.Count == 0) MainWindow.Instance.ShowMessage("Свободных мест нет.");
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }

    private async void SaveClick(object? sender, RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
        try
        {
            if (VariantBox.SelectedItem is not LessonVariant variant)
                throw new AppException("Выберите поток.");

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
                throw new AppException("Количество должно быть числом больше нуля.");

            if (DateBox.SelectedDate == null)
                throw new AppException("Выберите дату занятия.");

            DateOnly lessonDate = DateOnly.FromDateTime(DateBox.SelectedDate.Value);

            int clientId = Session.CurrentUser!.UserId;
            if (Session.IsStaff)
            {
                if (ClientBox.SelectedItem is not User client)
                    throw new AppException("Выберите клиента.");
                clientId = client.UserId;
            }

            int orderId = await OrderService.CreateOrderAsync(variant.VariantId, quantity, clientId, shownPrice, lessonDate);

            MainWindow.Instance.OpenPage(new CatalogView(), clearHistory: true);
            MainWindow.Instance.ShowMessage($"Заказ № {orderId} успешно оформлен на {lessonDate:dd.MM.yyyy}!");
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
            SaveButton.IsEnabled = true;
        }
    }

    private async void ReloadClick(object? sender, RoutedEventArgs e) => await LoadCourseAsync();
}