using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomerServicePortal.Models
{
 
    public class ClaimDetailDashBoardModel
    {
        public List<ClaimDetailModel> claimDetailModels { get; set; }
        public List<DependentDetailModel> dependentDetailModels { get; set; }
        public EMPdetails eMPdetails { get; set; }
        public  List<DEDMETModel> dEDMETModel { get; set; }
    }
    public class DependentDetailModel
    {
        public string Relation { get; set; }
        public string DependenetName{ get; set; }
        public string Status { get; set; }
        public string Plan { get; set; }
        public string Class { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string SSN { get; set; }
        public string DependentSeq { get; set; }
    }
    public class EMPdetails
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Id { get; set; }
        public string DOBYear { get; set; }
        public string DOBMonth { get; set; }
        public string DOBDay { get; set; }
        public decimal EMSSN { get; set; }

    }
    public class ClaimDetailModel
    {
        #region Instance Properties

        [Display(Name = "ID")]
        [Required]
        public Int64 ID { get; set; }

        [Display(Name = "SSN")]
        [StringLength(50)]
        public String SSN { get; set; }

        [Display(Name = "Spouse")]
        public Boolean? Spouse { get; set; }

        [Display(Name = "EOBNO")]
        [StringLength(50)]
        public String EOBNO { get; set; }

        [Display(Name = "ClaimNo")]
        [StringLength(50)]
        public String ClaimNo { get; set; }

        public string ClaimYear { get; set; }
        public string ClaimMonth { get; set; }
        public string ClaimDate { get; set; }

        [Display(Name = "For")]
        [StringLength(50)]
        public String For { get; set; }

        [Display(Name = "Type")]
        [StringLength(50)]
        public String Type { get; set; }

        [Display(Name = "Provider")]
        [StringLength(50)]
        public String Provider { get; set; }

        [Display(Name = "Total")]
        public string Total { get; set; }

        [Display(Name = "PlanPaid")]
        public string PlanPaid { get; set; }

        [Display(Name = "MemerResp")]
        public string MemerResp { get; set; }

        #endregion Instance Properties
    }
}