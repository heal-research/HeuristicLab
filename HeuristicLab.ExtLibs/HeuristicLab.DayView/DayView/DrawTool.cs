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

namespace Calendar
{
    public class DrawTool : ITool
    {
        DateTime m_SelectionStart;
        bool m_SelectionStarted;
        public int Column;

        public void Reset()
        {
            m_SelectionStarted = false;
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (e.Button == MouseButtons.Left)
            {
                if (m_SelectionStarted)
                {
                    DateTime m_Time = m_DayView.GetTimeAt(e.X, e.Y, ref Column);
                    m_Time = m_Time.AddMinutes(60 / m_DayView.SlotsPerHour);

                    if (m_Time < m_SelectionStart)
                    {
                        m_DayView.SelectionStart = m_Time;
                        m_DayView.SelectionEnd = m_SelectionStart;
                    }
                    else
                    {
                        m_DayView.SelectionEnd = m_Time;
                    }

                    m_DayView.Invalidate();
                }
            }
        }

        public void MouseUp(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (e.Button == MouseButtons.Left)
            {
                m_DayView.Capture = false;
                m_SelectionStarted = false;

                m_DayView.RaiseSelectionChanged(EventArgs.Empty);

                if (Complete != null)
                    Complete(this, EventArgs.Empty);
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (e.Button == MouseButtons.Left || (e.Button == MouseButtons.Right && DayView.RightMouseSelect))
            {
                m_SelectionStart = m_DayView.GetTimeAt(e.X, e.Y, ref Column);

                m_DayView.SelectionStart = m_SelectionStart;
                m_DayView.SelectionEnd = m_SelectionStart.AddMinutes(60 / m_DayView.SlotsPerHour);
                m_DayView.SelectedColumn = Column;

                m_SelectionStarted = true;

                m_DayView.Invalidate();
                m_DayView.Capture = true;
            }
        }

        private DayView m_DayView;

        public DayView DayView
        {
            get { return m_DayView; }
            set { m_DayView = value; }
        }

        public event EventHandler Complete;
    }
}
