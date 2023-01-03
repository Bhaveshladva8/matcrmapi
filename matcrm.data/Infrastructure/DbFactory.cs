using matcrm.data.Context;

namespace matcrm.data
{
    public class DbFactory: Disposable, IDbFactory
    {
         OneClappContext dbContext;

        public OneClappContext Init(){          
            return dbContext ?? (dbContext = new OneClappContext());
        }

        protected override void DisposeCore(){
            if(dbContext != null){
                dbContext.Dispose();
            }
        }
    }
}