using System.Data.Common;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Chirp.Web.Tests;


//Sources: 
// https://stackoverflow.com/questions/55811147/seed-test-data-for-every-test-in-asp-net-core-ef-core-xunit-net-integration
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0&pivots=xunit
public class RazorPageWebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //Removes old db context
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                                    typeof(IDbContextOptionsConfiguration<
                                                                        ChirpDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            //Removes old db context
            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                                                                       typeof(DbConnection));

            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            // Opens sqlite connection to ensure sqlite stays in memory
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            //Create new DB context for testing
            services.AddDbContext<ChirpDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();

            //Ensure fresh database in memory before seeding
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            DbInitializer.SeedDatabase(dbContext);

        });

        //Use the development enviroment
        builder.UseEnvironment("Development");
    }
}
    