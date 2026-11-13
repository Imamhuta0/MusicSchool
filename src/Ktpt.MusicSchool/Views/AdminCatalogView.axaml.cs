using Avalonia.Controls;
using Avalonia.Interactivity;
using Ktpt.MusicSchool.Data;
using Ktpt.MusicSchool.Models;
using Ktpt.MusicSchool.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Views;

public partial class AdminCatalogView : UserControl
{
    public AdminCatalogView()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadLessonsAsync();
    }

    private async Task LoadLessonsAsync()
    {
        try
        {
            using var db = Db.Create();
            LessonsList.ItemsSource = await db.Lessons.Include(l => l.Category).AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }

    private async void ReloadClick(object? sender, RoutedEventArgs e) => await LoadLessonsAsync();

    private async void SavePriceClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not Button btn || btn.Tag is not Lesson lesson) return;

            if (btn.Parent is not Grid grid) return;
            var priceBox = grid.Children.OfType<TextBox>().FirstOrDefault();
            if (priceBox == null) return;

            if (!decimal.TryParse(priceBox.Text, out decimal newPrice) || newPrice <= 0)
                throw new AppException("Введите корректную цену больше нуля.");

            using var db = Db.Create();
            var dbLesson = await db.Lessons.FindAsync(lesson.LessonId);
            if (dbLesson == null) throw new AppException("Занятие не найдено.");

            dbLesson.BasePrice = newPrice;
            await db.SaveChangesAsync();
            MainWindow.Instance.ShowMessage("Цена успешно обновлена!");
            await LoadLessonsAsync();
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }
}