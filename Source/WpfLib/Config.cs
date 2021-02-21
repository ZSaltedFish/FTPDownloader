using System;

namespace WpfLib
{
    public class Config
    {
        public int DLLRelease;
        public int VP;
        public string IPAddress;

        public Config(string data)
        {
            Config con = ReadFrom(data);
            DLLRelease = con.DLLRelease;
            VP = con.VP;
            IPAddress = con.IPAddress;
        }

        private Config ()
        {
        }

        public bool IsNew(string comdata)
        {
            Config newConfig = ReadFrom(comdata);
            return newConfig > this;
        }

        public override string ToString()
        {
            return $"{DLLRelease}.{VP}\n{IPAddress}";
        }

        public void Update(string data)
        {
            Config con = ReadFrom(data);
            DLLRelease = con.DLLRelease;
            VP = con.VP;
            IPAddress = con.IPAddress;
        }

        public static bool operator >(Config a, Config b)
        {
            if (a.DLLRelease > b.DLLRelease)
            {
                return true;
            }

            if (a.VP > b.VP)
            {
                return true;
            }

            return false;
        }

        public static  bool operator < (Config a, Config b)
        {
            if (a.DLLRelease > b.DLLRelease)
            {
                return false;
            }

            if (a.VP > b.VP)
            {
                return false;
            }

            return true;
        }

        public static Config ReadFrom(string data)
        {
            string[] dataPic = data.Split('\n');
            string[] versionDatas = dataPic[0].Split('.');
            Config config = new Config
            {
                IPAddress = dataPic[1],
                DLLRelease = int.Parse(versionDatas[0]),
                VP = int.Parse(versionDatas[1])
            };
            return config;
        }
    }
}
