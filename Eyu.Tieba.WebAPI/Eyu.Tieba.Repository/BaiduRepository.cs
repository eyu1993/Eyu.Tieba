using Eyu.Tieba.IRepository;
using Eyu.Tieba.Model;
using System.Data.Entity;

namespace Eyu.Tieba.Repository
{
    public partial class BaiduRepository : GenericRepository<Baidu>, IBaiduRepository
    {
        public BaiduRepository(DbContext db) : base(db)
        {

        }
    }
}
