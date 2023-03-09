using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatrickParker.EmpowerID.Demo.Interfaces;
using PatrickParker.EmpowerID.Demo.Repository.Data;
using PatrickParker.EmpowerID.Demo.Repository.Repositories;
using PatrickParker.EmpowerID.Demo.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (connectionString.Contains("(localdb)"))
{
    var _dbName = builder.Configuration.GetSection("Database").Value;
    using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Master;Integrated Security=True;MultipleActiveResultSets=True;"))
    {
        connection.Open();

        new SqlCommand($"IF EXISTS(select * from sys.databases where name='{_dbName}') DROP DATABASE [{_dbName}]", connection).ExecuteNonQuery();
        new SqlCommand($"CREATE DATABASE [{_dbName}]", connection).ExecuteNonQuery();
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
#region Repositories
builder.Services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
builder.Services.AddTransient<IEmployeeRepositoryAsync, EmployeeRepositoryAsync>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
#endregion
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IRazorRenderService, RazorRenderService>();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var datacontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    datacontext.Database.Migrate();
}

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
    