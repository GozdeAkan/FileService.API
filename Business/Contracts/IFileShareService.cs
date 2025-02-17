using Business.DTOs.File;
using Business.DTOs.FileShare;
using System;
using System.Threading.Tasks;
using FileShare = Domain.Entities.FileShare;

namespace Business.Contracts
{
    public interface IFileShareService : IBaseService<FileShare, FileShareDto, FileShareUpdateDto>
    {
        Task<string> ShareFileAsync(FileShareDto fileShareDto);

        Task<AccessSharedFileResponse> GetSharedItemByTokenAsync(string token);
    }
}
