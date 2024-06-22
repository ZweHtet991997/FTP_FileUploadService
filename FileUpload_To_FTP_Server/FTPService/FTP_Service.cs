using System.Net;

namespace FileUpload_To_FTP_Server.FTPService
{
    public class FTP_Service
    {
        private IConfiguration _configuration;
        private HttpRequest httpRequest;

        public FTP_Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Upload(IFormFile file, string ID, string ftpUrl)
        {
            try
            {
                string fileName = string.Empty;
                if (!CheckDirectoryExist(ftpUrl))
                {
                    CreateDirectory(ID, ftpUrl);
                    fileName = await UploadFileToFTP(file, ftpUrl + file.FileName);
                }
                else
                {
                    fileName = await UploadFileToFTP(file, ftpUrl + file.FileName);
                }
                return fileName;
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                throw ex;
            }
        }

        public void CreateDirectory(string ID, string ftpUrl)
        {
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
                ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpWebRequest.EnableSsl = true;
                ftpWebRequest.Credentials = new NetworkCredential(_configuration.GetValue<string>("FtpUserName"),
                    _configuration.GetValue<string>("FtpPassword"));

                using (var response = (FtpWebResponse)ftpWebRequest.GetResponse())
                {
                    Console.WriteLine(response.StatusCode.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckDirectoryExist(string ftpUrl)
        {
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpWebRequest.EnableSsl = true;
                ftpWebRequest.Credentials = new NetworkCredential(_configuration.GetValue<string>("FtpUserName"),
                    _configuration.GetValue<string>("FtpPassword"));
                FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();
                return true;

            }
            catch (WebException ex)
            {
                bool isExist = false;
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return isExist;
                    }
                }
                return isExist;
            }
        }

        public async Task<string> UploadFileToFTP(IFormFile file, string ftpUrl)
        {
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
                ftpWebRequest.Credentials = new NetworkCredential(_configuration.GetValue<string>("FtpUserName"),
                _configuration.GetValue<string>("FtpPassword"));
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpWebRequest.EnableSsl = true;

                using (Stream ftpStream = ftpWebRequest.GetRequestStream())
                {
                    file.CopyTo(ftpStream);
                }
                return file.FileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteFile(string fileName, string ID, string ftpUrl)
        {
            try
            {
                ftpUrl = ftpUrl + fileName;
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
                ftpWebRequest.Credentials = new NetworkCredential(_configuration.GetValue<string>("FtpUserName"),
                    _configuration.GetValue<string>("FtpPassword"));
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpWebRequest.EnableSsl = true;
                FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();

                if (response.StatusCode == FtpStatusCode.FileActionOK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
