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
    /// Working hours event arguments
    /// </summary>
    public class WorkingHoursEventArgs : EventArgs
    {
        #region Private Members

        private int _workingHourStart;
        private int _workingMinuteStart;
        private int _workingHourFinish;
        private int _workingMinuteFinish;
        private DateTime _date;
        private int _column;
        private bool _canWork;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="date">Date for column</param>
        /// <param name="column">Column number</param>
        public WorkingHoursEventArgs(DateTime date, int column)
        {
            _canWork = true;
            _date = date;
            _column = column;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Indicates wether this is a working day
        /// </summary>
        public bool CanWork
        {
            get
            {
                return (_canWork);
            }

            set
            {
                _canWork = value;
            }
        }

        /// <summary>
        /// Column for working hours
        /// </summary>
        public int Column
        {
            get
            {
                return (_column);
            }
        }

        /// <summary>
        /// Date 
        /// </summary>
        public DateTime Date
        {
            get
            {
                return (_date);
            }
        }

        /// <summary>
        /// Working hour start
        /// </summary>
        public int WorkingHourStart
        {
            get
            {
                return (_workingHourStart);
            }

            set
            {
                _workingHourStart = value;
            }
        }

        /// <summary>
        /// Working minute start
        /// </summary>
        public int WorkingMinuteStart
        {
            get
            {
                return (_workingMinuteStart);
            }

            set
            {
                _workingMinuteStart = value;
            }
        }

        /// <summary>
        /// Working Hour finishes
        /// </summary>
        public int WorkingHourFinish
        {
            get
            {
                return (_workingHourFinish);
            }

            set
            {
                _workingHourFinish = value;
            }
        }

        /// <summary>
        /// Working minute finishes
        /// </summary>
        public int WorkingMinuteFinish
        {
            get
            {
                return (_workingMinuteFinish);
            }

            set
            {
                _workingMinuteFinish = value;
            }
        }

        #endregion Properties
    }

    public delegate void WorkingHoursEventHandler(object sender, WorkingHoursEventArgs e);
}
