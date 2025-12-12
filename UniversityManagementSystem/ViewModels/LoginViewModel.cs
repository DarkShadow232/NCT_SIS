using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _rememberMe;
    
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }
    
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }
    
    public ICommand LoginCommand { get; }
    public ICommand ExitCommand { get; }
    
    public event EventHandler? LoginSuccessful;
    
    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsLoading);
        ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
    }
    
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password";
            return;
        }
        
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var (success, message) = await AuthenticationService.Instance.LoginAsync(Username, Password);
            
            if (success)
            {
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

