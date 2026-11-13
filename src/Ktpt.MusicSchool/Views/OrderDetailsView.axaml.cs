using Avalonia.Controls;
using Avalonia.Interactivity;
using Ktpt.MusicSchool.Models;
using Ktpt.MusicSchool.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Views;

public partial class OrderDetailsView : UserControl
{
    public int OrderId { get; set; }

    public OrderDetailsView()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadOrderAsync();
    }

    private async Task LoadOrderAsync()
    {
        try
        {
            ConfirmPanel.IsVisible = false;
            Order? order = await OrderService.GetOrderAsync(OrderId);

            if (order == null)
            {
                MainWindow.Instance.OpenPage(new OrdersView(), clearHistory: true);
                MainWindow.Instance.ShowMessage("Заказ удалён.");
                return;
            }

            HeaderText.Text = $"Заказ № {order.OrderId}";
            ClientText.Text = "Клиент: " + order.Client?.FullName;
            DateText.Text = "Дата: " + order.OrderDate.ToString("dd.MM.yyyy");
            TotalText.Text = $"Итого: {order.OrderItems.Sum(i => i.Total):N2} руб.";

            ItemsList.ItemsSource = order.OrderItems.ToList();
            DeleteOrderBtn.IsVisible = Session.IsAdmin;
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }

    private async void ReloadClick(object? sender, RoutedEventArgs e) => await LoadOrderAsync();

    private void DeleteOrderClick(object? sender, RoutedEventArgs e)
    {
        ConfirmText.Text = "Удалить весь заказ? Все места вернутся в потоки.";
        ConfirmPanel.IsVisible = true;
    }

    private void CancelClick(object? sender, RoutedEventArgs e) => ConfirmPanel.IsVisible = false;

    private async void ConfirmClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            await OrderService.DeleteAsync(OrderId, null);
            MainWindow.Instance.ShowMessage("Заказ удалён, места возвращены.");
            await LoadOrderAsync();
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }
}