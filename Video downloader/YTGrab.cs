using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace YouTubeDownloader
{
    internal class YTGrab
    {
        public async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            // Ein neuer YoutubeClient wird erstellt, um mit der YouTube-API zu interagieren.
            var youtube = new YoutubeClient();

            // Das Video wird asynchron anhand seiner URL abgerufen.
            var video = await youtube.Videos.GetAsync(videoUrl);

            // Der Titel des Videos wird bereinigt, um ungültige Zeichen aus dem Dateinamen zu entfernen.
            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            // Die Manifestdatei des Streams des Videos wird asynchron abgerufen.
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

            // Es werden alle verfügbaren Muxed-Streams (Video- und Audio-Daten kombiniert) abgerufen und nach Videoqualität absteigend sortiert.
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            // Überprüfung, ob mindestens ein Muxed-Stream vorhanden ist.
            if (muxedStreams.Any())
            {
                // Auswahl des ersten Streams (mit höchster Qualität).
                var streamInfo = muxedStreams.First();

                // Ein HttpClient wird erstellt, um den Video-Stream herunterzuladen.
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);

                // Aktuelles Datum und Uhrzeit werden abgerufen.
                var datetime = DateTime.Now;

                // Der Dateipfad, in dem das Video gespeichert wird, wird erstellt.
                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");

                // Eine Ausgabedatei wird erstellt, um den heruntergeladenen Stream zu speichern.
                using var outputStream = System.IO.File.Create(outputFilePath);

                // Der heruntergeladene Stream wird in die Ausgabedatei kopiert.
                await stream.CopyToAsync(outputStream);

                // Meldung über den erfolgreichen Download und Informationen zur gespeicherten Datei.
                Console.WriteLine("Download completed!");
                Console.WriteLine($"Video saved as: {outputFilePath} {datetime}");
            }
            else
            {
                // Wenn kein passender Video-Stream gefunden wurde, wird eine Meldung ausgegeben.
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
    }
}
