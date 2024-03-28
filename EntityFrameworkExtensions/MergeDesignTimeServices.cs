using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkExtensions
{
    public class MergeDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            //Debugger.Launch();
            serviceCollection.AddSingleton<IMigrationsCodeGenerator, CSharpMergeMigrationsGenerator>();
            serviceCollection.AddSingleton<ICSharpMigrationOperationGenerator, CSharpMergeMigrationOperationGenerator>();
        }
    }
}
