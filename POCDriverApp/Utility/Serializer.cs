using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace POCDriverApp.Utility
{
    public static class Serializer
    {
        public static string Serialize<T>(this T input)
        {
            var x = new XmlSerializer(input.GetType());

            using (var ms = new MemoryStream())
            {
                x.Serialize(ms, input);

                ms.Position = 0;

                using (var sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static T Deserialize<T>(string s)
        {
            var x = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(s))
            {
                return (T)x.Deserialize(sr);
            }
        }

        public static void SerializeFile<T>(string filePath, object content)
        {
            try
            {
                //Serialize the operation list again
                using (var streamWriter = new StreamWriter(filePath))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, content);
                }
            }
            catch (Exception ex)
            {
              
            }
        }

        public static T DeserializeFile<T>(string filePath)
        {
            T result = default(T);
            try
            {
                if (File.Exists(filePath))
                {
                    using (var xmlReader = XmlReader.Create(filePath))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(T));
                        result = (T)xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return result;
        }
    }
}