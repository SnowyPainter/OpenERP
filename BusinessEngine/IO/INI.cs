using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BusinessEngine.IO
{
    public static class INI
    {
        #region dllimport
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
        #endregion

        public static int KeySize = 255;

        public static void Write(string section, string key, string value, string file)
        {
            WritePrivateProfileString(section, key, value, file);
        }

        public static string Read(string section, string key, string file)
        {
            var sb = new StringBuilder(KeySize);
            GetPrivateProfileString(section, key, "", sb, KeySize, file);
            return sb.ToString();
        }
    }
}
