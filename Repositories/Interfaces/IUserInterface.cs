using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Models;

namespace Kendo.Repositories.Interfaces
{
    public interface IUserInterface
    {
        Task<int> Register(User userData);
        Task<User> Login(Login user);
        Task<User> GetUser(int userid);
    }
}