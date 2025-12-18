using Microsoft.Win32;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides static methods for reading from and writing to string, boolean, and integer values in the
    /// HKEY_CURRENT_USER\SOFTWARE\ProdInfoSystemDemo registry subkey.
    /// </summary>
    /// <remarks>This class is intended to simplify access to application-specific registry values under the
    /// current user's profile. All methods operate on the HKEY_CURRENT_USER\SOFTWARE\ProdInfoSystemDemo subkey. The
    /// class does not provide thread safety; callers should ensure appropriate synchronization if accessing registry
    /// values concurrently.</remarks>
    public class RegistryManagement
    {
        public static void WriteStringRegistryKey(string registryKey, string registryValue)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                key?.SetValue(registryKey, registryValue);
            }
        }

        /// <summary>
        /// Writes a Boolean value to the specified registry key under the current user's "SOFTWARE\ProdInfoSystemDemo"
        /// subkey.
        /// </summary>
        /// <remarks>If the specified registry value does not exist, it is created. The value is stored
        /// under HKEY_CURRENT_USER in the "SOFTWARE\ProdInfoSystemDemo" subkey. The method overwrites any existing
        /// value with the same name.</remarks>
        /// <param name="registryKey">The name of the registry value to set. Cannot be null or empty.</param>
        /// <param name="registryValue">The Boolean value to write to the specified registry key.</param>
        public static void WriteBoolRegistryKey(string registryKey, bool registryValue)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                key?.SetValue(registryKey, registryValue);
            }
        }

        /// <summary>
        /// Writes an integer value to the specified registry key under the current user's "SOFTWARE\ProdInfoSystemDemo"
        /// subkey.
        /// </summary>
        /// <remarks>If the specified registry value already exists, its value is overwritten. The method
        /// creates the "SOFTWARE\ProdInfoSystemDemo" subkey if it does not exist.</remarks>
        /// <param name="registryKey">The name of the registry value to set. Cannot be null or empty.</param>
        /// <param name="registryValue">The integer value to write to the specified registry key.</param>
        public static void WriteIntRegistryKey(string registryKey, int registryValue)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ProdInfoSystemDemo"))
            {
                key?.SetValue(registryKey, registryValue);
            }
        }

        /// <summary>
        /// Retrieves the string value associated with the specified registry key from the
        /// HKEY_CURRENT_USER\SOFTWARE\ProdInfoSystemDemo subkey.
        /// </summary>
        /// <remarks>This method accesses the current user's registry hive. If the specified value does
        /// not exist or is not set, the method returns an empty string rather than null.</remarks>
        /// <param name="registryKey">The name of the registry value to retrieve from the ProdInfoSystemDemo subkey. Cannot be null.</param>
        /// <returns>The string value associated with the specified registry key, or an empty string if the key does not exist or
        /// has no value.</returns>
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

        /// <summary>
        /// Reads a Boolean value from the specified registry key under the current user's "SOFTWARE\ProdInfoSystemDemo"
        /// subkey.
        /// </summary>
        /// <remarks>If the specified registry value does not exist or its value is not the string "True",
        /// the method returns false. The comparison is case-sensitive.</remarks>
        /// <param name="registryKey">The name of the registry value to read from the "SOFTWARE\ProdInfoSystemDemo" subkey. Cannot be null.</param>
        /// <returns>true if the registry value exists and its string representation is "True"; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves the integer value associated with the specified registry key from the
        /// HKEY_CURRENT_USER\SOFTWARE\ProdInfoSystemDemo subkey.
        /// </summary>
        /// <remarks>If the specified registry value does not exist or cannot be parsed as an integer, the
        /// method returns 12 as a default value.</remarks>
        /// <param name="registryKey">The name of the registry value to retrieve from the ProdInfoSystemDemo subkey. Cannot be null.</param>
        /// <returns>The integer value of the specified registry key if it exists and can be parsed; otherwise, 12.</returns>
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
