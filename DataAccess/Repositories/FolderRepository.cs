using DataAccess.Context;
using Domain.Entities;

namespace DataAccess.Repositories
{
    public class FolderRepository : BaseRepository<Folder>, IFolderRepository
    {
        public FolderRepository(AppDbContext context) : base(context)
        {
        }

    }
}
