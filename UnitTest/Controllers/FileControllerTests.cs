using Business.Contracts;
using Business.DTOs.File;
using Domain.Entities;
using FileService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File = Domain.Entities.File;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class FileControllerTests
    {
        private Mock<IFileService> _mockFileService;
        private FileController _fileController;

        [SetUp]
        public void Setup()
        {
            _mockFileService = new Mock<IFileService>();
            _fileController = new FileController(_mockFileService.Object);
        }

        [Test]
        public async Task GetFiles_ReturnsOkResult_WithListOfFiles()
        {
            // Arrange
            var files = new List<File> { new File { Id = Guid.NewGuid(), Name = "TestFile" } };
            _mockFileService.Setup(service => service.GetAllAsync(null, null, null)).ReturnsAsync(files);

            // Act
            var result = await _fileController.GetFiles();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnValue = okResult.Value as List<File>;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetFile_ReturnsOkResult_WithFile()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var file = new File { Id = fileId, Name = "TestFile" };
            _mockFileService.Setup(service => service.GetByIdAsync(fileId, null)).ReturnsAsync(file);

            // Act
            var result = await _fileController.GetFile(fileId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnValue = okResult.Value as File;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Id, Is.EqualTo(fileId));
        }

        [Test]
        public async Task GetFile_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            _mockFileService.Setup(service => service.GetByIdAsync(fileId, null)).ReturnsAsync((File)null);

            // Act
            var result = await _fileController.GetFile(fileId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateFile_ReturnsOkResult()
        {
            // Arrange
            var fileCreateDto = new FileCreateDto { Name = "TestFile" };

            // Act
            var result = await _fileController.CreateFile(fileCreateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task UpdateFile_ReturnsNoContentResult()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileUpdateDto = new FileUpdateDto { Name = "UpdatedFile" };

            // Act
            var result = await _fileController.UpdateFile(fileId, fileUpdateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task UpdateFile_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileUpdateDto = new FileUpdateDto { Name = "UpdatedFile" };
            _mockFileService.Setup(service => service.UpdateFileAsync(fileId, fileUpdateDto)).Throws<KeyNotFoundException>();

            // Act
            var result = await _fileController.UpdateFile(fileId, fileUpdateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteFile_ReturnsNoContentResult()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            // Act
            var result = await _fileController.DeleteFile(fileId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteFile_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            _mockFileService.Setup(service => service.DeleteAsync(fileId)).Throws<KeyNotFoundException>();

            // Act
            var result = await _fileController.DeleteFile(fileId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetFileVersions_ReturnsOkResult_WithListOfFileVersions()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileVersions = new List<FileVersion> { new FileVersion { FileId = fileId, VersionNumber = 1 } };
            _mockFileService.Setup(service => service.GetVersionsById(fileId)).ReturnsAsync(fileVersions);

            // Act
            var result = await _fileController.GetFileVersions(fileId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnValue = okResult.Value as List<FileVersion>;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Count, Is.EqualTo(1));
        }


        [Test]
        public async Task RevertToFileVersion_ReturnsNoContentResult()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var versionNumber = 1;

            // Act
            var result = await _fileController.RevertToFileVersion(fileId, versionNumber);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task RevertToFileVersion_ReturnsNotFound_WhenFileOrVersionDoesNotExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var versionNumber = 1;
            _mockFileService.Setup(service => service.RevertToVersionAsync(fileId, versionNumber)).Throws<KeyNotFoundException>();

            // Act
            var result = await _fileController.RevertToFileVersion(fileId, versionNumber);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
