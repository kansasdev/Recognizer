using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
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
                //test
                var configuration = new ConfigurationBuilder().AddJsonFile("config.json").Build();
                if (configuration != null)
                {
                    string endpoint = configuration["endpoint"];
                    string subscriptionKey = configuration["key"];
                    string requestData = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\FingerPaint-20200719-112850786.json");

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
                            JToken tok = JToken.Parse(result);
                            string recognizedText = string.Empty;
                            WalkNode(tok, n =>
                            {
                                JToken token = n["recognizedText"];
                                if (string.IsNullOrEmpty(recognizedText))
                                {
                                    if (token != null && token.Type == JTokenType.String)
                                    {
                                        recognizedText = token.Value<string>();

                                    }
                                }
                            });

                            if(recognizedText!="")
                            {
                                Trace.WriteLine("Recognized: " + recognizedText);
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

        static void WalkNode(JToken node, Action<JObject> action)
        {
            if (node.Type == JTokenType.Object)
            {
                action((JObject)node);

                foreach (JProperty child in node.Children<JProperty>())
                {
                    WalkNode(child.Value, action);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    WalkNode(child, action);
                }
            }
        }

        [TestMethod]
         public async Task  SayTest()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            if (configuration != null)
            {
                string speechKey = configuration["keyspeech"];
                 string endpoint = configuration["endpointspeech"];
                 string speechregion = configuration["region"];

                var config = SpeechConfig.FromSubscription(speechKey, speechregion);
                
                config.SpeechSynthesisLanguage = "en-US";
                 MediaPlayer mediaPlayer = new MediaPlayer();

                // Creates a speech synthesizer.
                    using (var synthesizer = new SpeechSynthesizer(config, null))
                    {
                    // Receive a text from TextForSynthesis text box and synthesize it to speaker.
                        using (var result = await synthesizer.SpeakTextAsync("stuff").ConfigureAwait(false))
                        {
                        // Checks result.
                            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                            {

                                using (var audioStream = AudioDataStream.FromResult(result))
                                {
                                    StorageFolder picturesDirectory = KnownFolders.PicturesLibrary;
                                    // Save synthesized audio data as a wave file and use MediaPlayer to play it
                                    var filePath = Path.Combine(picturesDirectory.Path, "outputaudio.wav");
                                    await audioStream.SaveToWaveFileAsync(filePath);
                                    //mediaPlayer.Source = Windows.Media.Core.MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(filePath));
                                mediaPlayer.Play();
                            }
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                                                        
                            throw new Exception("Error: "+cancellation.ErrorDetails);
                        }
                    }
                }
            }

            await Task.CompletedTask; 
        }
    
    }
}
