using AutoMapper;
using Business.Contracts;
using Business.DTOs.File;
using DataAccess.Repositories;
using DataAccess.Utils;
using Domain.Entities;
using Infrastructure.BlobStorage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using File = Domain.Entities.File;

namespace Business.Services
{
    public class FileService : BaseService<File, FileCreateDto, FileUpdateDto>, IFileService
    {
        private readonly IMapper _mapper;
        private readonly IFileVersionRepository _fileVersionRepository;
        private readonly IBlobStorageService _blobStorageService; 

        public FileService(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : base(unitOfWork)
        {
            _mapper = mapper;
            _fileVersionRepository = serviceProvider.GetRequiredService<IFileVersionRepository>();
            _blobStorageService = serviceProvider.GetRequiredService<IBlobStorageService>(); 

        }



        public async Task UpdateFileAsync(Guid id, FileUpdateDto fileUpdateDto)
        {

            var file = await GetByIdAsync(id);

            if (fileUpdateDto.File != null)
            {
                var fileVersion = new FileVersion { FileId = file.Id, VersionNumber = file.CurrentVersion, FilePath = file.BlobStoragePath, UpdatedTime = DateTime.Now };

                await _fileVersionRepository.AddAsync(fileVersion);
                var version = await _fileVersionRepository.GetVersionByFileId(id);
                var fileUrl = await _blobStorageService.UploadFileAsync(fileUpdateDto.File);
                file.BlobStoragePath = fileUrl;
                file.CurrentVersion = version;

            }

            file = MapDtoToEntity(fileUpdateDto, file);
           

            await UpdateEntityAsync(file);
        }

        public async Task<List<FileVersion>> GetVersionsById(Guid id)
        {
            var versions = await _fileVersionRepository.FindAsync(s => s.FileId == id);
            return versions.ToList();
        }

  

    public async Task RevertToVersionAsync(Guid id, int versionNumber)
        {
            // Retrieve the specified version of the file
            var version = await _fileVersionRepository.GetByFileIdAndVersionNumberAsync(id, versionNumber);
            if (version == null)
                throw new Exception("Selected version not found!");

            // Retrieve the current file
            var file = await GetByIdAsync(id);
            if (file == null)
                throw new Exception("File not found!");

            // Check if the current version of the file is already backed up
            var existingVersion = await _fileVersionRepository.GetByFileIdAndVersionNumberAsync(file.Id, file.CurrentVersion);
            if (existingVersion == null)
            {
                // Backup the current version if it is not already backed up
                var currentVersionBackup = new FileVersion
                {
                    FileId = file.Id,
                    VersionNumber = file.CurrentVersion,
                    FilePath = file.BlobStoragePath
                };
                await _fileVersionRepository.AddAsync(currentVersionBackup);
            }

            // Revert the file to the specified version
            file.BlobStoragePath = version.FilePath;
            file.CurrentVersion = version.VersionNumber;

            // Update the file entity in the database
            await UpdateEntityAsync(file);
        }

        public override File MapToEntity(FileCreateDto dto)
        {
            return _mapper.Map<File>(dto);
        }

        public override File MapDtoToEntity(FileUpdateDto dto, File entity)
        {
            _mapper.Map(dto, entity);
            return entity;
        }

        public async Task<IActionResult> CreateFileAsync(FileCreateDto fileCreateDto) 
        {
            if (fileCreateDto.File?.Length == 0)
                return new BadRequestObjectResult("File was not uploaded.");

            var entity = MapToEntity(fileCreateDto);
            var fileUrl = await _blobStorageService.UploadFileAsync(fileCreateDto.File);

            entity.BlobStoragePath = fileUrl;
            entity.CurrentVersion = 0;
            entity.FileType = fileCreateDto.File.ContentType;
            entity.Size = fileCreateDto.File.Length;    



            await CreateEntityAsync(entity);

            return new OkResult();
        }
    }
}
