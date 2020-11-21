using System.Collections.Generic;
using System.Linq;

namespace CustomerServicePortal.DAL
{
    public static class Repository
    {
        private static List<User> users = new List<User>() {
        new User() {Email="abc@gmail.com",Roles="Staff",Password="abcadmin" },
        new User() {Email="xyz@gmail.com",Roles="User",Password="xyzeditor" }
    };

        public static User GetUserDetails(User user)
        {
            return users.Where(u => u.Email.ToLower() == user.Email.ToLower() &&
            u.Password == user.Password).FirstOrDefault();
        }
    }
}