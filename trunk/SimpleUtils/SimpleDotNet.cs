using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleUtils
{
    public class SimpleDotNet
    {
        public const string Author = "WiCKY Hu";
        public const string GoogleSitesUrl = "http://sites.google.com";
        public const string WebSiteUrl = GoogleSitesUrl + "/site/simpledotnet";
        //public const string DiscussionWebSiteUrl = "http://groups.google.com/group/simpledotnet";
        public const string EmailAddress = "simpledotnet@gmail.com";
        
        public static string ReportUrl = null;

        public static void OpenWebSite()
        {
            SimpleProcess.Start(WebSiteUrl);
        }

        //public static void OpenDiscussionWebSite()
        //{
        //    SimpleProcess.Start(DiscussionWebSiteUrl);
        //}

        public static void SendEmail(string subject, string body)
        {
            string mailto = String.Format("mailto:{0}?subject={1}&body={2}",
                EmailAddress,
                subject,
                EncodeEmailBody(body)
                );

            const int MAX_LENGTH = 2000;
            if (mailto.Length > MAX_LENGTH) mailto = mailto.Substring(0, MAX_LENGTH);

            SimpleProcess.Start(mailto);
        }

        private static string EncodeEmailBody(string body)
        {
            if (body == null) return String.Empty;
            return body.Replace("\n", "%0d%0a");
            //return HttpUtility.UrlEncode(body).Replace("+", " "); //chinese corrupt
        }

    }
}
