using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PowerBiDiffer
{
    public static class StringTemplatizer
    {
        public static string InstantiateTemplate(this string template, Dictionary<string, string> templateData, bool replaceUnmatchedTemplatesWithEmptyString = false)
        {
            if (!string.IsNullOrWhiteSpace(template))
            {
                template = Regex.Replace(template,
                    @"(\{.+?\})",
                    m => templateData.ContainsKey(m.Groups[1].Value)
                        ? templateData[m.Groups[1].Value]
                        : replaceUnmatchedTemplatesWithEmptyString
                            ? string.Empty
                            : m.Groups[1].Value);
            }

            return template;
        }
    }
}