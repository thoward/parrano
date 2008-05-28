using System;
using NUnit.Framework;
using System.IO;
using System.Diagnostics;
using Parrano.Api;

namespace Parrano.Tests
{

    [TestFixture]
    public class PSLibExtensionsTests
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

        [Test]
        public void TestOpenPSDocHandleFilename()
        {
            string filename = tempFilename;
            
            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(filename);
            
            Assert.That(psdoc.ToInt32() > 0, "OpenPSDocHandle method returned unexpected result.");

            PSLibExtensions.ClosePSDocHandle(psdoc);

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
        public void TestOpenPSDocHandleFilenamePageSize()
        {
            string filename = tempFilename;
            PageSize pageSize = PageSize.Letter;
            IntPtr expectedIntPtr = IntPtr.Zero;
            IntPtr resultIntPtr = IntPtr.Zero;

            resultIntPtr = PSLibExtensions.OpenPSDocHandle(filename, pageSize);
            
            Assert.Less(expectedIntPtr.ToInt32(), resultIntPtr.ToInt32(), "OpenPSDocHandle method returned unexpected result.");            

            PSLibExtensions.ClosePSDocHandle(resultIntPtr);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(
                resultFileContents.Contains(
                    "%%Page: 1 1" + Environment.NewLine + 
                    "%%PageBoundingBox: 0 0 612 792"
                    ), 
                    "File was created, but the contents are wrong.");

        }

        [Test]
        public void TestOpenPSDocHandleFilenamePageWidthPageHeight()
        {
            string filename = tempFilename;
            
            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(filename, PSLibExtensions.LetterWidth, PSLibExtensions.LetterHeight);

            Assert.That(psdoc.ToInt32() > 0, "OpenPSDocHandle method returned unexpected result.");

            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(
                resultFileContents.Contains(
                "%%Page: 1 1" + Environment.NewLine +
                "%%PageBoundingBox: 0 0 " + PSLibExtensions.LetterWidth + " " + PSLibExtensions.LetterHeight), 
                "File was created, but the contents are wrong.");
        }
        private static string GetPSLibVersionString()
        {
            return "pslib " + PSLib.PS_get_majorversion() + "." + PSLib.PS_get_minorversion() + "." + PSLib.PS_get_subminorversion();
        }

        [Test]
        public void TestOpenPSDocHandleFilenamePageWidthPageHeightInfoValues()
        {
            string filename = tempFilename;
            
            InfoFields infoFields = new InfoFields();
            infoFields.Author = "Ursula Kroeber Le Guin";
            infoFields.Subject = "Testing Parrano";
            infoFields.Title = "A test document";
            infoFields.Creator = "Parrano v1.0";

            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(filename, PSLibExtensions.LetterWidth, PSLibExtensions.LetterHeight, infoFields);

            Assert.That(psdoc.ToInt32() > 0, "OpenPSDocHandle method returned unexpected result.");

            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            string version = GetPSLibVersionString();

            // validate doc created 

            Assert.That(
                resultFileContents.Contains(
                "%%Page: 1 1" + Environment.NewLine +
                "%%PageBoundingBox: 0 0 " + PSLibExtensions.LetterWidth + " " + PSLibExtensions.LetterHeight),
                "File was created, but the contents are wrong.");

            // validate DSC comments.
            Assert.That(
                resultFileContents.Contains(
                string.Format("%%{0}: {1} ({2})", "Creator", infoFields.Creator, version)),
                    "File contents look wrong.");

            Assert.That(
                resultFileContents.Contains(
                string.Format("%%{0}: {1}", "Author", infoFields.Author)),
                    "File contents look wrong.");

            Assert.That(
                resultFileContents.Contains(
                string.Format("%%{0}: {1}", "Title", infoFields.Title)),
                    "File contents look wrong.");

            // validate pdfmarks 
            Assert.That(
                resultFileContents.Contains(
                string.Format("/{0} ({1})", "Subject", infoFields.Subject)),
                    "File contents look wrong.");

            Assert.That(
                resultFileContents.Contains(
                string.Format("/{0} ({1})", "Author", infoFields.Author)),
                    "File contents look wrong.");

            Assert.That(
                resultFileContents.Contains(
                string.Format("/{0} ({1})", "Title", infoFields.Title)),
                    "File contents look wrong.");

            Assert.That(
                resultFileContents.Contains(
                string.Format("/{0} ({1} \\({2}\\))", "Creator", infoFields.Creator, version)),
                    "File contents look wrong.");
        }           

        [Test]
        public void TestDrawLine()
        {
            Random r = new Random();
            float x1 = r.Next(256);
            float y1 = r.Next(256);
            float x2 = r.Next(256);
            float y2 = r.Next(256);

            string filename = tempFilename;

            IntPtr psdoc = PSLibExtensions.OpenPSDocHandle(filename);

            Assert.That(psdoc.ToInt32() > 0, "OpenPSDocHandle method returned unexpected result.");


            PSLibExtensions.DrawLine(psdoc, x1, y1, x2, y2);

            PSLibExtensions.ClosePSDocHandle(psdoc);

            string resultFileContents = File.ReadAllText(tempFilename);

            Trace.WriteLine("Contents:");
            Trace.WriteLine(resultFileContents);

            Assert.That(
                resultFileContents.StartsWith("%!PS-Adobe-3.0"),
                    "File was created, but beginning looks wrong.");

            Assert.That(
                resultFileContents.Contains(
                    "%%BeginSetup" + Environment.NewLine +
                    "PslibDict begin" + Environment.NewLine +
                    "%%EndSetup"),
                    "File was created, but setup looks wrong.");

            string expectedLinePhrase = "newpath" + Environment.NewLine +
                   string.Format("{0:0.00} {1:0.00} a", x1, y1) + Environment.NewLine +
                   string.Format("{0:0.00} {1:0.00} l", x2, y2) + Environment.NewLine +
                   "stroke";

            Trace.WriteLine(string.Empty);
            Trace.WriteLine("Expected line phrase:");
            Trace.WriteLine(expectedLinePhrase);
            Trace.WriteLine(string.Empty);

            Assert.That(
                resultFileContents.Contains(expectedLinePhrase),
                   "File was created, but line phrase looks wrong.");

            Assert.That(
                resultFileContents.EndsWith("%%EOF"),
                    "File was created, but end looks wrong.");

        }

        [Test]
        public void TestClosePSDocHandle()
        {
            IntPtr psdoc = IntPtr.Zero;
            
            // make sure that calling this with a bad handle causes the expected exception

            try
            {
                PSLibExtensions.ClosePSDocHandle(psdoc);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is AccessViolationException);
            }

            // get a valid handle
            psdoc = PSLibExtensions.OpenPSDocHandle(tempFilename);

            Assert.That(psdoc.ToInt32() > 0, "OpenPSDocHandle method returned unexpected result.");

            try
            {
                // close the valid handle
                PSLibExtensions.ClosePSDocHandle(psdoc);
            }
            catch (AccessViolationException avEx)
            {
                // if closing a valid handle causes this exception
                // well, that's really weird.
                Assert.Fail("The pointer returned seems to be valid, but caused an exception when closing it. Exception was: " + avEx.Message);
            }
            catch (Exception ex)
            {
                // if some other exception happens, just pass it along.
                Assert.Fail("An unknown exception occured when closing the handle. Exception was: " + ex.Message);
            }

            // check the file for valid contents.
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
    }
}
