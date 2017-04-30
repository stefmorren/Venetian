using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Venetian.DataLayer;
using Venetian.DomainClasses;

namespace Venetian.DataLayer
{
    public class UserRepository
    {
        private readonly VenetianContext _venetianContext;

        public UserRepository(VenetianContext venetianContext)
        {
            _venetianContext = venetianContext;
        }
        public User LoginUser(string username, string password)
        {
            return _venetianContext.Users
                .Where(u => u.Username == username)
                .FirstOrDefault(u => u.Password == password);
        }

        public void AddUser(User user)
        {
            _venetianContext.Users.Add(user);
            _venetianContext.SaveChanges();
        }

        public bool CheckIfUsernameAllreadyExists(string username)
        {
            var user = _venetianContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public string ReturnUsersSalt(string username)
        {
            var user = _venetianContext.Users.FirstOrDefault(u => u.Username == username);
            if (user!=null)
            {
                return user.Salt;
            }
            return "";
        }

        public List<User> GetReceiversExceptUser(User user)
        {
            return _venetianContext.Users.Where(u => u.Username != user.Username).ToList();
        }
    }
}
