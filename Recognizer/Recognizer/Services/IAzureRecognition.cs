using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Recognizer.Services
{
    public interface IAzureRecognition
    {
        string[] GetOCR(string jsonStrokes);

        Task SayIT(string text);
    }
}
