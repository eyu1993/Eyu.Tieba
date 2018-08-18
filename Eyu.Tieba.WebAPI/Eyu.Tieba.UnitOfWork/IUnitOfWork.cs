using Eyu.Tieba.IRepository;
using System;
using System.Threading.Tasks;

namespace Eyu.Tieba.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IUserRepository UserRepository { get; }
        IBaiduRepository BaiduRepository { get; }
        IBlackListRepository BlackListRepository { get; }
    }
}
