using DataAccess.Context;
using Domain.Entities;
using File = Domain.Entities.File;

namespace DataAccess.Repositories
{
    public class FileRepository : BaseRepository<File>, IFileRepository
    {
        public FileRepository(AppDbContext context) : base(context)
        {
        }
    }
}
