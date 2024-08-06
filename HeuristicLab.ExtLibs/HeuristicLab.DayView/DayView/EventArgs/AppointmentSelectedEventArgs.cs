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

namespace Calendar
{
    /// <summary>
    /// Appointment Selected Event Arguments
    /// </summary>
    public class AppointmentSelectedEventArgs : EventArgs
    {
        #region Private Members

        private Appointment _appointment;
        private bool _selected;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appointment">Appointment being selected/deselected</param>
        /// <param name="selected">Indicates selected/deselected</param>
        public AppointmentSelectedEventArgs(Appointment appointment, bool selected)
        {
            _appointment = appointment;
            _selected = selected;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Indicates wether the appointment is being selected/deselected
        /// </summary>
        public bool Selected
        {
            get 
            { 
                return (_selected); 
            }
        }

        /// <summary>
        /// Appointemnt being selected/deselected or null if nothing previously selected
        /// </summary>
        public Appointment Appointment
        {
            get 
            { 
                return _appointment; 
            }
        }

        #endregion Properties
    }

    public delegate void AppointmentSelectedEventHandler(object sender, AppointmentSelectedEventArgs e);

}
