using Business.Contracts;
using Business.DTOs.File;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using File = Domain.Entities.File;

namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // GET: api/File
        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles()
        {
            var files = await _fileService.GetAllAsync();
            return Ok(files);
        }

        // GET: api/File/5
        [HttpGet("{id}")]
        public async Task<ActionResult<File>> GetFile(Guid id)
        {
            var file = await _fileService.GetByIdAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        // POST: api/File
        [HttpPost]
        public async Task<IActionResult> CreateFile(FileCreateDto fileCreateDto)
        {
            await _fileService.CreateFileAsync(fileCreateDto); 
            return Ok();
        }

        // PUT: api/File/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(Guid id, FileUpdateDto fileUpdateDto)
        {
            try
            {
                await _fileService.UpdateFileAsync(id, fileUpdateDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/File/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            try
            {
                await _fileService.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
        // GET: api/File/5/versions
        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<FileVersion>>> GetFileVersions(Guid id)
        {
            var fileVersions = await _fileService.GetVersionsById(id);

            return Ok(fileVersions);
        }
        // POST: api/File/5/revert
        [HttpPost("{id}/revert")]
        public async Task<IActionResult> RevertToFileVersion(Guid id, [FromBody] int versionNumber)
        {
            try
            {
                await _fileService.RevertToVersionAsync(id, versionNumber);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        
    }
}
