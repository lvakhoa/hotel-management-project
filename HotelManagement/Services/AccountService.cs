using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services;

public class AccountService : IAccountService
{
    public Task<IEnumerable<Account>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Account> Get(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Account> Create(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<Account> Update(int id, Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> GetByUsername(string username)
    {
        await using HotelManagementContext context = new HotelManagementContext();
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.Username == username);
    }
}