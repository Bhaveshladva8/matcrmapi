using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace matcrm.data {
    public interface IRepository<T> where T : class {
        T Add (T entity);

        Task<T> AddAsync (T entity);
        Task<List<T>> AddRange (List<T> entities);

        Task<List<T>> UpdateRange (List<T> entities);

        T Update (T entity, int Key);

        Task<T> UpdateAsync (T entity, int key);

        Task<T> UpdateAsync (T entity, long key);

        void Delete (T entity);

        Task<int> DeleteAsync (T entity);

        int Count ();

        Task<int> CountAsync ();

        int Delete (Expression<Func<T, bool>> where);

        Task<int> DeleteAsync (Expression<Func<T, bool>> where);

        ICollection<T> GetAll ();

        Task<IQueryable<T>> GetAllAsync ();

        T Get (int id);

        Task<T> GetAsync (int id);

        T Find (Expression<Func<T, bool>> match);

        Task<T> FindAsync (Expression<Func<T, bool>> match);

        ICollection<T> FindAll (Expression<Func<T, bool>> match);

        Task<ICollection<T>> FindAllAsync (Expression<Func<T, bool>> match);

        IEnumerable<T> ExecuteSP<T> (string query, params SqlParameter[] SqlPrms) where T : new ();

        void Truncate (String TableName);

        Task TruncateAsync (String TableName);

        Task<IQueryable<T>> GetMany (Expression<Func<T, bool>> where);
    }
}