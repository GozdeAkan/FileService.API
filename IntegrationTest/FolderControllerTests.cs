using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using FileService;
using Domain.Entities;
using Business.DTOs.File;
using Business.DTOs.FileShare;
using Business.DTOs.Folder;
using DataAccess.Context;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTest
{
    [TestFixture]
    public class FolderControllerTests
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
            var folder = new Folder
            {
                Id = Guid.NewGuid(),
                Name = "TestFolder",
                OwnerId = "Owner1"
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task GetFolders_ShouldReturnFolders()
        {
            // Act
            var response = await _client.GetAsync("/api/Folder");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var folders = JsonConvert.DeserializeObject<List<Folder>>(responseContent);

            Assert.That(folders, Is.Not.Null);
            Assert.That(folders.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetFolderById_ShouldReturnFolder()
        {
            // Arrange
            var folderId = _context.Folders.First().Id;

            // Act
            var response = await _client.GetAsync($"/api/Folder/{folderId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var folder = JsonConvert.DeserializeObject<Folder>(responseContent);

            Assert.That(folder, Is.Not.Null);
            Assert.That(folder.Id, Is.EqualTo(folderId));
        }

        [Test]
        public async Task CreateFolder_ShouldReturnCreatedFolder()
        {
            // Arrange
            var folderCreateDto = new FolderCreateDto
            {
                Name = "NewTestFolder",
                ParentFolderId = null
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Folder", folderCreateDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }



        [Test]
        public async Task UpdateFolder_ShouldReturnOk()
        {
            // Arrange
            var folderId = _context.Folders.First().Id;
            var folderUpdateDto = new FolderUpdateDto
            {
                Name = "UpdatedTestFolder",
                ParentFolderId = null
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/Folder/{folderId}", folderUpdateDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var updatedFolder = await _context.Folders.FindAsync(folderId);
            Assert.That(updatedFolder.Name, Is.EqualTo("UpdatedTestFolder"));
        }

        [Test]
        public async Task DeleteFolder_ShouldReturnOk()
        {
            // Arrange
            var folderId = _context.Folders.First().Id;

            // Act
            var response = await _client.DeleteAsync($"/api/Folder/{folderId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var deletedFolder = await _context.Folders.FindAsync(folderId);
            Assert.That(deletedFolder, Is.Null);
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
