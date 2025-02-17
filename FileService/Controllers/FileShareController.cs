using Business.Contracts;
using Business.DTOs.File;
using Business.DTOs.FileShare;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileShareController : ControllerBase
    {
        private readonly IFileShareService _fileShareService;
        private readonly IFolderService _folderService;

        public FileShareController(IFileShareService fileShareService, IFolderService folderService)
        {
            _fileShareService = fileShareService;
            _folderService = folderService;
        }

        // POST: api/FileShare/share
        [HttpPost("share")]
        public async Task<IActionResult> ShareFile([FromBody] FileShareDto fileShareDto)
        {
            try
            {
                var result = await _fileShareService.ShareFileAsync(fileShareDto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpGet("{token}")]
        public async Task<IActionResult> AccessSharedFile(string token)
        {
            AccessSharedFileResponse sharedItems = new();
            try
            {
                sharedItems = await _fileShareService.GetSharedItemByTokenAsync(token);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("The shared link is invalid.");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("This shared link has expired.");
            }

            return Ok(sharedItems);
        }

       
    }
}
