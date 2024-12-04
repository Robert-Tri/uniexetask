using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetDocumentsByProjectId(int projectId);
        Task<Document> UploadDocument(Document document);
        Task<Document?> GetDocumentByName(string name);
        Task<Document?> DeleteDocumentById(int id);
        Task<Document?> GetDocumentById(int id);
        Task<Document?> OverWriteDocument(Document document);
    }
}
