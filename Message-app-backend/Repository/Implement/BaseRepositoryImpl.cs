using Message_app_backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Message_app_backend.Repository.Implement
{
    public class BaseRepositoryImpl<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected DbSet<T> Model;
        protected AppDb DbContext;
        protected BaseRepositoryImpl(AppDb appDb)
        {
            DbContext = appDb;
            Model = appDb.Set<T>();
        }

        public T Create(T entity)
        {
            Model.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public List<T> CreateRange(List<T> entity)
        {
            Model.AddRange(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public bool Delete(T entity)
        {
            Model.Remove(entity);
            DbContext.SaveChanges();
            return true;
        }

        public bool DeleteRange(List<T> entites)
        {
            Model.RemoveRange(entites);
            DbContext.SaveChanges();
            return true;
        }

        public List<T> FindAll()
        {
            return Model.ToList();
        }

        public T? FindById(int id)
        {
            return Model.Find(id);
        }

        public T Update(T entity)
        {
            Model.Update(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public List<T> UpdateRange(List<T> entites)
        {
            Model.UpdateRange(entites);
            DbContext.SaveChanges();
            return entites;
        }

        public DatabaseFacade Database()
        {
            return DbContext.Database;
        }
    }
}
