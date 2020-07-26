using Acr.UserDialogs;
using Recognizer.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Recognizer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ConfigPage : ContentPage
    {
        private AzureConfiguration _ac;
        private IRecognizerConfiguration _irc;

        public ConfigPage()
        {
            InitializeComponent();
            //string speechKey = AppSettingsManager.Settings["keyspeech"];
            //string endpoint = AppSettingsManager.Settings["endpointspeech"];
            _irc = DependencyService.Get<IRecognizerConfiguration>();
            if(_irc==null)
            {
                UserDialogs.Instance.Alert("Recognzier configuration is null", "Can't get Recognizer Configuration");
            }
            else
            {
                _ac = _irc.GetConfig();
                if(_ac!=null)
                {
                    txtKeyOcr.Text = _ac.KeyOcr;
                    txtEndpointAzure.Text = _ac.ApiAddress;
                    txtKeySpeech.Text = _ac.KeySpeech;
                    txtLanguage.Text = _ac.Language;
                    txtRegion.Text = _ac.AzureRegion;
                }
                else
                {
                    UserDialogs.Instance.Alert("No data in config files", "Can't get detailed configuration");
                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await SetIndicator(true);
                await _irc.SetConfig(_ac);
                
            }
            catch(Exception ex)
            {
                await UserDialogs.Instance.AlertAsync("Detailed error: " + ex.Message, "Error during configuration exception");
            }
            finally
            {
                await SetIndicator(false);
                if (_irc != null)
                {
                    _ac = _irc.GetConfig();
                    if (_ac != null)
                    {
                        txtKeyOcr.Text = _ac.KeyOcr;
                        txtEndpointAzure.Text = _ac.ApiAddress;
                        txtKeySpeech.Text = _ac.KeySpeech;
                        txtLanguage.Text = _ac.Language;
                        txtRegion.Text = _ac.AzureRegion;
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("No data in config files", "Can't get detailed configuration");
                    }
                }
            }
        }

        private async Task SetIndicator(bool enabled)
        {
            waitIndicator.IsRunning = enabled;
            btnUpdate.IsEnabled = !enabled;
            txtEndpointAzure.IsEnabled = !enabled;
            txtKeyOcr.IsEnabled = !enabled;
            txtKeySpeech.IsEnabled = !enabled;
            txtLanguage.IsEnabled = !enabled;
            txtRegion.IsEnabled = !enabled;

            await Task.CompletedTask;
        }
    }
}