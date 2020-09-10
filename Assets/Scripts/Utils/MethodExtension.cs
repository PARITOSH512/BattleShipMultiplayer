using UnityEngine;
namespace Applications.Utils
{
   

    public static class MethodExtension
    {
        public static string RemoveQuotes(this string values)
        {
            return values.Replace("\"", "");
        }

        public static float TwoDecimals(this float values)
        {
            return Mathf.Round(values * 1000.0f) / 1000.0f;
        }
    }
}