using BeHiveV2Server.Services.Creators;
using BeHiveV2Server.Services.Database;
using BeHiveV2Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ServerDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddIdentity<UserIdentity, RoleIdentity>(options => {
    options.SignIn.RequireConfirmedAccount = false;

    //password
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;

}).AddEntityFrameworkStores<ServerDBContext>().AddDefaultTokenProviders(); //add identity service
builder.Services.AddScoped<RoleCreator>();
builder.Services.AddScoped<DefaultAdminCreator>();
builder.Services.AddScoped<TestingDeviceCreator>();
builder.Services.AddMvc();

var app = builder.Build();

var scope = app.Services.CreateScope();
ServerDBContext db = scope.ServiceProvider.GetService<ServerDBContext>();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

var roleCreation = scope.ServiceProvider.GetService<RoleCreator>().CreateRoles();
roleCreation.Wait();
var adminCreation = scope.ServiceProvider.GetService<DefaultAdminCreator>().CreateAdmin();
adminCreation.Wait();
var testDeviceCreation = scope.ServiceProvider.GetService<TestingDeviceCreator>().CreateSHB1WithData();
testDeviceCreation.Wait();

scope.Dispose();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "",
    defaults: new { controller = "Main", action = "index" });

app.MapAreaControllerRoute(
    name: "authentication",
    pattern: "authentication/{action}",
    defaults: new { controller = "Authentication" },
    areaName: "UserArea");

app.MapAreaControllerRoute(
    name: "adminDefault",
    pattern: "admin",
    defaults: new { controller = "HomeAdmin", action = "Admin" },
    areaName: "AdminArea");

app.MapAreaControllerRoute(
    name: "adminUsersDefault",
    pattern: "admin/users",
    defaults: new { controller = "UserAdminManagement", action = "Users" },
    areaName: "AdminArea");

app.MapAreaControllerRoute(
    name: "adminUsersAction",
    pattern: "admin/users/{action}/{id?}",
    defaults: new { controller = "UserAdminManagement" },
    areaName: "AdminArea");

app.MapAreaControllerRoute(
    name: "adminDevicesDefault",
    pattern: "admin/devices",
    defaults: new { controller = "DeviceAdminManagement", action = "Devices" },
    areaName: "AdminArea");

app.MapAreaControllerRoute(
    name: "adminDevicesAction",
    pattern: "admin/devices/{action}/{id?}",
    defaults: new { controller = "DeviceAdminManagement" },
    areaName: "AdminArea");

app.MapAreaControllerRoute(
    name: "UserDevicesDefault",
    pattern: "devices",
    defaults: new { controller = "Device", action = "Devices" },
    areaName: "UserArea");

app.MapAreaControllerRoute(
    name: "UserDevicesAction",
    pattern: "devices/{action}/{id?}",
    defaults: new { controller = "Device" },
    areaName: "UserArea");

app.MapAreaControllerRoute(
    name: "RestDeviceDefault",
    pattern: "restapi/{action}",
    defaults: new { controller = "RestAPIDevice" },
    areaName: "RestArea");

app.Run();
