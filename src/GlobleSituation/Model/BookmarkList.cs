using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GlobleSituation.Model
{
    [XmlType(TypeName = "BookmarkList")]
    public class BookmarkList
    {
        [XmlElement("Bookmark")]
        public List<Bookmark> BookmarkArr { get; set; }

        public BookmarkList()
        {
            BookmarkArr = new List<Bookmark>();
        }
    }
}
