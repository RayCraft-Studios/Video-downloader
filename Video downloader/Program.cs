using YoutubeExplode;

namespace YouTubeDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Linkreader lr = new Linkreader();

            //Check if Data exists
            lr.DataCheck();

            //Create list to load txt file line for line in it
            List<string> ytlinks = new List<string>();
            using (StreamReader sr = new StreamReader(lr.GetFile()))
            {
                string zeile;
                while ((zeile = sr.ReadLine()) != null)
                {
                    if (zeile.Trim().StartsWith("http"))
                    {
                        ytlinks.Add(zeile);
                    }
                }
            }

            /**
             * Download Videos
             */

            //check if list is empty and if not start Downloader for every entry in list
            if (ytlinks != null && ytlinks.Count > 0)
            {
                Console.WriteLine("Start Downloading...");
                Console.WriteLine();
                try
                {
                    foreach (var videoUrl in ytlinks)
                    {
                        YTGrab grabYT = new YTGrab();
                        await grabYT.DownloadYouTubeVideo(videoUrl, lr.GetVideoPath());
                        lr.RemoveLink(videoUrl);
                    }

                }
                catch (Exception ex)
                {
                    //Download Error
                    Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No links in grablist");
                Console.WriteLine();
            }

        }

    }
}