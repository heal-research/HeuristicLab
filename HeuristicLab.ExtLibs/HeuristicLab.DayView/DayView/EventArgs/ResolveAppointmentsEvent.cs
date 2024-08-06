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
using System.Collections.Generic;

namespace Calendar
{
    /// <summary>
    /// Appointment Event Arguments
    /// </summary>
    public class ResolveAppointmentsEventArgs : EventArgs
    {
        #region Private Members

        private DateTime _startDate;
        private int _column;
        private DateTime _endDate;
        private List<Appointment> _appointments;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor for when an idividual column (TeamView) of appointments is sought
        /// </summary>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        /// <param name="column">Column Index</param>
        public ResolveAppointmentsEventArgs(DateTime start, DateTime end, int column)
        {
            _startDate = start;
            _endDate = end;
            _column = column;
            _appointments = new List<Appointment>();
        }

        /// <summary>
        /// Constructor for when appointments are sought
        /// </summary>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        public ResolveAppointmentsEventArgs(DateTime start, DateTime end)
        {
            _startDate = start;
            _endDate = end;
            _appointments = new List<Appointment>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Start Date for appointments
        /// </summary>
        public DateTime StartDate
        {
            get 
            { 
                return _startDate; 
            }

            set 
            { 
                _startDate = value; 
            }
        }

        /// <summary>
        /// Column whose appointments are being sought
        /// </summary>
        public int Column
        {
            get
            {
                return (_column);
            }

            set
            {
                _column = value;
            }
        }

        /// <summary>
        /// End Date for appointments
        /// </summary>
        public DateTime EndDate
        {
            get 
            { 
                return (_endDate); 
            }

            set 
            { 
                _endDate = value; 
            }
        }

        /// <summary>
        /// List of appointments for the date period
        /// </summary>
        public List<Appointment> Appointments
        {
            get 
            { 
                return (_appointments); 
            }

            set 
            { 
                _appointments = value; 
            }
        }

        #endregion Properties
    }

    public delegate void ResolveAppointmentsEventHandler(object sender, ResolveAppointmentsEventArgs e);
}
