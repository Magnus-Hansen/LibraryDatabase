using LibraryAPI;
using LibraryAPI.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LibrarySQLBackend.Context;
using LibrarySQLBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace IntegrationTest;

[TestClass]
public sealed class LibraryApiTests
{
    private HttpClient _httpClient = null!;
    private WebApplicationFactory<Program> _factory = null!;

    private const string JwtKey = "integration-test-key-integration-test-key-123456";
    private const string JwtIssuer = "integration-tests";
    private const string JwtAudience = "integration-tests";

    [TestInitialize]
    public void Setup()
    {
        var inMemoryDbName = $"integrationtests-{Guid.NewGuid()}";

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    var overrides = new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=LibraryTest;User=root;Password=password;",
                        ["Jwt:Key"] = JwtKey,
                        ["Jwt:Issuer"] = JwtIssuer,
                        ["Jwt:Audience"] = JwtAudience,
                        ["Ai:Enabled"] = "false"
                    };

                    config.AddInMemoryCollection(overrides);
                });

                builder.ConfigureTestServices(services =>
                {
                    services.PostConfigureAll<JwtBearerOptions>(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = JwtIssuer,
                            ValidAudience = JwtAudience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey))
                        };
                    });

                    // Use EF Core InMemory instead of MySQL for tests.
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.RemoveAll<AppDbContext>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(inMemoryDbName));
                });
            });

        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CreateJwt());

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureCreated();

        var language = new Language { Id = 1, Language1 = "English" };
        var publisher = new Publisher { Id = 1, Name = "Test Publisher" };

        db.Languages.Add(language);
        db.Publishers.Add(publisher);
        db.Items.Add(new Item
        {
            Id = 1,
            Name = "Seed Item",
            ReleaseYear = 2020,
            MediaType = "book",
            AverageStars = 4.5m,
            LanguageId = language.Id,
            PublisherId = publisher.Id,
            Language = language,
            Publisher = publisher
        });

        db.SaveChanges();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _httpClient.Dispose();
        _factory.Dispose();
    }

    [TestMethod]
    public async Task Get_Items_ReturnsList()
    {
        var response = await _httpClient.GetAsync("/api/items");
        var body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, body);

        var items = JsonSerializer.Deserialize<List<ItemDto>>(
            body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.IsNotNull(items);
        Assert.IsTrue(items.Count > 0);
    }

    [TestMethod]
    public async Task Get_Items_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/items");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Get_ItemById_ReturnsDetails()
    {
        var response = await _httpClient.GetAsync("/api/items/1");
        var body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, body);

        var item = JsonSerializer.Deserialize<ItemDetailsDto>(
            body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.IsNotNull(item);
        Assert.AreEqual(1, item.Id);
        Assert.AreEqual("Seed Item", item.Name);
        Assert.AreEqual("English", item.Language);
        Assert.AreEqual("Test Publisher", item.Publisher);
    }

    [TestMethod]
    public async Task Get_ItemById_Unknown_ReturnsNotFound()
    {
        var response = await _httpClient.GetAsync("/api/items/999");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Put_Item_Unknown_ReturnsNotFound()
    {
        var dto = new UpdateItemDto { Name = "Does not matter" };

        var response = await _httpClient.PutAsJsonAsync("/api/items/999", dto);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Put_Item_Existing_ReturnsNoContent_AndPersistsChanges()
    {
        var dto = new UpdateItemDto
        {
            Name = "Updated Seed Item",
            ReleaseYear = 2021,
            Description = "Updated description",
            MediaType = "book",
            Image = "updated-image.png",
            LanguageId = 1,
            PublisherId = 1,
            Book = new BookDto
            {
                Isbn = "978-1-4028-9462-6",
                NoOfPages = 321,
                Version = "2nd"
            }
        };

        var putResponse = await _httpClient.PutAsJsonAsync("/api/items/1", dto);
        Assert.AreEqual(HttpStatusCode.NoContent, putResponse.StatusCode);

        var getResponse = await _httpClient.GetAsync("/api/items/1");
        var body = await getResponse.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode, body);

        var item = JsonSerializer.Deserialize<ItemDetailsDto>(
            body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.IsNotNull(item);
        Assert.AreEqual(1, item.Id);
        Assert.AreEqual("Updated Seed Item", item.Name);
        Assert.AreEqual(2021, item.ReleaseYear);
        Assert.AreEqual("Updated description", item.Description);
        Assert.AreEqual("book", item.MediaType);
        Assert.AreEqual("updated-image.png", item.Image);
        Assert.AreEqual("English", item.Language);
        Assert.AreEqual("Test Publisher", item.Publisher);

        Assert.IsNotNull(item.BookDetails);
        Assert.AreEqual("978-1-4028-9462-6", item.BookDetails.Isbn);
        Assert.AreEqual(321, item.BookDetails.NoOfPages);
        Assert.AreEqual("2nd", item.BookDetails.Version);
    }

    [TestMethod]
    public async Task Delete_Item_Unknown_ReturnsNotFound()
    {
        var response = await _httpClient.DeleteAsync("/api/items/999");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static string CreateJwt()
    {
        var keyBytes = Encoding.UTF8.GetBytes(JwtKey);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "integration-test-user"),
            new Claim(ClaimTypes.Name, "Integration Test"),
        };

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
