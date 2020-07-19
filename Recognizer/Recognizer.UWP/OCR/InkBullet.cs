using Newtonsoft.Json;

namespace Recognizer.UWP.OCR
{
    public class InkBullet : InkRecognitionUnit
    {
        [JsonProperty(PropertyName = "recognizedText")]
        public string RecognizedText { get; set; }
    }
}
