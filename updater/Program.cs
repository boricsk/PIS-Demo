
namespace updater
{
    /// <summary>
    /// Provides the entry point and core update logic for the Production Information System Updater application.
    /// </summary>
    /// <remarks>This class is intended for internal use as the main executable logic of the updater tool. It
    /// reads configuration values from the registry, validates required settings, and performs the update process by
    /// copying files from the update source to the local installation directory. The class is not intended to be
    /// instantiated or used as a library component.</remarks>
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

        /// <summary>
        /// Copies all files and subdirectories from the specified source directory to the specified destination
        /// directory.
        /// </summary>
        /// <remarks>Files whose names contain the substring "updater" are excluded from copying. The
        /// method copies all files and subdirectories recursively.</remarks>
        /// <param name="sourceDir">The path of the directory to copy from. Must refer to an existing directory.</param>
        /// <param name="destinationDir">The path of the directory to copy to. The directory will be created if it does not exist.</param>
        /// <param name="overwrite">true to overwrite existing files in the destination directory; otherwise, false. The default is true.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory specified by sourceDir does not exist.</exception>
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

