using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly string _bucketName = "exeunitask.appspot.com";
        private readonly StorageClient _storageClient;
        private readonly IProjectService _projectService;
        public DocumentController(StorageClient storageClient, IProjectService projectService)
        {
            _storageClient = storageClient;
            _projectService = projectService;
        }
        [HttpGet("files/{studentId}")]
        public async Task<IActionResult> GetFiles(int studentId)
        {
            var project = await _projectService.GetProjectByStudentId(studentId);
            if (project == null)
            {
                return NotFound("Project not found for the given student.");
            }

            var prefix = $"Project{project.ProjectId}/";
            var files = new List<string>();

            var storageObjects = _storageClient.ListObjects(_bucketName, prefix).ToList();
            foreach (var obj in storageObjects)
            {
                files.Add(obj.Name);
            }

            return Ok(files);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, int studentId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var project = await _projectService.GetProjectByStudentId(studentId);
            if (project == null)
            {
                return NotFound("Project not found for the given student.");
            }

            var fileName = file.FileName;
            var filePath = $"Project{project.ProjectId}/{fileName}";

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, filePath, file.ContentType, stream);
            }

            return Ok(new { FileName = filePath });
        }
    }
}
