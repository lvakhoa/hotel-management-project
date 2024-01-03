using HotelManagement.Model;

namespace HotelManagement.Services.AuthenticationService;

public interface IAuthenticationService
{
    Task<Account> Login(string username, string password);
}