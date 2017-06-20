using System.Collections.Generic;

namespace Avon.PedidosIvr.Data.Repositories
{
    public interface IGenericRepository<T, TId> where T : class
    {
        ICollection<T> GetAll();
        T GetById(TId id);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        void Save();
    }
}
