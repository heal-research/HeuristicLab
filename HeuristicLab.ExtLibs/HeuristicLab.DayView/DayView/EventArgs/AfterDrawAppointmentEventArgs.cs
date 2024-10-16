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
    /// After Draw Appointment Event Arguments
    /// </summary>
    public class AfterDrawAppointmentEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="g">Graphics object where appointment has been drawn</param>
        /// <param name="rect">Rectangle where appointment has been drawn</param>
        /// <param name="appointment">The appointment being drawn</param>
        /// <param name="isSelected">Indicates wether the appointment is selected or not</param>
        /// <param name="gripRect">Grip Rectangle if appointment is selected</param>
        public AfterDrawAppointmentEventArgs(Graphics g, Rectangle rect, Appointment appointment, bool isSelected, Rectangle gripRect)
        {
            Graphics = g;
            Rectangle = rect;
            Appointment = appointment;
            Selected = isSelected;
            GripRectangle = gripRect;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Graphics object where appointment has been drawn
        /// </summary>
        public Graphics Graphics { get; private set; }

        /// <summary>
        /// Rectangle where appointment has been drawn
        /// </summary>
        public Rectangle Rectangle { get; private set; }

        /// <summary>
        /// The appointment being drawn
        /// </summary>
        public Appointment Appointment { get; private set; }

        /// <summary>
        /// Indicates wether the appointment is selected or not
        /// </summary>
        public bool Selected { get; private set; }

        /// <summary>
        /// Grip Rectangle if appointment is selected
        /// </summary>
        public Rectangle GripRectangle { get; private set; }

        #endregion Properties
    }


    public delegate void AfterDrawAppointmentEventHandler(object sender, AfterDrawAppointmentEventArgs e);
}
