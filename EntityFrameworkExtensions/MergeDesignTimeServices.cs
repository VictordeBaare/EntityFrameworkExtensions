using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EntityFrameworkExtensions
{
    public class MergeDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            Debugger.Launch();
            serviceCollection.AddSingleton<IMigrationsCodeGenerator, CSharpMergeMigrationsGenerator>();
            serviceCollection.AddSingleton<ICSharpMigrationOperationGenerator, CSharpMergeMigrationOperationGenerator>();
        }
    }
}
