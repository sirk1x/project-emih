using Discord;
using Newtonsoft.Json;
using OpenAI_API.Images;

namespace project_emih
{
    internal class InvisionConfig
    {
        public string OpenAI_Api_Key { get; set; }
        public string Discord_Bot_Token { get; set; }
        public ulong Discord_App_Id { get; set; }
        public int Dalle_Images_Per_Request = 1;
        public string Dalle_Image_Size = "1024x1024";

        [JsonIgnore]
        public ImageSize Dalle_Image_Size_Internal
        {
            get
            {
                if (Dalle_Image_Size == "1024x1024")
                    return ImageSize._1024;
                else if (Dalle_Image_Size == "512x512")
                    return ImageSize._512;
                else if (Dalle_Image_Size == "256x256")
                    return ImageSize._256;
                return ImageSize._1024;
            }
        }

        public UserStatus BotStatus = UserStatus.Online;
        public string BotGame = "";
        public string BotName = "Emih";

        private static InvisionConfig cConfig;
        public static InvisionConfig Current
        {
            get
            {
                if (!System.IO.File.Exists(Invision.Directory + "//config.json"))
                {
                    cConfig = new InvisionConfig();
                    System.IO.File.WriteAllText(
                        Invision.Directory + "//config.json",
                        JsonConvert.SerializeObject(cConfig, Formatting.Indented));
                    Console.WriteLine("New config generated = " + Invision.Directory + "//config.json");
                    Console.ReadKey();
                    Environment.Exit(0);
                    return null;
                }
                else
                {
                    if (cConfig == null)
                        cConfig = JsonConvert.DeserializeObject<InvisionConfig>(File.ReadAllText(Invision.Directory + "//config.json"));
                    return cConfig;
                }


            }
        }
    }
}
