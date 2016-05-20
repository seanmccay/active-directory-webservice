using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RemoteWindowsAgent
{
    public class AgentConfigurator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("AgentConfigurator");
        private static bool allowDebug = log.IsDebugEnabled;        

        public static string ReadSetting(string setting)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Avatier\Remote Windows Agent");
                string result = key.GetValue(setting).ToString();
                key.Close();
                return result;
            }
            catch(Exception e)
            {
                log.Error("Error on read, make sure config values have been set: ", e);
                throw new Exception("Error on read: ", e);
            }
        }

        public static byte[] ReadByteSetting(string setting)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Avatier\Remote Windows Agent");
                byte[] result = (byte[]) key.GetValue(setting);
                key.Close();
                return result;
            }
            catch (Exception e)
            {
                log.Error("Error on read, make sure config values have been set: ", e);
                throw new Exception("Error on read: ", e);
            }
        }

        public static void WriteSetting(string name, byte[] value)
        {
            UTF8Encoding utf8 = new UTF8Encoding();

            try
            {
                RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Avatier\Remote Windows Agent");
                key.SetValue(name, value);
                key.Close();
            }
            catch
            {
                throw;
            }
        }

        public static void WriteSetting(string name, string value)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Avatier\Remote Windows Agent");
                key.SetValue(name, value);
                key.Close();
            }
            catch
            {
                throw;
            }
        }

        public static byte[] Encrypt(string securityKey, string toEncrypt)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider(); ;

            tdes.Key = md5.ComputeHash(utf8.GetBytes(securityKey));
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            byte[] toEncryptArray = utf8.GetBytes(toEncrypt);

            ICryptoTransform cTransform = tdes.CreateEncryptor();

            byte[] result = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return result;
        }

        public static string Decrypt(string securityKey, byte[] toDecryptArray)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();       
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = md5.ComputeHash(utf8.GetBytes(securityKey));
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
      
            string result = utf8.GetString(cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length));
                
            tdes.Clear();
            return result;
        }
    }
}