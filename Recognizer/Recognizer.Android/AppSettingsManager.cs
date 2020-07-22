using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace Recognizer.Droid
{
    public class AppSettingsManager
    {
        private static AppSettingsManager _instance;
        private JObject _secrets;

        private const string Namespace = "Recognizer.Droid";
        private const string FileName = "config.json";

        private AppSettingsManager()
        {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppSettingsManager)).Assembly;
                var stream = assembly.GetManifestResourceStream($"{Namespace}.{FileName}");
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    _secrets = JObject.Parse(json);
                }
        }

        public static AppSettingsManager Settings
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettingsManager();
                }
                return _instance;
            }
        }

        public string this[string name]
        {
            get
            {
                   var path = name.Split(':');
                   JToken node = _secrets[path[0]];
                    for (int index = 1; index < path.Length; index++)
                    {
                        node = node[path[index]];
                    }
                    return node.ToString();
            }
        }
    }
}