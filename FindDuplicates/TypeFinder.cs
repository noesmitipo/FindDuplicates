using System;
using System.Runtime.InteropServices;
using System.IO;

namespace FindDuplicates
{
    class TypeFinder
    {
        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I1, SizeParamIndex=3)]
            byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags,
            out IntPtr ppwzMimeOut,
            int dwReserved);


        public static string getMimeFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found");

            byte[] buffer;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }
            try
            {
                IntPtr mimetype;
                FindMimeFromData(IntPtr.Zero, null, buffer, 256, null, 0, out mimetype, 0);
                string mime = Marshal.PtrToStringUni(mimetype);
                Marshal.FreeCoTaskMem(mimetype);
                return mime;
            }
            catch (Exception)
            {
                return "unknown/unknown";
            }
        }
    }
}
