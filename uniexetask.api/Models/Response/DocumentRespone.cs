namespace uniexetask.api.Models.Response
{
    public class DocumentRespone
    {
        public int DocumentId { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Url { get; set; } = null!;

        public int UploadBy { get; set; }

        public bool IsFinancialReport { get; set; }

        public string TypeFile { get; set; } = null!;

        public long Size { get; set; } 
    }
}
