using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Email

        public string BuildFromForm(NameValueCollection values)
        {
            return BuildFromForm(values, new List<string>(), new List<string>());
        }

        public string BuildFromForm(NameValueCollection values, bool reorder)
        {
            return BuildFromForm(values, new List<string>(), new List<string>(), reorder);
        }

        public string BuildFromForm(NameValueCollection values, List<string> ignoreValues)
        {
            return BuildFromForm(values, ignoreValues, new List<string>());
        }

        public string BuildFromForm(NameValueCollection values, List<string> ignoreValues, List<string> properNamesCommaDelimited)
        {
            return BuildFromForm(values, ignoreValues, properNamesCommaDelimited, true);
        }

        public string BuildFromForm(NameValueCollection values, List<string> ignoreValues, List<string> properNamesCommaDelimited, bool reorder)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ignoreValues.Add("Submit");
            List<EmailField> list = new List<EmailField>();
            foreach (string key in values.AllKeys)
            {
                bool valid = true;
                if (ignoreValues != null && ignoreValues.Count > 0)
                {
                    foreach (string ignoreValue in ignoreValues)
                    {
                        if (ignoreValue == key)
                        {
                            valid = false;
                        }
                    }
                }
                if (valid == true)
                {
                    EmailField fld = new EmailField { Name = key, Value = values[key] };
                    if (properNamesCommaDelimited != null && properNamesCommaDelimited.Count > 0)
                    {
                        fld.ProperName = properNamesCommaDelimited[i];
                    }
                    list.Add(fld);
                }
                i++;
            }
            sb.Append("<table>\n");
            if (reorder)
            {
                list = list.OrderBy(n => n.Name).ToList();
            }
            foreach (EmailField field in list)
            {
                string name = field.Name;
                string value = field.Value;
                if (!String.IsNullOrEmpty(field.ProperName))
                {
                    name = field.ProperName;
                }
                if (String.IsNullOrEmpty(value))
                {
                    value = "&nbsp;";
                }
                sb.Append("<tr><td class=\"label\" align=\"right\">" + name + ":</td><td class=\"field\">" + value + "</td></tr>\n");
            }
            sb.Append("</table>\n");
            return sb.ToString();
        }
        public string BuildHTMLFromForm(NameValueCollection values, List<string> ignoreValues, Dictionary<string, string> properNames)
        {
            return BuildHTMLFromForm(values, ignoreValues, properNames, true);
        }

        public string BuildHTMLFromForm(NameValueCollection values, List<string> ignoreValues, Dictionary<string, string> properNames, bool? order)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ignoreValues.Add("Submit");
            List<EmailField> list = new List<EmailField>();
            foreach (string key in values.AllKeys)
            {
                try
                {
                    bool valid = true;
                    if (ignoreValues != null && ignoreValues.Count > 0)
                    {
                        foreach (string ignoreValue in ignoreValues)
                        {
                            if (ignoreValue == key)
                            {
                                valid = false;
                            }
                        }
                    }
                    if (valid == true)
                    {
                        EmailField fld = new EmailField { Name = key, Value = values[key] };
                        if (properNames.ContainsKey(fld.Name))
                        {
                            fld.ProperName = properNames[fld.Name];
                        }
                        list.Add(fld);
                    }
                }
                catch { }
                i++;
            }
            sb.Append("<table>\n");
            if (!order.GetValueOrDefault())
            {
                list = list.OrderBy(n => n.Name).ToList();
            }
            foreach (EmailField field in list)
            {
                string name = field.Name;
                string value = field.Value;
                if (!String.IsNullOrEmpty(field.ProperName))
                {
                    name = field.ProperName;
                }
                if (String.IsNullOrEmpty(value))
                {
                    value = "&nbsp;";
                }
                sb.Append("<tr><td class=\"label\" align=\"right\">" + name + ":</td><td class=\"field\">" + value + "</td></tr>\n");
            }
            sb.Append("</table>\n");
            return sb.ToString();
        }

        public string BuildHTMLFromForm(NameValueCollection values, List<string> ignoreValues, Dictionary<string, string> properNames, bool reOrder)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ignoreValues.Add("Submit");
            List<EmailField> list = new List<EmailField>();
            foreach (string key in values.AllKeys)
            {
                try
                {
                    bool valid = true;
                    if (ignoreValues != null && ignoreValues.Count > 0)
                    {
                        foreach (string ignoreValue in ignoreValues)
                        {
                            if (ignoreValue == key)
                            {
                                valid = false;
                            }
                        }
                    }
                    if (valid == true)
                    {
                        EmailField fld = new EmailField { Name = key, Value = values[key] };
                        if (properNames.ContainsKey(fld.Name))
                        {
                            fld.ProperName = properNames[fld.Name];
                        }
                        list.Add(fld);
                    }
                }
                catch { }
                i++;
            }
            sb.Append("<table cellpadding=\"3\" cellspacing=\"3\">\n");
            if (reOrder)
            {
                list = list.OrderBy(n => n.Name).ToList();
            }
            foreach (EmailField field in list)
            {
                string name = field.Name;
                string value = field.Value;
                if (!String.IsNullOrEmpty(field.ProperName))
                {
                    name = field.ProperName;
                }
                if (String.IsNullOrEmpty(value))
                {
                    value = "&nbsp;";
                }
                sb.Append("<tr><td valign=\"top\" class=\"label\" align=\"right\">" + name);
                if (name.LastIndexOf(":") != (name.Length - 1))
                { sb.Append(":"); }
                sb.Append("</td><td valign=\"top\" class=\"field\">" + value + "</td></tr>\n");
            }
            sb.Append("</table>\n");
            return sb.ToString();
        }

        public string BuildHTMLFromDictionary(Dictionary<string, string> values)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            List<EmailField> list = new List<EmailField>();
            foreach (string key in values.Keys)
            {
                EmailField fld = new EmailField { Name = key, Value = values[key] };
                list.Add(fld);
                i++;
            }
            sb.Append("<table>\n");
            foreach (EmailField field in list)
            {
                string name = field.Name;
                string value = field.Value;
                if (!String.IsNullOrEmpty(field.ProperName))
                {
                    name = field.ProperName;
                }
                if (String.IsNullOrEmpty(value))
                {
                    value = "&nbsp;";
                }
                sb.Append("<tr><td class=\"label\" align=\"right\">" + name + ":</td><td class=\"field\">" + value + "</td></tr>\n");
            }
            sb.Append("</table>\n");
            return sb.ToString();
        }

        public string BuildHTMLFromObject(object entity, List<string> ignoreValues, Dictionary<string, string> properNames)
        {
            return BuildHTMLFromObject(entity, ignoreValues, properNames, new Dictionary<string, string>());
        }

        public string BuildHTMLFromObject(object entity, List<string> ignoreValues, Dictionary<string, string> properNames, Dictionary<string, string> additionalFields)
        {
            HttpContext.Current.Trace.Warn("build HTML from object");
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ignoreValues.Add("Submit");
            List<EmailField> list = new List<EmailField>();
            foreach (string key in additionalFields.Keys)
            {
                EmailField field = new EmailField { Name = key, ProperName = key, Value = additionalFields[key] };
                list.Add(field);
            }
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                try
                {
                    HttpContext.Current.Trace.Warn("checking " + prop.Name);
                    object val = prop.GetValue(prop, null);
                    HttpContext.Current.Trace.Warn("got object");
                    HttpContext.Current.Trace.Warn("object type: " + val.GetType());
                    string str = val.ToString();
                    HttpContext.Current.Trace.Warn("value: " + str);
                    bool valid = true;
                    if (ignoreValues != null && ignoreValues.Count > 0)
                    {
                        foreach (string ignoreValue in ignoreValues)
                        {
                            if (ignoreValue.ToLower() == prop.Name.ToLower())
                            {
                                valid = false;
                                HttpContext.Current.Trace.Warn("don't show " + prop.Name);
                            }
                        }
                    }
                    if (valid == true)
                    {
                        HttpContext.Current.Trace.Warn(prop.Name + " is valid");
                        EmailField fld = new EmailField { Name = prop.Name, Value = str };
                        if (properNames.ContainsKey(fld.Name))
                        {
                            fld.ProperName = properNames[fld.Name];
                            HttpContext.Current.Trace.Warn("Proper name: " + fld.ProperName);
                        }
                        list.Add(fld);
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Trace.Warn(ex.ToString());
                }
                i++;
            }
            sb.Append("<table>\n");
            foreach (EmailField field in list)
            {
                string name = field.Name;
                string value = field.Value;
                if (!String.IsNullOrEmpty(field.ProperName))
                {
                    name = field.ProperName;
                }
                if (String.IsNullOrEmpty(value))
                {
                    value = "&nbsp;";
                }
                sb.Append("<tr><td class=\"label\" style=\"vertical-align:top\">" + name + ":</td><td class=\"field\">" + value + "</td></tr>\n");
            }
            sb.Append("</table>\n");
            return sb.ToString();
        }

        public string BuildHTMLFromForm(NameValueCollection values, List<string> ignoreValues, Dictionary<string, string> properNames, bool reOrder, bool alignInCenter)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ignoreValues.Add("Submit");
            List<EmailField> list = new List<EmailField>();
            foreach (string key in values.AllKeys)
            {
                bool valid = true;
                if (ignoreValues != null && ignoreValues.Count > 0)
                {
                    foreach (string ignoreValue in ignoreValues)
                    {
                        if (ignoreValue == key)
                        {
                            valid = false;
                        }
                    }
                }
                if (valid == true)
                {
                    EmailField fld = new EmailField { Name = key, Value = values[key] };
                    if (properNames.ContainsKey(fld.Name))
                    {
                        fld.ProperName = properNames[fld.Name];
                    }
                    list.Add(fld);
                }
                i++;
            }
            sb.Append("<table>\n");
            if (reOrder)
            {
                list = list.OrderBy(n => n.Name).ToList();
            }
            foreach (EmailField field in list)
            {
                string name = field.Name;
                string value = field.Value;
                if (!String.IsNullOrEmpty(field.ProperName))
                {
                    name = field.ProperName;
                }
                if (String.IsNullOrEmpty(value))
                {
                    value = "&nbsp;";
                }
                if (alignInCenter)
                {
                    sb.Append("<tr><td class=\"label\" align=\"right\">" + name + ":</td><td class=\"field\">" + value + "</td></tr>\n");
                }
                else
                {
                    sb.Append("<tr><td class=\"label\" style=\"vertical-align:top\">" + name + ":</td><td class=\"field\">" + value + "</td></tr>\n");
                }
            }
            sb.Append("</table>\n");
            return sb.ToString();
        }

        public NameValueCollection CreateReplacementsFromForm(NameValueCollection values)
        {
            NameValueCollection replacements = new NameValueCollection();
            foreach (string key in values.Keys)
            {
                replacements.Add(key, values[key]);
            }
            return replacements;
        }

        public NameValueCollection CreateReplacementsFromObject(Object obj)
        {
            NameValueCollection replacements = new NameValueCollection();
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                try
                {
                    object val = prop.GetValue(prop, null);
                    string str = val.ToString();
                    replacements.Add(prop.Name, str);
                }
                catch { }
            }
            return replacements;
        }

        private void BaseEmailSend(List<string> recipients, List<string> ccRecipients, string subject, NameValueCollection replacements, string templateFullPath, string smtpFrom)
        {
            BaseEmailSend(recipients, ccRecipients, subject, replacements, templateFullPath, smtpFrom, new List<string>(), null);
        }

        private void BaseEmailSend(List<string> recipients, List<string> ccRecipients, string subject, NameValueCollection replacements, string templateFullPath, string smtpFrom, List<string> bccRecipients)
        {
            BaseEmailSend(recipients, ccRecipients, subject, replacements, templateFullPath, smtpFrom, bccRecipients, null);
        }

        public void SimpleEmailSend(string from, string recipients, string subject, string body)
        {
            try
            {
                string smtpServer = SmtpServer;
                int smtpPort = SmtpPort;
                string smtpUsername = SmtpUsername;
                string smtpPassword = SmtpPassword;
                bool smtpSSL = SmtpEnableSSL;
                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (!String.IsNullOrEmpty(smtpUsername) && !String.IsNullOrEmpty(smtpPassword))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }
                smtp.EnableSsl = smtpSSL;
                smtp.ServicePoint.MaxIdleTime = 1;
                MailMessage message = new MailMessage(from, recipients, subject, body);
                message.IsBodyHtml = true;
                message.Body = body;
                smtp.Send(message);
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
        }

        private void BaseEmailSend(List<string> recipients, List<string> ccRecipients, string subject, NameValueCollection replacements, string templateFullPath, string smtpFrom, List<string> bccRecipients, EmailSettings settings)
        {
            try
            {
                HttpContext.Current.Trace.Warn("base email send");
                string emailFrom = SmtpFrom;
                if (!String.IsNullOrEmpty(smtpFrom))
                {
                    emailFrom = smtpFrom;
                }
                else if (settings != null && !String.IsNullOrEmpty(settings.SmtpFrom))
                {
                    emailFrom = settings.SmtpFrom;
                }
                HttpContext.Current.Trace.Warn("sending from " + emailFrom);
                TemplateMailMessageNet message = new TemplateMailMessageNet(templateFullPath);
                HttpContext.Current.Trace.Warn("template path: " + templateFullPath);
                HttpContext.Current.Trace.Warn(replacements.Count.ToString() + " replacements");
                message.AddReplacementCollection(replacements);
                foreach (string recip in recipients)
                {
                    if (!String.IsNullOrEmpty(recip))
                    {
                        message.To.Add(recip);
                        HttpContext.Current.Trace.Warn("sending to " + recip);
                    }
                }
                foreach (string ccRecip in ccRecipients)
                {
                    if (!String.IsNullOrEmpty(ccRecip))
                    {
                        message.CC.Add(ccRecip);
                    }
                }
                foreach (string bccRecip in bccRecipients)
                {
                    if (!String.IsNullOrEmpty(bccRecip))
                    {
                        message.Bcc.Add(bccRecip);
                    }
                }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = message.Body.Replace("${Message}", "$0");
                string smtpServer = SmtpServer;
                int smtpPort = SmtpPort;
                string smtpUsername = SmtpUsername;
                string smtpPassword = SmtpPassword;
                bool smtpSSL = SmtpEnableSSL;
                if (settings != null)
                {
                    smtpServer = settings.SmtpServer;
                    smtpPort = settings.SmtpPort;
                    smtpUsername = settings.SmtpUsername;
                    smtpPassword = settings.SmtpPassword;
                    smtpSSL = settings.SmtpEnableSSL;
                    message.From = new MailAddress(settings.SmtpFrom);
                    HttpContext.Current.Trace.Warn("setting custom settings for " + settings.SmtpServer);
                }
                else
                {
                    message.From = new MailAddress(emailFrom);
                }
                HttpContext.Current.Trace.Warn("starting smtp send for " + smtpServer);
                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (!String.IsNullOrEmpty(smtpUsername) && !String.IsNullOrEmpty(smtpPassword))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }
                smtp.EnableSsl = smtpSSL;
                smtp.ServicePoint.MaxIdleTime = 1;
                smtp.Send(message);
                smtp.Dispose();
                message.Dispose();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
        }

        private string SmtpFrom
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpFrom"]))
                {
                    return ConfigurationManager.AppSettings["SmtpFrom"];
                }
                else
                {
                    return null;
                }
            }
        }

        private string SmtpServer
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpServer"]))
                {
                    return ConfigurationManager.AppSettings["SmtpServer"];
                }
                else
                {
                    return null;
                }
            }
        }

        private string SmtpUsername
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpUsername"]))
                {
                    return ConfigurationManager.AppSettings["SmtpUsername"];
                }
                else
                {
                    return null;
                }
            }
        }

        private string SmtpPassword
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpPassword"]))
                {
                    return ConfigurationManager.AppSettings["SmtpPassword"];
                }
                else
                {
                    return null;
                }
            }
        }

        private int SmtpPort
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpPort"]))
                {
                    try
                    {
                        return Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
                    }
                    catch
                    {
                        return 25;
                    }
                }
                else
                {
                    return 25;
                }
            }
        }

        private bool SmtpEnableSSL
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpEnableSSL"]))
                {
                    try
                    {
                        return Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSSL"]);
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }


        public void SendEmail(string emailAddress, string subject, string msg)
        {
            SendEmailNoTemplate(emailAddress, subject, msg, ConfigurationManager.AppSettings["SmtpFrom"]);
        }

        public void SendEmail(string emailAddress, string subject, string msg, EmailSettings settings)
        {
            SendEmailNoTemplate(emailAddress, subject, msg, settings.SmtpFrom, String.Empty, settings);
        }

        //public void SendEmail(string emailAddress, string subject, string msg)
        //{
        //    SendEmailNoTemplate(emailAddress, subject, msg, ConfigurationManager.AppSettings["SmtpFrom"]);
        //}

        public void SendEmailExtras(string emailAddress, string subject, string msg, string CC)
        {
            SendEmailNoTemplate(emailAddress, subject, msg, ConfigurationManager.AppSettings["SmtpFrom"], CC);
        }

        public void SendEmailNoTemplate(string emailAddress, string subject, string msg, string smtp)
        {
            SendEmailNoTemplate(emailAddress, subject, msg, smtp, "", null);
        }

        public void SendEmailNoTemplate(string emailAddress, string subject, string msg, string smtp, string CC)
        {
            SendEmailNoTemplate(emailAddress, subject, msg, smtp, CC, null);
        }

        public void SendEmailNoTemplate(string emailAddress, string subject, string msg, string smtp, string CC, EmailSettings settings)
        {
            try
            {
                string template = "";
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Content/emailTemplates/Generic.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("~/Content/emailTemplates/Generic.xml");
                }
                else if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("/Content/emailTemplates/Generic.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("/Content/emailTemplates/Generic.xml");
                }
                else if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/media/emailTemplates/Generic.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("~/media/emailTemplates/Generic.xml");
                }
                else if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Areas/GivingBooth/Content/emailTemplates/Generic.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("~/Areas/GivingBooth/Content/emailTemplates/Generic.xml");
                }
                else if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Areas/GivingBooth/Content/emailTemplates/Default.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("~/Areas/GivingBooth/Content/emailTemplates/Default.xml");
                }
                NameValueCollection replacements = new NameValueCollection();
                replacements.Add("Message", msg);
                try
                {
                    replacements.Add("SiteAddress", ConfigurationManager.AppSettings["SiteAddress"]);
                }
                catch { }
                BaseEmailSend(emailAddress.Split(',').ToList(), CC.Split(',').ToList(), subject, replacements, template, smtp, new List<string>(), settings);
            }
            catch
            {
                throw;
            }
        }

        public void SendEmail(string emailAddress, string subject, string msg, string header)
        {
            SendEmail(emailAddress, subject, msg, header, ConfigurationManager.AppSettings["SmtpFrom"]);
        }

        public void SendEmail(string emailAddress, string subject, string msg, string header, string smtpfrom)
        {
            try
            {
                string template = "";
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Content/emailTemplates/Generic.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("~/Content/emailTemplates/Generic.xml");
                }
                else if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/media/emailTemplates/Generic.xml")))
                {
                    template = System.Web.HttpContext.Current.Server.MapPath("~/media/emailTemplates/Generic.xml");
                }
                NameValueCollection r = new NameValueCollection();
                r.Add("Header", header);
                r.Add("Message", msg);
                try
                {
                    r.Add("SiteAddress", ConfigurationManager.AppSettings["SiteAddress"]);
                }
                catch { }
                BaseEmailSend(emailAddress.Split(',').ToList(), new List<string>(), subject, r, template, smtpfrom);
            }
            catch
            {
                throw;
            }
        }

        public void SendTemplateEmail(string templateFileName, string emailAddress, string subject, string msg)
        {
            SendTemplateEmail(templateFileName, emailAddress, subject, msg, "");
        }

        public void SendTemplateEmail(string templateFileName, string emailAddress, string subject, string msg, string CC)
        {
            try
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string template = "";
                if (System.IO.File.Exists(context.Server.MapPath("~/Content/emailTemplates/" + templateFileName)))
                {
                    template = context.Server.MapPath("~/Content/emailTemplates/" + templateFileName);
                }
                else if (System.IO.File.Exists(context.Server.MapPath("~/media/emailTemplates/" + templateFileName)))
                {
                    template = context.Server.MapPath("~/media/emailTemplates/" + templateFileName);
                }
                NameValueCollection r = new NameValueCollection();
                r.Add("Message", msg);
                try
                {
                    r.Add("SiteAddress", ConfigurationManager.AppSettings["SiteAddress"]);
                }
                catch { }
                BaseEmailSend(emailAddress.Split(',').ToList(), CC.Split(',').ToList(), subject, r, template, null);
            }
            catch
            {
                throw;
            }
        }

        public void SendTemplateEmail(NameValueCollection replacements, string templateFileName, string emailAddress, string subject, string CC, string BCC, string from)
        {
            try
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string template = "";
                if (System.IO.File.Exists(context.Server.MapPath("~/Content/emailTemplates/" + templateFileName)))
                {
                    template = context.Server.MapPath("~/Content/emailTemplates/" + templateFileName);
                }
                else if (System.IO.File.Exists(context.Server.MapPath("~/media/emailTemplates/" + templateFileName)))
                {
                    template = context.Server.MapPath("~/media/emailTemplates/" + templateFileName);
                }
                BaseEmailSend(emailAddress.Split(',').ToList(), CC.Split(',').ToList(), subject, replacements, template, from, BCC.Split(',').ToList());
            }
            catch
            {
                throw;
            }
        }

        public void SendTemplateEmail(string templateFileName, List<string> recipients, string subject, string msg)
        {
            try
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string template = "";
                if (System.IO.File.Exists(context.Server.MapPath("~/Content/emailTemplates/" + templateFileName)))
                {
                    template = context.Server.MapPath("~/Content/emailTemplates/" + templateFileName);
                }
                else if (System.IO.File.Exists(context.Server.MapPath("~/media/emailTemplates/" + templateFileName)))
                {
                    template = context.Server.MapPath("~/media/emailTemplates/" + templateFileName);
                }
                NameValueCollection r = new NameValueCollection();
                r.Add("Message", msg);
                try
                {
                    r.Add("SiteAddress", ConfigurationManager.AppSettings["SiteAddress"]);
                }
                catch { }
                BaseEmailSend(recipients, new List<string>(), subject, r, template, null);
            }
            catch
            {
                throw;
            }
        }

        public void SendTemplateEmail(NameValueCollection replacements, string templateFileName, string emailAddress, string subject)
        {
            SendTemplateEmail(replacements, templateFileName, emailAddress, subject, "");
        }

        public void SendTemplateEmail(NameValueCollection replacements, string templateFileName, List<string> recipients, string subject)
        {
            string str = "";
            foreach (string r in recipients)
            {
                if (String.IsNullOrEmpty(r))
                {
                    str += ",";
                }
                str += r;
            }
            replacements.Add("IntendedRecipients", str);
            SendTemplateEmail(replacements, templateFileName, str, subject, "");
        }

        public void SendTemplateEmail(NameValueCollection replacements, string templateFileName, string emailAddress, string subject, string CC)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string template = "";
            if (System.IO.File.Exists(context.Server.MapPath("~/Content/emailTemplates/" + templateFileName)))
            {
                template = context.Server.MapPath("~/Content/emailTemplates/" + templateFileName);
            }
            else if (System.IO.File.Exists(context.Server.MapPath("~/media/emailTemplates/" + templateFileName)))
            {
                template = context.Server.MapPath("~/media/emailTemplates/" + templateFileName);
            }
            try
            {
                replacements.Add("SiteAddress", ConfigurationManager.AppSettings["SiteAddress"]);
            }
            catch { }
            BaseEmailSend(emailAddress.Split(',').ToList(), CC.Split(',').ToList(), subject, replacements, template, null);
        }

        public string GenerateEmailBody(string templateFullPath, NameValueCollection replacements)
        {
            try
            {
                TemplateMailMessageNet message = new TemplateMailMessageNet(templateFullPath);
                message.AddReplacementCollection(replacements);
                return message.Body;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
                return String.Empty;
            }
        }

        # endregion
    }
}
