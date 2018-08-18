using Eyu.Tieba.IRepository;
using Eyu.Tieba.Model;
using System.Data.Entity;

namespace Eyu.Tieba.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContext db) : base(db)
        {
        }
    }
}
