using System;
using System.Net;
using SnmpSharpNet;

namespace snmpget
{
    class Program
    {
        static void Main(string[] args)
        {
            // SNMP community name
            OctetString community = new OctetString("public");

            // Define agent parameters class
            AgentParameters param = new AgentParameters(community);
            // Set SNMP version to 1 (or 2)
            param.Version = SnmpVersion.Ver1;
            // Construct the agent address object
            // IpAddress class is easy to use here because
            //  it will try to resolve constructor parameter if it doesn't
            //  parse to an IP address
            IpAddress agent = new IpAddress("192.168.0.4");

            // Construct target
            UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add("1.3.6.1.2.1.1.1.0"); //sysDescr
            pdu.VbList.Add("1.3.6.1.2.1.1.2.0"); //sysObjectID
            pdu.VbList.Add("1.3.6.1.2.1.1.3.0"); //sysUpTime
            pdu.VbList.Add("1.3.6.1.2.1.1.4.0"); //sysContact
            pdu.VbList.Add("1.3.6.1.2.1.1.5.0"); //sysName

            // Make SNMP request
            SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

            // If result is null then agent didn't reply or we couldn't parse the reply.
            if (result != null)
            {
                // ErrorStatus other then 0 is an error returned by 
                // the Agent - see SnmpConstants for error definitions
                if (result.Pdu.ErrorStatus != 0)
                {
                    // agent reported an error with the request
                    Console.WriteLine("Error in SNMP reply. Error {0} index {1}",
                        result.Pdu.ErrorStatus,
                        result.Pdu.ErrorIndex);
                }
                else
                {
                    // Reply variables are returned in the same order as they were added
                    //  to the VbList
                    Console.WriteLine("sysDescr({0}) ({1}): {2}",
                        result.Pdu.VbList[0].Oid.ToString(),
                        SnmpConstants.GetTypeName(result.Pdu.VbList[0].Value.Type),
                        result.Pdu.VbList[0].Value.ToString());
                    Console.WriteLine("sysObjectID({0}) ({1}): {2}",
                        result.Pdu.VbList[1].Oid.ToString(),
                        SnmpConstants.GetTypeName(result.Pdu.VbList[1].Value.Type),
                        result.Pdu.VbList[1].Value.ToString());
                    Console.WriteLine("sysUpTime({0}) ({1}): {2}",
                        result.Pdu.VbList[2].Oid.ToString(),
                        SnmpConstants.GetTypeName(result.Pdu.VbList[2].Value.Type),
                        result.Pdu.VbList[2].Value.ToString());
                    Console.WriteLine("sysContact({0}) ({1}): {2}",
                        result.Pdu.VbList[3].Oid.ToString(),
                        SnmpConstants.GetTypeName(result.Pdu.VbList[3].Value.Type),
                        result.Pdu.VbList[3].Value.ToString());
                    Console.WriteLine("sysName({0}) ({1}): {2}",
                        result.Pdu.VbList[4].Oid.ToString(),
                        SnmpConstants.GetTypeName(result.Pdu.VbList[4].Value.Type),
                        result.Pdu.VbList[4].Value.ToString());
                }
            }
            else
            {
                Console.WriteLine("No response received from SNMP agent.");
            }
            target.Close();
        }
    }
}