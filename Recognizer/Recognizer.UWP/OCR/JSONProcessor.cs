using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizer.UWP.OCR
{
    public static class JSONProcessor
    {
        public static string CreateInkRecognitionRequest(IReadOnlyList<InkRecognizerStroke> strokes)
        {
            try
            {
                InkRecognitionRequest request = new InkRecognitionRequest(strokes);
                var requestJson = JsonConvert.SerializeObject(request);
                return requestJson;
            }
            catch (Exception e)
            {
                throw new JsonReaderException(e.Message);
            }
        }

        public static InkRecognitionRoot ParseInkRecognizerResponse(string responseJson)
        {
            try
            {
                var responseObj = JsonConvert.DeserializeObject<InkRecognitionResponse>(responseJson,
                                                    new InkRecognitionResponseConverter());
                var result = new InkRecognitionRoot(responseObj);
                return result;
            }
            catch (Exception e)
            {
                throw new JsonWriterException(e.Message);
            }
        }

        public static HttpErrorDetails ParseInkRecognitionError(string errorJson)
        {
            try
            {
                var error = JsonConvert.DeserializeObject<HttpErrorDetails>(errorJson);
                return error;
            }
            catch (Exception e)
            {
                throw new JsonReaderException(e.Message);
            }

        }
    }
}
