using Microsoft.AspNetCore.Identity;
using SalaryCalculator.Model;
using SalaryCalculatorReact.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.Auth.Abstract
{
    public interface IAuthService
    {
        Task<AuthData> GetAuthDataAsync(string username);
        IEnumerable<Field> GetForm();
        Task<SignInResult> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<List<Field>> GetFormAsync(int id);
        Task<IdentityResult> EditAccount(int id, string username, string password);
        Task<IdentityResult> DeleteAccount(int id);
        Task<IdentityResult> AddRole(string rolename);
        Task<IdentityResult> DeleteRole(string rolename);
        void GetNameAndRole(string username, out string name, out string role);
    }
}
