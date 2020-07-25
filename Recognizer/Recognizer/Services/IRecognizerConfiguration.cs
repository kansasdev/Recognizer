using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Recognizer.Services
{
    public interface IRecognizerConfiguration
    {
        AzureConfiguration GetConfig();

        Task SetConfig(AzureConfiguration ac);       

        
    }

    public class AzureConfiguration
    {
        public string Language { get; set; }
        public string ApiAddress { get; set; }
        public string AzureRegion { get; set; }

        public string KeyOcr { get; set; }

        public string KeySpeech { get; set; }
    }
}
