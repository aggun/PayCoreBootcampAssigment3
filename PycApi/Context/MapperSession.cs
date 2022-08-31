using NHibernate;
using System.Linq;

namespace PycApi.Context
{
    //CRUD interface'nin implemantasyonu ve içinin doldurulması.
    public class MapperSession<T> : IMapperSession<T> where T : class
    {
        private readonly ISession session;
        private ITransaction transaction;

        public MapperSession(ISession session)
        {
            this.session = session;
        }

        public IQueryable<T> entity => session.Query<T>();


        public void BeginTransaction()
        {
            transaction = session.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

        public void CloseTransaction()
        {
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Save(T entity)
        {
            session.Save(entity);
        }
        public void Update(T entity)
        {
            session.Update(entity);
        }
        public void Delete(T entity)
        {
            session.Delete(entity);
        }
    }
}
