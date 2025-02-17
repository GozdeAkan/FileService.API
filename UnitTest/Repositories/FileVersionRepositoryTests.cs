using DataAccess.Context;
using DataAccess.Repositories;
using DataAccess.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;


namespace UnitTest.Repositories
{
    [TestFixture]
    public class FileVersionRepositoryTests
    {
        private Mock<AppDbContext> _mockContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBaseRepository<FileVersion>> _mockBaseRepository;
        private FileVersionRepository _fileVersionRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                  .UseInMemoryDatabase(databaseName: "TestDatabase")
                  .Options;

            _mockContext = new Mock<AppDbContext>(options);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBaseRepository = new Mock<IBaseRepository<FileVersion>>();

            _mockUnitOfWork.Setup(u => u.GetRepository<FileVersion>()).Returns(_mockBaseRepository.Object);

            _fileVersionRepository = new FileVersionRepository(_mockContext.Object, _mockUnitOfWork.Object);
        }


        [Test]
        public async Task GetByFileIdAndVersionNumberAsync_ShouldReturnFileVersion()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var versionNumber = 1;
            var fileVersion = new FileVersion { FileId = fileId, VersionNumber = versionNumber, FilePath = "path/to/file" };


            _mockBaseRepository.Setup(r => r.FindAsync(fv => fv.FileId == fileId && fv.VersionNumber == versionNumber, null)).ReturnsAsync(new List<FileVersion>() { fileVersion });

            // Act
            var result = await _fileVersionRepository.GetByFileIdAndVersionNumberAsync(fileId, versionNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileId, Is.EqualTo(fileId));
            Assert.That(result.VersionNumber, Is.EqualTo(versionNumber));
        }

        [Test]
        public async Task GetVersionByFileId_ShouldReturnNextVersionNumber()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var fileVersions = new List<FileVersion>
            {
                new FileVersion { FileId = fileId, VersionNumber = 1 },
                new FileVersion { FileId = fileId, VersionNumber = 2 }
            };

            _mockBaseRepository.Setup(r => r.FindAsync(fv => fv.FileId == fileId, null)).ReturnsAsync(fileVersions);

            // Act
            var result = await _fileVersionRepository.GetVersionByFileId(fileId);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task GetVersionByFileId_ShouldReturnOne_WhenNoVersionsExist()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            _mockBaseRepository.Setup(r => r.FindAsync(fv => fv.FileId == fileId, null)).ReturnsAsync(new List<FileVersion>());

            // Act
            var result = await _fileVersionRepository.GetVersionByFileId(fileId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }
    }
}

