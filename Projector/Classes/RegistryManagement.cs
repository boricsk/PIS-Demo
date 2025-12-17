using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
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
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystem"))
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
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ProdInfoSystem"))
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
    }
}
