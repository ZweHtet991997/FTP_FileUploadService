namespace FileUpload_To_FTP_Server.Model
{
    public class RequestModel
    {
        public int ID { get; set; }
        public IFormFile file { get; set; }
    }
}
