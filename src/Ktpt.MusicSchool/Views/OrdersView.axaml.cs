using Avalonia.Controls;
using Avalonia.Interactivity;
using Ktpt.MusicSchool.Models;
using Ktpt.MusicSchool.Services;
using System;
using System.Threading.Tasks;

namespace Ktpt.MusicSchool.Views;

public partial class OrdersView : UserControl
{
    public OrdersView()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            OrdersList.ItemsSource = await OrderService.GetOrdersAsync();
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }

    private async void ReloadClick(object? sender, RoutedEventArgs e) => await LoadOrdersAsync();

    private void OpenClick(object? sender, RoutedEventArgs e)
    {
        if (OrdersList.SelectedItem is Order order)
            MainWindow.Instance.OpenPage(new OrderDetailsView { OrderId = order.OrderId });
        else
            MainWindow.Instance.ShowMessage("Выберите заказ в списке.");
    }
}