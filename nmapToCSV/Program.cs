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
            var xelement = XElement.Load(args[0]);
            var output = new List<string>
            {
                "Scan Information:",
                "Number of services: " + xelement.Element("scaninfo")?.Attribute("numservices")?.Value ?? "Not Found",
                "Start Time: " + xelement.Attribute("startstr")?.Value ?? "Not Found",
                "Finish Time: " + xelement.Element("runstats")?.Element("finished")?.Attribute("timestr")?.Value ?? "Not Found",
                "Scan arguments: " + xelement.Attribute("args")?.Value ?? "Not Found"
            };

            var hosts = xelement.Elements("host");

            output.Add("Host Name,Ip Address,MAC Address,OS Name,OS Family,OS Generation,OS Accuracy,Port,Service Name,Service Product,Service Version,Service Confidence");
            foreach (var host in hosts)
            {
                var name = host.Element("hostnames")?.Element("hostname")?.Attribute("name")?.Value ?? "";
                string mac = "", ip = "";
                if (host.Element("address")?.Attribute("addrtype")?.Value == "mac")
                    mac = host.Element("address").Attribute("addr").Value;
                else
                    ip = host.Element("address")?.Attribute("addr")?.Value ?? "No IP or MAC found";

                var osMatch = host.Element("os")?.Element("osmatch");
                var osName = osMatch?.Attribute("name")?.Value ?? "";
                var osFamily = osMatch?.Element("osclass")?.Attribute("osfamily")?.Value ?? "";
                var osGen = osMatch?.Element("osclass")?.Attribute("osgen")?.Value ?? "";
                var accuracy = osMatch?.Element("osclass")?.Attribute("accuracy")?.Value ?? "";

                //Service Name,Service Product,Service Version,Service Confidence
                var ports = host.Element("ports")?.Elements("port");

                output.AddRange(from port in ports
                    let portId = port.Attribute("portid")?.Value ?? ""
                    let serviceName = port.Element("service")?.Attribute("name")?.Value ?? ""
                    let serviceProduct = port.Element("service")?.Attribute("product")?.Value ?? ""
                    let confidence = port.Element("service")?.Attribute("conf")?.Value ?? ""
                    let version = port.Element("service")?.Attribute("version")?.Value ?? ""
                    select name + "," + ip + "," + mac + "," + osName + "," + osFamily + "," + osGen + "," + accuracy + "," + portId + "," + serviceName + "," + serviceProduct + "," + version + "," + confidence
                );
            }
            //Remove old file type and append .csv (or append .csv if no file type found)
            var extensionPos = args[0].LastIndexOf(".");
            if (extensionPos == -1)
                extensionPos = args[0].Length;
            System.IO.File.WriteAllLines(args[0].Substring(0, extensionPos) + ".csv", output);
        }
    }
}
