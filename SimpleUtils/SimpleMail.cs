using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using Microsoft.Win32;

namespace SimpleUtils
{
    public class SimpleMail
    {
        #region Properties
        private string _smtpServer = null;

        public string SmtpServer
        {
            get { return _smtpServer; }
        }

        private string _userName = null;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password = null;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private bool _useDefaultCredentials = false;
        public bool UseDefaultCredentials
        {
            get { return _useDefaultCredentials; }
            set { _useDefaultCredentials = value; }
        }

        private int _port = 0;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private MailAddress _fromAddress = null;

        public MailAddress FromAddress
        {
            get { return _fromAddress; }
            set { _fromAddress = value; }
        }
        #endregion Properties

        #region Constructors
        public SimpleMail(string smtpServer, string fromAddress)
        {
            Init(smtpServer, fromAddress, null);
        }

        public SimpleMail(string smtpServer, string fromAddress, string fromDisplayName)
        {
            Init(smtpServer, fromAddress, fromDisplayName);
        }

        protected void Init(string smtpServer, string fromAddress, string fromDisplayName)
        {
            _smtpServer = smtpServer;
            _fromAddress = new MailAddress(fromAddress, fromDisplayName);
        }
        #endregion Constructors

        #region Send Mail Functions
        public void Send(string receipt, string receiptName, string subject, string body)
        {
            Send(receipt, receiptName, subject, body, null, null, null);
        }

        public void Send(string receipt, string receiptName, string subject, string body, string[] attachments)
        {
            Send(receipt, receiptName, subject, body, null, null, attachments);
        }

        public void Send(string receipt, string receiptName, string subject, string body, string cc)
        {
            Send(receipt, receiptName, subject, body, cc, null, null);
        }

        public void Send(string receipt, string receiptName, string subject, string body, string cc, string[] attachments)
        {
            Send(receipt, receiptName, subject, body, cc, null, attachments);
        }

        public void Send(string receipt, string receiptName, string subject, string body, string cc, string bcc, string[] attachments)
        {

            MailMessage msg = new MailMessage();

            msg.From = _fromAddress;

            AddAddresses(receipt, receiptName, msg.To);
            AddAddresses(cc, msg.CC);
            AddAddresses(bcc, msg.Bcc);

            msg.Subject = subject;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Body = body;

            if (attachments != null)
            {
                for (int i = 0; i < attachments.Length; i++)
                {
                    if (String.IsNullOrEmpty(attachments[i])) continue;
                    msg.Attachments.Add(new Attachment(attachments[i]));
                }
            }

            SmtpClient smtpClient = new SmtpClient(_smtpServer);
            if (_port > 0) smtpClient.Port = _port;
            if (IsAuthenticationRequired)
            {
                CredentialCache myCache = new CredentialCache();
                myCache.Add(_smtpServer, smtpClient.Port, "login", new NetworkCredential(UserName, Password));
                smtpClient.Credentials = myCache;
            }
            else
            {
                if (_useDefaultCredentials) smtpClient.UseDefaultCredentials = _useDefaultCredentials;
            }
            smtpClient.Send(msg);
        }

        private bool IsAuthenticationRequired
        {
            get { return !String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password); }
        }

        private char[] _addressSeparator = new char[] { ';' };
        protected void AddAddresses(string addresses, MailAddressCollection mac)
        {
            if (addresses == null) return;

            string[] s = addresses.Split(_addressSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (s != null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    mac.Add(s[i]);
                }
            }

        }

