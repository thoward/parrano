using System;
using Parrano.Api;
using System.Threading;

namespace Parrano.Examples.PSClock
{
    class PSClock
    {
        static void Main()
        {

            const float radius = 200;
            const float margin = 20;
            float pagecount = 100;

            IntPtr p1 = PSLib.PS_new();

            PSLib.PS_open_file(p1, "psclock.ps");
            Console.Write("Creating psclock.ps ... ");
            PSLib.PS_set_parameter(p1, "warning", "true");

            PSLib.PS_set_info(p1, "Creator", "Parrano");
            PSLib.PS_set_info(p1, "Author", "Uwe Steinmann & Stefan Schroeder & Troy Howard");
            PSLib.PS_set_info(p1, "Title", "Analog Clock");
            PSLib.PS_set_info(p1, "Keywords", "Analog Clock, Time");

            while(pagecount > 0)
            {
                pagecount--;
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;
                int second = DateTime.Now.Second;

                Thread.Sleep(50);

                PSLib.PS_begin_page(p1, 2*(radius + margin), 2*(radius + margin));

                PSLib.PS_set_parameter(p1, "transition", "wipe");
                PSLib.PS_set_value(p1, "duration", .5f);

                PSLib.PS_translate(p1, radius + margin, radius + margin);
                PSLib.PS_save(p1);

            	// minute strokes
                PSLib.PS_setlinewidth(p1, 2);
                for (int i = 0; i < 60; i++)
			    {
                    PSLib.PS_rotate(p1, 6);
                    PSLib.PS_moveto(p1, radius, 0);
                    PSLib.PS_lineto(p1, radius - margin/3, 0);
                    PSLib.PS_stroke(p1);
                    PSLib.PS_restore(p1);
                    PSLib.PS_save(p1);
			    }

                //  5 minute strokes 
                PSLib.PS_setlinewidth(p1, 3);

                for (int i = 0; i < 12; i++)
                {
                    PSLib.PS_rotate(p1, 30);
                    PSLib.PS_moveto(p1, radius, 0);
                    PSLib.PS_lineto(p1, radius - margin, 0);
                    PSLib.PS_stroke(p1);
                }

                // draw hour hand 
                PSLib.PS_restore(p1);
                PSLib.PS_save(p1);
                PSLib.PS_rotate(p1, -((minute/60.0f) + hour - 3.0f)*30.0f);
                PSLib.PS_moveto(p1, -radius/10, -radius/20);
                PSLib.PS_lineto(p1, radius/2, 0.0f);
                PSLib.PS_lineto(p1, -radius/10, radius/20);
                PSLib.PS_closepath(p1);
                PSLib.PS_fill(p1);
                PSLib.PS_restore(p1);

                // draw minute hand 
                PSLib.PS_save(p1);
                PSLib.PS_rotate(p1, -((second/60.0f) + minute - 15.0f)*6.0f);
                PSLib.PS_moveto(p1, -radius/10, -radius/20);
                PSLib.PS_lineto(p1, radius*0.8f, 0.0f);
                PSLib.PS_lineto(p1, -radius/10, radius/20);
                PSLib.PS_closepath(p1);
                PSLib.PS_fill(p1);
                PSLib.PS_restore(p1);

                // draw second hand 
                //    PSLib.PS_setrgbcolor(p1, 1.0, 0.0, 0.0)
                
                PSLib.PS_setlinewidth(p1, 2);
                PSLib.PS_save(p1);
                PSLib.PS_rotate(p1, -((second - 15.0f)*6.0f));
                PSLib.PS_moveto(p1, -radius/5, 0.0f);
                PSLib.PS_lineto(p1, radius, 0.0f);
                PSLib.PS_stroke(p1);
                PSLib.PS_restore(p1);

                // draw little circle at center 
                PSLib.PS_circle(p1, 0, 0, radius/30);
                PSLib.PS_fill(p1);

                PSLib.PS_end_page(p1);
            }

            PSLib.PS_close(p1);
            PSLib.PS_delete(p1);
            Console.WriteLine("Finished.");
        }
    }
}
