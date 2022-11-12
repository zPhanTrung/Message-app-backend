namespace Message_app_backend.Repository
{
    public interface IBaseRepository<T> 
    {
        public List<T> FindAll();

        public T Create(T entity);

        public T Update(T entity);

        public bool Delete(T entity);
    }
}
