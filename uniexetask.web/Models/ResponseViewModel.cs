namespace uniexetask.web.Models
{
    public class ResponseViewModel<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public ResponseViewModel()
        {
            Success = true;
        }
    }
}
