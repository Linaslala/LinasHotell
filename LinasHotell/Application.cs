using LinasHotell.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace LinasHotell
{
    public class Application
    {
        public void Run()
        {
            var builder = new AppBuilder();
            using var db = builder.BuildApp();

            db.Database.Migrate();

            DataSeeder.Seed(db);
        }
    }
}
