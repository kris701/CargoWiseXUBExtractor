using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CargoWiseXUBExtractor
{
	public static class XMLHelper
	{
		public static string ConvertXDocumentToString(XDocument doc)
		{
			return ConvertXElementToString(doc.Root);
		}

		public static string ConvertXElementToString(XElement element)
		{
			string result;

			// Set the declaration for the document to indicate that it's using UTF-8 encoding.
			if (element.Document != null)
				element.Document.Declaration = new XDeclaration("1.0", "utf-8", null);

			// Save the XElement to a StringWriter and get the resulting string.
			using (StringWriter writer = new StringWriter())
			{
				element.Save(writer);
				result = writer.ToString();
			}

			// Replace the declaration, which is set to UTF-16 by default, with one that specifies UTF-8 encoding.
			result = result.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?>");

			return result;
		}
	}
}
