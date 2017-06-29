using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using System.Xml;

namespace Decipher.Model.Entities
{
    public class EmailSettings
    {
        public string SmtpFrom { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpEnableSSL { get; set; }
    }

    public class EmailField
    {
        public string Name { get; set; }
        public string ProperName { get; set; }
        public string Value { get; set; }
    }

    public class TemplateMailMessageNet : System.Net.Mail.MailMessage
    {
        private const string TOKEN_MESSAGE = "message";
        private const string TOKEN_HEADER = "header";
        private const string TOKEN_FROM = "from";
        private const string TOKEN_NAME = "name";
        private const string TOKEN_RECIPIENTS = "recipients";
        private const string TOKEN_TO = "to";
        private const string TOKEN_CC = "cc";
        private const string TOKEN_BCC = "bcc";
        private const string TOKEN_SUBJECT = "subject";
        private const string TOKEN_PRIORITY = "priority";
        private const string TOKEN_BODY = "body";
        private const string TOKEN_FORMAT = "format";
        private const string TOKEN_ENCODING = "encoding";

        new private Hashtable Fields
        {
            get { return null; }
        }


        /// <summary>
        /// Executes a group of substitution replacements using a <see cref="System.Collections.Specialized.NameValueCollection"/>
        /// </summary>
        public void AddReplacementCollection(NameValueCollection collection)
        {
            //if(IsMassCommunication && IsRapidDisbursement){
            //    this.PrepareMassCommunication(collection);
            //}else{
            //    this.PrepareSingleCommunication(collection);
            //}
            for (int i = 0; i < collection.Count; i++)
            {
                string name = collection.GetKey(i);
                string replacement = String.Empty;
                try
                {
                    replacement = collection[i];
                }
                catch
                {
                    // value was probably null
                }
                if (replacement == null)
                {
                    replacement = String.Empty;
                }
                AddReplacement(name, replacement);
            }
        }

        /// <summary>
        /// Executes a substitution replacement in the message body using a name/replacement pair.
        /// </summary>
        /// <remarks>
        /// <para>Substitution placeholders in the message body text must use the format ${name} where "name" contains only letters and numbers.</para>
        /// </remarks>
        /// <param name="name">The name of the message body token to replace. Must contain only letters and numbers (Ex. "username")</param>
        /// <param name="replacement">The string to replace the message body token with.</param>
        public void AddReplacement(string name, string replacement)
        {
            if (replacement == null)
            {
                replacement = String.Empty;
            }
            VerifyName(name);
            string pattern = "\\$\\{" + name + "\\}";

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
            base.Body = regex.Replace(base.Body, replacement);
        }

        private void VerifyName(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                if (!Char.IsLetterOrDigit(name[i]))
                {
                    throw new ApplicationException("Invalid replacement name: \"" + name + "\". Only letters and numbers are allowed.");
                }
            }
        }

        private System.Text.Encoding ParseEncoding(string value)
        {
            switch (value.Trim().ToLower())
            {
                case "ascii":
                    return System.Text.Encoding.ASCII;
                case "utf8":
                    return System.Text.Encoding.UTF8;
                case "utf7":
                    return System.Text.Encoding.UTF7;
                case "unicode":
                    return System.Text.Encoding.Unicode;
                default:
                    return System.Text.Encoding.Default;
            }
        }

        private MailPriority ParsePriority(string value)
        {
            switch (value.Trim().ToLower())
            {
                case "low":
                    return MailPriority.Low;
                case "high":
                    return MailPriority.High;
                default:
                    return MailPriority.Normal;
            }
        }

        //private MailFormat ParseFormat(string value)
        //{
        //    switch (value.Trim().ToLower())
        //    {
        //        case "html":
        //            return MailFormat.Html;
        //        default:
        //            return MailFormat.Text;
        //    }
        //}

        private string ParseEmail(XmlReader reader)
        {
            string name = reader.GetAttribute(TOKEN_NAME);
            string email = reader.ReadString();

            if (email == null || email.Length == 0)
            {
                return String.Empty;
            }

            if (name != null && name.Length > 0)
            {
                return String.Format("{0} <{1}>", name, email);
            }
            else
            {
                return email;
            }
        }

        private string ReadCDataNode(XmlReader reader)
        {
            reader.MoveToContent();
            string thisElement = reader.LocalName;

            while (reader.Read() && reader.NodeType != System.Xml.XmlNodeType.CDATA)
            {
                if (reader.LocalName == thisElement && reader.NodeType == XmlNodeType.EndElement)
                {
                    // Reader reached the end of the element it started reading from, with no CDATA node found
                    return String.Empty;
                }
            }

            return reader.Value.Trim();
        }

        private void Initialize(XmlReader reader)
        {
            if (reader.ReadState != ReadState.Initial)
            {
                throw new ApplicationException("The specified XmlReader must be in the initial, unread state.");
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case TOKEN_FROM:
                            base.From = new MailAddress(ParseEmail(reader));
                            break;
                        case TOKEN_RECIPIENTS:
                            System.Text.StringBuilder to = new System.Text.StringBuilder(35);
                            System.Text.StringBuilder cc = new System.Text.StringBuilder(35);
                            System.Text.StringBuilder bcc = new System.Text.StringBuilder(35);

                            while (reader.Read())
                            {
                                if (reader.LocalName == TOKEN_RECIPIENTS && reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }
                                switch (reader.LocalName)
                                {
                                    case TOKEN_TO:
                                        if (to.Length > 0)
                                        {
                                            to.Append(", ");
                                        }
                                        to.Append(ParseEmail(reader));
                                        break;
                                    case TOKEN_CC:
                                        if (cc.Length > 0)
                                        {
                                            cc.Append(", ");
                                        }
                                        cc.Append(ParseEmail(reader));
                                        break;
                                    case TOKEN_BCC:
                                        if (bcc.Length > 0)
                                        {
                                            bcc.Append(", ");
                                        }
                                        bcc.Append(ParseEmail(reader));
                                        break;
                                }
                            }
                            //if (to.Length > 0)
                            //{
                            //    base.To = to.ToString();
                            //}
                            //if (cc.Length > 0)
                            //{
                            //    base.Cc = cc.ToString();
                            //}
                            //if (bcc.Length > 0)
                            //{
                            //    base.Bcc = bcc.ToString();
                            //}
                            break;
                        case TOKEN_SUBJECT:
                            base.Subject = ReadCDataNode(reader);
                            break;
                        case TOKEN_PRIORITY:
                            base.Priority = ParsePriority(reader.ReadString());
                            break;
                        case TOKEN_BODY:
                            //base.BodyFormat = ParseFormat(reader.GetAttribute(TOKEN_FORMAT));
                            base.BodyEncoding = ParseEncoding(reader.GetAttribute(TOKEN_ENCODING));
                            base.Body = ReadCDataNode(reader);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="TemplateMailMessage"/> object using a specified physical document path
        /// </summary>
        public TemplateMailMessageNet(string templatePath)
        {
            System.Xml.XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(templatePath);
                Initialize(reader);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="TemplateMailMessage"/> object using a specified <see cref="System.Xml.XmlReader"/>
        /// </summary>
        public TemplateMailMessageNet(XmlReader reader)
        {
            Initialize(reader);
        }

        /// <summary>
        /// Intantiates a new <see cref="TemplateMailMessage"/> object using a specified <see cref="System.IO.Stream"/>
        /// </summary>
        public TemplateMailMessageNet(System.IO.Stream stream)
        {
            System.Xml.XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(stream);
                Initialize(reader);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
            }
        }
    }
}
