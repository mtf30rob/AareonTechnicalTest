using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AareonTechnicalTest.Tests
{

    /*
        ================================================
                    NOT MY OWN WORK    
        ================================================

        Taken from : https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0

    */

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));

                services.Remove(descriptor);

                services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        SeedTestDb(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }

        private static void SeedTestDb(ApplicationContext db)
        {
            var personAdmin = db.Persons.Add(new Models.Person() { Forename = "Bob", Surname = "Dylan", IsAdmin = true });
            var personNonAdmin = db.Persons.Add(                                        new Models.Person() { Forename = "Bob", Surname = "Marley", IsAdmin = false });

            var ticket = db.Tickets.Add(new Models.Ticket() { Content = "Issue logging on", Person = personAdmin.Entity});
            var ticketNote = db.TicketNotes.Add(new Models.TicketNote() {
                NoteContent = "Found issue. Asigning ticket to support team",Person = personNonAdmin.Entity, Ticket = ticket.Entity
            });
            
            db.SaveChanges();
        }
    }


}