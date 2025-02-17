using AutoMapper;
using Business.DTOs.File;
using Business.Services;
using DataAccess.Repositories;
using DataAccess.Utils;
using Domain.Entities;
using Infrastructure.BlobStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File = Domain.Entities.File;
using FileService = Business.Services.FileService;
using System.IO;
using System.Text;

namespace UnitTest.Services
{
    [TestFixture]
    public class FileServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<IFileVersionRepository> _mockFileVersionRepository;
        private Mock<IBlobStorageService> _mockBlobStorageService;
        private Business.Services.FileService _fileService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockFileVersionRepository = new Mock<IFileVersionRepository>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();

            var mockFileRepository = new Mock<IBaseRepository<File>>();
            _mockUnitOfWork.Setup(u => u.GetRepository<File>()).Returns(mockFileRepository.Object);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IFileVersionRepository))).Returns(_mockFileVersionRepository.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IBlobStorageService))).Returns(_mockBlobStorageService.Object);

            _fileService = new Business.Services.FileService(_mockUnitOfWork.Object, _mockMapper.Object, serviceProvider.Object);
        }

        [Test]
        public async Task CreateFileAsync_ShouldReturnOkResult()
        {
            // Arrange

            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Sample data")), 0, 10, "Data", "testfile.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var fileCreateDto = new FileCreateDto
            {
                Name = "testfile",
                File = file
            };

            var fileEntity = new File
            {
                Id = Guid.NewGuid(),
                Name = fileCreateDto.Name,
                BlobStoragePath = "https://blobstorage/testfile.txt",
                CurrentVersion = 0,
                FileType = fileCreateDto.File.ContentType,
                Size = fileCreateDto.File.Length,
                OwnerId = "1"
            };

            _mockMapper.Setup(m => m.Map<File>(fileCreateDto)).Returns(fileEntity);
            _mockBlobStorageService.Setup(b => b.UploadFileAsync(fileCreateDto.File)).ReturnsAsync(fileEntity.BlobStoragePath);

            // Act
            var result = await _fileService.CreateFileAsync(fileCreateDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task UpdateFileAsync_ShouldUpdateFile()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileUpdateDto = new FileUpdateDto
            {
                Name = "updatedfile",
                File = new FormFile(new MemoryStream(), 0, 10, "Data", "updatedfile.txt")
            };

            var fileEntity = new File
            {
                Id = fileId,
                Name = "testfile",
                BlobStoragePath = "https://blobstorage/testfile.txt",
                CurrentVersion = 0,
                FileType = "text/plain",
                Size = 10,
                OwnerId = "1"
            };

            var fileVersion = new FileVersion
            {
                FileId = fileId,
                VersionNumber = 1,
                FilePath = "https://blobstorage/testfile_v1.txt"
            };

            _mockUnitOfWork.Setup(u => u.GetRepository<File>().GetByIdAsync(fileId, null)).ReturnsAsync(fileEntity);
            _mockFileVersionRepository.Setup(f => f.GetVersionByFileId(fileId)).ReturnsAsync(1);
            _mockBlobStorageService.Setup(b => b.UploadFileAsync(fileUpdateDto.File)).ReturnsAsync("https://blobstorage/updatedfile.txt");

            // Act
            await _fileService.UpdateFileAsync(fileId, fileUpdateDto);

            // Assert
            _mockUnitOfWork.Verify(u => u.GetRepository<File>().UpdateAsync(It.IsAny<File>()), Times.Once);
        }

        [Test]
        public async Task GetVersionsById_ShouldReturnFileVersions()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileVersions = new List<FileVersion>
            {
                new FileVersion { FileId = fileId, VersionNumber = 1, FilePath = "https://blobstorage/testfile_v1.txt" },
                new FileVersion { FileId = fileId, VersionNumber = 2, FilePath = "https://blobstorage/testfile_v2.txt" }
            };

            _mockFileVersionRepository.Setup(f => f.FindAsync(v => v.FileId == fileId, null)).ReturnsAsync(fileVersions);

            // Act
            var result = await _fileService.GetVersionsById(fileId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task RevertToVersionAsync_ShouldRevertToSelectedVersion()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var versionNumber = 1;

            var fileEntity = new File
            {
                Id = fileId,
                Name = "testfile",
                BlobStoragePath = "https://blobstorage/testfile.txt",
                CurrentVersion = 2,
                FileType = "text/plain",
                Size = 10,
                OwnerId = "1"
            };

            var fileVersion = new FileVersion
            {
                FileId = fileId,
                VersionNumber = versionNumber,
                FilePath = "https://blobstorage/testfile_v1.txt"
            };

            _mockUnitOfWork.Setup(u => u.GetRepository<File>().GetByIdAsync(fileId, null)).ReturnsAsync(fileEntity);
            _mockFileVersionRepository.Setup(f => f.GetByFileIdAndVersionNumberAsync(fileId, versionNumber)).ReturnsAsync(fileVersion);

            // Act
            await _fileService.RevertToVersionAsync(fileId, versionNumber);

            // Assert
            _mockUnitOfWork.Verify(u => u.GetRepository<File>().UpdateAsync(It.IsAny<File>()), Times.Once);
        }
    }
}
