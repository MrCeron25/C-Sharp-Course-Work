using System.IO;

namespace WpfApp1
{
    public static class Saver
    {
        public static void Save(string SaveFolder, string FileName, string SaveData)
        {
            FileName += $".txt";
            string FullPathNewFile = $"{SaveFolder}\\{FileName}";
            using (FileStream fs = new FileStream(FullPathNewFile, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(SaveData);
                }
            }
        }
    }
}
