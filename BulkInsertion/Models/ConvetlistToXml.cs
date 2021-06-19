using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace BulkInsertion.Models
{
    public static class ConvetlistToXml
    {
        public static String ObjectToXMLGeneric<T>(T obj)
        {

            if (obj == null)
            {
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                //Encoding = new UnicodeEncoding(false, false), // no BOM in a .NET string
                Indent = false,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8


            };
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, obj);
                }
                string xml = textWriter.ToString();
                return xml;
            }
        }

        
    }
}