using DataAccess.Context;
using DataAccess.Utils;
using Domain.Entities;

namespace DataAccess.Repositories
{
    public class FileVersionRepository : BaseRepository<FileVersion>, IFileVersionRepository
    {
        private readonly IBaseRepository<FileVersion> _fileVersionRepository;
        public FileVersionRepository(AppDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _fileVersionRepository = unitOfWork.GetRepository<FileVersion>();
        }

        public async Task<FileVersion> GetByFileIdAndVersionNumberAsync(Guid fileId, int versionNumber)
        {
            var fileVersions = await _fileVersionRepository.FindAsync(fv => fv.FileId == fileId && fv.VersionNumber == versionNumber);
            return fileVersions.FirstOrDefault();
        }

        public async Task<int> GetVersionByFileId(Guid fileId)
        {
            var file = await _fileVersionRepository.FindAsync(s =>s.FileId == fileId);
            return file.Count() == 0 ? 0 : file.Max(s => s.VersionNumber) + 1;
        }
    }
}
