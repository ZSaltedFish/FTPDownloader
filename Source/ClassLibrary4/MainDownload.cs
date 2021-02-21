using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FreLib
{
    public class MainDownload
    {
        public const int BUFFER_SIZE = 1024 * 1024;
        public string Host { get; private set; }

        private string _user, _pass;
        private FtpWebRequest _ftpReq;
        private FtpWebResponse _ftpRep;
        private Stream _ftpStream;

        public bool BinaryMode { get; set; } = true;

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
            Console.WriteLine($"创建URL:ftp://{Host}/{fileName} 成功");
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

        public async void Upload(string filePath, string path)
        {
            try
            {
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    Stream ftpStream = CreateUploadStream(filePath);
                    byte[] bytes = new byte[BUFFER_SIZE];
                    int readSize = 0;
                    do
                    {
                        readSize = await file.ReadAsync(bytes, 0, BUFFER_SIZE);
                        await ftpStream.WriteAsync(bytes, 0, readSize);
                    }
                    while (readSize != 0);
                }
                Console.WriteLine("写入完成");
            }
            catch(Exception err)
            {
                throw err;
            }
            finally
            {
                Close();
            }
        }

        public async Task Download(string filePath, string name)
        {
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Create))
                {
                    Stream ftpStream = await CreateDownloadStream(name);
                    byte[] bytes = new byte[BUFFER_SIZE];
                    int size = 0;
                    do
                    {
                        size = ftpStream.Read(bytes, 0, BUFFER_SIZE);
                        file.Write(bytes, 0, size);
                    }
                    while (size != 0);
                }
            }
            catch (Exception err)
            {
                Close();
                throw err;
            }
        }

        public void Close()
        {
            _ftpStream?.Close();
            _ftpRep?.Close();

            _ftpStream = null;
            _ftpRep = null;
        }

        public MemoryStream GetToMemory(string remodeFile)
        {
            try
            {
                _ftpReq = (FtpWebRequest)WebRequest.Create($"ftp://{Host}/{remodeFile}");
                _ftpReq.Credentials = new NetworkCredential(_user, _pass);
                _ftpReq.UseBinary = BinaryMode;
                _ftpReq.KeepAlive = true;
                _ftpReq.UsePassive = true;
                _ftpReq.Method = WebRequestMethods.Ftp.DownloadFile;

                _ftpRep = (FtpWebResponse)_ftpReq.GetResponse();
                _ftpStream = _ftpRep.GetResponseStream();
                MemoryStream memory = new MemoryStream();
                byte[] bytes = new byte[BUFFER_SIZE];
                try
                {
                    int bytesRead = _ftpStream.Read(bytes, 0, BUFFER_SIZE);
                    memory.Write(bytes, 0, bytesRead);
                }

                catch (Exception err)
                {
                    Console.WriteLine(err);
                }
                finally
                {
                    _ftpStream.Close();
                    _ftpRep.Close();
                    _ftpReq = null;
                }
                memory.Seek(0, SeekOrigin.Begin);
                return memory;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
