using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FreLib
{
    public class MainDownload
    {
        public enum ActionResult
        {
            Error,
            DownloadFinished,
            UploadFinished,
        }
        public const int BUFFER_SIZE = 1024 * 1024;
        public string Host { get; private set; }
        public Action<ActionResult, string> FinishCallback;

        #region 私有变量
        private string _user, _pass;
        private FtpWebRequest _ftpReq;
        private FtpWebResponse _ftpRep;
        private Stream _ftpStream;
        private long _fileSize, _nowSize;
        #endregion

        public bool BinaryMode { get; set; } = true;
        public bool IsRunning { get; private set; } = false;
        public int Proc
        {
            get
            {
                double d = _nowSize / (double)_fileSize;
                return (int)(d * 100);
            }
        }

        public static MainDownload Instance { get; } = new MainDownload(@"34.84.177.172", "newftpuser", "showme");

        public MainDownload(string hostIP, string user, string pass)
        {
            _user = user;
            _pass = pass;
            Host = hostIP;
        }

        private async Task<Stream> CreateDownloadStream(string fileName)
        {
            _ftpReq = (FtpWebRequest)WebRequest.Create($"ftp://{Host}/{fileName}");
            _ftpReq.Credentials = new NetworkCredential(_user, _pass);
            _ftpReq.UseBinary = BinaryMode;
            _ftpReq.KeepAlive = false;
            _ftpReq.ReadWriteTimeout = 12000;
            _ftpReq.Timeout = -1;
            _ftpReq.UsePassive = true;
            _ftpReq.Method = Ftp.DownloadFile;

            _ftpRep = (await _ftpReq.GetResponseAsync()) as FtpWebResponse;
            _ftpStream = _ftpRep.GetResponseStream();
            return _ftpStream;
        }

        private Stream CreateUploadStream(string fileName)
        {
            _ftpReq = (FtpWebRequest)WebRequest.Create($"ftp://{Host}/{fileName}");
            _ftpReq.Credentials = new NetworkCredential(_user, _pass);
            _ftpReq.UseBinary = BinaryMode;
            _ftpReq.KeepAlive = false;
            _ftpReq.ReadWriteTimeout = 12000;
            _ftpReq.Timeout = -1;
            _ftpReq.UsePassive = true;
            _ftpReq.Method = Ftp.UploadFile;
            _ftpStream = _ftpReq.GetRequestStream();
            return _ftpStream;
        }

        #region 上传与下载
        public async void Upload(string filePath, string path)
        {
            try
            {
                IsRunning = true;
                _nowSize = 0;
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    _fileSize = _ftpReq.ContentLength;
                    Stream ftpStream = CreateUploadStream(filePath);
                    byte[] bytes = new byte[BUFFER_SIZE];
                    int readSize = 0;
                    do
                    {
                        readSize = await file.ReadAsync(bytes, 0, BUFFER_SIZE);
                        _nowSize += readSize;
                        await ftpStream.WriteAsync(bytes, 0, readSize);
                    }
                    while (readSize != 0);
                }
                FinishCallback?.Invoke(ActionResult.UploadFinished, "");
            }
            catch(Exception err)
            {
                FinishCallback?.Invoke(ActionResult.Error, err.ToString());
            }
            finally
            {
                Close();
                IsRunning = false;
            }
        }

        public async Task<bool> Download(string filePath, string downName)
        {
            try
            {
                _nowSize = 0;
                IsRunning = true;
                using (FileStream file = new FileStream(filePath, FileMode.Create))
                {
                    Stream ftpStream = await CreateDownloadStream(downName);
                    _fileSize = _ftpRep.ContentLength;
                    byte[] bytes = new byte[BUFFER_SIZE];
                    int size = 0;
                    do
                    {
                        size = ftpStream.Read(bytes, 0, BUFFER_SIZE);
                        _nowSize += size;
                        file.Write(bytes, 0, size);
                    }
                    while (size != 0);
                    FinishCallback?.Invoke(ActionResult.DownloadFinished, "");
                    return true;
                }
            }
            catch (Exception err)
            {
                FinishCallback?.Invoke(ActionResult.Error, err.ToString());
                return false;
            }
            finally
            {
                Close();
                IsRunning = false;
            }
        }

        public void Close()
        {
            _ftpStream?.Close();
            _ftpRep?.Close();

            _ftpStream = null;
            _ftpRep = null;
        }
        #endregion
    }
}
