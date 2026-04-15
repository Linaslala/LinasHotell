using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.Builders
{
    public class AppBuilder
    {
        public ApplicationDbContext BuildApp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' saknas.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connString)
                .EnableSensitiveDataLogging(false)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
