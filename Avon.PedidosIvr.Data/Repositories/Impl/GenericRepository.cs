using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Avon.PedidosIvr.Data.Repositories.Impl
{
    public abstract class GenericRepository<C, T, TId> : IGenericRepository<T, TId> where T : class where C : DbContext, new()
    {
        private C _entities = new C();
        public C Context
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public virtual ICollection<T> GetAll()
        {
            ICollection<T> query = _entities.Set<T>().ToList();
            return query;
        }

        public virtual T GetById(TId id)
        {
            return _entities.Set<T>().Find(id);
        }

        public virtual void Add(T entity)
        {
            _entities.Set<T>().Add(entity);
        }

        public virtual void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }
    }
}
