using Newtonsoft.Json.Linq;
using System.Text;
using static RedisWindowsClient.Models.Enum;

namespace RedisWindowsClient.Converters
{
    public static class JsonHtml
    {
        /// <summary>
        /// get a html code from json line
        /// json should be show in a html table
        /// </summary>
        /// <param name="json">must be a json object , except json array</param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static string GetHtml(string json, HtmlStyle style = HtmlStyle.HasHtmlHeader)
        {
            var setting = new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Load
            };

            var jb = JObject.Parse(json, setting);
            var str = new StringBuilder();

            if (style != HtmlStyle.NoHtmlHeader)
            {
                str.Append("<!DOCTYPE html><html>" + GetTableBeautifulCss() + "<body>");
            }
            else
            {
                str.Append(GetTableBeautifulCss());
            }

            BuildTableFromJsonObject(jb, str);
            if (style != HtmlStyle.NoHtmlHeader)
            {
                str.Append("</body></html>");
            }
            return str.ToString();
        }

        /// <summary>
        /// extend
        /// from array node get html
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string GetHtmlFromJsonArray(JArray arr, HtmlStyle style = HtmlStyle.HasHtmlHeader)
        {
            JsonLoadSettings setting = new JsonLoadSettings();
            setting.CommentHandling = CommentHandling.Load;
            StringBuilder str = new StringBuilder();
            if (style != HtmlStyle.NoHtmlHeader)
            {
                str.Append("<!DOCTYPE html><html>" + GetTableBeautifulCss() + "<body>");
            }
            else
            {
                str.Append(GetTableBeautifulCss());
            }
            BuildTableFromJsonArray(arr, str);
            if (style != HtmlStyle.NoHtmlHeader)
            {
                str.Append("</body></html>");
            }
            return str.ToString();
        }

        private static void BuildTableFromJsonObject(JObject jb, StringBuilder str)
        {
            str.Append("<table class=\"tablecss\">");
            str.Append("<tr>");
            foreach (var pro in jb.Properties())
            {
                str.Append("<td>" + pro.Name + "</td>");
            }
            str.Append("</tr>");
            BuildRowFromJsonObject(jb, str);
            str.Append("</table>");
        }

        private static void BuildRowFromJsonObject(JObject jb, StringBuilder str)
        {
            str.Append("<tr>");
            foreach (var pro in jb.Properties())
            {
                switch (pro.Value.Type)
                {
                    case JTokenType.Array:
                        str.Append("<td>");
                        BuildTableFromJsonArray(JArray.Parse(pro.Value.ToString()), str);
                        str.Append("</td>");
                        break;

                    case JTokenType.Object:
                        str.Append("<td>");
                        BuildTableFromJsonObject(JObject.Parse(pro.Value.ToString()), str);
                        str.Append("</td>");
                        break;

                    default:
                        str.Append("<td>" + pro.Value.ToString() + "</td>");
                        break;
                }
            }
            str.Append("</tr>");
        }

        private static void BuildTableFromJsonArray(JArray arr, StringBuilder str)
        {
            if (arr.Count == 0)
                return;
            str.Append("<table class=\"tablecss\" >");
            if (arr[0].Type == JTokenType.Object)
            {
                str.Append("<tr>");
                foreach (var pro in JObject.Parse(arr[0].ToString()).Properties())
                {
                    str.Append("<td>" + pro.Name + "</td>");
                }
                str.Append("</tr>");
            }
            foreach (var pro in arr)
            {
                switch (pro.Type)
                {
                    case JTokenType.Array:
                        str.Append("<tr>");
                        BuildTableFromJsonArray(JArray.Parse(pro.ToString()), str);
                        str.Append("</tr>");
                        break;

                    case JTokenType.Object:
                        BuildRowFromJsonObject(JObject.Parse(pro.ToString()), str);
                        break;

                    default:
                        str.Append("<tr>");
                        str.Append("<td>" + pro.ToString() + "</td>");
                        str.Append("</tr>");
                        break;
                }
            }
            str.Append("</tr>");
            str.Append("</table>");
        }

        private static string GetTableBeautifulCss()
        {
            return "<style type=\"text/css\">"
                    + "table.tablecss {"
                      + " font-family: verdana,arial,sans-serif;"
                      + " font-size:11px;"
                      + " border-width: 1px;"
                      + " border-color: #000000;"
                      + " border-collapse: collapse;"
                   + " }"
                   + " table.tablecss th {"
                       + " border-width: 1px;"
                       + " padding: 8px;"
                       + " border-style: solid;"
                       + " border-color: #000000;"
                   + " }"
                   + " table.tablecss td {"
                       + " border-width: 1px;"
                       + " padding: 8px;"
                       + " border-style: solid;"
                       + " border-color: #000000;"
                   + " }"
                    + "</style>";
        }
    }
}