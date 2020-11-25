using System.ComponentModel.DataAnnotations;

namespace CustomerServicePortal.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }

        public string SSN { get; set; }
    }

    public class UserModel
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}