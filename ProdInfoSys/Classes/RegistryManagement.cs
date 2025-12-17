using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Classes
{
    public class RegistryManagement
    {
        /// <summary>
        /// Write string registry key
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="registryValue"></param>
        public static void WriteStringRegistryKey(string registryKey, string registryValue)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                key?.SetValue(registryKey, registryValue);
            }
        }

        public static void WriteBoolRegistryKey(string registryKey, bool registryValue)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                key?.SetValue(registryKey, registryValue);
            }
        }

        public static void WriteIntRegistryKey(string registryKey, int registryValue)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                key?.SetValue(registryKey, registryValue);
            }
        }

        /// <summary>
        /// Read string registry key
        /// </summary>
        /// <param name="registryKey"></param>
        /// <returns></returns>
        public static string ReadStringRegistryKey(string registryKey)
        {
            string ret = string.Empty;
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                if (key != null)
                {
                    var value = key.GetValue(registryKey);
                    if (value != null)
                    {
                        ret = value.ToString() ?? string.Empty;
                    }
                }
            }
            return ret;
        }

        public static bool ReadBoolRegistryKey(string registryKey)
        {
            bool ret = false;
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                if (key != null)
                {
                    var value = key.GetValue(registryKey);
                    if (value != null)
                    {
                        if (value.ToString() == "True")
                        {
                            ret = true;
                        }
                    }
                }
                return ret;
            }
        }

        public static int ReadIntRegistryKey(string registryKey)
        {
            int ret = 12;
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                if (key != null)
                {
                    var value = key.GetValue(registryKey);
                    if (value != null)
                    {
                        int.TryParse(value.ToString(), out ret);
                    }
                }
                return ret;
            }
        }
    }
}
