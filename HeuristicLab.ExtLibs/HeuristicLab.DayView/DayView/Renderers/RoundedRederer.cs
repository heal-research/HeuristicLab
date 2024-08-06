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
using System.Windows.Forms;
using System.Drawing;
using Plasmoid.Extensions;

namespace Calendar
{
    public class RoundedRender : AbstractRenderer
    {
        private int _radius = 3;
        private RectangleEdgeFilter _edgeFilter = RectangleEdgeFilter.All;

        protected override void Dispose(bool mainThread)
        {
            base.Dispose(mainThread);

            if (minuteFont != null)
                minuteFont.Dispose();
        }

        public RoundedRender(DayView dayview)
            : base (dayview)
        {
            
        }

        private Font minuteFont;

        public override Font MinuteFont
        {
            get
            {
                if (minuteFont == null)
                    minuteFont = new Font(BaseFont, FontStyle.Italic);

                return minuteFont;
            }
        }

        public override void DrawHourLabel(Graphics g, Rectangle rect, int hour, bool ampm)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            g.DrawString(hour.ToString("##00", System.Globalization.CultureInfo.InvariantCulture), HourFont, SystemBrushes.ControlText, rect);

            rect.X += 27;

            g.DrawString("00", MinuteFont, SystemBrushes.ControlText, rect);
        }

        public override void DrawMinuteLine(Graphics g, Rectangle rect, int minute)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            Color m_Color = ControlPaint.LightLight(SystemColors.WindowFrame);

            m_Color = ControlPaint.Light(m_Color);

            using (Pen m_Pen = new Pen(m_Color))
                g.DrawLine(m_Pen, rect.Left, rect.Y, rect.Width, rect.Y);

            if (dayView.ShowMinutes && minute > 0)
            {
                rect.X += 27;
                g.DrawString(minute.ToString(), MinuteFont, SystemBrushes.ControlText, rect);
            }
        }

        public override void DrawDayHeader(Graphics g, Rectangle rect, string Header, DateTime date)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.FormatFlags = StringFormatFlags.NoWrap;
                format.LineAlignment = StringAlignment.Center;
                rect.X = rect.X + dayView.appointmentGripWidth;
                rect.Width = rect.Width - (dayView.appointmentGripWidth + 1);
                ControlPaint.DrawButton(g, rect, ButtonState.Inactive);
                ControlPaint.DrawBorder3D(g, rect, Border3DStyle.Etched);

                if (dayView.ViewType == DayView.DayViewType.TeamView)
                {
                    g.DrawString(
                        Header,
                        BaseFont,
                        SystemBrushes.WindowText,
                        rect,
                        format
                        );
                }
                else
                {
                    g.DrawString(
                        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek),
                        BaseFont,
                        SystemBrushes.WindowText,
                        rect,
                        format
                        );
                }
            }
        }

        public override void DrawDayBackground(Graphics g, Rectangle rect)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (Brush m_Brush = new SolidBrush(this.HourColor))
                g.FillRectangle(m_Brush, rect);
        }

        public override void DrawAppointment(Graphics g, Rectangle rect, Appointment appointment, bool isSelected, Rectangle gripRect)
        {
            if (appointment == null)
                throw new ArgumentNullException("appointment");

            if (g == null)
                throw new ArgumentNullException("g");

            rect.X = rect.Left + dayView.appointmentGripWidth + 1;
            rect.Width = rect.Width - dayView.appointmentGripWidth + 1;

            if (rect.Width != 0 && rect.Height != 0)
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;

                    if ((appointment.Locked) && isSelected)
                    {
                        // Draw back
                        using (Brush m_Brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Wave, Color.LightGray, appointment.Color))
                            g.FillRoundedRectangle(m_Brush, rect, _radius, _edgeFilter);
                    }
                    else if (appointment.UseCustomHatchStyle)
                    {
                        // Draw back
                        using (Brush m_Brush = new System.Drawing.Drawing2D.HatchBrush(appointment.CustomHatch, Color.LightGray, appointment.Color))
                            g.FillRectangle(m_Brush, rect);
                    }
                    else
                    {
                        // Draw back
                        using (SolidBrush m_Brush = new SolidBrush(appointment.Color))
                            g.FillRoundedRectangle(m_Brush, rect, _radius, _edgeFilter);
                    }

                    if (isSelected)
                    {
                        //using (Pen m_Pen = new Pen(appointment.BorderColor, 4))
                        //    g.DrawRoundedRectangle(m_Pen, rect, _radius, _edgeFilter);

                        Rectangle m_BorderRectangle = rect;

                        m_BorderRectangle.Inflate(2, 2);

                        using (Pen m_Pen = new Pen(SystemColors.WindowFrame, 1))
                            g.DrawRoundedRectangle(m_Pen, m_BorderRectangle, _radius, _edgeFilter);

                        m_BorderRectangle.Inflate(-4, -4);

                        using (Pen m_Pen = new Pen(SystemColors.WindowFrame, 1))
                            g.DrawRoundedRectangle(m_Pen, m_BorderRectangle, _radius, _edgeFilter);
                    }
                    else
                    {
                        // Draw gripper
                        gripRect.Width += 1;

                        //using (SolidBrush m_Brush = new SolidBrush(appointment.BorderColor))
                        //    g.FillRoundedRectangle(m_Brush, gripRect, _radius, _edgeFilter);

                        using (Pen m_Pen = new Pen(SystemColors.WindowFrame, 1))
                            g.DrawRoundedRectangle(m_Pen, rect, _radius, _edgeFilter);
                    }

                    rect.X += gripRect.Width;

                    int oldRectY = rect.Y;

                    if (dayView.AlwaysShowAppointmentText)
                    {
                        if ((rect.Height - rect.Y) > 100 && rect.Y < dayView.HeaderHeight)
                        {
                            if (rect.Y + dayView.HeaderHeight < rect.Height)
                            {
                                rect.Y = dayView.HeaderHeight;

                                if ((rect.Y - oldRectY) > (rect.Height - 40))
                                    rect.Y = oldRectY;
                            }
                        }
                    }

                    g.DrawString(appointment.Title, this.BaseFont, new SolidBrush(appointment.TextColor), rect, format);
                    rect.Y = oldRectY;
                }

            //Partial Owner Draw Appointment Here
            DoAfterDrawAppointment(g, rect, appointment, isSelected, gripRect);
        }
    }
}
