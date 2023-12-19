using HotelManagement.Model;

namespace HotelManagement.Services.AuthenticationService;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAccountService _accountService = new AccountService();

    public async Task<Account> Login(string username, string password)
    {
        Account? currentAccount = await _accountService.GetByUsername(username);

        if (currentAccount == null)
            throw new UserNotFoundException(username);

        if (currentAccount.Password != password)
            throw new InvalidPasswordException(username, password);

        return currentAccount;
    }
}

public class UserNotFoundException : Exception
{
    public string Username { get; set; }

    public UserNotFoundException(string username)
    {
        Username = username;
    }
}

public class InvalidPasswordException : Exception
{
    public string Username { get; set; }
    public string Password { get; set; }

    public InvalidPasswordException(string username, string password)
    {
        Username = username;
        Password = password;
    }
}