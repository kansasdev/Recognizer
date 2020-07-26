using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Recognizer.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        public ConfigViewModel()
        {
            Title = "Configuration";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://github.com/kansasdev/Recognizer"));
        }

        public ICommand OpenWebCommand { get; }

        public ICommand UpdateConfigCommand { get; }
    }
}