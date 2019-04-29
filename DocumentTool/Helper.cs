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
    class Helper
    {
        public static bool isFileExist(string filePath)
        {
            return File.Exists(filePath);
        }
        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        public static string ExtractLangDetails(string s)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(s), Newtonsoft.Json.Formatting.Indented);
        }
        public static string TextFromPdf(string pdfPath)
        {
            PdfReader reader = new PdfReader(pdfPath);
            StringWriter output = new StringWriter();
            var strPDFOutput = "";
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                //output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));
                strPDFOutput += PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy());
            }
            //Console.WriteLine(strPDFOutput);
            return strPDFOutput;

        }
        public static string TextFromWord(string filePath)
        {
            const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            StringBuilder textBuilder = new StringBuilder();
            
            using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(filePath, false))
            {
                // Manage namespaces to perform XPath queries.  
                NameTable nt = new NameTable();
                XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                nsManager.AddNamespace("w", wordmlNamespace);

                // Get the document part from the package.  
                // Load the XML in the document part into an XmlDocument instance.  
                XmlDocument xdoc = new XmlDocument(nt);
                xdoc.Load(wdDoc.MainDocumentPart.GetStream());

                XmlNodeList paragraphNodes = xdoc.SelectNodes("//w:p", nsManager);
                foreach (XmlNode paragraphNode in paragraphNodes)
                {
                    XmlNodeList textNodes = paragraphNode.SelectNodes(".//w:t", nsManager);
                    foreach (System.Xml.XmlNode textNode in textNodes)
                    {
                        textBuilder.Append(textNode.InnerText);
                    }
                    textBuilder.Append(Environment.NewLine);
                }

            }
            return textBuilder.ToString();
        }
    }
}
