using System;
using System.Diagnostics;
using System.IO;

namespace Parrano.Api
{
    public static class PSLibExtensions
    {
        public const int A4Height = 842;
        public const int A4Width = 596;
        public const int LetterHeight = 792;
        public const int LetterWidth = 612;


        public static IntPtr OpenPSDocHandle(string filename)
        {
            return OpenPSDocHandle(filename, PageSize.A4);
        }

        public static IntPtr OpenPSDocHandle(string filename, PageSize pageSize)
        {
            switch (pageSize)
            {
                case PageSize.Letter:
                    return OpenPSDocHandle(filename, LetterWidth, LetterHeight);
                    //case PageSize.A4:
                default:
                    return OpenPSDocHandle(filename, A4Width, A4Height);
            }
        }

        public static IntPtr OpenPSDocHandle(string filename, int pageWidth, int pageHeight)
        {
            return OpenPSDocHandle(filename, pageWidth, pageHeight, null);
        }

        public static IntPtr OpenPSDocHandle(string filename, int pageWidth, int pageHeight, InfoFields infoFields)
        {
            IntPtr psdoc = PSLib.PS_new();

            Debug.Assert(default(IntPtr) != psdoc);

            if (0 > PSLib.PS_open_file(psdoc, filename))
            {
                throw new IOException(string.Format("Cannot open PostScript file \"{0}\" .", filename));
            }

            if (null != infoFields)
            {
                infoFields.UpdateDocument(psdoc);
            }

            PSLib.PS_begin_page(psdoc, pageWidth, pageHeight);

            return psdoc;
        }

        public static void DrawLine(IntPtr psdoc, float x1, float y1, float x2, float y2)
        {
            PSLib.PS_moveto(psdoc, x1, y1);
            PSLib.PS_lineto(psdoc, x2, y2);
            PSLib.PS_stroke(psdoc);
        }

        public static void ClosePSDocHandle(IntPtr psdoc)
        {
            PSLib.PS_end_page(psdoc);
            PSLib.PS_close(psdoc);
            PSLib.PS_delete(psdoc);
        }
    }
}