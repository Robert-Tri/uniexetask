namespace uniexetask.shared.Models.Response
{
    public class DocumentRespone
    {
        public int DocumentId { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Url { get; set; } = null!;

        public string? UploadBy { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long Size { get; set; } 
    }
}
