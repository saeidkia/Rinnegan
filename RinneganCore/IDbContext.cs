namespace RinneganCore
{
    public abstract class IDbContext<T> where T : class
    {
        protected T Db { get; set; }
        protected IDbContext(T db)
        {
            Db = db;
        }
    }
}
