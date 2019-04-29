using System;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Newtonsoft.Json;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net.Http;

namespace DocumentTool
{
    class APIFunction
    {
        public string AzureHostUrl { get; set; }
        public string AzureHostRoute { get; set; }
        public string AzureHostKey { get; set; }
        public int AzureHostMaxLengthTextSample { get; set; }
        
        public string GetLanguange(string contentText)
        {
            System.Object[] body = new System.Object[] { new { Text = contentText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Set the method to POST
                request.Method = HttpMethod.Post;

                // Construct the full URI
                request.RequestUri = new Uri(AzureHostUrl + AzureHostRoute);

                // Add the serialized JSON object to your request
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Add the authorization header
                request.Headers.Add("Ocp-Apim-Subscription-Key", AzureHostKey);

                // Send request, get response
                var response = client.SendAsync(request).Result;
                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                // Pretty print the response
                return Helper.ExtractLangDetails(jsonResponse);
            }
        }
    }
}
