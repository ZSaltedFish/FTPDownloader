using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FreLib
{
    public class Coding
    {
        public const int BUFFER_SIZE = 1024 * 1024;
        public string Path;

        public Coding(string path)
        {
            Path = path;
        }

        public async void Encoding(string name)
        {
            FileInfo file = new FileInfo(Path);
            string fileName = file.Name;
            Console.WriteLine(fileName);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(fileName);
            byte lengthByte = Convert.ToByte(bytes.Length);
            byte[] buffer = new byte[BUFFER_SIZE];

            using (FileStream stream = new FileStream($"{name}.dat", FileMode.Create))
            {
                stream.WriteByte(lengthByte);
                await stream.WriteAsync(bytes, 0, bytes.Length);

                int readSize = 0;
                using (FileStream readFile = new FileStream(Path, FileMode.Open))
                {
                    do
                    {
                        readSize = await readFile.ReadAsync(buffer, 0, BUFFER_SIZE);
                        await stream.WriteAsync(buffer, 0, readSize);
                    }
                    while (readSize != 0);
                }
            }
        }

        public async Task Decoding()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            using (FileStream reader = new FileStream(Path, FileMode.Open))
            {
                int length = Convert.ToInt32(reader.ReadByte());
                byte[] nameArray = new byte[length];
                await reader.ReadAsync(nameArray, 0, length);

                string name = System.Text.Encoding.UTF8.GetString(nameArray);
                int readSize = 0;
                using (FileStream writer = new FileStream(name, FileMode.Create))
                {
                    do
                    {
                        readSize = await reader.ReadAsync(buffer, 0, BUFFER_SIZE);
                        await writer.WriteAsync(buffer, 0, readSize);
                    }
                    while (readSize != 0);
                }
            }
        }
    }
}
