using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SimpleUtils.Win
{
    public class SimpleUpdater
    {
        private class UpdateParameter
        {
            public bool Silent;
            public string XmlUrl;
            public string HomeUrl;
        }

        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public static void CheckForUpdate(string xmlUrl, string homeUrl, bool silent)
        {
            UpdateParameter up = new UpdateParameter();
            up.XmlUrl = xmlUrl;
            up.HomeUrl = homeUrl;
            up.Silent = silent;

            Thread t = new Thread(new ParameterizedThreadStart(UpdateThread));
            t.IsBackground = true;
            t.Start(up);
        }

        private static void UpdateThread(object threadParam)
        {
            UpdateParameter up = (UpdateParameter)threadParam;
            try
            {
                string url = up.XmlUrl;
                WebClient wc = new WebClient();
                Stream respStream = wc.OpenRead(url);

                StreamReader sr = new StreamReader(respStream);
                string s = sr.ReadToEnd();
                s = System.Net.WebUtility.HtmlDecode(s);
                string start = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                int p1 = s.IndexOf(start);
                string end = "</AutoUpdate>";
                int p2 = s.IndexOf(end, p1);
                s = s.Substring(p1, p2 - p1 + end.Length);
                s = s.Replace("<br />", "\n");
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(s);

                XmlNode VersionNode = XmlDoc.SelectSingleNode(@"//Version");
                string version = VersionNode.InnerText;

                XmlNode Information = XmlDoc.SelectSingleNode(@"//Information");
                string information = Information.InnerText;

                Version curVersion = Assembly.GetEntryAssembly().GetName().Version;
                Version newVersion = new Version(version);
                if (newVersion > curVersion)
                {
                    string prompt = String.Format("New version detected.\r\nDo you want to update now?\r\n\r\nNew version: {0}\r\nYour version: {1}\r\n\r\nInformation: \r\n{2}\r\n",
                        newVersion.ToString(), curVersion.ToString(), String.IsNullOrEmpty(information) ? "Author is lazy, doesn't leave any information." : information);

                    if (SimpleMessage.Confirm(prompt) == DialogResult.Yes)
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = up.HomeUrl;
                        p.Start();
                    }
                }
                else
                {
                    if (!up.Silent)
                    {
                        SimpleMessage.ShowInfo("You already have latest version.", "Check for Update");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!up.Silent)
                {
                    SimpleMessage.ShowException(ex, SimpleDotNet.ReportUrl);
                }
            }

        }//updateThread()

    }//end of class
}
