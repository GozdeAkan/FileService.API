using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DataAccess.Context;
using File = Domain.Entities.File;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTest
{
    [TestFixture]
    public class FileControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private AppDbContext _context;


        [SetUp]
        public void Setup()
        {
            _factory = new MockWebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase("TestDatabase")
                            .Options;

                        _context = new AppDbContext(options);
                        services.AddSingleton(_context);
                    });
                });
            _client = _factory.CreateClient();

            SeedDatabase().Wait();
        }

        private async Task SeedDatabase()
        {
            var file = new File
            {
                Id = Guid.NewGuid(),
                Name = "TestFile.txt",
                BlobStoragePath = "https://blobstorage/testfile.txt",
                CurrentVersion = 1,
                FileType = "text/plain",
                Size = 1024,
                OwnerId = "Owner1"
            };

            _context.Files.Add(file);
            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task GetFiles_ShouldReturnFiles()
        {
            // Act
            var response = await _client.GetAsync("/api/File");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<List<File>>(responseContent);

            Assert.That(files, Is.Not.Null);
            Assert.That(files.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task CreateFile_ShouldReturnOk()
        {
            // Arrange
            var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "File", "NewTestFile.txt");
            formData.Add(new StringContent("NewTestFile.txt"), "Name");

            // Act
            var response = await _client.PostAsync("/api/File", formData);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }


        [Test]
        public async Task GetFileById_ShouldReturnFile()
        {
            // Arrange
            var fileId = _context.Files.First().Id;

            // Act
            var response = await _client.GetAsync($"/api/File/{fileId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var file = JsonConvert.DeserializeObject<File>(responseContent);

            Assert.That(file, Is.Not.Null);
            Assert.That(file.Id, Is.EqualTo(fileId));
        }

        [Test]
        public async Task UpdateFile_ShouldReturnOk()
        {
            // Arrange
            var fileId = _context.Files.First().Id;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("Updated Name"), "Name");

            // Act
            var response = await _client.PutAsync($"/api/File/{fileId}", formData);
            // Assert
            response.EnsureSuccessStatusCode();
            var updatedFile = await _context.Files.FindAsync(fileId);
            Assert.That(updatedFile.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public async Task DeleteFile_ShouldReturnNoContent()
        {
            // Arrange
            var fileId = _context.Files.First().Id;

            // Act
            var response = await _client.DeleteAsync($"/api/File/{fileId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            var deletedFile = await _context.Files.FindAsync(fileId);
            Assert.That(deletedFile, Is.Null);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
