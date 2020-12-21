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
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
    public  class ChangepassWord
    {
    
        [Required(ErrorMessage = "Password required")]
        public string NewPassWord { get; set; }
        [Required(ErrorMessage = "ConfirmPassword required")]
        [Compare("NewPassWord", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }
    }
}