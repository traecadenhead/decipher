//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Decipher.Model.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Translation
    {
        public string TranslationID { get; set; }
        public string LanguageID { get; set; }
        public string Text { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual Language Language { get; set; }
    }
}