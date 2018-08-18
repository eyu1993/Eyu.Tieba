using System.Data.Entity;
using System.Threading.Tasks;
using Eyu.Tieba.IRepository;
using Eyu.Tieba.Repository;

namespace Eyu.Tieba.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _db;
        public UnitOfWork()
        {
            _db = _db ?? new Eyu.Tieba.Model.Tieba();
        }
        private IUserRepository _userRepository;
        public IUserRepository UserRepository { get => _userRepository ?? new UserRepository(_db); }

        private IBaiduRepository _baiduRepository;
        public IBaiduRepository BaiduRepository { get => _baiduRepository ?? new BaiduRepository(_db); }

        private IBlackListRepository _blackListRepository;
        public IBlackListRepository BlackListRepository { get => _blackListRepository ?? new BlackListRepository(_db); }

        #region Methods
        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
        public int SaveChanges()
        {
            return _db.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
        #endregion
    }
}
