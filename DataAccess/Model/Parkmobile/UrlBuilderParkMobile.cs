using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace DataAccess.Model.Parkmobile
{
   public class UrlBuilderParkMobile
   {
       public static Uri BuildUri(string root, NameValueCollection query)
       {
           var collection = HttpUtility.ParseQueryString(string.Empty);

           foreach (var key in query.Cast<string>().Where(key => !string.IsNullOrEmpty(query[key])))
           {
               collection[key] = query[key];
           }

           var builder = new UriBuilder(root) { Query = collection.ToString() };
           return builder.Uri;
       }


       public static Uri BuildUri2(string root)
       {
           var builder = new UriBuilder(root);
           return builder.Uri;
       }

       public static Uri BuildPlateUrl(string root)
       {
           var builder = new UriBuilder(root);
           return builder.Uri;
       }

       public static Uri BuildSpaceUrl(string root)
       {
           var builder = new UriBuilder(root);
           return builder.Uri;
       }

       //public static string BuildPlateUrl(Uri plateUri, string plate)
       //{
       //    string currentUrl = string.Empty;
       //    currentUrl = plateUri.ToString();
       //    currentUrl = currentUrl + (HttpUtility.UrlEncode(string.Format("{{{0}}}", plate)).ToUpper());
       //    return currentUrl;
       //}

       //public static string BuildSpaceUrl(Uri spaceUri, string ZoneCode, string SpaceNumber)
       //{
       //    string currentUrl = string.Empty;
       //    currentUrl = spaceUri.ToString();
       //    currentUrl = currentUrl + (HttpUtility.UrlEncode(string.Format("{{{0}}}", ZoneCode)).ToUpper());
       //    currentUrl = currentUrl + "/";
       //    currentUrl = currentUrl + (HttpUtility.UrlEncode(string.Format("{{{0}}}", SpaceNumber)).ToUpper());
       //    return currentUrl;
       //}
   }
}
