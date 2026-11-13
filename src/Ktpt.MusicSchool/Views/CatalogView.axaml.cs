using Avalonia.Controls;
using Avalonia.Interactivity;
using Ktpt.MusicSchool.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Views;

public partial class CatalogView : UserControl
{
    private List<CourseCard> allCards = new();

    public CatalogView()
    {
        InitializeComponent();
        SortBox.SelectedIndex = 0;
        Loaded += async (s, e) => await LoadCardsAsync();
    }

    private async Task LoadCardsAsync()
    {
        try
        {
            allCards = await CatalogService.LoadCardsAsync();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }

    private void ApplyFilters()
    {
        string text = (SearchBox.Text ?? "").Trim();

        var rows = allCards.Where(c =>
            c.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
            c.Code.Contains(text, StringComparison.OrdinalIgnoreCase));

        if (SortBox.SelectedIndex == 1)
            rows = rows.OrderByDescending(c => c.Price);
        else
            rows = rows.OrderBy(c => c.Price);

        var result = rows.ToList();
        CardsList.ItemsSource = result;
        CountText.Text = $"Найдено: {result.Count} из {allCards.Count}";
    }

    private void SearchChanged(object? sender, TextChangedEventArgs e) => ApplyFilters();
    private void FilterChanged(object? sender, SelectionChangedEventArgs e) => ApplyFilters();
    private async void ReloadClick(object? sender, RoutedEventArgs e) => await LoadCardsAsync();

    private void CardSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (CardsList.SelectedItem is not CourseCard card) return;

        if (Session.IsGuest)
        {
            CardsList.SelectedItem = null;
            MainWindow.Instance.ShowMessage("Чтобы оформить заказ, войдите в систему.");
            return;
        }

        MainWindow.Instance.OpenPage(new CourseView { CourseId = card.Id });
    }
}