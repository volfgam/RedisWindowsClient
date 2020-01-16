using Newtonsoft.Json;
using System;
using System.Xml.Linq;

namespace RedisWindowsClient.Extensions
{
    internal static class StringExtensions
    {
        public static string PrettyPrint(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            try
            {
                return XDocument.Parse(input).ToString();
            }
            catch (Exception) { }

            try
            {
                var t = JsonConvert.DeserializeObject<object>(input);
                return JsonConvert.SerializeObject(t, Formatting.Indented);
            }
            catch (Exception) { }

            return input;
        }
    }
}