using LinasHotell;
using LinasHotell.Controllers;
using LinasHotell.Repositories.Interfaces;
using LinasHotell.Repositorys;
using LinasHotell.Services;
using LinasHotell.UIMenus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddSingleton<IRoomService, RoomService>();

services.AddSingleton<IRoomRepository, RoomRepository>();

services.AddSingleton<RoomController>();


services.AddTransient<MainMenu>();
services.AddTransient<RoomMenu>();
services.AddTransient<MenuNavigator>();


var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

var navigator = provider.GetRequiredService<MenuNavigator>();
await navigator.RunAsync();

