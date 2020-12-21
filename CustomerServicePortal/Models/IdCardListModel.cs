using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerServicePortal.Models
{
    public class IdCardListModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string MemberId { get; set; }

        public string EMSSN { get; set; }

        public bool Completestatus { get; set; }
        //public string DOBYear { get; set; }
        //public string DOBMonth { get; set; }
        //public string DOBDay { get; set; }     
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        //public string Addr4 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip1 { get; set; }
        //public string Zip2 { get; set; }
        //public string Zip3 { get; set; }
        //public bool ShowRequestId { get; set; }
    }
}