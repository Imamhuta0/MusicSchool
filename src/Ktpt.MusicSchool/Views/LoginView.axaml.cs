using Avalonia.Controls;
using Avalonia.Interactivity;
using Ktpt.MusicSchool.Services;
using System;

namespace Ktpt.MusicSchool.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private async void LoginClick(object? sender, RoutedEventArgs e)
    {
        string login = (LoginBox.Text ?? "").Trim();
        string password = (PasswordBox.Text ?? "").Trim();

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            MainWindow.Instance.ShowMessage("Введите логин и пароль.");
            return;
        }

        try
        {
            bool success = await Session.LoginAsync(login, password);
            if (success)
                MainWindow.Instance.OpenPage(new CatalogView(), clearHistory: true);
            else
                MainWindow.Instance.ShowMessage("Неверный логин или пароль.");
        }
        catch (Exception ex)
        {
            MainWindow.Instance.ShowError(ex);
        }
    }

    private void GuestClick(object? sender, RoutedEventArgs e)
    {
        Session.Logout();
        MainWindow.Instance.OpenPage(new CatalogView(), clearHistory: true);
    }
}