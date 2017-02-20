using System;
using System.Runtime.InteropServices;
using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsTest.FsCheckTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Func<string, bool> secureStringResolvesToOriginal = source => source.ToSecureString().GetString() == (source ?? String.Empty);
            Prop.ForAll(secureStringResolvesToOriginal).QuickCheckThrowOnFailure();
        }


    }

    static class SecureStringExtensions
    {
        public static string GetString(this System.Security.SecureString source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            string result;
            int length = source.Length;
            IntPtr pointer = IntPtr.Zero;
            char[] chars = new char[length];

            try
            {
                pointer = Marshal.SecureStringToBSTR(source);
                Marshal.Copy(pointer, chars, 0, length);

                result = string.Join("", chars);
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(pointer);
                }
            }

            return result;
        }

        public static System.Security.SecureString ToSecureString(this string source)
        {
            var plain = source ?? string.Empty;

            var secureString = new System.Security.SecureString();

            foreach (var chr in plain.ToCharArray())
            {
                secureString.AppendChar(chr);
            }

            secureString.MakeReadOnly();

            return secureString;
        }
    }

}
