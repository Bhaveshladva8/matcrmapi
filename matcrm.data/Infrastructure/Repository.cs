using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using matcrm.data.Context;
using matcrm.data.Helpers;

namespace matcrm.data.Infrastructure {
    public class Repository<T> : IRepository<T> where T : class {
        protected OneClappContext dbContext;

        //private UnitOfWork _unitOfWork;

        public IConfiguration config;

        public Repository (IDbFactory dbFactory) {
            dbContext = dbFactory.Init ();
        }

        public ICollection<T> GetAll () {
            return dbContext.Set<T> ().ToList ();
        }

        public async Task<IQueryable<T>> GetAllAsync () {
            return dbContext.Set<T> ();
        }

        public T Get (int id) {
            return dbContext.Set<T> ().Find (id);
        }

        public async Task<T> GetAsync (int id) {
            return await dbContext.Set<T> ().FindAsync (id);
        }

        public T Find (Expression<Func<T, bool>> match) {
            return dbContext.Set<T> ().SingleOrDefault (match);
        }

        public async Task<T> FindAsync (Expression<Func<T, bool>> match) {
            return await dbContext.Set<T> ().SingleOrDefaultAsync (match);
        }

        public ICollection<T> FindAll (Expression<Func<T, bool>> match) {
            return dbContext.Set<T> ().Where (match).ToList ();
        }

        public async Task<ICollection<T>> FindAllAsync (Expression<Func<T, bool>> match) {
            return await dbContext.Set<T> ().Where (match).ToListAsync ();
        }

        public T Add (T t) {
            try {
                foreach (var entity in dbContext.ChangeTracker.Entries ().Where (e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)) {
                    dbContext.Entry (entity.Entity).State = EntityState.Detached;
                }
                dbContext.Set<T> ().Add (t);
                dbContext.SaveChanges ();
                return t;
            } catch (Exception ex) {
                return t;
            }
        }

        public async Task<T> AddAsync (T t) {
            dbContext.Set<T> ().Add (t);
            await dbContext.SaveChangesAsync ();
            return t;
        }

        public async Task<List<T>> AddRange (List<T> t) {
            dbContext.AddAsync (t);
            await dbContext.SaveChangesAsync ();
            return t;
        }

        public async Task<List<T>> UpdateRange (List<T> tList) {
            foreach (var updated in tList) {
                // updated.
                // T existing = dbContext.Set<T> ().Find (updated["Id"]);
                // if (existing != null) {
                    dbContext.Entry (updated).CurrentValues.SetValues (updated);
                // }
            }
            dbContext.SaveChanges ();
            return tList;
        }

        public T Update (T updated, int key) {
            if (updated == null)
                return null;

            T existing = dbContext.Set<T> ().Find (key);
            if (existing != null) {
                dbContext.Entry (existing).CurrentValues.SetValues (updated);
                dbContext.SaveChanges ();
            }
            return existing;
        }

        public async Task<T> UpdateAsync (T updated, int key) {
            if (updated == null)
                return null;

            T existing = await dbContext.Set<T> ().FindAsync (key);
            if (existing != null) {
                dbContext.Entry (existing).CurrentValues.SetValues (updated);
                await dbContext.SaveChangesAsync ();
            }
            return existing;
        }

        public async Task<T> UpdateAsync (T updated, long key) {
            if (updated == null)
                return null;

            T existing = await dbContext.Set<T> ().FindAsync (key);
            if (existing != null) {
                dbContext.Entry (existing).CurrentValues.SetValues (updated);
                await dbContext.SaveChangesAsync ();
            }
            return existing;
        }

        public void Delete (T t) {
            dbContext.Set<T> ().Remove (t);
            dbContext.SaveChanges ();
        }

        public async Task<int> DeleteAsync (T t) {
            dbContext.Set<T> ().Remove (t);
            return await dbContext.SaveChangesAsync ();
        }

        public int Delete (Expression<Func<T, bool>> where) {
            var entities = dbContext.Set<T> ().Where (where);
            dbContext.Set<T> ().RemoveRange (entities);
            return dbContext.SaveChanges ();
        }

        public async Task<int> DeleteAsync (Expression<Func<T, bool>> where) {
            var entities = dbContext.Set<T> ().Where (where);
            dbContext.Set<T> ().RemoveRange (entities);
            return await dbContext.SaveChangesAsync ();
        }

        public int Count () {
            return dbContext.Set<T> ().Count ();
        }

        public async Task<int> CountAsync () {
            return await dbContext.Set<T> ().CountAsync ();
        }

        public IEnumerable<T> ExecuteSP<T> (string query, params SqlParameter[] SqlPrms) where T : new () {
            DataSet ds = new DataSet ();

            string connectionString = dbContext.Database.GetDbConnection ().ConnectionString;
            using (SqlConnection conn = new SqlConnection (connectionString)) {
                using (SqlCommand command = new SqlCommand (query, conn)) {
                    command.Parameters.AddRange (SqlPrms);
                    command.CommandTimeout = 0;
                    conn.Open ();
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter (command)) {
                        try {
                            dataAdapter.SelectCommand = command;
                            dataAdapter.Fill (ds);
                        } catch (Exception ex) {
                            throw ex;
                        } finally {
                            conn.Close ();
                        }
                    }
                }
            }

            return DataUtility.CreateListFromTable<T> (ds.Tables[0]);
        }

        public virtual void Truncate (String TableName) {
            // dbContext.Database.ExecuteSqlCommand("Truncate Table " + TableName);
            dbContext.Database.ExecuteSqlRaw ("Truncate Table " + TableName);
        }

        public virtual async Task TruncateAsync (String TableName) {
            // await dbContext.Database.ExecuteSqlCommandAsync("Truncate Table " + TableName);
            dbContext.Database.ExecuteSqlRaw ("Truncate Table " + TableName);
        }
        public async Task<IQueryable<T>> GetMany (Expression<Func<T, bool>> where) {
            return dbContext.Set<T> ().Where (where);
        }
    }
}