        protected void AddAddresses(string addresses, string displayNames, MailAddressCollection mac)
        {
            if (addresses == null) return;

            string[] addrs = addresses.Split(_addressSeparator, StringSplitOptions.RemoveEmptyEntries);
            string[] names;

            if (String.IsNullOrEmpty(displayNames)) names = null;
            else names = displayNames.Split(_addressSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (addrs != null)
            {
                for (int i = 0; i < addrs.Length; i++)
                {
                    string address = addrs[i].Trim();
                    string name;

                    if (names != null && i < names.Length) name = names[i];
                    else name = address;

                    MailAddress addr = new MailAddress(address, name);
                    mac.Add(addr);
                }
            }
        }

        #endregion Send Mail Functions

        //#region Email Checking functions
        //public bool IsValidEmailAddress(string emailAddress)
        //{
        //    if (!SimpleRegex.IsEmail(emailAddress))
        //    {
        //        return false;
        //    }

        //    bool valid = false;
        //    try
        //    {
        //        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        //        {
        //            //StringBuilder sb = new StringBuilder();

        //            int timeout = 10000;
        //            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
        //            int port = 25;
        //            socket.Connect(_smtpServer, port);
        //            byte[] buffer = new byte[0x400];

        //            int count = socket.Receive(buffer);
        //            string result = Encoding.ASCII.GetString(buffer, 0, count);
        //            //sb.Append(result);

        //            string command = String.Format("HELO {0}\r\n", SimpleSystemInfo.SimpleNetworkInfo.GetHostName());
        //            //sb.Append(command);
        //            byte[] bytes = Encoding.ASCII.GetBytes(command);
        //            socket.Send(bytes);
        //            count = socket.Receive(buffer);
        //            result = Encoding.ASCII.GetString(buffer, 0, count);
        //            //sb.Append(result);

        //            command = String.Format("MAIL FROM:<{0}>\r\n", _fromAddress);
        //            //sb.Append(command);
        //            bytes = Encoding.ASCII.GetBytes(command);
        //            socket.Send(bytes);

        //            count = socket.Receive(buffer);
        //            result = Encoding.ASCII.GetString(buffer, 0, count);
        //            //sb.Append(result);

        //            command = String.Format("RCPT TO:<{0}>\r\n", emailAddress.Trim());
        //            //sb.Append(command);
        //            bytes = Encoding.ASCII.GetBytes(command);
        //            socket.Send(bytes);
        //            count = socket.Receive(buffer);
        //            result = Encoding.ASCII.GetString(buffer, 0, count);
        //            //sb.Append(result);

        //            bool flag = this.IsGoodResponse(result);
        //            command = "QUIT\r\n";
        //            //sb.Append(command);
        //            bytes = Encoding.ASCII.GetBytes(command);
        //            socket.Send(bytes);
        //            count = socket.Receive(buffer);
        //            result = Encoding.ASCII.GetString(buffer, 0, count);
        //            //sb.Append(result);

        //            valid = flag;
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return valid;
        //}

        //private bool IsGoodResponse(string response)
        //{
        //    return response.Trim().StartsWith("250");
        //}
        //#endregion Email Checking functions

        //#region Internet Accounts Manager
        //public class InternetEmailAccount
        //{
        //    public string POP3Server;
        //    public string SMTPServer;
        //    public string SMTPEmailAddress;
        //    public string SMTPDisplayName;
        //}

        //public static InternetEmailAccount GetInternetEmailAccount()
        //{
        //    string defaultEntry = GetInternetEmailAccountDefaultEntry();
        //    if (String.IsNullOrEmpty(defaultEntry)) return null;
        //    return GetInternetEmailAccount(defaultEntry);
        //}

        //public static InternetEmailAccount GetInternetEmailAccount(string entry)
        //{
        //    InternetEmailAccount account = null;
        //    try
        //    {
        //        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Account Manager\Accounts\" + entry))
        //        {
        //            if (key != null)
        //            {
        //                account = new InternetEmailAccount();
        //                account.POP3Server = key.GetValue("POP3 Server", null) as string;
        //                account.SMTPServer = key.GetValue("SMTP Server", null) as string;
        //                account.SMTPEmailAddress = key.GetValue("SMTP Email Address", null) as string;
        //                account.SMTPDisplayName = key.GetValue("SMTP Display Name", null) as string;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    return account;
        //}

        //private static string GetInternetEmailAccountDefaultEntry()
        //{
        //    string s = null;
        //    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Account Manager"))
        //    {
        //        if (key != null)
        //            s = key.GetValue("Default Mail Account", null) as string;
        //    }
        //    return s;
        //}
        //#endregion Internet Accounts Manager

    } //end of class
}
