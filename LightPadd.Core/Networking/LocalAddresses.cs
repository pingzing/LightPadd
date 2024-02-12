using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LightPadd.Core.Networking;

public class LocalAddresses
{
    public static List<string> GetLocalIPv4Addresses()
    {
        List<string> ipAddrList = [];
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (
                //item.NetworkInterfaceType == interfaceType &&
                item.OperationalStatus == OperationalStatus.Up
            )
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddrList.Add(ip.Address.ToString());
                    }
                }
            }
        }
        return ipAddrList;
    }
}
