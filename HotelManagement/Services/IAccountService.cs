using HotelManagement.Model;

namespace HotelManagement.Services;

public interface IAccountService : IDataService<Account>
{
    Task<Account?> GetByUsername(string username);
}