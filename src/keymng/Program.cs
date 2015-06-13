using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace keymng
{
	public class Program
	{
		public void Main(string[] args)
		{
			var xml = File.ReadAllText(@"KeysExport.xml"); //yahyah, streamreading is faster, I know

			var xmldoc = new XmlDocument();
			xmldoc.LoadXml(xml);

			var obj = new Models.KeyCollection();
			obj.Product_Keys = new List<Models.Product_Key>();

			foreach (var node in xmldoc.DocumentElement.ChildNodes)
			{
				var x = (XmlElement)node;
				var keymodel = new Models.Product_Key
				{
					Name = x.Attributes["Name"].InnerText,
					Keys = new List<Models.Key>()
				};

				foreach (var key in x.ChildNodes)
				{
					var y = (XmlElement)key;
					var usd = y?.Attributes["IsUsed"]?.InnerText;

					if ((new List<string> { "TRUE", "true", "True" }).Contains(usd) != true) //casting .tolower would break if null
					{
						keymodel.Keys.Add(new Models.Key
						{
							ID = y.Attributes["ID"].InnerText,
							Type = y.Attributes["Type"].InnerText,
							ClaimedDate = y.Attributes["ClaimedDate"].InnerText,
							Value = y.InnerText
						});
					}
					else
					{
						Debug.WriteLine("Skipped used key");
					}
				}
				obj.Product_Keys.Add(keymodel);

			}
		}
	}
}
