using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;

namespace Parrano.Api
{
    public static class Native
    {
        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern IntPtr fopen(String filename, String mode);

        public static IntPtr fopen(string filename, FOpenMode mode)
        {
            return fopen(filename, GetFOpenModeDescription(mode));
        }

        public static string GetFOpenModeDescription(FOpenMode mode)
        {
            FieldInfo fi = typeof(FOpenMode).GetField(mode.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : mode.ToString();
        }

        public enum FOpenMode
        {
            [Description("r")]
            Read = 0,
            [Description("w")]
            Write,
            [Description("a")]
            Append,
            [Description("r+")]
            ReadPlus,
            [Description("w+")]
            WritePlus,
            [Description("a+")]
            AppendPlus
        }

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern Int32 fclose(IntPtr file);

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern int fseek(IntPtr file, long offset, int origin);

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern ulong fread(IntPtr intPtr, int size, int count, IntPtr file);

        [DllImport("msvcrt.dll", SetLastError = true)]
        internal static extern ulong fwrite(IntPtr buffer, int size, int number, IntPtr file);
        
        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern Int32 ferror(IntPtr file);

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern void clearerr(IntPtr file);

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern Int32 fflush(IntPtr file);

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern long ftell(IntPtr file);
    }
}
