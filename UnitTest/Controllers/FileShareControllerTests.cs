using Business.Contracts;
using Business.DTOs.FileShare;
using Domain.Entities;
using FileService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileShare = Domain.Entities.FileShare;
using File = Domain.Entities.File;
using Domain.Enums;
using Business.DTOs.File;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class FileShareControllerTests
    {
        private Mock<IFileShareService> _mockFileShareService;
        private Mock<IFolderService> _mockFolderService;
        private FileShareController _fileShareController;

        [SetUp]
        public void Setup()
        {
            _mockFileShareService = new Mock<IFileShareService>();
            _mockFolderService = new Mock<IFolderService>();
            _fileShareController = new FileShareController(_mockFileShareService.Object, _mockFolderService.Object);
        }

        [Test]
        public async Task ShareFile_ReturnsOkResult_WithShareFileResult()
        {
            // Arrange
            var fileShareDto = new FileShareDto { FileId = Guid.NewGuid(), SharedToEmail = "test@example.com" };
            var shareResult = "SharedFileToken";
            _mockFileShareService.Setup(service => service.ShareFileAsync(fileShareDto)).ReturnsAsync(shareResult);

            // Act
            var result = await _fileShareController.ShareFile(fileShareDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.EqualTo(shareResult));
        }

        [Test]
        public async Task ShareFile_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            var fileShareDto = new FileShareDto { FileId = Guid.NewGuid(), SharedToEmail = "test@example.com" };
            _mockFileShareService.Setup(service => service.ShareFileAsync(fileShareDto)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _fileShareController.ShareFile(fileShareDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task AccessSharedFile_ReturnsOkResult_WithAccessSharedFileResponse()
        {
            // Arrange
            var token = "SharedFileToken";
            var sharedItem = new AccessSharedFileResponse
            {
                AccessLevel = AccessLevel.View,
                Url = new List<string> { "file/path" }
            };
            _mockFileShareService.Setup(service => service.GetSharedItemByTokenAsync(token)).ReturnsAsync(sharedItem);

            // Act
            var result = await _fileShareController.AccessSharedFile(token);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnValue = okResult.Value as AccessSharedFileResponse;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.AccessLevel, Is.EqualTo(sharedItem.AccessLevel));
            Assert.That(returnValue.Url, Is.EquivalentTo(new List<string> { "file/path" }));
        }

    
    }
}
