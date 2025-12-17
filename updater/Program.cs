
namespace updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string updateSourceLocation = RegistryManagement.ReadStringRegistryKey("UpdateLocation");
            string localPath = RegistryManagement.ReadStringRegistryKey("InstallLocation");
            Console.WriteLine("Production Information System Updater");

            if (string.IsNullOrEmpty(updateSourceLocation) || string.IsNullOrEmpty(localPath))
            {
                Console.WriteLine("Hiányos beállítások. Futtassa a PIS-t és pótolja a hiányzó beállításokat!");
            }
            else
            {
                try
                {
                    CopyDirectory(updateSourceLocation, localPath);
                    Console.WriteLine("Sikeres frissítés!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sikertelen frissítés a következő hiba miatt : {ex.Message}");
                }

            }

            Console.ReadLine();
        }

        public static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = true)
        {
            // Ha a forrás nem létezik, hiba
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"A forráskönyvtár nem található: {sourceDir}");

            // Létrehozza a cél könyvtárat, ha nem létezik
            Directory.CreateDirectory(destinationDir);

            // Másolja a fájlokat
            foreach (var filePath in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(filePath);
                var destFile = Path.Combine(destinationDir, fileName);
                if (!fileName.Contains("updater"))
                {
                    File.Copy(filePath, destFile, overwrite);
                    Console.WriteLine($"Másolás ... {fileName}");
                }
            }

            // Rekurzívan másolja az almappákat
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destinationDir, dirName);
                CopyDirectory(subDir, destSubDir, overwrite);
            }
        }
    }
}

