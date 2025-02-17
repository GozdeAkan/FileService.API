using DataAccess.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FileShare = Domain.Entities.FileShare;

namespace DataAccess.Repositories
{
    public class FileShareRepository : BaseRepository<FileShare>, IFileShareRepository
    {
        public FileShareRepository(AppDbContext context) : base(context)
        {
        }
    }
}

