using Domain.Entities;

namespace DataAccess.Repositories
{
    public interface IFileVersionRepository : IBaseRepository<FileVersion>
    {
        Task<int> GetVersionByFileId(Guid fileId);
        Task<FileVersion> GetByFileIdAndVersionNumberAsync(Guid fileId, int versionNumber);
    }
}
