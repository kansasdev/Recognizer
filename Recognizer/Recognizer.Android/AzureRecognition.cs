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

[assembly: Xamarin.Forms.Dependency(typeof(Recognizer.Droid.AzureRecognition))]
namespace Recognizer.Droid
{
    public class AzureRecognition : IAzureRecognition
    {
        public string[] GetOCR(string jsonStrokes)
        {
            throw new NotImplementedException();
        }

        public Task SayIT(string text)
        {
            throw new NotImplementedException();
        }
    }
}