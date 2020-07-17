using Newtonsoft.Json;
using System.Collections.Generic;

namespace Recognizer.OCR
{
    public class InkLine : InkRecognitionUnit
    {
        [JsonProperty(PropertyName = "recognizedText")]
        public string RecognizedText { get; set; }

        [JsonProperty(PropertyName = "alternates")]
        public List<Alternates> Alternates { get; set; }
    }
}
