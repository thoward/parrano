using System;
using System.Runtime.InteropServices;
using System.Text;

using NUnit.Framework;
using System.IO;
using System.Diagnostics;
using Parrano.Api;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;


namespace Parrano.Tests
{
    [TestFixture]
    public class PSLibApiTests
    {

        [SetUp]
        public void SetUp()
        {
            tempFilename = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(tempFilename))
            {
                try
                {
                    File.Delete(tempFilename);
                }
                catch { }
            }
        }

        private string tempFilename;
        private const int defaultPageWidth = PSLibExtensions.A4Width;
        private const int defaultPageHeight = PSLibExtensions.A4Height;

        /// <summary>
        /// Opens a PS doc, creating a file handle and opening the specified filename. This is for testing purposes only.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>A <see cref="IntPtr"/>.</returns>
        private static IntPtr OpenPSDoc(string filename)
        {
            IntPtr psdoc = PSLib.PS_new();

            Assert.That(HandleIsValid(psdoc));

            if (0 > PSLib.PS_open_file(psdoc, filename))
            {
                throw new IOException(string.Format("Cannot open PostScript file \"{0}\" .", filename));
            }

            return psdoc;
        }

        private static bool HandleIsValid(IntPtr handle)
        {
            return handle.ToInt32() > 0;
        }

        private static void AssertFileExists(string filename)
        {
            Assert.That(File.Exists(filename), "Cannot find file needed to run test. File is: " + filename);
        }

        #region Tests
        
        [Test]
        public void PS_get_majorversion_Test()
        {
            const int expectedMajorVersion = 0;
            
            int resultMajorVersion = PSLib.PS_get_majorversion();

            Trace.WriteLine("MajorVersion: " + resultMajorVersion);

            Assert.AreEqual(expectedMajorVersion, resultMajorVersion, "PS_get_majorversion method returned unexpected result.");
        }

        [Test]
        public void PS_get_minorversion_Test()
        {
            const int expectedMinorVersion = 4;

            int resultMinorVersion = PSLib.PS_get_minorversion();

            Trace.WriteLine("MinorVersion: " + resultMinorVersion);

            Assert.AreEqual(expectedMinorVersion, resultMinorVersion, "PS_get_minorversion method returned unexpected result.");
        }

        [Test]
        public void PS_get_subminorversion_Test()
        {
            const int expectedSubMinorVersion = 0;

            int resultSubMinorVersion = PSLib.PS_get_subminorversion();

            Trace.WriteLine("SubMinorVersion: " + resultSubMinorVersion);

            Assert.AreEqual(expectedSubMinorVersion, resultSubMinorVersion, "PS_get_subminorversion method returned unexpected result.");
        }

        [Test]
        public void PS_boot_Test()
        {
            // i have no idea how to test this. there seems to be no 
            // obvious effect to doing this or not doing this..
            PSLib.PS_boot();
        }

        [Test]        
        public void PS_shutdown_Test()
        {
            // i have no idea how to test this. there seems to be no 
            // obvious effect to doing this or not doing this..
            PSLib.PS_shutdown();
        }

        [Test]        
        public void PS_set_info_Test()
        {
            IntPtr psdoc = OpenPSDoc(tempFilename);

            const string key = "Author";
            const string val = "Ursula Kroeber Le Guin";

            PSLib.PS_set_info(psdoc, key, val);

            PSLib.PS_begin_page(psdoc, defaultPageWidth, defaultPageHeight);

            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine(tempFilename);
            Trace.WriteLine(resultFileContents);

            Assert.That(
                resultFileContents.Contains(string.Format("%%{0}: {1}", key, val)),
                "File contents look wrong.");
        }

        [Test]        
        public void PS_new_Test()
        {
            IntPtr psdoc = PSLib.PS_new();
            
            Assert.That(HandleIsValid(psdoc), "PS_new method returned unexpected result.");
        }

