using Business.Contracts;
using Business.DTOs.Folder;
using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        // GET: api/Folder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Folder>>> GetFolders()
        {
            var folders = await _folderService.GetAllAsync();
            return Ok(folders);
        }

        // GET: api/Folder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Folder>> GetFolder(Guid id)
        {
            var folder = await _folderService.GetFolderByIdAsync(id);
            return folder is not null ? Ok(folder) : NotFound();
        }

        // POST: api/Folder
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] FolderCreateDto folderCreateDto) 
        {
            await _folderService.CreateAsync(folderCreateDto);
            return Ok();
        }

        // PUT: api/Folder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFolder(Guid id, [FromBody] FolderUpdateDto folderUpdateDto)
        {
            try
            {
                await _folderService.UpdateAsync(id, folderUpdateDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        // DELETE: api/Folder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(Guid id)
        {
            try
            {
                await _folderService.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
