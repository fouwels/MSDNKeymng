using System;
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
		const string fileName = "KeysExport.xml";
        public void Main(string[] args)
		{
			var xml = File.ReadAllText(fileName); //yahyah, streamreading is faster, I know

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

			while (true)
			{
				//Console.Clear(); // no console.clear in dnx core, wut?

				List<Models.Product_Key> products = new List<Models.Product_Key>();
				Console.WriteLine("Enter name of product");

				while (products.Count == 0)
				{
					var input = Console.ReadLine();
					products = obj.Product_Keys.Where(x => x.Name.ToLower().Contains(input)).ToList();

					if (products.Count == 0)
					{
						Console.WriteLine("Product not found, try again");
					}
				}
				Console.WriteLine("found:");

				var index = 0;
				foreach (var irem in products)
				{
					Console.WriteLine(index.ToString() + ": " + irem.Name);
					index++;
				}
				Console.WriteLine("Enter index of product");

				var validEntrySelected = false;
				Models.Product_Key selectedProduct = new Models.Product_Key();

                while (validEntrySelected == false)
				{
					try
					{
						selectedProduct = products[Convert.ToInt32(Console.ReadLine())];
						validEntrySelected = true;
					}
					catch
					{
						Console.WriteLine("You suck... try again");
					}
				}

				var latestKey = selectedProduct.Keys.FirstOrDefault();

				if (latestKey != null)
				{
					Console.WriteLine("\n==========");
					Console.WriteLine(JsonConvert.SerializeObject(latestKey, Newtonsoft.Json.Formatting.Indented));
					Console.WriteLine("==========\n");

					Console.Write("Mark is used? [Y/N]  ");
					if (Console.ReadLine().ToLower().Contains("y"))
					{
						Console.WriteLine("Marking as used. Press key to reset [ignore. disabled due to rendering junk in the xml, will fix later]");
						//mark as used
						foreach (var x in xmldoc.DocumentElement.ChildNodes)
						{
							foreach (var key in ((XmlElement)x).ChildNodes)
							{
								if (((XmlElement)key)?.InnerText == latestKey.Value)
								{
									((XmlElement)key).SetAttribute("IsUsed", "TRUE");
								}
							}

							//File.WriteAllText(fileName, xmldoc.Value );
							
						}
					}
					else
					{
						Console.WriteLine("\nNOT Marking as used. Press key to reset\n");
					}
				}
				else
				{
					Console.WriteLine("\n==========");
					Console.WriteLine("No product keys remaining for this product");
					Console.WriteLine("==========\n");

					Console.WriteLine("Press key to reset\n");
				}

				Console.ReadLine();

			}
		}
	}
}
