using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerServicePortal.Models
{
    public class MemeberDetailsModel
    {
        #region Instance Properties

        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "SSN")]
        public string SSN { get; set; }

        [Display(Name = "Member")]
        [StringLength(10)]
        public String Member { get; set; }

        [Display(Name = "City")]
        [StringLength(50)]
        public String City { get; set; }

        [Display(Name = "State")]
        [StringLength(50)]
        public String State { get; set; }

        [Display(Name = "Year")]
        public string Year { get; set; }

        [Display(Name = "Month")]
        public string Month { get; set; }

        [Display(Name = "Day")]
        public string Day { get; set; }

        #endregion Instance Properties
    }
}