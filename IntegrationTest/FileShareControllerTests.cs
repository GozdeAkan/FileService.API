using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using FileService;
using Domain.Entities;
using Business.DTOs.FileShare;
using DataAccess.Context;
using Microsoft.Extensions.DependencyInjection;
using File = Domain.Entities.File;
using Domain.Enums;
using FileShare = Domain.Entities.FileShare;
using Business.DTOs.File;

namespace FileService.IntegrationTest
{
    [TestFixture]
    public class FileShareControllerTests
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
        public async Task ShareFile_ShouldReturnOk()
        {
            // Arrange
            var fileId = _context.Files.First().Id;
            var fileShareDto = new FileShareDto
            {
                FileId = fileId,
                SharedToEmail = "test@example.com",
                AccessLevel = AccessLevel.View,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/FileShare/share", fileShareDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.That(responseContent, Is.Not.Null);
        }

        [Test]
        public async Task AccessSharedFile_ShouldReturnFile()
        {
            // Arrange
            var fileId = _context.Files.First().Id;
            var fileShare = new FileShare
            {
                FileId = fileId,
                OwnerUserId = "Owner1",
                SharedToEmail = "test@example.com",
                UniqueLinkToken = "unique-token",
                AccessLevel = AccessLevel.View,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };

            _context.FileShares.Add(fileShare);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/FileShare/{fileShare.UniqueLinkToken}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AccessSharedFileResponse>(responseContent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.Not.Empty);
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
