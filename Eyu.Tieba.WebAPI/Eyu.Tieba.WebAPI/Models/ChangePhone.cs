using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class ChangePhone
    {
        [Required]
        [RegularExpression(@"^1\d{10}$")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Code { get; set; }

        [Required]
        [RegularExpression(@"^1\d{10}$")]
        public string NewPhone { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string NewCode { get; set; }
    }
}