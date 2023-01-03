using Autofac;
using Microsoft.AspNetCore.Http;
using matcrm.data;
using matcrm.data.Infrastructure;
using matcrm.data.Models.Tables;

namespace matcrm.api
{
    public class AutofacModule : Module
    {
        public static void RegisterType(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerDependency();

            builder.RegisterAssemblyTypes(typeof(Repository<User>).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterAssemblyTypes(typeof(Service<User>).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<IFormFile>()
                .AsImplementedInterfaces().InstancePerDependency();
        }
    }
}