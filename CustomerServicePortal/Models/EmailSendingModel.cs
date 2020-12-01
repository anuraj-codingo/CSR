using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerServicePortal.Models
{
    public class EmailSendingModel
    {
        public string ToMail { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}