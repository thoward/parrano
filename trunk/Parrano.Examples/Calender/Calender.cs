using System;
using System.Globalization;
using Parrano.Api;

namespace Parrano.Examples.Calender
{
    class Calender
    {
        const int BOX_HEIGHT = 120;
        const int BOX_WIDTH = 80;
        const int BOX_TITLE_HEIGHT = 20;

        const int XSIZE=596;
        const int YSIZE=842;

        const int LEFT_BORDER = (XSIZE - 7 * BOX_WIDTH) / 2;

        const string TEXT_FONT = "Helvetica";
        const string TITLE_FONT = "Helvetica";
        
        private static void draw_calendar_box(IntPtr psdoc, DateTime date, int font)
        {
            float llx = LEFT_BORDER + (date.Day % 7) * BOX_WIDTH;
            float lly = YSIZE - 2 * BOX_HEIGHT - date.Day / 7 * BOX_HEIGHT;
            
            PSLib.PS_save(psdoc);
            PSLib.PS_translate(psdoc, llx, lly);
            PSLib.PS_setcolor(psdoc, ColorUsage.stroke, new GrayColor(.5f));
            PSLib.PS_rect(psdoc, 0, BOX_HEIGHT - BOX_TITLE_HEIGHT,
                       BOX_WIDTH, BOX_TITLE_HEIGHT);
            PSLib.PS_fill(psdoc);
            PSLib.PS_setcolor(psdoc, ColorUsage.stroke, new GrayColor(1));
            PSLib.PS_setfont(psdoc, font, 11.5f);

            string title = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek);

            PSLib.PS_show_xy(psdoc, title, 10, BOX_HEIGHT - BOX_TITLE_HEIGHT + 5);

            PSLib.PS_setcolor(psdoc, ColorUsage.stroke, new GrayColor(.1f));
            PSLib.PS_setfont(psdoc, font, 20);
            
            PSLib.PS_show_xy(psdoc, date.Day.ToString(), 10, 10);

            PSLib.PS_setlinewidth(psdoc, 1);
            PSLib.PS_setcolor(psdoc, ColorUsage.stroke, new GrayColor(0));
            PSLib.PS_rect(psdoc, 0, 0, BOX_WIDTH, BOX_HEIGHT);
            PSLib.PS_stroke(psdoc);
            
            PSLib.PS_moveto(psdoc, 0, BOX_HEIGHT - BOX_TITLE_HEIGHT);
            PSLib.PS_lineto(psdoc, BOX_WIDTH, BOX_HEIGHT - BOX_TITLE_HEIGHT);
            
            PSLib.PS_stroke(psdoc);
        }

        private static void end_calendar_box(IntPtr psdoc)
        {
            PSLib.PS_restore(psdoc);
        }

        static void Main()
        {
            DateTime date = DateTime.Now;
            
            IntPtr psdoc=PSLib.PS_new();
            
            PSLib.PS_set_info(psdoc, "Creator", "Parrano");
            PSLib.PS_set_info(psdoc, "Author", "Uwe Steinmann & Stefan Schroeder & Troy Howard");
            PSLib.PS_set_info(psdoc, "Title", "Calendar");

            PSLib.PS_open_file(psdoc, "calendar.ps");
            Console.Write("Creating calendar.ps ... ");
            PSLib.PS_begin_page(psdoc, XSIZE, YSIZE);

            // Assemble Title
            int psfont = PSLib.PS_findfont(psdoc, TITLE_FONT);

            PSLib.PS_setfont(psdoc, psfont, 35);

            string monthAndYear = date.ToString("y");
            float textwidth = PSLib.PS_stringwidth(psdoc, monthAndYear, psfont, 35);

            PSLib.PS_show_xy2(psdoc, monthAndYear, monthAndYear.Length, XSIZE/2f-textwidth/2, YSIZE-0.5f*BOX_HEIGHT);

            // Assemble Table
            psfont = PSLib.PS_findfont(psdoc, TEXT_FONT);
            
            int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(date.Year, date.Month);
            
            for (int i = 1; i < days+1; i++)
            {
                draw_calendar_box(psdoc, new DateTime(date.Year, date.Month, i), psfont);
                end_calendar_box(psdoc);
            }

            PSLib.PS_end_page(psdoc);
            PSLib.PS_close(psdoc);
            PSLib.PS_delete(psdoc);
            Console.WriteLine("Finished.");
        }
    }
}
