using Business.DTOs.File;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using File = Domain.Entities.File;

namespace Business.Contracts
{
    public interface IFileService : IBaseService<File, FileCreateDto, FileUpdateDto>
    {
        Task UpdateFileAsync(Guid id, FileUpdateDto fileUpdateDto);
        Task<IActionResult> CreateFileAsync(FileCreateDto fileCreateDto);

        Task<List<FileVersion>> GetVersionsById(Guid id);
        Task RevertToVersionAsync(Guid id, int versionNumber);
    }
}
