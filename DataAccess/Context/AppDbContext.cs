using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using File = Domain.Entities.File;
using FileShare = Domain.Entities.FileShare;

namespace DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DbSet<FileShare> FileShares { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await OnBeforeSaving();
            // Save current entity
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            return result;
        }

        private async Task OnBeforeSaving()
        {
            ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries();
            DateTime utcTime = DateTime.UtcNow;

            string currentUserId = WorkContext.CurrentUserId;
            foreach (var entry in entries)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entity.UpdatedTime = utcTime;
                            entity.UpdatedBy = currentUserId;
                            break;

                        case EntityState.Added:
                            entity.CreatedTime = utcTime;
                            entity.CreatedBy = currentUserId;
                            break;
                    }


                }
            }
        }
    }


}
