using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Recognizer.Services
{
    public interface IAzureRecognition
    {
        string[] GetOCR(string jsonStrokes, string url, string key, string lang);

        Task SayIT(string text, string key, string region,string language);
    }
}
