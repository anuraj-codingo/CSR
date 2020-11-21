using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerServicePortal.Models
{
    public class DEDMETModel
    {
        public int Year { get; set; }
        public List<DedmetDetail> dedmetDetails { get; set; }
    }
    public class DedmetDetail
    {
        public decimal DEDMET { get; set; }
        public decimal Remaining { get; set; }
        public string Status { get; set; }
    }
}