using System;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

namespace ProjetoChatCliente
{
    class FileReadWrite
    {
        public static void SaveTextFile(string path, string[] content, bool append)
        {
            try
            {
                if (append)
                    File.AppendAllLines(path, content);
                else
                    File.WriteAllLines(path, content);
            }
            catch (Exception ex){ throw ex;}
        }
       
        public static string[] ReadTextFile(string path)
        {
            string[] contents = null;
            try
            {
                contents = File.ReadAllLines(path);
                return contents;
            }
            catch (Exception ex){ throw ex;}
            return contents;
        }
        
        public static void SaveBinaryFile<T>(string path, T content)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, content);
                }
                catch (Exception ex){ throw ex;}
                finally{ fs.Close();}
            }
                
        }

        public static T ReadBinaryFile<T>(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                T content;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    content = (T)bf.Deserialize(fs);
                }
                catch (Exception ex){ throw ex;}
                finally{ fs.Close();}
                return content;
            }
        }
    }
}
