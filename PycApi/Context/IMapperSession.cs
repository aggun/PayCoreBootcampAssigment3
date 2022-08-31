using System.Linq;

namespace PycApi.Context
{
    //CRUD işlemleri için gerekli interface clasının oluşturulması
    public interface IMapperSession<T> where T : class
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
        void CloseTransaction();
        void Save(T entity);
        void Update(T entity);
        void Delete(T entity);

        IQueryable<T> entity { get; }
    }
}
