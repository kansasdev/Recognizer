using Newtonsoft.Json;

namespace Recognizer.OCR
{
    public class InkBullet : InkRecognitionUnit
    {
        [JsonProperty(PropertyName = "recognizedText")]
        public string RecognizedText { get; set; }
    }
}
