using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class FindPassword
    {
        public string Phone { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
    }
}