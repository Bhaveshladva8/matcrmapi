namespace matcrm.data {
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Threading.Tasks;
    using System;
    using matcrm.data.Context;

    public class Service<T> where T : class {
        protected OneClappContext dbContext;
        private readonly IUnitOfWork unitOfWork;

        public Service (IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IQueryable<T>> GetAllAsync () {
            var result = dbContext.Set<T> ();
            return result;
        }

        public async Task<IQueryable<T>> FindAllAsync (Expression<Func<T, bool>> match) {
            return dbContext.Set<T> ().Where (match);
        }

        public async Task<T> GetAsync (int id) {
            return await dbContext.Set<T> ().FindAsync (id);
        }

        public T Add (T t) {
            dbContext.Set<T> ().Add (t);
            return t;
        }

        public async Task<T> UpdateAsync (T updated, int key) {
            if (updated == null)
                return null;

            T existing = await dbContext.Set<T> ().FindAsync (key);
            if (existing != null) {
                dbContext.Entry (existing).CurrentValues.SetValues (updated);
            }
            return existing;
        }

        public async Task<T> UpdateAsync (T updated, long key) {
            if (updated == null)
                return null;

            T existing = await dbContext.Set<T> ().FindAsync (key);
            if (existing != null) {
                dbContext.Entry (existing).CurrentValues.SetValues (updated);
            }
            return existing;
        }

        public void Delete (T t) {
            dbContext.Set<T> ().Remove (t);
            dbContext.SaveChanges ();
        }

        public async Task<T> UpdateAsync (T updated, String key) {
            if (updated == null)
                return null;

            T existing = await dbContext.Set<T> ().FindAsync (key);
            if (existing != null) {
                dbContext.Entry (existing).CurrentValues.SetValues (updated);
            }
            return existing;
        }

        public async Task<T> GetAsync (String id) {
            return await dbContext.Set<T> ().FindAsync (id);
        }

        public async Task SaveAsync () {
            dbContext.SaveChanges ();
            await unitOfWork.CommitAsync ();
        }

        public void BeginTransaction () {
            unitOfWork.BeginTransaction ();
        }

        public void Rollback () {
            unitOfWork.Rollback ();
        }

        public async Task Commit () {
            await unitOfWork.Commit ();
        }

        public async Task<IQueryable<T>> GetMany (Expression<Func<T, bool>> where) {
            return dbContext.Set<T> ().Where (where);
        }

        public async Task<IList<T>> GetAllListAsnc () {
            IList<T> result = dbContext.Set<T> ().ToList ();
            return result;
        }
    }
}