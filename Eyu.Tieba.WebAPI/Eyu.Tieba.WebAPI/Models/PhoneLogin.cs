using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class PhoneLogin : Result
    {
        public string Token { get; set; }
        public string BAIDUID { get; set; }
        public string VCodeSign { get; set; }
        public string CodeString { get; set; }
        public string VerifyCode { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}