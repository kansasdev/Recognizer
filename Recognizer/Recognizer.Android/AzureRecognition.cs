using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.Media;
using Java.Net;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recognizer.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Recognizer.Droid.AzureRecognition))]
namespace Recognizer.Droid
{
    public class AzureRecognition : IAzureRecognition
    {
        public string[] GetOCR(string jsonStrokes,string url,string key,string language)
        {

                string endpoint = AppSettingsManager.Settings["endpoint"];
                string subscriptionKey = key;//AppSettingsManager.Settings["key"];

                string lang = language;//AppSettingsManager.Settings["language"];
                jsonStrokes = jsonStrokes.Replace("\"language\": \"en-US\"", "\"language\": \"" + lang + "\"");

                string requestData = jsonStrokes;
                string apiAddress = url;

                using (HttpClient client = new HttpClient { BaseAddress = new Uri(apiAddress) })
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                    var content = new StringContent(requestData, System.Text.Encoding.UTF8, "application/json");
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
                            if (recognizedText != "")
                            {
                                return new string[] { recognizedText };
                            }
                            else
                            {
                                throw new Exception("Can't recognize any text");
                            }
                    }
                    else
                    {
                        throw new Exception($"ErrorCode: {res.StatusCode}");
                    }
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

        public async Task SayIT(string text,string key,string region,string language)
        {
            string speechKey = key;
           

            var config = SpeechConfig.FromSubscription(speechKey, region);
            config.SpeechSynthesisLanguage = language;

            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.SetVolume(1f, 1f);
            // Creates a speech synthesizer.
            using (var synthesizer = new SpeechSynthesizer(config, null))
            {
                // Receive a text from TextForSynthesis text box and synthesize it to speaker.
                using (var result = await synthesizer.SpeakTextAsync(text).ConfigureAwait(false))
                {
                    // Checks result.
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {

                        using (var audioStream = AudioDataStream.FromResult(result))
                        {

                            MemoryStream ms = new MemoryStream(result.AudioData);
                            ms.Seek(0, SeekOrigin.Begin);
                            mediaPlayer.Prepared += (sender, e) =>
                            {
                                mediaPlayer.Start();
                            };
                            mediaPlayer.SetDataSource(new StreamMediaDataSource(ms));
                            
                            mediaPlayer.Prepare();
                        }
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");
                        sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        sb.AppendLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");

                        throw new Exception(sb.ToString());
                    }
                }
            }
        }

        public class StreamMediaDataSource : MediaDataSource
        {
            System.IO.Stream data;

            public StreamMediaDataSource(System.IO.Stream Data)
            {
                data = Data;
            }

            public override long Size
            {
                get
                {
                    return data.Length;
                }
            }

            public override int ReadAt(long position, byte[] buffer, int offset, int size)
            {
                data.Seek(position, System.IO.SeekOrigin.Begin);
                return data.Read(buffer, offset, size);
            }

            public override void Close()
            {
                if (data != null)
                {
                    data.Dispose();
                    data = null;
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (data != null)
                {
                    data.Dispose();
                    data = null;
                }
            }
        }
    }
}