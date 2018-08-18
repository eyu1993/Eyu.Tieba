using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class AccountLogin : Result
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string BAIDUID { get; set; }
        public string PubKey { get; set; }
        public string Key { get; set; }
        public string CodeString { get; set; }
        public string VerifyCode { get; set; }
    }
}