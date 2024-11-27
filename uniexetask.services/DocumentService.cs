using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class DocumentService : IDocumentService
    {
        public IUnitOfWork _unitOfWork;
        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Document?> GetDocumentByName(string name)
        {
            return (await _unitOfWork.Documents.GetAsync(d => d.Url == name)).FirstOrDefault();
        }

        public async Task<Document> UploadDocument(Document document)
        {
            await _unitOfWork.Documents.InsertAsync(document);
            _unitOfWork.Save();
            return document;
        }

        public async Task<Document?> DeleteDocumentById(int id)
        {
            var documentToDelete = await _unitOfWork.Documents.GetByIDAsync(id);
            if (documentToDelete == null) return null;

            _unitOfWork.Documents.Delete(documentToDelete);
            _unitOfWork.Save(); 

            return documentToDelete;
        }

        public async Task<Document?> OverWriteDocument(Document document)
        {
            var existedDocument = (await _unitOfWork.Documents.GetAsync(filter: d => d.Url == document.Url)).FirstOrDefault();
            if (existedDocument == null)
                return null;
            existedDocument.ModifiedBy = document.ModifiedBy;
            existedDocument.ModifiedDate = document.ModifiedDate;
            _unitOfWork.Documents.Update(existedDocument);
            _unitOfWork.Save();
            return existedDocument;
        }

        public async Task<Document?> GetDocumentById(int id)
        {
            return await _unitOfWork.Documents.GetByIDAsync(id);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByProjectId(int projectId)
        {
            return await _unitOfWork.Documents.GetAsync(filter: d => d.ProjectId == projectId);
        }
    }
}
