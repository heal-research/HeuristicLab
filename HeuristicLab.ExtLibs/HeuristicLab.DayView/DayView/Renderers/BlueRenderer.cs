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
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Calendar
{
    public class BlueRenderer : AbstractRenderer
    {
        protected override void Dispose(bool mainThread)
        {
            base.Dispose(mainThread);

            if (baseFont != null)
                baseFont.Dispose();
        }

        public BlueRenderer(DayView dayview)
            : base (dayview)
        {
            
        }

        Font baseFont;

        public override Font BaseFont
        {
            get
            {
                if (baseFont == null)
                {
                    baseFont = new Font("Segoe UI", 8, FontStyle.Regular);
                }

                return baseFont;
            }
        }

        /// <summary>
        /// Working Hour Color
        /// </summary>
        public override Color HourColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(230, 237, 247);
            }
        }

        public override Color HalfHourSeperatorColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(165, 191, 225);
            }
        }

        public override Color HourSeperatorColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(0, 0, 0);
            }
        }

        /// <summary>
        /// Working Hour Color
        /// </summary>
        public override Color WorkingHourColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(255, 255, 255);
            }
        }

        /// <summary>
        /// Non Working Hour Color
        /// </summary>
        public override System.Drawing.Color BackColor
        {
            get
            {
                return Color.FromArgb(54, 101, 171);  //0, 113, 225);
            }
        }

        public override Color SelectionColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(41, 76, 122);
            }
        }

        public Color TextColor
        {
            get
            {
                return Color.FromArgb(255, 255, 255); //101, 147, 207);
            }
        }

        public override void DrawHourLabel(System.Drawing.Graphics g, System.Drawing.Rectangle rect, int hour, bool ampm)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (SolidBrush brush = new SolidBrush(this.TextColor))
            {
                string ampmtime;

                if (ampm)
                {
                    if (hour < 12)
                        ampmtime = "AM";
                    else
                        ampmtime = "PM";

                    if (hour != 12)
                        hour = hour % 12;
                }
                else
                    ampmtime = "00";

                g.DrawString(hour.ToString("##00", System.Globalization.CultureInfo.InvariantCulture), HourFont, brush, rect);

                rect.X += 27;
                g.DrawString(ampmtime, MinuteFont, brush, rect);
            }
        }

        public override void DrawMinuteLine(Graphics g, Rectangle rect, int minute)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (Pen pen = new Pen(InterpolateColors(this.TextColor, Color.White, 0.5f)))
                g.DrawLine(pen, rect.Left, rect.Y, rect.Width, rect.Y);

            if (dayView.ShowMinutes && minute > 0)
            {
                rect.X += 27;
                using (SolidBrush brush = new SolidBrush(this.TextColor))
                {
                    g.DrawString(minute.ToString(), MinuteFont, brush, rect);
                }
            }
        }

        public override void DrawDayHeader(System.Drawing.Graphics g, System.Drawing.Rectangle rect, string Header, DateTime date)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.FormatFlags = StringFormatFlags.NoWrap;
                format.LineAlignment = StringAlignment.Center;

                using (StringFormat formatdd = new StringFormat())
                {
                    formatdd.Alignment = StringAlignment.Near;
                    formatdd.FormatFlags = StringFormatFlags.NoWrap;
                    formatdd.LineAlignment = StringAlignment.Center;

                    using (SolidBrush brush = new SolidBrush(this.BackColor))
                        g.FillRectangle(brush, rect);

                    //using (Pen aPen = new Pen(Color.FromArgb(205, 219, 238)))
                    //    g.DrawLine(aPen, rect.Left, rect.Top + (int)rect.Height / 2, rect.Right, rect.Top + (int)rect.Height / 2);

                    using (Pen aPen = new Pen(Color.FromArgb(141, 174, 217)))
                        g.DrawRectangle(aPen, rect);

                    rect.X += 1;
                    rect.Width -= 1;
                    using (Pen aPen = new Pen(Color.FromArgb(141, 174, 217)))
                        g.DrawRectangle(aPen, rect);

                    Rectangle topPart = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, (int)(rect.Height / 2) - 1);
                    Rectangle lowPart = new Rectangle(rect.Left + 1, rect.Top + (int)(rect.Height / 2) + 1, rect.Width - 1, (int)(rect.Height / 2) - 1);

                    using (LinearGradientBrush aGB = new LinearGradientBrush(topPart, this.BackColor, this.BackColor, LinearGradientMode.Vertical))
                        g.FillRectangle(aGB, topPart);

                    using (LinearGradientBrush aGB = new LinearGradientBrush(lowPart, this.BackColor, this.BackColor, LinearGradientMode.Vertical))
                        g.FillRectangle(aGB, lowPart);

                    if (date.Date.Equals(DateTime.Now.Date))
                    {
                        topPart.Inflate((int)(-topPart.Width / 4 + 1), 1); //top left orange area
                        topPart.Offset(rect.Left - topPart.Left + 1, 1);
                        topPart.Inflate(1, 0);
                        using (LinearGradientBrush aGB = new LinearGradientBrush(topPart, Color.FromArgb(247, 207, 114), Color.FromArgb(251, 230, 148), LinearGradientMode.Horizontal))
                        {
                            topPart.Inflate(-1, 0);
                            g.FillRectangle(aGB, topPart);
                        }

                        topPart.Offset(rect.Right - topPart.Right, 0);        //top right orange
                        topPart.Inflate(1, 0);
                        using (LinearGradientBrush aGB = new LinearGradientBrush(topPart, Color.FromArgb(251, 230, 148), Color.FromArgb(247, 207, 114), LinearGradientMode.Horizontal))
                        {
                            topPart.Inflate(-1, 0);
                            g.FillRectangle(aGB, topPart);
                        }

                        using (Pen aPen = new Pen(Color.FromArgb(128, 240, 154, 30))) //center line
                            g.DrawLine(aPen, rect.Left, topPart.Bottom - 1, rect.Right, topPart.Bottom - 1);

                        topPart.Inflate(0, -1);
                        topPart.Offset(0, topPart.Height + 1); //lower right
                        using (LinearGradientBrush aGB = new LinearGradientBrush(topPart, Color.FromArgb(240, 157, 33), Color.FromArgb(250, 226, 142), LinearGradientMode.BackwardDiagonal))
                            g.FillRectangle(aGB, topPart);

                        topPart.Offset(rect.Left - topPart.Left + 1, 0); //lower left
                        using (LinearGradientBrush aGB = new LinearGradientBrush(topPart, Color.FromArgb(240, 157, 33), Color.FromArgb(250, 226, 142), LinearGradientMode.ForwardDiagonal))
                            g.FillRectangle(aGB, topPart);
                        using (Pen aPen = new Pen(Color.FromArgb(238, 147, 17)))
                            g.DrawRectangle(aPen, rect);
                    }

                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    SolidBrush fntBrush;

                    if (date.Date.Equals(DateTime.Now.Date))
                        fntBrush = new SolidBrush(Color.Black);
                    else
                        fntBrush = new SolidBrush(Color.White);


                    if (dayView.ViewType == DayView.DayViewType.SingleView)
                    {
                        //get short dayabbr. if narrow dayrect
                        string sTodaysName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek);
                        if (rect.Width < 105)
                            sTodaysName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek);

                        rect.Offset(2, 1);



                        using (Font fntDay = new Font("Segoe UI", 8))
                            g.DrawString(sTodaysName, fntDay, fntBrush, rect, format);

                        rect.Offset(-2, -1);

                        using (Font fntDayDate = new Font("Segoe UI", 9, FontStyle.Bold))
                            g.DrawString(date.ToString(" d"), fntDayDate, fntBrush, rect, formatdd);
                    }
                    else
                    {
                        //get short dayabbr. if narrow dayrect
                        string sTodaysName = Header;
                        if (rect.Width < 105)
                            sTodaysName = Header.Substring(0, 3) + "...";

                        rect.Offset(2, 1);

                        using (Font fntDay = new Font("Segoe UI", 8))
                            g.DrawString(sTodaysName, fntDay, fntBrush, rect, format);

                        //rect.Offset(-2, -1);

                        //using (Font fntDayDate = new Font("Segoe UI", 9, FontStyle.Bold))
                        //    g.DrawString(Header, fntDayDate, SystemBrushes.WindowText, rect, formatdd);
                    }
                }
            }
        }

        public override void DrawDayBackground(System.Drawing.Graphics g, System.Drawing.Rectangle rect)
        {

            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(54, 101, 171))) //0xe6, 0xed, 0xf7)))
                g.FillRectangle(backBrush, rect);
        }

        public override void DrawAppointment(System.Drawing.Graphics g, System.Drawing.Rectangle rect, Appointment appointment, bool isSelected, System.Drawing.Rectangle gripRect)
        {
            if (appointment == null)
                throw new ArgumentNullException("appointment");

            if (g == null)
                throw new ArgumentNullException("g");

            /*
             * Logic for drawing the appointment: 
             * 1) Do something messy with the colours
             * 
             * 2) Determine background pattern
             * 2.1) App is locked -> HatchBrush
             * 2.2) Normal app -> Nothing
             * 
             * 3) Draw the background of appointment
             * 
             * 4) Draw the edges of appointment
             * 4.1) If app is selected -> just draw the selection rectangle
             * 4.2) If not -> draw the gripper, border (if required) and shadows
             */

            if (rect.Width != 0 && rect.Height != 0)
            {

                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;

                    Color start = InterpolateColors(appointment.Color, Color.White, 0.4f);
                    Color end = InterpolateColors(appointment.Color, Color.FromArgb(191, 210, 234), 0.7f);

                    // if appointment is locked, draw different background pattern
                    if ((appointment.Locked && !appointment.AllDayEvent))
                    {
                        // Draw back
                        using (Brush m_Brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LargeConfetti, Color.Blue, appointment.Color))
                            g.FillRectangle(m_Brush, rect);

                        // little transparent
                        start = Color.FromArgb(230, start);
                        end = Color.FromArgb(180, end);

                        GraphicsPath path = new GraphicsPath();
                        path.AddRectangle(rect);

                    }
                    else if (appointment.UseCustomHatchStyle)
                    {
                        using (Brush m_Brush = new System.Drawing.Drawing2D.HatchBrush(appointment.CustomHatch, Color.Pink, appointment.Color))
                            g.FillRectangle(m_Brush, rect);

                        // little transparent
                        start = Color.FromArgb(230, start);
                        end = Color.FromArgb(180, end);

                        GraphicsPath path = new GraphicsPath();
                        path.AddRectangle(rect);
                    }

                    // Draw the background of the appointment

                    using (LinearGradientBrush aGB = new LinearGradientBrush(rect, start, end, LinearGradientMode.Vertical))
                        g.FillRectangle(aGB, rect);

                    // If the appointment is selected, only need to draw the selection frame

                    if (isSelected)
                    {
                        Rectangle m_BorderRectangle = rect;

                        using (Pen m_Pen = new Pen(appointment.BorderColor, 3))
                            g.DrawRectangle(m_Pen, rect);

                        m_BorderRectangle.Inflate(2, 2);

                        using (Pen m_Pen = new Pen(SystemColors.WindowFrame, 1))
                            g.DrawRectangle(m_Pen, m_BorderRectangle);

                        m_BorderRectangle.Inflate(-4, -4);

                        using (Pen m_Pen = new Pen(SystemColors.WindowFrame, 1))
                            g.DrawRectangle(m_Pen, m_BorderRectangle);
                    }
                    else
                    {
                        // Draw gripper

                        gripRect.Width += 1;

                        start = InterpolateColors(appointment.BorderColor, appointment.Color, 0.2f);
                        end = InterpolateColors(appointment.BorderColor, Color.White, 0.6f);

                        using (LinearGradientBrush aGB = new LinearGradientBrush(rect, start, end, LinearGradientMode.Vertical))
                            g.FillRectangle(aGB, gripRect);

                        //  Draw border if needed
                        if (appointment.DrawBorder)
                            using (Pen m_Pen = new Pen(SystemColors.WindowFrame, 1))
                                g.DrawRectangle(m_Pen, rect);

                        // Draw shadow lines
                        int xLeft = rect.X + 6;
                        int xRight = rect.Right + 1;
                        int yTop = rect.Y + 1;
                        int yButton = rect.Bottom + 1;

                        for (int i = 0; i < 5; i++)
                        {
                            using (Pen shadow_Pen = new Pen(Color.FromArgb(70 - 12 * i, Color.Black)))
                            {
                                g.DrawLine(shadow_Pen, xLeft + i, yButton + i, xRight + i - 1, yButton + i); //horisontal lines
                                g.DrawLine(shadow_Pen, xRight + i, yTop + i, xRight + i, yButton + i); //vertical
                            }
                        }

                    }

                    // draw appointment text
                    rect.X += gripRect.Width;
                    
                    // width of shadow is 6.
                    rect.Width -= 6;

                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    int oldRectY = rect.Y;

                    if (dayView.AlwaysShowAppointmentText)
                    {
                        if ((rect.Height - rect.Y) >= (dayView.HalfHourHeight * 3) && rect.Y < dayView.HeaderHeight)
                        {
                            if (rect.Y + dayView.HeaderHeight < rect.Height)
                            {
                                rect.Y = dayView.HeaderHeight;

                                if ((rect.Y - oldRectY) > (rect.Height - 40))
                                    rect.Y = oldRectY;
                            }
                        }

                        format.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
                    }

                    g.DrawString(appointment.Title, this.BaseFont, new SolidBrush(appointment.TextColor), rect, format);
                    rect.Y = oldRectY;

                    g.TextRenderingHint = TextRenderingHint.SystemDefault;

                    //Partial Owner Draw Appointment Here
                    DoAfterDrawAppointment(g, rect, appointment, isSelected, gripRect);
                }
            }
        }
    }
}
