using System;
using System.IO;

namespace YouTubeDownloader
{
    internal class Linkreader
    {
        /**
        * Set filepath and folderpath
        */
        private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string rootPath = @"C:\Users\Public\Videos";

        // FolderPaths
        private static string folderPath = Path.Combine(rootPath, "AutoUploader");
        private static string videoPath = Path.Combine(folderPath, "Downloaded_Videos");
        private static string filePath = Path.Combine(folderPath, "grablist.txt");
        private static string settingsPath = Path.Combine(folderPath, "settings.txt");
        private static string tagsPath = Path.Combine(folderPath, "tags.txt");
        private static string descriptionPath = Path.Combine(folderPath, "description.txt");

        private static bool IsChanged = false;

        // Get methods
        public String GetFolder() { return folderPath; }
        public String GetVideoPath() { return videoPath; }
        public String GetFile() { return filePath; }
        public String GetSettings() { return settingsPath; }
        public String GetTags() { return tagsPath; }
        public String GetDescription() { return descriptionPath; }

        /*
         Check if data exist
         */
        public void DataCheck()
        {
            // Check if Main folder for Downloaded videos exists
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"{folderPath} not found. Start creating folder.");
                Directory.CreateDirectory(folderPath);
                CreateShortcut(desktopPath, "AutoUploader.lnk", folderPath);
                Console.WriteLine($"{folderPath} and shortcut created.");
                IsChanged = true;
            }

            // Check if Downloaded_Video folder for Downloaded videos exists
            if (!Directory.Exists(videoPath))
            {
                Console.WriteLine($"{videoPath} not found. Start creating folder.");
                Directory.CreateDirectory(videoPath);
                Console.WriteLine($"{videoPath} created.");
                IsChanged = true;
            }

            // Check if grablist file for video scraping exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} not found. Start creating file.");
                File.WriteAllText(filePath, "//Paste Links here! One line per Link. Don't make empty lines between.");
                Console.WriteLine($"{filePath} created.");
                Console.WriteLine("Fill your links in grablist and restart this application.");
                IsChanged = true;
            }

            if (IsChanged)
            {
                Console.WriteLine("Press enter to close.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        // Create shortcut to Desktop
        private void CreateShortcut(string targetDirectory, string shortcutName, string targetPath)
        {
            string shortcutPath = Path.Combine(targetDirectory, shortcutName);
            string shortcutContent = $@"
                [InternetShortcut]
                URL=file:///{targetPath.Replace("\\", "/")}
                IconIndex=0
                IconFile={targetPath.Replace("\\", "/")}
            ";
            File.WriteAllText(shortcutPath, shortcutContent);
        }

        public void RemoveLink(string link)
        {
            // Temporäre Datei erstellen
            string tempDatei = Path.Combine(rootPath, "tmp.txt");
            string deleted = null;

            // StreamReader zum Lesen der aktuellen Datei
            using (StreamReader sr = new StreamReader(filePath))
            {
                // StreamWriter zum Schreiben auf die temporäre Datei
                using (StreamWriter sw = new StreamWriter(tempDatei))
                {
                    string zeile;
                    while ((zeile = sr.ReadLine()) != null)
                    {
                        if (!zeile.Contains(link))
                        {
                            sw.WriteLine(zeile);
                        }
                        else
                        {
                            deleted = zeile; // Speichere die gelöschte Zeile, falls gewünscht
                        }
                    }
                }
            }

            // Kopiere die temporäre Datei zurück in die ursprüngliche Datei
            File.Copy(tempDatei, filePath, true);

            // Lösche die temporäre Datei
            File.Delete(tempDatei);

            Console.WriteLine($"{deleted} wurde aus der Liste entfernt.");
        }

    }
}
