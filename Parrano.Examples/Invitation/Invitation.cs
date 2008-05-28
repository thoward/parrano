using System;
using Parrano.Api;

namespace Parrano.Examples.Invitation
{
    class Invitation
    {
        static void Main()
        {
            // This example demostrates how to make a serial letter.


            const int boxwidth = 200;
            const int boxheight = 630;
            const int baseline = 100;
            const int colsep = 20;
            const int leftmargin = 100;

            IntPtr psdoc = PSLib.PS_new();
            PSLib.PS_open_file(psdoc, "invitation.ps");
            Console.WriteLine("Creating inivitation.ps ... ");

            PSLib.PS_set_info(psdoc, "Creator", "Parrano");
            PSLib.PS_set_info(psdoc, "Author", "Stefan Schroeder & Troy Howard");
            PSLib.PS_set_info(psdoc, "Title", "Serial letter example");
            PSLib.PS_set_info(psdoc, "Keywords", "Invitation, letter");
            PSLib.PS_set_info(psdoc, "BoundingBox", "0 0 596 842");

            int antiqua = PSLib.PS_findfont(psdoc, "Helvetica");

            string[] names = { "Alice", "Bob", "Charles" };

            foreach (string name in names)
            {
                PSLib.PS_begin_page(psdoc, 596, 842);
                PSLib.PS_setfont(psdoc, antiqua, 25);
                PSLib.PS_set_value(psdoc, "charspacing", 2);

                string text = "Hello " + name + " you are invited!";

                PSLib.PS_show_xy2(
                    psdoc, 
                    text, 
                    text.Length, 
                    leftmargin + boxwidth + colsep/2 - 200,
                    baseline + boxheight + 20
                    );

                PSLib.PS_end_page(psdoc);
            }

            PSLib.PS_deletefont(psdoc, antiqua);
            PSLib.PS_close(psdoc);
            PSLib.PS_delete(psdoc);

            Console.WriteLine("Finished.");
        }
    }
}
