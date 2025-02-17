using AutoMapper;
using Business.Contracts;
using Business.DTOs.Folder;
using DataAccess.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class FolderService : BaseService<Folder, FolderCreateDto, FolderUpdateDto>, IFolderService
    {
        private readonly IMapper _mapper;

        public FolderService(IUnitOfWork unitOfWork, IMapper mapper):base(unitOfWork)
        {
            _mapper = mapper;
        }

        public Task<Folder> GetFolderByIdAsync(Guid Id)
        {
            return GetByIdAsync(Id, include: inc => inc.Include(f => f.Files).Include(f => f.ParentFolder));
        }
        public override Folder MapToEntity(FolderCreateDto dto)
        {
            return _mapper.Map<Folder>(dto);
        }

        public override Folder MapDtoToEntity(FolderUpdateDto dto, Folder entity)
        {
            entity = _mapper.Map(dto, entity);
            return entity;
        }

      
    }
}
