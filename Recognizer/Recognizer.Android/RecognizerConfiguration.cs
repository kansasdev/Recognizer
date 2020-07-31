using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Recognizer.Services;

[assembly: Xamarin.Forms.Dependency(typeof(Recognizer.Droid.RecognizerConfiguration))]
namespace Recognizer.Droid
{
    public class RecognizerConfiguration : IRecognizerConfiguration
    {
        public AzureConfiguration GetConfig()
        {
            
                AzureConfiguration ac = new AzureConfiguration();

                ac.ApiAddress = AppSettingsManager.Settings["apiaddress"];
                ac.KeyOcr = AppSettingsManager.Settings["key"];
                ac.Language = AppSettingsManager.Settings["language"];
                ac.KeySpeech = AppSettingsManager.Settings["keyspeech"];
                ac.AzureRegion = AppSettingsManager.Settings["region"];

                return ac;
        }

        public Task SetConfig(AzureConfiguration ac)
        {
            throw new NotImplementedException("Not needed at the moment, using Xamarin plugin");
        }
    }
}