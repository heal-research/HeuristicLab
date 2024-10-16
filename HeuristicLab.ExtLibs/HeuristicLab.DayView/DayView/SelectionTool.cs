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

namespace Calendar
{
    /// <summary>
    /// Selection Tool
    /// </summary>
    public class SelectionTool : ITool
    {
        #region Private Members

        private DayView dayView;
        private DateTime startDate;
        private TimeSpan length;
        private SelectionMode mode;
        private TimeSpan delta;
        private int _column;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Dayview object being manipulated
        /// </summary>
        public DayView DayView
        {
            get
            {
                return dayView;
            }
            set
            {
                dayView = value;
            }
        }

        public int Column
        {
            get
            {
                return (_column);
            }
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Resets the current selection
        /// </summary>
        public void Reset()
        {
            length = TimeSpan.Zero;
            delta = TimeSpan.Zero;
        }

        public void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            Appointment selection = dayView.SelectedAppointment;
            Rectangle viewrect = dayView.GetTrueRectangle();
            Rectangle fdrect = dayView.GetFullDayApptsRectangle();

            if (viewrect.Contains(e.Location) || fdrect.Contains(e.Location))
            {
                if ((selection != null) && (!selection.Locked))
                {
                    switch (e.Button)
                    {
                        case System.Windows.Forms.MouseButtons.Left:

                            // Get time at mouse position
                            DateTime m_Date = dayView.GetTimeAt(e.X, e.Y, ref _column);

                            switch (mode)
                            {
                                case SelectionMode.Move:
                                    //can the appointment be moved?
                                    AppointmentMoveEventArgs args = new AppointmentMoveEventArgs(selection);
                                    dayView.RaiseBeforeAppointmentMoved(args);

                                    if (!args.AllowMove)
                                        mode = SelectionMode.None;

                                    // This works for regular (i.e. non full-day or multi-day appointments)

                                    if (!selection.AllDayEvent && viewrect.Contains(e.Location))
                                    {
                                        // add delta value
                                        m_Date = m_Date.Add(delta);

                                        if (length == TimeSpan.Zero)
                                        {
                                            startDate = selection.StartDate;
                                            length = selection.EndDate - startDate;
                                        }
                                        else
                                        {
                                            DateTime m_EndDate = m_Date.Add(length);

                                            if (m_EndDate.Day == m_Date.Day)
                                            {
                                                if (selection.StartDate != m_Date || selection.Column != Column)
                                                    selection.AppointmentMoved = true;

                                                selection.StartDate = m_Date;
                                                selection.EndDate = m_EndDate;
                                                selection.Column = _column;
                                                dayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                                dayView.Invalidate();
                                            }
                                            else
                                            {
                                                if (selection.StartDate != m_Date || selection.Column != _column)
                                                    selection.AppointmentMoved = true;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (fdrect.Contains(e.Location))
                                        {
                                            m_Date = m_Date.Add(delta);

                                            int m_DateDiff = m_Date.Subtract(selection.StartDate).Days;

                                            if (m_DateDiff != 0)
                                            {
                                                if (selection.StartDate.AddDays(m_DateDiff) > dayView.StartDate)
                                                {
                                                    selection.StartDate = selection.StartDate.AddDays(m_DateDiff);
                                                    selection.EndDate = selection.EndDate.AddDays(m_DateDiff);
                                                    dayView.Invalidate();
                                                    dayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                                }
                                            }
                                        }
                                    }

                                    break;

                                case SelectionMode.ResizeBottom:
                                    if (!dayView.AllowAppointmentResize)
                                        break;

                                    if (m_Date > selection.StartDate)
                                    {
                                        if (selection.EndDate.Day == m_Date.Day)
                                        {
                                            dayView.SelectedAppointment.AppointmentMoved = true;
                                            selection.EndDate = m_Date;
                                            dayView.Invalidate();
                                            dayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));

                                        }
                                    }

                                    break;

                                case SelectionMode.ResizeTop:
                                    if (!dayView.AllowAppointmentResize)
                                        break;

                                    if (m_Date < selection.EndDate)
                                    {
                                        if (selection.StartDate.Day == m_Date.Day)
                                        {
                                            dayView.SelectedAppointment.AppointmentMoved = true;
                                            selection.StartDate = m_Date;
                                            dayView.Invalidate();
                                            dayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                        }
                                    }
                                    break;

                                case SelectionMode.ResizeLeft:
                                    if (!selection.AllDayEvent || !dayView.AllowAppointmentResize)
                                        break;

                                    if (m_Date.Date < selection.EndDate.Date)
                                    {
                                        dayView.SelectedAppointment.AppointmentMoved = true;
                                        selection.StartDate = m_Date.Date;
                                        dayView.Invalidate();
                                        dayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                    }

                                    break;

                                case SelectionMode.ResizeRight:
                                    if (!selection.AllDayEvent || !dayView.AllowAppointmentResize)
                                        break;

                                    if (m_Date.Date >= selection.StartDate.Date)
                                    {
                                        dayView.SelectedAppointment.AppointmentMoved = true;
                                        selection.EndDate = m_Date.Date.AddDays(1);
                                        dayView.Invalidate();
                                        dayView.RaiseAppointmentMove(new AppointmentEventArgs(selection));
                                    }

                                    break;
                            }

                            break;

                        default:

                            SelectionMode tmpNode = GetMode(e);

                            switch (tmpNode)
                            {
                                case SelectionMode.Move:
                                    dayView.Cursor = System.Windows.Forms.Cursors.Default;
                                    break;
                                case SelectionMode.ResizeBottom:
                                case SelectionMode.ResizeTop:
                                    if (!selection.AllDayEvent & dayView.AllowAppointmentResize)
                                        dayView.Cursor = System.Windows.Forms.Cursors.SizeNS;
                                    break;
                                case SelectionMode.ResizeLeft: // changed by Gimlei
                                case SelectionMode.ResizeRight:
                                    if (selection.AllDayEvent & dayView.AllowAppointmentResize)
                                        DayView.Cursor = System.Windows.Forms.Cursors.SizeWE;
                                    break;
                            }

                            break;
                    }
                }
            }
        }

