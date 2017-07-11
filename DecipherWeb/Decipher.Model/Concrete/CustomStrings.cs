using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Custom Strings    

        public List<CustomString> GetCustomStrings(string language = "en")
        {
            List<CustomString> list = new List<CustomString>();
            try
            {
                var xml = XElement.Load(HttpContext.Current.Server.MapPath(CustomStringXMLPath));
                foreach (XElement el in xml.Elements("CustomString"))
                {
                    list.Add(ParseCustomString(el));
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return TranslateCustomStrings(list, language);
        }

        public CustomString GetCustomString(string id)
        {
            return GetCustomStrings().Where(n => n.CustomStringID == id).FirstOrDefault();
        }

        private CustomString ParseCustomString(XElement el)
        {
            var entity = new CustomString
            {
                CustomStringID = SafeXMLString(el.Element("CustomStringID")),
                Text = SafeXMLString(el.Element("Text"))
            };
            return entity;
        }

        private string CustomStringXMLPath
        {
            get
            {
                return "~/App_Data/CustomStrings.xml";
            }
        }

        public List<CustomString> TranslateCustomStrings(List<CustomString> customStrings, string language)
        {
            List<CustomString> list = new List<CustomString>();
            try
            {
                if(language == GetConfig("DefaultLanguage"))
                {
                    // already translated to English so no translation necessary
                    list.AddRange(customStrings);
                }
                else
                {
                    var translations = Translations.Where(n => n.TranslationID.IndexOf("CustomString.") == 0).Where(n => n.LanguageID == language).ToList();
                    foreach(var customString in customStrings)
                    {
                        var translation = translations.Where(n => n.TranslationID == "CustomString." + customString.CustomStringID).FirstOrDefault();
                        if(translation == null)
                        {
                            // get translation and store it
                            string str = TranslateString(customString.Text, language);
                            if (!String.IsNullOrEmpty(str))
                            {
                                translation = new Translation
                                {
                                    TranslationID = "CustomString." + customString.CustomStringID,
                                    Text = str,
                                    LanguageID = language
                                };
                                SaveTranslation(translation);
                            }
                        }
                        if(translation != null)
                        {
                            list.Add(new CustomString
                            {
                                CustomStringID = customString.CustomStringID,
                                Text = translation.Text
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        #endregion
    }
}
