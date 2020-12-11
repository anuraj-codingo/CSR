using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerServicePortal.Models
{
    public class UserRegModel
    {
        [Required]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter your Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [System.Web.Mvc.Remote("UsernameExists", "Users", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "User name already registered.")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

     

        [Display(Name = "Fund")]
        [Required(ErrorMessage = "{0} is required.")]
        public string[] fundMultiple { get; set; }

        [Display(Name = "Fund")]
        [Required(ErrorMessage = "{0} is required.")]
        public string fundSingle { get; set; }

        [Required]
        public string Roles { get; set; }

        [Required]
        public string AuthenticationType { get; set; }

        public List<SelectListItem> FundList { get; set; }
    }
}