        [Test]        
        [Ignore("Not implemented.")]
        public void PS_new2ErrorhandlerAllocprocReallocprocFreeprocOpaque_Test()
        {
            ErrorHandler errorhandler = default(ErrorHandler);
            AllocProc allocproc = default(AllocProc);
            ReAllocProc reallocproc = default(ReAllocProc);
            FreeProc freeproc = default(FreeProc);

            IntPtr opaque = IntPtr.Zero;
            IntPtr expectedIntPtr = IntPtr.Zero;
            IntPtr resultIntPtr = IntPtr.Zero;
            resultIntPtr = PSLib.PS_new2(errorhandler, allocproc, reallocproc, freeproc, opaque);
            Assert.AreEqual(expectedIntPtr, resultIntPtr, "PS_new2 method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        #region PS_new2Errorhandler_Test

        [Test]
        public void PS_new2Errorhandler_Test()
        {
            ErrorHandler errorhandler = PS_new2Errorhandler_Test_ErrorHandler;

            IntPtr psdoc = PSLib.PS_new2(errorhandler);

            // got a valid pointer
            Assert.That(HandleIsValid(psdoc), "PS_new2 method returned unexpected result.");

            PS_new2Errorhandler_Test_DocPointer = psdoc;

            // this should create an error, which will be handled by the error handler callback.
            PSLib.PS_setfont(psdoc, 256, 0);

            Assert.That(PS_new2Errorhandler_Test_ErrorHandlerWasCalled, "Error handler wasn't called as expected.");

        }

        private IntPtr PS_new2Errorhandler_Test_DocPointer;
        private bool PS_new2Errorhandler_Test_ErrorHandlerWasCalled = false;

        public void PS_new2Errorhandler_Test_ErrorHandler(IntPtr psdoc, int level, string message, IntPtr data)
        {
            const int expectedLevel = 3;
            const string expectedMessage = "??? must be called within 'page', 'pattern', or 'template' scope.";
            IntPtr expectedData = IntPtr.Zero;

            Trace.WriteLine(string.Format("PSLIB Error [{0}] : {1}", level, message));

            Assert.That(psdoc == PS_new2Errorhandler_Test_DocPointer, "Pointers aren't the same.");
            Assert.That(level == expectedLevel, "Wrong error level returned.");
            Assert.That(expectedMessage == message, "Wrong error message returned.");
            Assert.That(expectedData == data, "Wrong data pointer returned.");

            PS_new2Errorhandler_Test_ErrorHandlerWasCalled = true;
        }

        #endregion PS_new2Errorhandler_Test
        
        [Test]
        public void PS_open_fp_Test()
        {
            IntPtr psdoc = PSLib.PS_new();

            using(NativeFileStream nfs = new NativeFileStream(tempFilename, Native.FOpenMode.Write))
            {
                if (0 > PSLib.PS_open_fp(psdoc, nfs.FileHandle))
                {
                    throw new IOException("Could not open file.");
                }
                
                PSLib.PS_begin_page(psdoc, defaultPageWidth, defaultPageHeight);

                PSLibExtensions.ClosePSDocHandle(psdoc);
            }

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(
                    resultFileContents.StartsWith("%!PS-Adobe-3.0"),
                    "File was created, but the contents are wrong.");

            Assert.That(resultFileContents.Contains(
                    "%%BeginSetup" + Environment.NewLine +
                    "PslibDict begin" + Environment.NewLine +
                    "%%EndSetup"),
                    "File was created, but the contents are wrong.");

            Assert.That(resultFileContents.EndsWith("%%EOF"),
                    "File was created, but the contents are wrong.");

        }

        [Test]
        [Ignore("Not implemented.")]        
        public void PS_get_opaque_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            IntPtr expectedIntPtr = IntPtr.Zero;
            IntPtr resultIntPtr = IntPtr.Zero;
            resultIntPtr = PSLib.PS_get_opaque(psdoc);
            Assert.AreEqual(expectedIntPtr, resultIntPtr, "PS_get_opaque method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        #region PS_open_mem_Test
        [Test]
        public void PS_open_mem_Test()
        {
            IntPtr psdoc = PSLib.PS_new();

            WriteProcCallBack writeproc = new WriteProcCallBack(PS_open_mem_Test_writeProcedure);
            PS_open_mem_Test_memoryStream = new MemoryStream();

            if (0 > PSLib.PS_open_mem(psdoc, writeproc))
            {
                Trace.WriteLine("Cannot open PostScript file");
                Assert.Fail();
            }

            PSLib.PS_begin_page(psdoc, defaultPageWidth, defaultPageHeight);

            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = 
                Encoding.ASCII.GetString(
                    PS_open_mem_Test_memoryStream.ToArray()
                    );

            PS_open_mem_Test_memoryStream.Dispose();

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(
                    resultFileContents.StartsWith("%!PS-Adobe-3.0"),
                    "File was created, but the beginning is wrong.");
            
            // note that line ending are in UNIX format when does this way.. 
            // no carriage return. 

            Assert.That(resultFileContents.Contains(
                    "%%BeginSetup\n" + //Environment.NewLine +
                    "PslibDict begin\n" + //Environment.NewLine +
                    "%%EndSetup"),
                    "File was created, but the middle is wrong.");

            Assert.That(resultFileContents.EndsWith("%%EOF"),
                    "File was created, but the end is wrong.");
        }

        private MemoryStream PS_open_mem_Test_memoryStream;

        public int PS_open_mem_Test_writeProcedure(IntPtr psdoc, IntPtr data, int size)
        {
            if(PS_open_mem_Test_memoryStream != null)
            {
                byte[] buf = new byte[size];

                Marshal.Copy(data, buf, 0, size);

                PS_open_mem_Test_memoryStream.Write(buf, 0, size);
            }

            return size;
            //string foo = Marshal.PtrToStringAnsi(data, size);
            //Debug.Write(foo);
            //return foo.Length;
        }
        #endregion PS_open_mem_Test
        
        [Test]
        public void PS_open_file_Test()
        {
            if(File.Exists(tempFilename))
            {
                File.Delete(tempFilename);
            }

            Assert.That(!File.Exists(tempFilename));

            IntPtr psdoc = PSLib.PS_new();
            
            Assert.That(default(IntPtr) != psdoc);

            if (0 > PSLib.PS_open_file(psdoc, tempFilename))
            {
                Assert.Fail("Cannot open PostScript file.");
            }

            PSLibExtensions.ClosePSDocHandle(psdoc);

            Assert.That(File.Exists(tempFilename));
        }

        [Test]
        public void PS_close_Test()
        {
            
            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(tempFilename);

            PSLib.PS_close(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(resultFileContents.EndsWith("%%EOF"),
                    "File was created, but the contents are wrong.");

            PSLib.PS_delete(psdoc);
        }

        [Test]
        public void PS_delete_Test()
        {
            IntPtr psdoc = PSLib.PS_new();
            PSLib.PS_delete(psdoc);

            if (0 != PSLib.PS_open_file(psdoc, tempFilename))
            {
                Assert.Fail("Postsript document handle not properly closed.");
            }
        }

        [Test]
        public void PS_begin_page_Test()
        {
            IntPtr psdoc = OpenPSDoc(tempFilename);
            const float pageWidth = 37;
            const float pageHeight = 42;

            PSLib.PS_begin_page(psdoc, pageWidth, pageHeight);
            
            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(
                resultFileContents.Contains(
                "%%Page: 1 1" + Environment.NewLine +
                "%%PageBoundingBox: 0 0 " + pageWidth + " " + pageHeight),
                "File was created, but the contents are wrong.");
        }

        [Test]
        public void PS_end_page_Test()
        {
            int noOfPages = 3;

            IntPtr psdoc = OpenPSDoc(tempFilename);


            for (int i = 0; i < noOfPages; i++)
            {
                PSLib.PS_begin_page(psdoc, PSLibExtensions.A4Width, PSLibExtensions.A4Height);
                PSLib.PS_end_page(psdoc);
            }

            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            // look for all three page endings.

            for (int i = 1; i < noOfPages + 1; i++)
            {
                Assert.That(
                    resultFileContents.Contains(
                    "%%Page: " + i + " " + i + Environment.NewLine),
                    "File was created, but the page " + i + " tag was wrong.");

                Assert.That(
                    resultFileContents.Contains(
                    "restore" + Environment.NewLine +
                    "save" + Environment.NewLine +
                    i + " PslibPageEndHook" + Environment.NewLine +
                    "restore" + Environment.NewLine +
                    "showpage" + Environment.NewLine),
                    "File was created, but the contents of page " + i + " are wrong.");
            }

            Assert.That(
                resultFileContents.Contains(
                "%%Pages: " + noOfPages + Environment.NewLine),
                "File was created, but the ending page count was wrong.");
        }

        [Test]
        public void PS_set_parameter_Test()
        {
            AssertFileExists("hyph_en.dic");

            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(tempFilename);

            // get the values before the set parameter.
            string beforeHyphenDic = PSLib.PS_get_parameter(psdoc, "hyphendic", default(float));
            string beforePTest = PSLib.PS_get_parameter(psdoc, "ParranoTest", default(float));

            Trace.WriteLine("Before Parameters:");
            Trace.WriteLine(string.Format("{0}: {1}", "hyphendic", beforeHyphenDic));
            Trace.WriteLine(string.Format("{0}: {1}", "ParranoTest", beforePTest));
            
            PSLib.PS_set_parameter(psdoc, "hyphendic", "hyph_en.dic");
            PSLib.PS_set_parameter(psdoc, "ParranoTest", "Foo");

            // get the values after the set parameter.
            string hyphendic = PSLib.PS_get_parameter(psdoc, "hyphendic", default(float));
            string pTest = PSLib.PS_get_parameter(psdoc, "ParranoTest", default(float));


            Trace.WriteLine("\r\nAfter Parameters:");
            Trace.WriteLine(string.Format("{0}: {1}", "hyphendic", hyphendic));
            Trace.WriteLine(string.Format("{0}: {1}", "ParranoTest", pTest));
            
            Assert.That(hyphendic != beforeHyphenDic);
            Assert.That(pTest != beforePTest);

            PSLibExtensions.ClosePSDocHandle(psdoc);
        }

        [Test]
        public void PS_get_parameter_Test()
        {

            AssertFileExists("AlteSchwabacher.afm");
            AssertFileExists("AlteSchwabacher.enc");
            AssertFileExists("hyph_en.dic");

            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(tempFilename);

            // set a font and get the resource id
            int fontid = PSLib.PS_findfont(psdoc, "AlteSchwabacher", "AlteSchwabacher.enc", false);
            PSLib.PS_setfont(psdoc, fontid, 12);

            // set hyphen dictionary
            PSLib.PS_set_parameter(psdoc, "hyphendic", "hyph_en.dic");
            
            PSLib.PS_set_parameter(psdoc, "ParranoTest", "Foo");
            // user-defined
            


            // built ins
            string fontName = PSLib.PS_get_parameter(psdoc, "fontname", fontid);
            string fontEncoding = PSLib.PS_get_parameter(psdoc, "fontencoding", fontid);
            string version = PSLib.PS_get_parameter(psdoc, "dottedversion", default(float));
            string scope = PSLib.PS_get_parameter(psdoc, "scope", default(float));

            // random functional parameter.
            string hyphendic = PSLib.PS_get_parameter(psdoc, "hyphendic", default(float));

            // user-defined
            string pTest = PSLib.PS_get_parameter(psdoc, "ParranoTest", default(float));
            

            Trace.WriteLine("Parameters:");
            
            // built ins
            Trace.WriteLine(string.Format("{0}: {1}", "fontname", fontName));
            Trace.WriteLine(string.Format("{0}: {1}", "fontencoding", fontEncoding));
            Trace.WriteLine(string.Format("{0}: {1}", "dottedversion", version));
            Trace.WriteLine(string.Format("{0}: {1}", "hyphendic", hyphendic));

            // random functional parameter.
            Trace.WriteLine(string.Format("{0}: {1}", "scope", scope));

            // user-defined
            Trace.WriteLine(string.Format("{0}: {1}", "ParranoTest", pTest));


            // built ins
            Assert.That(fontName == "AlteSchwabacher");
            Assert.That(fontEncoding == "CorkEncoding");
            Assert.That(version == "0.4.0");
            Assert.That(scope == "page");

            // random functional parameter.
            Assert.That(hyphendic == "hyph_en.dic");

            // user-defined
            Assert.That(pTest == "Foo");

            PSLibExtensions.ClosePSDocHandle(psdoc);
        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_set_value_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string name = string.Empty;
            float value = -1;
            PSLib.PS_set_value(psdoc, name, value);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_get_value_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string name = string.Empty;
            float modifier = -1;
            float expectedSingle = 0;
            float resultSingle = 0;
            resultSingle = PSLib.PS_get_value(psdoc, name, modifier);
            Assert.AreEqual(expectedSingle, resultSingle, "PS_get_value method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_list_values_Test()
        {
            IntPtr psdoc = OpenPSDoc(tempFilename);

            PSLib.PS_list_values(psdoc);

            PSLibExtensions.ClosePSDocHandle(psdoc);

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_list_parameters_Test()
        {
            IntPtr psdoc = OpenPSDoc(tempFilename);
            
            PSLib.PS_list_parameters(psdoc);

            PSLibExtensions.ClosePSDocHandle(psdoc);
        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_list_resources_Test()
        {
            IntPtr psdoc = OpenPSDoc(tempFilename);

            PSLib.PS_list_resources(psdoc);

            PSLibExtensions.ClosePSDocHandle(psdoc);
        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_set_text_pos_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            PSLib.PS_set_text_pos(psdoc, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setlinewidth_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float width = -1;
            PSLib.PS_setlinewidth(psdoc, width);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setlinecap_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            LineCapType type = default(LineCapType);
            PSLib.PS_setlinecap(psdoc, type);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setlinejoin_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            LineJoinType type = default(LineJoinType);
            PSLib.PS_setlinejoin(psdoc, type);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setmiterlimit_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float value = -1;
            PSLib.PS_setmiterlimit(psdoc, value);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setflat_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float value = -1;
            PSLib.PS_setflat(psdoc, value);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setoverprintmode_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int mode = -1;
            PSLib.PS_setoverprintmode(psdoc, mode);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setsmoothness_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float smoothness = -1;
            PSLib.PS_setsmoothness(psdoc, smoothness);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setdash_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float on = -1;
            float off = -1;
            PSLib.PS_setdash(psdoc, on, off);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setpolydashPsdocArr_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float[] arr = null;
            PSLib.PS_setpolydash(psdoc, arr);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_curveto_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x1 = -1;
            float y1 = -1;
            float x2 = -1;
            float y2 = -1;
            float x3 = -1;
            float y3 = -1;
            PSLib.PS_curveto(psdoc, x1, y1, x2, y2, x3, y3);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_rect_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            float width = -1;
            float height = -1;
            PSLib.PS_rect(psdoc, x, y, width, height);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_circle_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            float radius = -1;
            PSLib.PS_circle(psdoc, x, y, radius);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_arc_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            float radius = -1;
            float alpha = -1;
            float beta = -1;
            PSLib.PS_arc(psdoc, x, y, radius, alpha, beta);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_arcn_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            float radius = -1;
            float alpha = -1;
            float beta = -1;
            PSLib.PS_arcn(psdoc, x, y, radius, alpha, beta);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_clip_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_clip(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setgray_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float gray = -1;
            PSLib.PS_setgray(psdoc, gray);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_closepath_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_closepath(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_closepath_stroke_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_closepath_stroke(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_fill_stroke_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_fill_stroke(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_stroke_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_stroke(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_fill_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_fill(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_save_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_save(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_restore_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_restore(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_show_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            PSLib.PS_show(psdoc, text);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_show2_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int xlen = -1;
            PSLib.PS_show2(psdoc, text, xlen);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_show_xy_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            float x = -1;
            float y = -1;
            PSLib.PS_show_xy(psdoc, text, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_show_xy2_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int xlen = -1;
            float x = -1;
            float y = -1;
            PSLib.PS_show_xy2(psdoc, text, xlen, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_show_boxed_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            float left = -1;
            float bottom = -1;
            float width = -1;
            float height = -1;
            string hmode = string.Empty;
            string feature = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_show_boxed(psdoc, text, left, bottom, width, height, hmode, feature);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_show_boxed method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_continue_text_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            PSLib.PS_continue_text(psdoc, text);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_continue_text2_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int len = -1;
            PSLib.PS_continue_text2(psdoc, text, len);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setcolorPsdocTypeColorspaceC1C2C3C4_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            ColorUsage usage = default(ColorUsage);
            ColorSpace colorspace = default(ColorSpace);
            float c1 = -1;
            float c2 = -1;
            float c3 = -1;
            float c4 = -1;
            PSLib.PS_setcolor(psdoc, usage, colorspace, c1, c2, c3, c4);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_makespotcolor_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string name = string.Empty;
            int reserved = -1;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_makespotcolor(psdoc, name, reserved);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_makespotcolor method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_findfont_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string fontname = string.Empty;
            string encoding = string.Empty;
            bool embed = false;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_findfont(psdoc, fontname, encoding, embed);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_findfont method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_stringwidth_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int fontid = -1;
            float size = -1;
            float expectedSingle = 0;
            float resultSingle = 0;
            resultSingle = PSLib.PS_stringwidth(psdoc, text, fontid, size);
            Assert.AreEqual(expectedSingle, resultSingle, "PS_stringwidth method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_stringwidth2_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int xlen = -1;
            int fontid = -1;
            float size = -1;
            float expectedSingle = 0;
            float resultSingle = 0;
            resultSingle = PSLib.PS_stringwidth2(psdoc, text, xlen, fontid, size);
            Assert.AreEqual(expectedSingle, resultSingle, "PS_stringwidth2 method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_string_geometryPsdocTextXlenFontidSizeDimension_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int xlen = -1;
            int fontid = -1;
            float size = -1;
            float[] dimension = null;
            float[] expecteddimension = null;
            float expectedSingle = 0;
            float resultSingle = 0;
            resultSingle = PSLib.PS_string_geometry(psdoc, text, xlen, fontid, size, out dimension);
            Assert.AreEqual(expectedSingle, resultSingle, "PS_string_geometry method returned unexpected result.");
            Assert.IsNotNull(expecteddimension, "dimension out parameter should not be null");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_string_geometryPsdocTextXlenFontidSize_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int xlen = -1;
            int fontid = -1;
            float size = -1;
            StringGeometry expectedStringGeometry = default(StringGeometry);
            StringGeometry resultStringGeometry = default(StringGeometry);
            resultStringGeometry = PSLib.PS_string_geometry(psdoc, text, xlen, fontid, size);
            Assert.AreEqual(expectedStringGeometry, resultStringGeometry, "PS_string_geometry method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_deletefont_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int fontid = -1;
            PSLib.PS_deletefont(psdoc, fontid);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_setfont_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int fontid = -1;
            float size = -1;
            PSLib.PS_setfont(psdoc, fontid, size);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_getfont_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_getfont(psdoc);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_getfont method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_moveto_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            PSLib.PS_moveto(psdoc, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_lineto_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            PSLib.PS_lineto(psdoc, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_rotate_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            PSLib.PS_rotate(psdoc, x);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_translate_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            PSLib.PS_translate(psdoc, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_scale_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float x = -1;
            float y = -1;
            PSLib.PS_scale(psdoc, x, y);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_get_bufferPsdocSize_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            long size = 0;
            long expectedsize = 0;
            IntPtr expectedIntPtr = IntPtr.Zero;
            IntPtr resultIntPtr = IntPtr.Zero;
            resultIntPtr = PSLib.PS_get_buffer(psdoc, ref size);
            Assert.AreEqual(expectedIntPtr, resultIntPtr, "PS_get_buffer method returned unexpected result.");
            Assert.AreEqual(expectedsize, size, "size ref parameter has unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_get_bufferPsdoc_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string expectedString = string.Empty;
            string resultString = string.Empty;
            resultString = PSLib.PS_get_buffer(psdoc);
            Assert.AreEqual(expectedString, resultString, "PS_get_buffer method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_open_imagePsdocTypeDataLengthWidthHeightComponentsBpc_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            ImageType type = default(ImageType);
            string data = string.Empty;
            long length = 0;
            int width = -1;
            int height = -1;
            int components = -1;
            int bpc = -1;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_open_image(psdoc, type, data, length, width, height, components, bpc);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_open_image method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_open_image_filePsdocTypeFilename_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            ImageType type = default(ImageType);
            string filename = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_open_image_file(psdoc, type, filename);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_open_image_file method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_open_image_filePsdocTypeFilenameMaskTypeImageId_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            ImageType type = default(ImageType);
            string filename = string.Empty;
            PngMaskType maskType = default(PngMaskType);
            int imageId = -1;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_open_image_file(psdoc, type, filename, maskType, imageId);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_open_image_file method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_place_image_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int imageid = -1;
            float x = -1;
            float y = -1;
            float scale = -1;
            PSLib.PS_place_image(psdoc, imageid, x, y, scale);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_close_image_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int imageid = -1;
            PSLib.PS_close_image(psdoc, imageid);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_weblink_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            string url = string.Empty;
            PSLib.PS_add_weblink(psdoc, llx, lly, urx, ury, url);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_pdflinkPsdocLlxLlyUrxUryFilenamePageDest_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            string filename = string.Empty;
            int page = -1;
            string dest = string.Empty;
            PSLib.PS_add_pdflink(psdoc, llx, lly, urx, ury, filename, page, dest);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_pdflinkPsdocLlxLlyUrxUryFilenamePageDocViewType_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            string filename = string.Empty;
            int page = -1;
            DocumentViewType docViewType = default(DocumentViewType);
            PSLib.PS_add_pdflink(psdoc, llx, lly, urx, ury, filename, page, docViewType);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_locallinkPsdocLlxLlyUrxUryPageDest_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            int page = -1;
            string dest = string.Empty;
            PSLib.PS_add_locallink(psdoc, llx, lly, urx, ury, page, dest);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_locallinkPsdocLlxLlyUrxUryPageDocViewType_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            int page = -1;
            DocumentViewType docViewType = default(DocumentViewType);
            PSLib.PS_add_locallink(psdoc, llx, lly, urx, ury, page, docViewType);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_launchlink_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            string filename = string.Empty;
            PSLib.PS_add_launchlink(psdoc, llx, lly, urx, ury, filename);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_bookmark_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            int parent = -1;
            bool open = false;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_add_bookmark(psdoc, text, parent, open);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_add_bookmark method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_notePsdocLlxLlyUrxUryContentsTitleIconOpen_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float llx = -1;
            float lly = -1;
            float urx = -1;
            float ury = -1;
            string contents = string.Empty;
            string title = string.Empty;
            string icon = string.Empty;
            bool open = false;
            PSLib.PS_add_note(psdoc, llx, lly, urx, ury, contents, title, icon, open);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_set_border_stylePsdocStyleWidth_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string style = string.Empty;
            float width = -1;
            PSLib.PS_set_border_style(psdoc, style, width);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_set_border_color_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float red = -1;
            float green = -1;
            float blue = -1;
            PSLib.PS_set_border_color(psdoc, red, green, blue);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_set_border_dash_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float black = -1;
            float white = -1;
            PSLib.PS_set_border_dash(psdoc, black, white);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_begin_template_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float width = -1;
            float height = -1;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_begin_template(psdoc, width, height);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_begin_template method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_end_template_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_end_template(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_begin_pattern_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            float width = -1;
            float height = -1;
            float xstep = -1;
            float ystep = -1;
            PostscriptPatternPaintType painttype = default(PostscriptPatternPaintType);
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_begin_pattern(psdoc, width, height, xstep, ystep, painttype);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_begin_pattern method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_end_pattern_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_end_pattern(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_shfill_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int shading = -1;
            PSLib.PS_shfill(psdoc, shading);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_shading_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string shtype = string.Empty;
            float x0 = -1;
            float y0 = -1;
            float x1 = -1;
            float y1 = -1;
            float c1 = -1;
            float c2 = -1;
            float c3 = -1;
            float c4 = -1;
            string optlist = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_shading(psdoc, shtype, x0, y0, x1, y1, c1, c2, c3, c4, optlist);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_shading method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_shading_patternPsdocShadingOptlist_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int shading = -1;
            string optlist = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_shading_pattern(psdoc, shading, optlist);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_shading_pattern method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_shading_patternPsdocShading_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int shading = -1;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_shading_pattern(psdoc, shading);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_shading_pattern method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_create_gstate_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string optlist = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_create_gstate(psdoc, optlist);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_create_gstate method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_hyphenatePsdocText_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string text = string.Empty;
            string expectedString = string.Empty;
            string resultString = string.Empty;
            resultString = PSLib.PS_hyphenate(psdoc, text);
            Assert.AreEqual(expectedString, resultString, "PS_hyphenate method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_symbol_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            char c = default(char);
            PSLib.PS_symbol(psdoc, c);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_symbol_width_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            char c = default(char);
            int fontid = -1;
            float size = -1;
            float expectedSingle = 0;
            float resultSingle = 0;
            resultSingle = PSLib.PS_symbol_width(psdoc, c, fontid, size);
            Assert.AreEqual(expectedSingle, resultSingle, "PS_symbol_width method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_symbol_namePsdocCFontid_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            char c = default(char);
            int fontid = -1;
            string expectedString = string.Empty;
            string resultString = string.Empty;
            resultString = PSLib.PS_symbol_name(psdoc, c, fontid);
            Assert.AreEqual(expectedString, resultString, "PS_symbol_name method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_include_file_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string filename = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_include_file(psdoc, filename);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_include_file method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_begin_fontPsdocFontnameReserverdABCDEFOptlist_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string fontname = string.Empty;
            int reserverd = -1;
            double a = 0;
            double b = 0;
            double c = 0;
            double d = 0;
            double e = 0;
            double f = 0;
            string optlist = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_begin_font(psdoc, fontname, reserverd, a, b, c, d, e, f, optlist);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_begin_font method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_begin_fontPsdocFontname_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string fontname = string.Empty;
            int expectedInt32 = 0;
            int resultInt32 = 0;
            resultInt32 = PSLib.PS_begin_font(psdoc, fontname);
            Assert.AreEqual(expectedInt32, resultInt32, "PS_begin_font method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_end_font_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_end_font(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_begin_glyph_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string glyphname = string.Empty;
            double wx = 0;
            double llx = 0;
            double lly = 0;
            double urx = 0;
            double ury = 0;
            bool expectedBoolean = false;
            bool resultBoolean = false;
            resultBoolean = PSLib.PS_begin_glyph(psdoc, glyphname, wx, llx, lly, urx, ury);
            Assert.AreEqual(expectedBoolean, resultBoolean, "PS_begin_glyph method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_end_glyph_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            PSLib.PS_end_glyph(psdoc);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_kerning_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int fontid = -1;
            string glyphname1 = string.Empty;
            string glyphname2 = string.Empty;
            int kern = -1;
            PSLib.PS_add_kerning(psdoc, fontid, glyphname1, glyphname2, kern);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_add_ligature_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int fontid = -1;
            string glyphname1 = string.Empty;
            string glyphname2 = string.Empty;
            string glyphname3 = string.Empty;
            PSLib.PS_add_ligature(psdoc, fontid, glyphname1, glyphname2, glyphname3);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_glyph_show_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string name = string.Empty;
            PSLib.PS_glyph_show(psdoc, name);
            //Assert.Fail("Create or modify test(s).");

        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_glyph_listPsdocFontid_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            int fontid = 0;
            PSLib.PS_glyph_list(psdoc, fontid);
            //Assert.Fail("Create or modify test(s).");
        }

        [Test]
        [Ignore("Not implemented.")]
        public void PS_glyph_width_Test()
        {
            IntPtr psdoc = IntPtr.Zero;
            string glyphname = string.Empty;
            int fontid = -1;
            float size = -1;
            float expectedSingle = 0;
            float resultSingle = 0;
            resultSingle = PSLib.PS_glyph_width(psdoc, glyphname, fontid, size);
            Assert.AreEqual(expectedSingle, resultSingle, "PS_glyph_width method returned unexpected result.");
            //Assert.Fail("Create or modify test(s).");

        }

        #endregion Tests
    }
}
