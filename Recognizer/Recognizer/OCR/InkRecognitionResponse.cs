using Newtonsoft.Json;
using System.Collections.Generic;

namespace Recognizer.OCR
{
    public class InkRecognitionResponse
    {
        [JsonProperty(PropertyName = "recognitionUnits")]
        public List<InkRecognitionUnit> RecognitionUnits { get; set; }
    }
}
