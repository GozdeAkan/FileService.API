using Business.DTOs.Folder;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Business.Contracts
{
    public interface IFolderService: IBaseService<Folder, FolderCreateDto, FolderUpdateDto> 
    {
        Task<Folder> GetFolderByIdAsync(Guid Id);
    }
}
