using AutoMapper;
using Business.Contracts;
using Business.DTOs.File;
using Business.DTOs.FileShare;
using Business.Services;
using DataAccess.Repositories;
using DataAccess.Utils;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileShare = Domain.Entities.FileShare;
using File = Domain.Entities.File;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Domain.Enums;

namespace UnitTest.Services
{
    [TestFixture]
    public class FileShareServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IFileRepository> _mockFileRepository;
        private Mock<IFolderRepository> _mockFolderRepository;
        private Mock<IFileShareRepository> _mockFileShareRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private FileShareService _fileShareService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFileRepository = new Mock<IFileRepository>();
            _mockFolderRepository = new Mock<IFolderRepository>();
            _mockFileShareRepository = new Mock<IFileShareRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _fileShareService = new FileShareService(
                _mockUnitOfWork.Object,
                _mockFileRepository.Object,
                _mockFolderRepository.Object,
                _mockFileShareRepository.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Test]
        public async Task ShareFileAsync_ShouldReturnSharedLink()
        {
            // Arrange
            var fileShareDto = new FileShareDto { FileId = Guid.NewGuid(), SharedToEmail = "test@example.com" };
            var fileShare = new FileShare { UniqueLinkToken = Guid.NewGuid().ToString("N") };
            _mockFileRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<File>, IIncludableQueryable<File, object>>>())).ReturnsAsync(new File { Id = Guid.NewGuid(), Name = "TestFile", FileType = "txt", Size = 1024, OwnerId = "ownerId", BlobStoragePath = "path/to/blob", CurrentVersion = 1 });

            _mockMapper.Setup(m => m.Map<FileShare>(fileShareDto)).Returns(fileShare);
            _mockHttpContextAccessor.Setup(h => h.HttpContext.Request.Scheme).Returns("https");
            _mockHttpContextAccessor.Setup(h => h.HttpContext.Request.Host).Returns(new HostString("localhost"));
            _mockHttpContextAccessor.Setup(h => h.HttpContext.Request.PathBase).Returns("");

            // Act
            var result = await _fileShareService.ShareFileAsync(fileShareDto);

            // Assert
            Assert.That(result, Is.EqualTo($"https://localhost/api/FileShare/{fileShare.UniqueLinkToken}"));
        }

        [Test]
        public void ShareFileAsync_ShouldThrowArgumentException_WhenFileIdAndFolderIdAreNull()
        {
            // Arrange
            var fileShareDto = new FileShareDto { SharedToEmail = "test@example.com" };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _fileShareService.ShareFileAsync(fileShareDto));
        }

        [Test]
        public void ShareFileAsync_ShouldThrowKeyNotFoundException_WhenFileNotFound()
        {
            // Arrange
            var fileShareDto = new FileShareDto { FileId = Guid.NewGuid(), SharedToEmail = "test@example.com" };
            _mockFileRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<File>, IIncludableQueryable<File, object>>>())).ReturnsAsync((File?)null);


            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _fileShareService.ShareFileAsync(fileShareDto));
        }

        [Test]
        public void ShareFileAsync_ShouldThrowKeyNotFoundException_WhenFolderNotFound()
        {
            // Arrange
            var fileShareDto = new FileShareDto { FolderId = Guid.NewGuid(), SharedToEmail = "test@example.com" };
            _mockFolderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Folder>, IIncludableQueryable<Folder, object>>>())).ReturnsAsync((Folder)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _fileShareService.ShareFileAsync(fileShareDto));
        }

        [Test]
        public async Task GetSharedItemByTokenAsync_ShouldReturnSharedItem()
        {
            // Arrange
            var token = "testtoken";
            var fileShare = new FileShare
            {
                UniqueLinkToken = token,
                AccessLevel = AccessLevel.View,
                File = new File { BlobStoragePath = "file/path" },
                Folder = new Folder { Files = new List<File> { new File { BlobStoragePath = "folder/file/path" } } }
            };

            _mockFileShareRepository
            .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FileShare, bool>>>(), It.IsAny<Func<IQueryable<FileShare>, IIncludableQueryable<FileShare, object>>>()))
            .ReturnsAsync(new List<FileShare> { fileShare });

            // Act
            var result = await _fileShareService.GetSharedItemByTokenAsync(token);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessLevel, Is.EqualTo(AccessLevel.View));
            Assert.That(result.Url, Does.Contain("file/path"));
        }

        [Test]
        public void GetSharedItemByTokenAsync_ShouldThrowKeyNotFoundException_WhenSharedItemNotFound()
        {
            // Arrange
            var token = "InvalidToken";
            _mockFileShareRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FileShare, bool>>>(), It.IsAny<Func<IQueryable<FileShare>, IIncludableQueryable<FileShare, object>>>())).ReturnsAsync((IEnumerable<FileShare>)null);
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _fileShareService.GetSharedItemByTokenAsync(token));
        }
    }
}

