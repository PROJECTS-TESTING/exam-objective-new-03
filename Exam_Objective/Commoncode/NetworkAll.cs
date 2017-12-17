using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Web;


namespace Networks
{
    class NetworkAll
    {
       
    }
    class NetworkClient
    {
       
        public static string GetIPClien() => IPClient();

        private static string IPClient()
        {
           HttpContext context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

    }

    class NetworkHost
    {
        public static string GetIPHost() => IPHosts();
        public static string GetMacHost() => MacHost();

        private static string IPHosts()
        {
            IPHostEntry host;
            string ipaddress = "";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress item in host.AddressList)
            {
                if (item.AddressFamily.ToString().Equals(ProtocolFamily.InterNetwork.ToString()))
                {

                    ipaddress = item.ToString();
                }
            }

            return ipaddress;
        }

        private static string MacHost()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

    }
    
}
