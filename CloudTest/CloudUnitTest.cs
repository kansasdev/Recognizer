using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recognizer.OCR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;

namespace CloudTest
{
    [TestClass]
    public class CloudUnitTest
    {
        [TestMethod]
        public void TestAzureInkRecognzier()
        {
            try
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("config.json").Build();
                if (configuration != null)
                {
                    string endpoint = configuration["endpoint"];
                    string subscriptionKey = configuration["key"];
                    string requestData = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\FingerPaint-20200715-033126209.json");

                    string apiAddress = configuration["apiaddress"];


                    using (HttpClient client = new HttpClient { BaseAddress = new Uri(apiAddress) })
                    {
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                        var content = new StringContent(requestData, Encoding.UTF8, "application/json");
                        var res = client.PutAsync(endpoint, content).Result;
                        if (res.IsSuccessStatusCode)
                        {
                            string result = res.Content.ReadAsStringAsync().Result;

                            InkRecognitionRoot root = JSONProcessor.ParseInkRecognizerResponse(result);

                            if (root != null)
                            {
                                List<InkLine> lstLines = root.GetLines().ToList();
                                string tekst = string.Empty;
                                foreach (InkLine line in lstLines)
                                {
                                    tekst += tekst + line.RecognizedText;
                                }

                                Trace.WriteLine(tekst);
                            }
                        }
                        else
                        {
                            throw new Exception($"ErrorCode: {res.StatusCode}");
                        }
                    }
                }
                else
                {
                    throw new Exception("Can't parse config file");
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("B³¹d: " + ex.Message);
            }
        }
    }
}
