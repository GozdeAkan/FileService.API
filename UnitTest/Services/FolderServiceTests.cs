using AutoMapper;
using Business.DTOs.Folder;
using Business.Services;
using DataAccess.Repositories;
using DataAccess.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using File = Domain.Entities.File;

namespace UnitTest.Services
{
    [TestFixture]
    public class FolderServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private FolderService _folderService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            var mockFolderRepository = new Mock<IBaseRepository<Folder>>();
            _mockUnitOfWork.Setup(u => u.GetRepository<Folder>()).Returns(mockFolderRepository.Object);

            _folderService = new FolderService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetFolderByIdAsync_ShouldReturnFolderWithFiles()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            var folder = new Folder
            {
                Id = folderId,
                Name = "Test Folder",
                Files = new List<File>
                {
                    new File { Id = Guid.NewGuid(), Name = "File1" },
                    new File { Id = Guid.NewGuid(), Name = "File2" }
                }
            };

            _mockUnitOfWork.Setup(u => u.GetRepository<Folder>().GetByIdAsync(folderId, It.IsAny<Func<IQueryable<Folder>, IIncludableQueryable<Folder, object>>>())).ReturnsAsync(folder);

            // Act
            var result = await _folderService.GetFolderByIdAsync(folderId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(folderId));
            Assert.That(result.Files.Count, Is.EqualTo(2));
        }

        [Test]
        public void MapToEntity_ShouldMapFolderCreateDtoToFolder()
        {
            // Arrange
            var folderCreateDto = new FolderCreateDto
            {
                Name = "New Folder"
            };

            var folder = new Folder
            {
                Id = Guid.NewGuid(),
                Name = folderCreateDto.Name
            };

            _mockMapper.Setup(m => m.Map<Folder>(folderCreateDto)).Returns(folder);

            // Act
            var result = _folderService.MapToEntity(folderCreateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(folderCreateDto.Name));
        }

    
    }
}
