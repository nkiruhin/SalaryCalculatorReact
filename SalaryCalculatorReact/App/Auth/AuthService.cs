using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SalaryCalculator.Model;
using SalaryCalculatorReact.App.Auth.Abstract;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.Auth
{
    public class AuthService: IAuthService
    {
        private const string _jwtSecret= "U2FsYXJ5Q2FsY3VsYXRvclNlY3JldEtleQ==";
        private const int _jwtLifespan= 2592000;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmployeeRepository _employee;
        public AuthService( SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IEmployeeRepository employee)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _employee = employee;            
        }

        /// <summary>
        /// Check present user account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="AccountId">return AccountId if present</param>
        /// <returns></returns>
        private bool HasAccount(int id, out string AccountId)
        {
            AccountId = _employee.GetSingle(id)?.AccountId;
            if (AccountId != null) { return true; }
            else { return false; }
        }
        public async Task<AuthData> GetAuthDataAsync(string username)
        {
            User user = await _userManager.FindByNameAsync(username);
            string name;
            var roles = await _userManager.GetRolesAsync(user);

            var role = roles.FirstOrDefault();
            if (role == "Administrator")
            {
                name = "Администратор";
            }
            else
            {
                name = _employee.FindBy(n => n.AccountId == user.Id)?.First().Surname;
            }
            var expirationTime = DateTime.UtcNow.AddSeconds(_jwtLifespan);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, role)
            }),

                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return new AuthData
            {
                Token = token,
                TokenExpirationTime = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds(),
                Id = user.Id.ToString(),
                Role = role,
                name = name
            };
        }
        public async Task<List<Field>> GetFormAsync(int id)
        {
            User user = null;
            if (HasAccount(id, out string AccountId))
            {
                user = await _userManager.FindByIdAsync(AccountId);
            }
            var form = new List<Field>
            {
                new Field
                {
                    Name="username",
                    Type="String",
                    IsRequired=true,
                    DisplayName="Имя пользователя",
                    Value = user?.UserName
                },
                new Field
                {
                    Name="password",
                    Type="Password",
                    IsRequired=true,
                    DisplayName="Пароль"
                }
            };
            return form;
        }
        public async Task<IdentityResult> EditAccount(int id, string username, string password)
        {
            User user = null;
            if (HasAccount(id, out string AccountId))
            {
                user = await _userManager.FindByIdAsync(AccountId);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, password);
                return result;
            }
            else
            {
                user = new User { UserName = username };
                var employee = _employee.GetSingle(n => n.Id == id, p => p.Position);
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    return result;
                }

                await _userManager.AddToRoleAsync(user, employee.Position.Name);
                employee.AccountId = await _userManager.GetUserIdAsync(user);
                _employee.Update(employee);
                await _employee.CommitAsync();
                return result;
            }
        }
        public async Task<IdentityResult> DeleteAccount(int id)
        {
            var employee = await _employee.GetSingleAsync(id);
            var AccountId = employee?.AccountId;
            User user = await _userManager.FindByIdAsync(AccountId);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            employee.AccountId = null;
            await _employee.CommitAsync();
            return result;
        }
        public async Task<IdentityResult> AddRole(string rolename) => await _roleManager.CreateAsync(new IdentityRole(rolename));
        public async Task<IdentityResult> DeleteRole(string rolename) {
            IdentityRole role = await _roleManager.FindByNameAsync(rolename);
            return await _roleManager.DeleteAsync(role);
        }
        public IEnumerable<Field> GetForm()
        {
            var form = new List<Field>
            {
                new Field
                {
                    Name="username",
                    Type="String",
                    IsRequired=true,
                    DisplayName="Имя пользователя",

                },
                new Field
                {
                    Name="password",
                    Type="Password",
                    IsRequired=true,
                    DisplayName="Пароль"
                }
            };
            return form;
        }
        public async Task<SignInResult> LoginAsync(string username, string password) => await _signInManager.PasswordSignInAsync(username, password, true, true);
        public async Task LogoutAsync() => await _signInManager.SignOutAsync();
        public void GetNameAndRole(string username, out string name, out string role)
        {
            User user = _userManager.FindByNameAsync(username).Result;
            var roles = _userManager.GetRolesAsync(user).Result;
            role = roles.FirstOrDefault();
            if (role == "Administrator")
            {
                name = "Администратор";
            }
            else
            {
                name = _employee.FindBy(n => n.AccountId == user.Id)?.First().Surname;
            }
        }
    }
}
