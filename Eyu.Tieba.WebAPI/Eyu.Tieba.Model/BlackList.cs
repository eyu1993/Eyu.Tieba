using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Model
{
    public class BlackList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        [Required]
        [StringLength(128)]
        public string BarName { get; set; }

        [Required]
        [StringLength(32)]
        public string BarFid { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public int BaiduId { get; set; }

        [StringLength(128)]
        public string Reason { get; set; }
    }
}
