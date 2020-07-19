using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Recognizer.UWP.OCR;
using Recognizer.Services;
using Microsoft.CognitiveServices.Speech;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Storage;
using Microsoft.CognitiveServices.Speech.Audio;

[assembly: Xamarin.Forms.Dependency(typeof(Recognizer.UWP.AzureRecognition))]
namespace Recognizer.UWP
{


    public class AzureRecognition : IAzureRecognition
    {
        public string[] GetOCR(string jsonStrokes)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            if (configuration != null)
            {
                string endpoint = configuration["endpoint"];
                string subscriptionKey = configuration["key"];

                string lang = configuration["language"];
                jsonStrokes = jsonStrokes.Replace("\"language\": \"en-US\"", "\"language\": \"" + lang + "\"");

                string requestData = jsonStrokes;
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
                            string[] words = lstLines.Select(q => q.RecognizedText).ToArray();
                            return words;
                        }
                        else
                        {
                            throw new Exception("Can't parse response");
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

        public async Task SayIT(string text)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            if (configuration != null)
            {
                string speechKey = configuration["keyspeech"];
                string endpoint = configuration["endpointspeech"];

                var config = SpeechConfig.FromSubscription(speechKey,"westeurope");
                                
                MediaPlayer mediaPlayer = new MediaPlayer();

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


                                //var picLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

                                // Save synthesized audio data as a wave file and use MediaPlayer to play it
                                //string filePath = picLibrary.SaveFolder.Path+"\\outputaudio.wav";
                                //await audioStream.SaveToWaveFileAsync(filePath);
                                //mediaPlayer.Source = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(filePath));
                                MemoryStream ms = new MemoryStream(result.AudioData);
                                ms.Seek(0, SeekOrigin.Begin);
                                mediaPlayer.SetStreamSource(ms.AsRandomAccessStream());
                                mediaPlayer.Play();
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

            await Task.CompletedTask; 
        }
    }
}
