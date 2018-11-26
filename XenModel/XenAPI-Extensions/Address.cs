using System.Collections.Generic;
using System.Linq;

namespace XenAPI
{
    public abstract class Address
    {
        public static List<string> FindIpAddresses(Dictionary<string, string> networks, string device)
        {
            if (networks == null || string.IsNullOrWhiteSpace(device))
                return new List<string>();

            // PR-1373 - VM_guest_metrics.networks is a dictionary of IP addresses in the format:
            // [["0/ip", <IPv4 address>], 
            //  ["0/ipv4/0", <IPv4 address>], ["0/ipv4/1", <IPv4 address>],
            //  ["0/ipv6/0", <IPv6 address>], ["0/ipv6/1", <IPv6 address>]]

            return
                (from network in networks
                    where network.Key.StartsWith(string.Format("{0}/ip", device))
                    orderby network.Key
                    select network.Value).Distinct().ToList();
        }
    }
}
