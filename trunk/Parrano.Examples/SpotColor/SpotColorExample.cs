using System;
using System.Collections.Generic;
using Parrano.Api;

namespace Parrano.Examples.SpotColorExample
{
    class SpotColorExample
    {
        static void Main()
        {
            List<SpotColor> spotcolors = new List<SpotColor>();

            spotcolors.Add(new SpotColor("PANTONE Violet C", new CMYKColor(0.75f, 0.94f, 0, 0)));
            spotcolors.Add(new SpotColor("PANTONE 114 C", new CMYKColor(0, 0.11f, 0.69f, 0)));
            spotcolors.Add(new SpotColor("PANTONE 5565 C", new CMYKColor(0.37f, 0, 0.34f, 0.34f)));
            spotcolors.Add(new SpotColor("RGB Blue", new RGBColor(0, 0, 1)));
            spotcolors.Add(new SpotColor("Gray Black", new GrayColor(0)));
          
            
            PSLib.PS_boot();

            IntPtr psdoc = PSLib.PS_new();

            PSLib.PS_open_file(psdoc, "spotcolor.ps");
            Console.Write("Creating spotcolor.ps ... ");
            PSLib.PS_set_parameter(psdoc, "warning", "true");

            PSLib.PS_set_info(psdoc, "Creator", "Parrano");
            PSLib.PS_set_info(psdoc, "Author", "Uwe Steinmann and Stefan Schroeder and Troy Howard");
            PSLib.PS_set_info(psdoc, "Title", "Spotcolor demonstration");
            PSLib.PS_set_info(psdoc, "Keywords", "Spot color");

            foreach(SpotColor spot in spotcolors)
            {
                PSLib.PS_setcolor(psdoc, ColorUsage.fill, spot.Color);
                spot.ResourceID = PSLib.PS_makespotcolor(psdoc, spot.Name, 0);
            }

            PSLib.PS_begin_page(psdoc, 596, 842);
            footer(psdoc);

            int psfont = PSLib.PS_findfont(psdoc, "Helvetica");

            PSLib.PS_setfont(psdoc, psfont, 7);

            for (int i = 0; i < 11; i++)
            {
                PSLib.PS_show_xy(psdoc, (i * 10).ToString(), 60, 55 + i * 65);
            }

            int position = 100;
            foreach (SpotColor spot in spotcolors)
            {
                colorline(psdoc, position, spot);
                position += 90;
            }


            PSLib.PS_end_page(psdoc);
            PSLib.PS_close(psdoc);
            PSLib.PS_delete(psdoc);
            PSLib.PS_shutdown();

            Console.WriteLine("Finished.");
        }
        
        private static void colorline(IntPtr psdoc, int leftborder, SpotColor spot)
        {
            for (int i = 0; i < 11; i++)
            {
                PSLib.PS_setcolor(psdoc, ColorUsage.fill, ColorSpace.spot, spot.ResourceID, i * 0.1f, 0, 0);
                PSLib.PS_rect(psdoc, leftborder, 35 + i * 65, 50, 50);
                PSLib.PS_fill(psdoc);

                PSLib.PS_setcolor(psdoc, ColorUsage.stroke, ColorSpace.gray, 0, 0, 0, 0);
                int psfont = PSLib.PS_findfont(psdoc, "Helvetica");
                PSLib.PS_setfont(psdoc, psfont, 7);
                PSLib.PS_show_xy(psdoc, spot.Name, leftborder, 100 + 10 * 65 + 13);

                string buffer = string.Empty;
                CMYKColor cmyk;
                RGBColor rgb;

                if ((cmyk = spot.Color as CMYKColor) != null)
                {
                    buffer = string.Format(
                        "{0:0.00}, {1:0.00}, {2:0.00}, {3:0.00}",
                        cmyk.Cyan,
                        cmyk.Magenta,
                        cmyk.Yellow,
                        cmyk.Black);
                }
                else if ((rgb = spot.Color as RGBColor) != null)
                {
                    buffer = string.Format(
                        "{0:0.00}, {1:0.00}, {2:0.00}",
                        rgb.Red,
                        rgb.Green,
                        rgb.Blue);
                }

                PSLib.PS_show_xy(psdoc, buffer, leftborder, 100 + 10 * 65 + 3);
            }
        }

        const int LEFT_BORDER = 50;
        const int BOTTOM_BORDER = 25;

        private static void footer(IntPtr psdoc)
        {
            int psfont = PSLib.PS_findfont(psdoc, "Helvetica");
            PSLib.PS_setfont(psdoc, psfont, 8);
            string buffer = "This file has been created with pslib " + PSLib.PS_get_parameter(psdoc, "dottedversion", default(float));
            PSLib.PS_show_xy(psdoc, buffer, LEFT_BORDER, BOTTOM_BORDER);
        }
    }
}
