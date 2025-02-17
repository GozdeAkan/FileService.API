using AutoMapper;
using Business.Contracts;
using Business.DTOs.File;
using Business.DTOs.FileShare;
using DataAccess.Repositories;
using DataAccess.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FileShare = Domain.Entities.FileShare;

namespace Business.Services
{
    public class FileShareService : BaseService<FileShare, FileShareDto, FileShareUpdateDto>, IFileShareService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IFileShareRepository _fileShareRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileShareService(IUnitOfWork unitOfWork,
            IFileRepository fileRepository,
            IFolderRepository folderRepository,
            IFileShareRepository fileShareRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
            _fileShareRepository = fileShareRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> ShareFileAsync(FileShareDto fileShareDto)
        {
            if (fileShareDto.FileId == null && fileShareDto.FolderId == null)
                throw new ArgumentException("Either FileId or FolderId must be provided.");

            if (fileShareDto.FileId != null)
            {
                var file = await _fileRepository.GetByIdAsync(fileShareDto.FileId.Value);
                if (file == null)
                    throw new KeyNotFoundException("File not found");
            }

            if (fileShareDto.FolderId != null)
            {
                var folder = await _folderRepository.GetByIdAsync(fileShareDto.FolderId.Value);
                if (folder == null)
                    throw new KeyNotFoundException("Folder not found");
            }

            string uniqueToken = Guid.NewGuid().ToString("N");

            var sharedItem = MapToEntity(fileShareDto);
            sharedItem.UniqueLinkToken = uniqueToken;

            await _fileShareRepository.AddAsync(sharedItem);

            string _baseUrl = GetBaseUrl();

            return $"{_baseUrl}/api/FileShare/{uniqueToken}"; // It should be frontend link
        }
        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

        public override FileShare MapToEntity(FileShareDto dto)
        {
            return _mapper.Map<FileShare>(dto);
        }

        public override FileShare MapDtoToEntity(FileShareUpdateDto dto, FileShare entity)
        {
            _mapper.Map(dto, entity);
            return entity;
        }

        public async Task<AccessSharedFileResponse> GetSharedItemByTokenAsync(string token)
        {
            var sharedItems = await _fileShareRepository.FindAsync(fs => fs.UniqueLinkToken == token, include: query => query.Include(fs => fs.File).Include(fs => fs.Folder));
            if (sharedItems == null || !sharedItems.Any())
                throw new KeyNotFoundException("Shared item not found");

            var sharedItem = sharedItems.First();

            if (sharedItem.ExpirationDate.HasValue && sharedItem.ExpirationDate.Value < DateTime.UtcNow)
                throw new InvalidOperationException("The shared item has expired");

            List<string> sharedFiles = new();
            if (sharedItem.File != null)
            {
                sharedFiles.Add(sharedItem.File.BlobStoragePath);
            }
            if (sharedItem.FolderId != null)
            {
                var folder = await _folderRepository.GetByIdAsync((Guid)sharedItem.FolderId, include: i => i.Include(f => f.Files));
                sharedFiles.AddRange(folder.Files.Select(s => s.BlobStoragePath));
            }

            return new AccessSharedFileResponse { AccessLevel = sharedItem.AccessLevel, Url = sharedFiles };
        }
    }
}
