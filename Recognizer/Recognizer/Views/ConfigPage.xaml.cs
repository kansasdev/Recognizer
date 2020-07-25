using Acr.UserDialogs;
using Recognizer.Services;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Recognizer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ConfigPage : ContentPage
    {


        public ConfigPage()
        {
            InitializeComponent();
            //string speechKey = AppSettingsManager.Settings["keyspeech"];
            //string endpoint = AppSettingsManager.Settings["endpointspeech"];
            IRecognizerConfiguration irc = DependencyService.Get<IRecognizerConfiguration>();
            if(irc==null)
            {
                UserDialogs.Instance.Alert("Recognzier configuration is null", "Can't get Recognizer Configuration");
            }
            else
            {
                AzureConfiguration ac = irc.GetConfig();
                if(ac!=null)
                {

                }
                else
                {

                }
            }
        }
    }
}