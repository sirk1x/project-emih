using project_emih;
using Discord;
using System.Net;

namespace project_emih
{
    static class IVHelper
    {
        public static async Task<string> DownloadImage(string _uri)
        {
            if (!Directory.Exists(Invision.ImageDir)) { Directory.CreateDirectory(Invision.ImageDir); }
            Uri uri = new Uri(_uri);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            var fname = Invision.ImageDir + filename;
            using var wc = new WebClient();
            await wc.DownloadFileTaskAsync(uri, fname);
            return fname;
        }

        public static void CleanUp(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
                if (File.Exists(files[i]))
                    File.Delete(files[i]);

        }
        public static IEnumerable<string> Chunk(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }
        public static Embed ReturnError(string msg)
        {
            return ReturnMessage("Error", msg, Color.Red);
        }
        public static Embed ReturnMessage(string title, string msg, Color clr)
        {
            return new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(msg)
                .WithColor(clr).Build();
        }
    }
}
