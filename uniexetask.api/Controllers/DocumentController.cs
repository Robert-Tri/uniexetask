using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
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
        private readonly IDocumentService _documentService;
        public DocumentController(StorageClient storageClient, IProjectService projectService, IDocumentService documentService)
        {
            _storageClient = storageClient;
            _projectService = projectService;
            _documentService = documentService;
        }
        [HttpGet("documents/{studentId}")]
        public async Task<IActionResult> GetDocuments(int studentId)
        {
            var project = await _projectService.GetProjectByStudentId(studentId);
            if (project == null)
            {
                return NotFound("Project not found for the given student.");
            }
            var documents = await _documentService.GetDocumentsByProjectId(project.ProjectId);
            ApiResponse<IEnumerable<Document>> respone = new ApiResponse<IEnumerable<Document>>();
            respone.Data = documents;
            return Ok(respone);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file, int studentId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var project = await _projectService.GetProjectByStudentId(studentId);
            if (project == null)
                return NotFound("Project not found for the given student.");

            var existedDocument = await _documentService.GetDocumentByName($"Project{project.ProjectId}/{file.FileName}");
            if(existedDocument != null)
                return Conflict(new { Message = "Document with the same name already exists." });

            var document = await _documentService.UploadDocument(new core.Models.Document
            {
                Name = file.FileName,
                ProjectId = project.ProjectId,
                Type = "Status 1",
                Url = $"Project{project.ProjectId}/{file.FileName}",
                UploadBy = studentId,
                IsFinancialReport = false,
            });

            var fileName = file.FileName;
            var filePath = document.Url;

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, filePath, file.ContentType, stream);
            }
            ApiResponse<Document> respone = new ApiResponse<Document>();
            respone.Data = document;

            return Ok(respone);
        }
        [HttpGet("download")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var document = await _documentService.GetDocumentById(documentId);
            if (document == null)
                return NotFound("Document not found.");

            await _storageClient.GetObjectAsync(_bucketName, document.Url);

            var credential = GoogleCredential.FromFile(
                Path.Combine(Directory.GetCurrentDirectory(), "exeunitask-firebase-adminsdk-3jz7t-66373e3f35.json")
            ).UnderlyingCredential as ServiceAccountCredential;

            if (credential == null)
                return StatusCode(500, "Failed to load service account credentials.");

            var signedUrl = UrlSigner.FromCredential(credential).Sign(
                _bucketName,
                document.Url,
                TimeSpan.FromHours(1),
                HttpMethod.Get
            );

            return Ok(new { Url = signedUrl });
        }


        [HttpDelete("documentid")]
        public async Task<IActionResult> DeleteFile(int documentId)
        {
            var documentToDelete = await _documentService.DeleteDocumentById(documentId);
            if (documentToDelete == null)
            {
                return NotFound("Document not found.");
            }

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, documentToDelete.Url);
                return Ok(new { Message = "File deleted successfully" });
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return NotFound("File not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }
    }
}
