using Business.Contracts;
using Business.DTOs.Folder;
using Domain.Entities;
using FileService.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class FolderControllerTests
    {
        private Mock<IFolderService> _mockFolderService;
        private FolderController _folderController;

        [SetUp]
        public void Setup()
        {
            _mockFolderService = new Mock<IFolderService>();
            _folderController = new FolderController(_mockFolderService.Object);
        }

        [Test]
        public async Task GetFolders_ReturnsOkResult_WithListOfFolders()
        {
            // Arrange
            var folders = new List<Folder> { new Folder { Id = Guid.NewGuid(), Name = "TestFolder" } };
            _mockFolderService.Setup(service => service.GetAllAsync(null, null, null)).ReturnsAsync(folders);

            // Act
            var result = await _folderController.GetFolders();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnValue = okResult.Value as List<Folder>;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetFolder_ReturnsOkResult_WithFolder()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            var folder = new Folder { Id = folderId, Name = "TestFolder" };
            _mockFolderService.Setup(service => service.GetFolderByIdAsync(folderId)).ReturnsAsync(folder);

            // Act
            var result = await _folderController.GetFolder(folderId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnValue = okResult.Value as Folder;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Id, Is.EqualTo(folderId));
        }

        [Test]
        public async Task GetFolder_ReturnsNotFound_WhenFolderDoesNotExist()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            _mockFolderService.Setup(service => service.GetFolderByIdAsync(folderId)).ReturnsAsync((Folder)null);

            // Act
            var result = await _folderController.GetFolder(folderId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateFolder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var folderCreateDto = new FolderCreateDto { Name = "TestFolder" };
            var folder = new Folder { Id = Guid.NewGuid(), Name = "TestFolder" };
            _mockFolderService.Setup(service => service.CreateAsync(folderCreateDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _folderController.CreateFolder(folderCreateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task UpdateFolder_ReturnsOk()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            var folderUpdateDto = new FolderUpdateDto { Name = "UpdatedFolder" };

            // Act
            var result = await _folderController.UpdateFolder(folderId, folderUpdateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task UpdateFolder_ReturnsNotFound_WhenFolderDoesNotExist()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            var folderUpdateDto = new FolderUpdateDto { Name = "UpdatedFolder" };
            _mockFolderService.Setup(service => service.UpdateAsync(It.IsAny<Guid>(), It.Is<FolderUpdateDto>(dto => dto != null), null)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _folderController.UpdateFolder(folderId, folderUpdateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteFolder_ReturnsOkResult()
        {
            // Arrange
            var folderId = Guid.NewGuid();

            // Act
            var result = await _folderController.DeleteFolder(folderId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task DeleteFolder_ReturnsNotFound_WhenFolderDoesNotExist()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            _mockFolderService.Setup(service => service.DeleteAsync(folderId)).Throws<KeyNotFoundException>();

            // Act
            var result = await _folderController.DeleteFolder(folderId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
