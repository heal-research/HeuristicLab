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
    /// Appointment Move Event Arguments
    /// </summary>
    public class AppointmentMoveEventArgs : EventArgs
    {
        #region Private Members

        private Appointment _appointment;
        private bool _allowMove;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appointment">Applintment to be moved</param>
        public AppointmentMoveEventArgs(Appointment appointment)
        {
            _appointment = appointment;
            _allowMove = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Determines wether an appointment can be moved or not
        /// </summary>
        public bool AllowMove
        {
            get
            {
                return (_allowMove);
            }

            set
            {
                _allowMove = value;
            }
        }

        /// <summary>
        /// Appointment to be moved
        /// </summary>
        public Appointment Appointment
        {
            get { return _appointment; }
        }

        #endregion Properties
    }

    public delegate void BeforeMoveAppointmentEventHandler(object sender, AppointmentMoveEventArgs e);
}
