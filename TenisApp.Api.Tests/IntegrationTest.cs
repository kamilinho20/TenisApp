using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TenisApp.Api.Controllers;
using TenisApp.DataAccess.DbContext;
using TenisApp.DataAccess.Repository;
using Moq;
using TenisApp.Shared.ViewModel;
using Microsoft.AspNetCore.Http;
using TenisApp.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace TenisApp.Api.Tests;

public class IntegrationTest : IDisposable
{
    private AppDbContext _context;

    public static IConfiguration InitializeConfiguration() {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json")
        .AddEnvironmentVariables().Build();
        return config;
    }

    public IntegrationTest()
    {
        var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkSqlServer()
        .BuildServiceProvider();
        var config = InitializeConfiguration();
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionStringBuilder = new SqlConnectionStringBuilder();
        connectionStringBuilder.Password = config["Database:Password"];
        connectionStringBuilder.UserID = config["Database:User"];
        connectionStringBuilder.DataSource = config["Database:Server"];
        connectionStringBuilder.InitialCatalog = config["Database:DbName"];
        var connectionString = connectionStringBuilder.ConnectionString;
        builder.UseSqlServer(connectionString)
        .UseInternalServiceProvider(serviceProvider);
        _context = new AppDbContext(builder.Options);
        _context.Database.EnsureCreated();        
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }

    [Fact]
    public async Task Test1()
    {
        // Arrange
        var repo = new CourtRepository(_context);
        var loggerMock = new Mock<ILogger<CourtController>>();
        var controller = new CourtController(loggerMock.Object, repo);
        var model = new CourtViewModel() {
            Court = new Core.Model.Court() {
                Name = "TestCourt",
                PricePerHour = 10,
                SurfaceType = Core.Enum.Surface.Clay,
                OpeningTime = DateTime.Parse("8:00"),
                ClosingTime = DateTime.Parse("22:00")
            }
        };

        // Act
        var resultResponse = await controller.Add(model);
        var courtsResponse = await controller.Get();

        // Assert
        var result = Assert.IsType<OkObjectResult>(resultResponse);
        var getResults = Assert.IsType<OkObjectResult>(courtsResponse);

        var courtVM = result.Value as CourtViewModel;
        var courts = getResults.Value as IEnumerable<Court>;

        Assert.NotNull(courtVM);
        Assert.NotNull(courts);
        Assert.Contains<Court>(courtVM!.Court, courts);
    }
}