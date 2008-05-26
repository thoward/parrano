using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using Parrano.Api;
using Parrano.Parser;

namespace Parrano.Tests
{
    [TestFixture]
    public class ParranoMiscTests
    {
        [Test]
        public void KitchenSink()
        {
            IntPtr ps;

            //ps = PSLibApi.PS_new();
            ErrorHandler errorHandler = new ErrorHandler(this.errorHandler);
            ps = PSLib.PS_new2(errorHandler);

            StringBuilder sb = new StringBuilder("example.ps");
            //PSLibApi.WriteProcCallBack writeproc = new PSLibApi.WriteProcCallBack(writeProcedure);
            //if (0 > PSLibApi.PS_open_mem(ps, writeproc))
            //{
            //    Console.WriteLine("Cannot open PostScript file");
            //    Assert.Fail();
            //}

            if (0 > PSLib.PS_open_file(ps, "example.ps"))
            {
                Console.WriteLine("Cannot open PostScript file");
                Assert.Fail();
            }

            PSLib.PS_set_info(ps, "Author", "Test PSLib.Net");

            PSLib.PS_begin_page(ps, 596, 842);
            PSLib.PS_set_parameter(ps, "transition", "wipe");
            PSLib.PS_set_value(ps, "duration", 0.5f);

            /*   int psfont = PS_findfont(ps, "Helvetica", "", 0);
               PS_setfont(ps, psfont, 12.0);
               PS_show_xy(ps, "hello", 10, 500);
               PS_show(ps, "abcABC");
               PS_symbol(ps, "\\337", "10");
            */

            PSLib.PS_setpolydash(ps, new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            PSLib.PS_moveto(ps, 0, 0);
            PSLib.PS_lineto(ps, 596, 842);
            PSLib.PS_stroke(ps);
            PSLib.PS_moveto(ps, 0, 842);
            PSLib.PS_lineto(ps, 596, 0);

            //string hyphens = PSLibDLLExports.PS_hyphenate(ps, "Thisisatestofalongstring");
                        
            //string buffstring = PSLibApi.PS_get_buffer(ps);
 
            PSLib.PS_stroke(ps);

            //int image = PSLibApi.PS_open_image_file(ps, ImageType.eps, @"C:\dev\code\pslib-0.4.0-win32\Parrano\Parrano.Tests\bin\Debug\dot.eps");            
            //int image = PSLibApi.PS_open_image_file(ps, ImageType.jpeg, @"C:\dev\code\pslib-0.4.0-win32\Parrano\Parrano.Tests\bin\Debug\dot.jpeg");
            //int image = PSLibApi.PS_open_image_file(ps, ImageType.png, @"C:\dev\code\pslib-0.4.0-win32\Parrano\Parrano.Tests\bin\Debug\dot.png");
            //int image = PSLibApi.PS_open_image_file(ps, ImageType.bmp, @"C:\dev\code\pslib-0.4.0-win32\Parrano\Parrano.Tests\bin\Debug\dot.bmp");
            //int image = PSLibApi.PS_open_image_file(ps, ImageType.tiff, @"C:\dev\code\pslib-0.4.0-win32\Parrano\Parrano.Tests\bin\Debug\dot.tif");
            int image = PSLib.PS_open_image_file(ps, ImageType.gif, @"C:\dev\code\pslib-0.4.0-win32\Parrano\Parrano.Tests\bin\Debug\dot.gif");
            PSLib.PS_place_image(ps, image, 100, 150, 1);
            PSLib.PS_close_image(ps, image);

            int fontid = PSLib.PS_findfont(ps, "Courier", null, false);
            PSLib.PS_setfont(ps, fontid, 12);
            string foo = PSLib.PS_symbol_name(ps, 'Z', fontid);
            string[] glyphList = PSLib.PS_glyph_list(ps, fontid);

            PSLib.PS_show_xy(ps, "This is a test", 0, 0);
            PSLib.PS_show_xy2(ps, "This is a test", 7, 100, 100);

            StringGeometry geo = PSLib.PS_string_geometry(ps, "This is a test", 7, fontid, 15);
            
            PSLib.PS_end_page(ps);
            PSLib.PS_close(ps);
            PSLib.PS_delete(ps);
        }

        public int writeProcedure(IntPtr psdoc, IntPtr data, int size)
        {
            string foo = Marshal.PtrToStringAnsi(data, size);
            Debug.Write(foo);
            return foo.Length;
        }

        public void errorHandler(IntPtr psdoc, int level, string message, IntPtr data)
        {
            Debug.WriteLine(string.Format("PSLIB Error [{0}] : {1}", level, message));
        }

    }



}
