using LinasHotell;
using LinasHotell.Controllers;
using LinasHotell.Repositories.Interfaces;
using LinasHotell.Repositorys;
using LinasHotell.Services;
using LinasHotell.Services.ServiceInterfaces;
using LinasHotell.UIMenus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LinasHotell.Repositories.RepositoryInterfaces;
using LinasHotell.Repositories;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddSingleton<IRoomService, RoomService>();
services.AddSingleton<IGuestService, GuestService>();

services.AddSingleton<IRoomRepository, RoomRepository>();
services.AddSingleton<IGuestRepository, GuestRepository>();

services.AddSingleton<RoomController>();
services.AddSingleton<GuestController>();


services.AddTransient<MainMenu>();
services.AddTransient<RoomMenu>();
services.AddTransient<GuestMenu>();
services.AddTransient<MenuNavigator>();


var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

var navigator = provider.GetRequiredService<MenuNavigator>();
await navigator.RunAsync();

