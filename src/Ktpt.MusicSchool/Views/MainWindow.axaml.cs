using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Ktpt.MusicSchool.Services;
using System;
using System.Collections.Generic;

namespace Ktpt.MusicSchool.Views;

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; } = null!;
    private readonly Stack<UserControl> history = new();

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        OpenPage(new LoginView(), clearHistory: true);
    }

    public void OpenPage(UserControl page, bool clearHistory = false)
    {
        if (clearHistory) history.Clear();
        else if (PageHost.Content is UserControl current) history.Push(current);

        PageHost.Content = page;
        MessageBorder.IsVisible = false;

        UserText.Text = Session.Caption;
        OrdersButton.IsVisible = !Session.IsGuest;
        AdminButton.IsVisible = Session.IsAdmin;
        CatalogButton.IsVisible = !Session.IsGuest;
    }

    public void ShowMessage(string text)
    {
        MessageText.Foreground = Brushes.Black;
        MessageText.Text = text;
        MessageBorder.Background = new SolidColorBrush(Color.Parse("#E8F5E9"));
        MessageBorder.IsVisible = true;
    }

    public void ShowError(Exception error)
    {
        System.Diagnostics.Trace.WriteLine(error);
        MessageText.Foreground = Brushes.DarkRed;
        MessageText.Text = UiSupport.ErrorText(error);
        MessageBorder.Background = new SolidColorBrush(Color.Parse("#FFEBEE"));
        MessageBorder.IsVisible = true;
    }

    private void CatalogClick(object? sender, RoutedEventArgs e) => OpenPage(new CatalogView());
    private void OrdersClick(object? sender, RoutedEventArgs e) => OpenPage(new OrdersView());
    private void AdminClick(object? sender, RoutedEventArgs e) => OpenPage(new AdminCatalogView());

    private void LogoutClick(object? sender, RoutedEventArgs e)
    {
        Session.Logout();
        OpenPage(new LoginView(), clearHistory: true);
    }
}