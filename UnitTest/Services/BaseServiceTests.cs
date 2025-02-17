using Business.Services;
using DataAccess.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using DataAccess.Utils;

namespace UnitTest.Services
{
    public class TestEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class TestCreateDto
    {
        public string Name { get; set; }
    }

    public class TestUpdateDto
    {
        public string Name { get; set; }
    }

    public class TestService : BaseService<TestEntity, TestCreateDto, TestUpdateDto>
    {
        public TestService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override TestEntity MapToEntity(TestCreateDto dto)
        {
            return new TestEntity { Name = dto.Name };
        }

        public override TestEntity MapDtoToEntity(TestUpdateDto dto, TestEntity entity)
        {
            entity.Name = dto.Name;
            return entity;
        }
    }

    [TestFixture]
    public class BaseServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBaseRepository<TestEntity>> _mockRepository;
        private TestService _testService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IBaseRepository<TestEntity>>();
            _mockUnitOfWork.Setup(u => u.GetRepository<TestEntity>()).Returns(_mockRepository.Object);

            _testService = new TestService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity2" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<TestEntity, TestEntity>>>(), null, null, null))
                .ReturnsAsync(entities);

            // Act
            var result = await _testService.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entity = new TestEntity { Id = entityId, Name = "Entity" };

            _mockRepository.Setup(r => r.GetByIdAsync(entityId, null)).ReturnsAsync(entity);

            // Act
            var result = await _testService.GetByIdAsync(entityId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(entityId));
        }

        [Test]
        public void GetByIdAsync_ShouldThrowException_WhenEntityNotFound()
        {
            // Arrange
            var entityId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(entityId, null)).ReturnsAsync((TestEntity)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _testService.GetByIdAsync(entityId));
            Assert.That(ex.Message, Is.EqualTo($"Entity of type {typeof(TestEntity).Name} with ID {entityId} not found."));
        }

        [Test]
        public async Task CreateAsync_ShouldAddEntity()
        {
            // Arrange
            var createDto = new TestCreateDto { Name = "New Entity" };
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = createDto.Name };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<TestEntity>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _testService.CreateAsync(createDto);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<TestEntity>(e => e.Name == createDto.Name)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var updateDto = new TestUpdateDto { Name = "Updated Entity" };
            var entity = new TestEntity { Id = entityId, Name = "Old Entity" };

            _mockRepository.Setup(r => r.GetByIdAsync(entityId, null)).ReturnsAsync(entity);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TestEntity>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _testService.UpdateAsync(entityId, updateDto);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<TestEntity>(e => e.Name == updateDto.Name)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_ShouldThrowException_WhenEntityNotFound()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var updateDto = new TestUpdateDto { Name = "Updated Entity" };

            _mockRepository.Setup(r => r.GetByIdAsync(entityId, null)).ReturnsAsync((TestEntity)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _testService.UpdateAsync(entityId, updateDto));
            Assert.That(ex.Message, Is.EqualTo("Entity not found"));
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entity = new TestEntity { Id = entityId, Name = "Entity" };

            _mockRepository.Setup(r => r.GetByIdAsync(entityId, null)).ReturnsAsync(entity);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<TestEntity>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _testService.DeleteAsync(entityId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(It.Is<TestEntity>(e => e.Id == entityId)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenEntityNotFound()
        {
            // Arrange
            var entityId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(entityId, null)).ReturnsAsync((TestEntity)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _testService.DeleteAsync(entityId));
            Assert.That(ex.Message, Is.EqualTo("Entity not found"));
        }
    }
}

