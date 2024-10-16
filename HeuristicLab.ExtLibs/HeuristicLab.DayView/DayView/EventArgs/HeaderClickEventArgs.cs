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
    /// Header Clicked Event Arguments
    /// </summary>
    public class HeaderClickEventArgs : EventArgs
    {
        #region Private Members

        private DateTime _dateTime;
        private int _column;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateTime">Current Date for Column</param>
        /// <param name="Column">Column Inex</param>
        public HeaderClickEventArgs(DateTime dateTime, int Column)
        {
            _dateTime = dateTime.Date;
            _column = Column;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Date of column being clicked
        /// </summary>
        public DateTime CurrentDate
        {
            get
            {
                return (_dateTime);
            }
        }

        /// <summary>
        /// Column being clicked
        /// </summary>
        public int Column
        {
            get
            {
                return (_column);
            }
        }

        #endregion Properties
    }

    public delegate void HeaderClickEventHandler(object sender, HeaderClickEventArgs e);
}