        public void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");


            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (GetMode(e) == SelectionMode.Move)
                {
                    if (dayView.SelectedAppointment.AppointmentMoved)
                    {
                        dayView.SelectedAppointment.AppointmentMoved = false;
                        dayView.RaiseAppointmentMoved(new AppointmentEventArgs(dayView.SelectedAppointment));
                        dayView.SelectedAppointment = null;
                        dayView.Invalidate();
                    }
                }

                if (Complete != null)
                    Complete(this, EventArgs.Empty);
            }

            dayView.RaiseSelectionChanged(EventArgs.Empty);

            mode = SelectionMode.Move;

            delta = TimeSpan.Zero;

            if (dayView.Cursor != System.Windows.Forms.Cursors.Default)
                dayView.Cursor = System.Windows.Forms.Cursors.Default;
        }

        public void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (dayView.SelectedAppointmentIsNew)
            {
                dayView.RaiseNewAppointment();
            }

            if (dayView.CurrentlyEditing)
                dayView.FinishEditing(false);

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                return;

            mode = GetMode(e);

            if (dayView.SelectedAppointment != null && e.Button != System.Windows.Forms.MouseButtons.Middle)
            {
                DateTime downPos = dayView.GetTimeAt(e.X, e.Y, ref _column);
                // Calculate delta time between selection and clicked point
                delta = dayView.SelectedAppointment.StartDate - downPos;
            }
            else
            {
                delta = TimeSpan.Zero;
            }

            length = TimeSpan.Zero;
        }

        #endregion Public Methods

        #region Private Methods

        private SelectionMode GetMode(System.Windows.Forms.MouseEventArgs e)
        {
            DayView.AppointmentView view = null;
            Boolean gotview = false;

            if (dayView.SelectedAppointment == null || e.Button == System.Windows.Forms.MouseButtons.Middle)
                return (SelectionMode.None);

            if (dayView.appointmentViews.ContainsKey(dayView.SelectedAppointment))
            {
                view = dayView.appointmentViews[dayView.SelectedAppointment];
                gotview = true;
            }

            else if (dayView.longappointmentViews.ContainsKey(dayView.SelectedAppointment))
            {
                view = dayView.longappointmentViews[dayView.SelectedAppointment];
                gotview = true;
            }

            if (gotview)
            {
                Rectangle topRect = view.Rectangle;
                Rectangle bottomRect = view.Rectangle;
                Rectangle leftRect = view.Rectangle;
                Rectangle rightRect = view.Rectangle;

                bottomRect.Y = bottomRect.Bottom - 5;
                bottomRect.Height = 5;
                topRect.Height = 5;
                leftRect.Width = 5;
                rightRect.X += rightRect.Width - 5;
                rightRect.Width = 5;

                if (topRect.Contains(e.Location))
                    return (SelectionMode.ResizeTop);
                else if (bottomRect.Contains(e.Location))
                    return (SelectionMode.ResizeBottom);
                else if (rightRect.Contains(e.Location)) // changed by Gimlei
                    return (SelectionMode.ResizeRight);
                else if (leftRect.Contains(e.Location))
                    return (SelectionMode.ResizeLeft);
                else
                    return (SelectionMode.Move);
            }

            return SelectionMode.None;
        }

        #endregion Private Methods

        #region Events

        /// <summary>
        /// Event when operation is completed
        /// </summary>
        public event EventHandler Complete;

        #endregion Events
    }
}
