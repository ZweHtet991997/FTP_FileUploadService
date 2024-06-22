using FileUpload_To_FTP_Server.Model;
using Microsoft.AspNetCore.Mvc;
using FileUpload_To_FTP_Server.FTPService;

namespace FileUpload_To_FTP_Server.Controllers
{
    public class FileUploadController : Controller
    {
        private IConfiguration _configuration;
        private FTP_Service _ftpService;

        public FileUploadController(IConfiguration configuration, FTP_Service ftpService)
        {
            _configuration = configuration;
            _ftpService = ftpService;
        }

        [HttpPost]
        [Route("api/upload")]
        public async Task<IActionResult> Upload([FromForm] RequestModel model)
        {
            if (model.file.Length > 0)
            {
                string ftpUrl = _configuration.GetValue<string>("FtpUrl");
                var dataResult = await _ftpService.Upload(model.file, model.ID.ToString(), ftpUrl + model.ID + "/");
                return Ok(dataResult);
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("api/upload")]
        public async Task<IActionResult> DeleteFile(string fileName, int Id)
        {
            string ftpUrl = _configuration.GetValue<string>("FtpUrl");
            var dataResult = _ftpService.DeleteFile(fileName, Id.ToString(), ftpUrl + Id + "/");
            return dataResult ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("api/get")]
        public IActionResult Index()
        {
            return Ok("Hit");
        }
    }
}
