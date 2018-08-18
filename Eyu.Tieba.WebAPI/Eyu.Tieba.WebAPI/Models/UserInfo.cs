using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class UserInfo : Result
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}