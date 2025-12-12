using System.Windows;
using System.Windows.Controls;
using UniversityManagementSystem.ViewModels;

namespace UniversityManagementSystem.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.LoginSuccessful += OnLoginSuccessful;
        }
    }
    
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.Password = passwordBox.Password;
        }
    }
    
    private void OnLoginSuccessful(object? sender, EventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}

