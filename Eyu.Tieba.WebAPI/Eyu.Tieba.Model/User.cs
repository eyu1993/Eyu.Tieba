using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Model
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(16)]
        public string Name { get; set; }

        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(11)]
        public string Phone { get; set; }

        public DateTime? CreatedTime { get; set; }

        public bool IsActive { get; set; }
    }
}
