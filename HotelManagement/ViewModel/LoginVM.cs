using System.Security.Principal;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.Model;
using HotelManagement.Services.AuthenticationService;

namespace HotelManagement.ViewModel;

public partial class LoginVM : ObservableObject
{
    private readonly IAuthenticationService _authenticationService = new AuthenticationService();
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _username;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _password;
    
    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] private bool _isViewVisible = true;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        try
        {
            Account currentAccount = await _authenticationService.Login(Username!, Password!);
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(currentAccount.StaffId), null);
            ErrorMessage = "";
            IsViewVisible = false;
        }
        catch (UserNotFoundException)
        {
            ErrorMessage = "Username does not exist";
        }
        catch(InvalidPasswordException)
        {
            ErrorMessage = "Incorrect password";
        }
        catch(Exception)
        {
            ErrorMessage = "Login failed";
        }
    }

    private bool CanLogin() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
}