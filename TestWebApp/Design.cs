using EntityFrameworkExtensions;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;

namespace TestWebApp
{
    public class Design : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMigrationsCodeGenerator, CSharpMergeMigrationsGenerator>();
            serviceCollection.AddSingleton<ICSharpMigrationOperationGenerator, CSharpMergeMigrationOperationGenerator>();
        }
    }
}
