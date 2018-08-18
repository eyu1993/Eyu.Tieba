namespace Eyu.Tieba.WinService
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.Win32;
    using System.ComponentModel.DataAnnotations;

    public partial class Tieba : DbContext
    {
        public Tieba()
           : base(GetConnectionString())
        {
        }
        private static string GetConnectionString()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("TIEBA").OpenSubKey("DATABASE", false);
                string uid = key.GetValue("uid").ToString();
                string pwd = key.GetValue("pwd").ToString();
                string db = key.GetValue("db").ToString();
                key.Close();
                return "server=.;uid=" + uid + ";pwd=" + pwd + ";database=" + db;
            }
            catch (Exception ex)
            {
                LogHelper.Error("Failed to read registry.\t" + ex.ToString());
                throw;
            }
        }

        public virtual DbSet<Baidu> Baidu { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
    [Table("User")]
    public partial class User
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

        public DateTime CreatedTime { get; set; }

        public bool IsActive { get; set; }
    }
    [Table("Baidu")]
    public partial class Baidu
    {
        public int ID { get; set; }

        [Required]
        public string BDUSS { get; set; }

        public string PTOKEN { get; set; }

        public string STOKEN { get; set; }

        public Guid UserId { get; set; }
    }
}
