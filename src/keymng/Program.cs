using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace keymng
{
    public class Program
    {
        public void Main(string[] args)
        {
			XmlDocument itemDoc = new XmlDocument();
			itemDoc.Load(@"KeysExport.xml");

		}
    }
}
