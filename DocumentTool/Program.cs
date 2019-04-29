using System;
using System.Text;
using System.IO;

using Utility.CommandLine;
using Microsoft.Extensions.Configuration;


namespace DocumentTool
{
    class Program
    {
        [Argument('f', "path")]
        private static string[] filePaths { get; set; }
        [Operands]
        private static string[] Operands { get; set; }
        
        public static IConfigurationBuilder builder = null;
        static void Main(string[] args)
        {

            builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Arguments.Populate();
            


            foreach (string s in filePaths)
            {
                Console.WriteLine("argument: " + s);
            }
            string filePath = filePaths[0];

            //CHECK File is EXIST
            if (Helper.isFileExist(filePaths[0]))
            {
                Console.WriteLine(detectLanguage(filePath));
            }
            else {
                Console.WriteLine(string.Format("Could not find your documents on the following path '{0}'", filePath));
            }
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
        }
        static string detectLanguage(string filePath)
        {
            IConfigurationRoot configuration = builder.Build();

            APIFunction AzVisionAPI = new APIFunction();

            AzVisionAPI.AzureHostUrl = configuration["AzureVisionApiUrl"].ToString();
            AzVisionAPI.AzureHostRoute = configuration["AzureVisionApiVersion"].ToString(); ;
            AzVisionAPI.AzureHostKey = configuration["AzureVisionApiKey"].ToString();
            AzVisionAPI.AzureHostMaxLengthTextSample = Convert.ToInt32(configuration["AzureHostMaxLengthTextSample"].ToString());


            var fileExt = Path.GetExtension(filePath).Trim().ToLower();
            string contentText = "";
            if (fileExt == ".pdf")
                contentText = Helper.TextFromPdf(filePath);
            else if (fileExt == ".doc" || fileExt == ".docx")
                contentText = Helper.TextFromWord(filePath);
            else
                return "";
            contentText = contentText.Length > AzVisionAPI.AzureHostMaxLengthTextSample ? Helper.Truncate(contentText, AzVisionAPI.AzureHostMaxLengthTextSample) : contentText;
            //Console.WriteLine(contentText);
            return AzVisionAPI.GetLanguange(contentText);
        }
    }
}
