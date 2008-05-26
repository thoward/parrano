using System;
using Parrano.Api;

namespace Hyperlinks
{
    class Hyperlinks
    {
        static void Main()
        {
            const int fontsize = 12; 

            IntPtr psdoc = PSLib.PS_new();
            PSLib.PS_open_file(psdoc, "hyperlinks.ps");

            Console.WriteLine("Creating hyperlinks.ps ... ");

            PSLib.PS_begin_page(psdoc, 596, 842);

            PSLib.PS_add_bookmark(psdoc, "First Page", 0, false);
            int psfont = PSLib.PS_findfont(psdoc, "Helvetica", null, false);
            PSLib.PS_setfont(psdoc, psfont, fontsize);
            PSLib.PS_set_value(psdoc, "leading", 14);

            PSLib.PS_show_xy(psdoc, "This is a web link", 100, 100);
            float len = PSLib.PS_stringwidth(psdoc, "This is a web link", psfont, fontsize);
            PSLib.PS_add_weblink(psdoc, 100, 100, 100 + len, 130, "http://www.kitadb.de");

            PSLib.PS_show_xy(psdoc, "This is a pdf link to an external document", 100, 150);
            len = PSLib.PS_stringwidth(psdoc, "This is a pdf link to an external document", psfont, fontsize);
            PSLib.PS_add_pdflink(psdoc, 100, 150, 100 + len, 180, "test.pdf", 1, DocumentViewType.fitpage);

            PSLib.PS_show_xy(psdoc, "This is a launch link", 100, 200);
            len = PSLib.PS_stringwidth(psdoc, "This is a launch link", psfont, fontsize);
            PSLib.PS_add_launchlink(psdoc, 100, 200, 100 + len, 230, "/usr/bin/gedit");

            PSLib.PS_show_xy(psdoc, "This is a pdf link within the document", 100, 250);
            len = PSLib.PS_stringwidth(psdoc, "This is a pdf link within the document", psfont, fontsize);
            PSLib.PS_add_locallink(psdoc, 100, 250, 100 + len, 280, 2, DocumentViewType.fitpage);

            PSLib.PS_end_page(psdoc);

            PSLib.PS_begin_page(psdoc, 300, 300);
            PSLib.PS_add_bookmark(psdoc, "Second Page", 0, false);
            PSLib.PS_add_note(psdoc, 100, 100, 200, 200, "This is the contents of the note", "Title of Note", PdfNoteIcon.help, true);
            PSLib.PS_end_page(psdoc);

            PSLib.PS_close(psdoc);
            PSLib.PS_delete(psdoc);

            Console.WriteLine("Finished");
        }
    }
}
