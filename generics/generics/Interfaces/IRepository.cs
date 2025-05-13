namespace generics.Interfaces
{
    public interface IRepository<TEntity, TKey>
    {
        void Add(TKey id, TEntity entity);
        TEntity Get(TKey id);
        IEnumerable<TEntity> GetAll();
        void Remove(TKey id);
    }
}
