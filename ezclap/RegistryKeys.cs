using System;

namespace ezclap
{
    /*
     *  Class name: RegistryKeys 
     *  Description: Class used for hardcode registry keys. Mostly consisted of class variables. 
     *  I think there should be a better way to do this, but I literally learned c# 3 days ago. 
     * 
     */
	public class RegistryKeys
	{
        public const string hkcuLowRiskFileType = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations";

        public const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string RunOnceKey = "Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce";
        public const string RunServices = "Software\\Microsoft\\Windows\\CurrentVersion\\RunServices";
        public const string RunServicesOnce = "Software\\Microsoft\\Windows\\CurrentVersion\\RunServicesOnce";
        public const string RunOnceEx = "Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce";

        public const string hklmUserInit = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";
        public const string hklmImagePath = "SYSTEM\\CurrentControlSet\\Services\\W32Time";

        public const string hklmImageFileExec = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
        public const string hklmSilentProcessExit = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SilentProcessExit";

        public const string hklmDefender = "SOFTWARE\\Policies\\Microsoft\\Windows Defender";

    }
}

