using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace nmapToCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Write("usage: nmaptocsv.exe <filename>.xml");
                return;
            }
            XElement xelement = XElement.Load(args[0]);
            List<string> output = new List<string>();

            output.Add("Scan Information:");
            output.Add("Number of services: " + xelement.Element("scaninfo").Attribute("numservices").Value);
            output.Add("Start Time: " + xelement.Attribute("startstr").Value);
            output.Add("Finish Time: " + xelement.Element("runstats").Element("finished").Attribute("timestr").Value);
            output.Add("Scan arguments: " + xelement.Attribute("args").Value);

            IEnumerable<XElement> hosts = xelement.Elements("host");

            output.Add("Host Name,Ip Address,MAC Address,OS Name,OS Family,OS Generation,OS Accuracy,Port,Service Name,Service Product,Service Version,Service Confidence");
            foreach (var host in hosts)
            {
                string name = host.Element("hostnames").Element("hostname").Attribute("name").Value ?? "";
                string mac = "", ip = "";
                if (host.Element("address").Attribute("addrtype").Value == "mac")
                    mac = host.Element("address").Attribute("addr").Value;
                else
                    ip = host.Element("address").Attribute("addr").Value;

                XElement osMatch = host.Element("os").Element("osmatch");
                string osName = osMatch.Attribute("name")?.Value ?? "";
                string osFamily = osMatch.Element("osclass").Attribute("osfamily")?.Value ?? "";
                string osGen = osMatch.Element("osclass").Attribute("osgen")?.Value ?? "";
                string accuracy = osMatch.Element("osclass").Attribute("accuracy")?.Value ?? "";

                //Service Name,Service Product,Service Version,Service Confidence
                IEnumerable<XElement> ports = host.Element("ports").Elements("port");

                foreach (var port in ports)
                {
                    string portId = port.Attribute("portid")?.Value ?? "";
                    string serviceName = port.Element("service").Attribute("name")?.Value ?? "";
                    string serviceProduct = port.Element("service").Attribute("product")?.Value ?? "";
                    string confidence = port.Element("service").Attribute("conf")?.Value ?? "";
                    string version = port.Element("service").Attribute("version")?.Value ?? "";
                    output.Add(name + "," + ip + "," + mac + "," + osName + "," + osFamily + "," + osGen + "," + accuracy + "," + portId + "," + serviceName + "," + serviceProduct + "," + version + "," + confidence);
                }
            }
            System.IO.File.WriteAllLines(args[0].Substring(0, args[0].IndexOf(".")) + ".csv", output);
        }
    }
}
