//DayView2 
//original code based on https://calendar.codeplex.com/
//
//modified by Simon Carter (s1cart3r@gmail.com)
//
//Redistribution and use in source and binary forms are permitted
//provided that the above copyright notice and this paragraph are
//duplicated in all such forms and that any documentation,
//advertising materials, and other materials related to such
//distribution and use acknowledge that the software was developed
//by techcoil.com and Simon Carter.  The name of the
//techcoil.com and Simon Carter may not be used to endorse or promote products derived
//from this software without specific prior written permission.
//THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
//IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
//change history
// 09/06/2013 - initial release 
//

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Calendar
{
    public abstract class AbstractRenderer : IDisposable
    {
        #region Destructors

        ~AbstractRenderer()
        {
            Dispose(false);
        }

        #endregion Destructors

        internal DayView dayView;

        public AbstractRenderer(DayView dayview)
        {
            dayView = dayview;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool mainThread)
        {
            if (hourFont != null)
                hourFont.Dispose();

            if (minuteFont != null)
                minuteFont.Dispose();
        }

        public void DoAfterDrawAppointment(Graphics g, Rectangle rect, Appointment appointment, bool isSelected, Rectangle gripRect)
        {
            dayView.RaiseAfterDrawAppointment(new AfterDrawAppointmentEventArgs(g, rect, appointment, isSelected, gripRect));
        }

        public virtual Color AllDayEventsBackColor
        {
            get
            {
                return InterpolateColors(this.BackColor, Color.Black, 0.5f);
            }
        }

        public virtual Font BaseFont
        {
            get
            {
                return Control.DefaultFont;
            }
        }

        public virtual Color HourSeperatorColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(234, 208, 152);
            }
        }

        public virtual Color HalfHourSeperatorColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(243, 228, 177);
            }
        }

        public virtual Color HourColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(255, 244, 188);
            }
        }

        public virtual Color WorkingHourColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(255, 255, 213);
            }
        }

        public virtual Color BackColor
        {
            get
            {
                return SystemColors.Control;
            }
        }

        public virtual Color SelectionColor
        {
            get
            {
                return SystemColors.Highlight;
            }
        }

        private Font hourFont;

        public virtual Font HourFont
        {
            get
            {
                if (hourFont == null)
                {
                    hourFont = new Font(BaseFont.FontFamily, 12, FontStyle.Regular);
                }

                return hourFont;
            }
        }

        private Font minuteFont;

        public virtual Font MinuteFont
        {
            get
            {
                if (minuteFont == null)
                {
                    minuteFont = new Font(BaseFont.FontFamily, 8, FontStyle.Regular);
                }

                return minuteFont;
            }
        }

        public abstract void DrawHourLabel(Graphics g, Rectangle rect, int hour, bool ampm);

        public abstract void DrawMinuteLine(Graphics g, Rectangle rect, int Minute);

        public abstract void DrawDayHeader(Graphics g, Rectangle rect, string Header, DateTime date);

        public abstract void DrawDayBackground(Graphics g, Rectangle rect);

        public virtual void DrawHourRange(Graphics g, Rectangle rect, bool drawBorder, bool hilight, bool canWork)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            Color color = this.SelectionColor;

            if (hilight && canWork)
                color = this.SelectionColor;
            else
            {
                if (canWork)
                    color = this.WorkingHourColor;
                else
                    color = this.BackColor;
            }

            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            if (drawBorder)
                g.DrawRectangle(SystemPens.WindowFrame, rect);
        }

        public virtual void DrawDayGripper(Graphics g, Rectangle rect, int gripWidth, DateTime date)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            Color color = Color.White;

            if (dayView.ViewType == DayView.DayViewType.SingleView)
                color = date.Date == DateTime.Now.Date ? Color.Gold : Color.White;

            using (Brush m_Brush = new SolidBrush(color))
                g.FillRectangle(m_Brush, rect.Left, rect.Top - 1, gripWidth, rect.Height);

            using (Pen m_Pen = new Pen(Color.Black))
                g.DrawRectangle(m_Pen, rect.Left, rect.Top - 1, gripWidth, rect.Height);
        }

        public abstract void DrawAppointment(Graphics g, Rectangle rect, Appointment appointment, bool isSelected, Rectangle gripRect);

        public void DrawAllDayBackground(Graphics g, Rectangle rect)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (Brush brush = new SolidBrush(InterpolateColors(this.BackColor, Color.Black, 0.5f)))
                g.FillRectangle(brush, rect);
        }

        public static Color InterpolateColors(Color color1, Color color2, float percentage)
        {
            int num1 = ((int)color1.R);
            int num2 = ((int)color1.G);
            int num3 = ((int)color1.B);
            int num4 = ((int)color2.R);
            int num5 = ((int)color2.G);
            int num6 = ((int)color2.B);
            byte num7 = Convert.ToByte(((float)(((float)num1) + (((float)(num4 - num1)) * percentage))));
            byte num8 = Convert.ToByte(((float)(((float)num2) + (((float)(num5 - num2)) * percentage))));
            byte num9 = Convert.ToByte(((float)(((float)num3) + (((float)(num6 - num3)) * percentage))));
            return Color.FromArgb(num7, num8, num9);
        }
    }
}
