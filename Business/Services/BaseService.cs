using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Business.Contracts;
using DataAccess.Repositories;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Query;

namespace Business.Services
{
    public abstract class BaseService<TEntity, TDto, TUpdateDto> : IBaseService<TEntity, TDto, TUpdateDto>
      where TEntity : class
      where TDto : class
      where TUpdateDto : class?
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<TEntity> _repository;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<TEntity>();
        }
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var entities = await _repository.GetAllAsync(s => s, predicate, include, orderBy);
            return entities.ToList();
        }
        public async Task<TEntity> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var entity = await _repository.GetByIdAsync(id, include);
            if (entity == null)
                throw new Exception($"Entity of type {typeof(TEntity).Name} with ID {id} not found.");
            return entity;
        }
        public async Task CreateAsync(TDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateEntityAsync(TEntity entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, TUpdateDto? dto, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var entity = await _repository.GetByIdAsync(id, include);
            if (entity == null)
                throw new KeyNotFoundException("Entity not found");

            var updatedEntity = MapDtoToEntity(dto, entity);
            await _repository.UpdateAsync(updatedEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateEntityAsync(TEntity entity)
        {
            await _repository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Entity not found");

            await _repository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public abstract TEntity MapToEntity(TDto dto);
        public abstract TEntity MapDtoToEntity(TUpdateDto? dto, TEntity entity);
    }
}
