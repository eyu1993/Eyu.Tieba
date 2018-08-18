using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class Register
    {
        [Required]
        [StringLength(16)]
        public string UserName { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^1\d{10}$", ErrorMessage = "手机号格式错误")]
        public string Phone { get; set; }

        [Required]
        public string Code { get; set; }
    }
}