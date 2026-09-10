using LinasHotell;
using LinasHotell.Controllers;
using LinasHotell.Repositories;
using LinasHotell.Repositories.RepositoryInterfaces;
using LinasHotell.Repositorys;
using LinasHotell.Services;
using LinasHotell.Services.ServiceInterfaces;
using LinasHotell.UIMenus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IRoomService, RoomService>();
services.AddScoped<IGuestService, GuestService>();
services.AddScoped<IBookingService, BookingService>();

services.AddScoped<IRoomRepository, RoomRepository>();
services.AddScoped<IGuestRepository, GuestRepository>();
services.AddScoped<IBookingRepository, BookingRepository>();

services.AddScoped<RoomController>();
services.AddScoped<GuestController>();
services.AddScoped<BookingController>();

services.AddTransient<MainMenu>();
services.AddTransient<RoomMenu>();
services.AddTransient<GuestMenu>();
services.AddTransient<BookingMenu>();
services.AddTransient<MenuNavigator>();

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

await db.Database.MigrateAsync();
await DbInitializer.SeedAsync(db);

var navigator = provider.GetRequiredService<MenuNavigator>();
await navigator.RunAsync();

