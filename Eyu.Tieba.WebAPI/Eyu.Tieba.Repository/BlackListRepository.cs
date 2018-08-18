using Eyu.Tieba.IRepository;
using Eyu.Tieba.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Repository
{
    public partial class BlackListRepository : GenericRepository<BlackList>, IBlackListRepository
    {
        public BlackListRepository(DbContext db) : base(db)
        {

        }
    }
}
