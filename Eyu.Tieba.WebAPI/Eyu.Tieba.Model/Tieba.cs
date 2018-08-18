namespace Eyu.Tieba.Model
{
    using Eyu.Tieba.Common;
    using Microsoft.Win32;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class Tieba : DbContext
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
                LogHelper.Error("Failed to read database connection string in registry. " + ex.ToString());
                throw;
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Baidu> Baidu { get; set; }
    }
}