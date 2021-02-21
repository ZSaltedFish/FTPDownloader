using FreLib;
using System;
using System.IO;

namespace FreLib
{
    public class EasyDown
    {
        public const string DEFAULT_TEMP_NAME = "tempdata.dat";

        private Action _finishCallbeck;
        private string _IP, _user, _pw, _fileName;

        public EasyDown(string IP, string user, string pw, string fileName, Action callback)
        {
            _finishCallbeck = callback;
            _IP = IP;
            _user = user;
            _fileName = fileName;
            _pw = pw;
        }

        public async void Download()
        {
            MainDownload down = new MainDownload(_IP, _user, _pw);
            Coding coding = new Coding(DEFAULT_TEMP_NAME);
            await down.Download(DEFAULT_TEMP_NAME, $"{_fileName}.dat");
            await coding.Decoding();
            File.Delete(DEFAULT_TEMP_NAME);
            _finishCallbeck?.Invoke();
        }
    }
}
