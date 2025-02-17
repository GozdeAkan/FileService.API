using DataAccess.Context;
using DataAccess.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UnitTest.Repositories
{
    [TestFixture]
    public class BaseRepositoryTests
    {
        private Mock<AppDbContext> _mockContext;
        private BaseRepository<Folder> _baseRepository;
        private Mock<DbSet<Folder>> _mockDbSet;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                  .UseInMemoryDatabase(databaseName: "TestDatabase")
                  .Options;

            _mockContext = new Mock<AppDbContext>(options);
            _mockDbSet = new Mock<DbSet<Folder>>();

            _mockContext.Setup(m => m.Set<Folder>()).Returns(_mockDbSet.Object);

            _baseRepository = new BaseRepository<Folder>(_mockContext.Object);
        }



        [Test]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var folder = new Folder { Name = "Folder1", OwnerId = "Owner1" };

            // Act
            await _baseRepository.AddAsync(folder);

            // Assert
            _mockDbSet.Verify(m => m.AddAsync(folder, default), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var folder = new Folder { Name = "Folder1", OwnerId = "Owner1" };

            // Act
            await _baseRepository.UpdateAsync(folder);

            // Assert
            _mockDbSet.Verify(m => m.Update(folder), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteEntity()
        {
            // Arrange
            var folder = new Folder { Name = "Folder1", OwnerId = "Owner1" };

            // Act
            await _baseRepository.DeleteAsync(folder);

            // Assert
            _mockDbSet.Verify(m => m.Remove(folder), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}
