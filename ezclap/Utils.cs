using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ezclap
{
	// Default constructor. Ignore this. 
	public class Utils
	{
        /*
        *  Description: Set CurrentUser (HKCU) registry keys. 
        *  
        *  Param:
        *      - string[] registry = List of registry keys to be set/added 
        *      - string value = Subkey value to be set/added 
        *      - string payload = Value of the subkey 
        */
        public static void setHKCUSubKey(string registry, string subkey, string value)
        {
            Microsoft.Win32.RegistryKey hkcuKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registry);
            hkcuKey.SetValue(subkey, value);

            Console.WriteLine("[DEBUG] Modified key: " + registry + " | Value: " + hkcuKey.GetValue(subkey));
            hkcuKey.Close();
        }

        /*
         *  Description: Set LocalMachine (HKLM) registry keys. 
         *  
         *  Param:
         *      - string[] registry = List of registry keys to be set/added 
         *      - string value = Subkey value to be set/added 
         *      - string payload = Value of the subkey 
         */
        public static void setHKLMSubKey(string registry, string subkey, object value)
        {
            Microsoft.Win32.RegistryKey hklmKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(registry);
            hklmKey.SetValue(subkey, value);

            Console.WriteLine("[DEBUG] Modified key: " + registry + " | Value: " + hklmKey.GetValue(subkey));
            hklmKey.Close();

        }

        public static List<string> getAllServices()
        {
            List<string> servicesList = new List<string>();
            System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
            foreach (var service in services)
            {
                servicesList.Add(service.ServiceName);
            }

            return servicesList;
        }

        public static string[] parseConfig(string sectionName, string keyName)
        {
            var config = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            string[] keys = config[keyName].Split(',');
            
            return keys;
        }

        /*
         * 
         *             var payloadConfig = ConfigurationManager.GetSection("payload") as NameValueCollection;
            string[] payloads = payloadConfig["name"].Split(',');
            Console.WriteLine(payloads);
            foreach (var payload in payloads)
            {
                Console.WriteLine(payload);
            }
         * */

    }



}