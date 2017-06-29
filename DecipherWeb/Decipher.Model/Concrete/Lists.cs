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
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Lists

        public ListHolder GetListHolder()
        {
            try
            {
                return new ListHolder
                {
                    Types = ListTypes("", "Any").Select(n => new ListItem { Value = n.Value, Text = n.Text}).ToList(),
                    Distance = ListPlaceDistances("", "Any").Select(n => new ListItem { Value = n.Value, Text = n.Text }).ToList(),
                    Diversity = ListDiversityIndexes("", "Any").Select(n => new ListItem { Value = n.Value, Text = n.Text }).ToList()
                };
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        # endregion
    }
}
