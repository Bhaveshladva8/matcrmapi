using Hangfire.Dashboard;

namespace matcrm.api.Middleware
{
    public class HangfireAuthorizationFilter: IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}