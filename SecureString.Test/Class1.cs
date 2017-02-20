using System;
using System.Runtime.InteropServices;
using FsCheck;
using NUnit.Framework;

namespace SecureString.Test
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void TestSecureString()
        {
            Func<string, bool> secureStringResolvesToOriginal = source => source.ToSecureString().GetString() == (source ?? string.Empty);
            Prop.ForAll(secureStringResolvesToOriginal).QuickCheckThrowOnFailure();
        }
    }

    static class SecureStringExtensions
    {
        public static string GetString(this System.Security.SecureString source)
        {
            if(source == null)
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
