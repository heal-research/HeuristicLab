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
    /// New appointment Event Arguments
    /// </summary>
    public class NewAppointmentEventArgs : EventArgs
    {
        #region Private Members

        private string _title;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _column;

        #endregion Private Members

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Title of appointment</param>
        /// <param name="start">Start Date/Time</param>
        /// <param name="end">End Date/Time</param>
        /// <param name="column">Column where appointment was created</param>
        public NewAppointmentEventArgs(string title, DateTime start, DateTime end, int column)
        {
            _title = title;
            _startDate = start;
            _endDate = end;
            _column = column;
        }

        /// <summary>
        /// Title of appointment
        /// </summary>
        public string Title
        {
            get 
            { 
                return (_title); 
            }
        }

        /// <summary>
        /// Start Date/Time of appointment
        /// </summary>
        public DateTime StartDate
        {
            get 
            { 
                return (_startDate); 
            }
        }

        /// <summary>
        /// End Date/Time for appointment
        /// </summary>
        public DateTime EndDate
        {
            get 
            { 
                return (_endDate); 
            }
        }

        /// <summary>
        /// Column appointment created in
        /// </summary>
        public int Column
        {
            get
            {
                return (_column);
            }
        }
    }

    public delegate void NewAppointmentEventHandler(object sender, NewAppointmentEventArgs e);
}
