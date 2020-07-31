using Microsoft.Extensions.Configuration;
using Recognizer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(Recognizer.UWP.RecognizerConfiguration))]
namespace Recognizer.UWP
{
    public class RecognizerConfiguration : IRecognizerConfiguration
    {
        public AzureConfiguration GetConfig()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            
            if (configuration != null)
            {
                AzureConfiguration ac = new AzureConfiguration();

                ac.ApiAddress = configuration["apiaddress"];
                ac.KeyOcr  = configuration["key"];
                ac.Language = configuration["language"];
                ac.KeySpeech = configuration["keyspeech"];
                ac.AzureRegion = configuration["region"];

                return ac;

            }
            else
            {
                
                throw new Exception("No configuration file or permission denied");
            }
        }

            public async Task SetConfig(AzureConfiguration ac)
            {
                 AddOrUpdateAppSetting<string>("endpoint", ac.ApiAddress);
                 AddOrUpdateAppSetting<string>("key", ac.KeyOcr);
                 AddOrUpdateAppSetting<string>("language", ac.Language);
                 AddOrUpdateAppSetting<string>("keyspeech", ac.KeySpeech);
                 AddOrUpdateAppSetting<string>("endpointspeech", ac.AzureRegion);
                 await Task.CompletedTask;
            }

            public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
            {
               
                    var filePath = Path.Combine(AppContext.BaseDirectory, "config.json");
                    string json = File.ReadAllText(filePath);
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    SetValueRecursively(sectionPathKey, jsonObj, value);

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(filePath, output);

                
            }


            private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
            {
                // split the string at the first ':' character
                var remainingSections = sectionPathKey.Split(":", 2);

                var currentSection = remainingSections[0];
                if (remainingSections.Length > 1)
                {
                    // continue with the procress, moving down the tree
                    var nextSection = remainingSections[1];
                    SetValueRecursively(nextSection, jsonObj[currentSection], value);
                }
                else
                {
                    // we've got to the end of the tree, set the value
                    jsonObj[currentSection] = value;
                }
            }

    }
}
