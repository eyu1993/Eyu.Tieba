using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Model
{
    public class Baidu
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string BDUSS { get; set; }

        public string PTOKEN { get; set; }

        public string STOKEN { get; set; }

        public string Name { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
