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
    /// <summary>
    /// Context Menu Event Arguments
    /// </summary>
    public class ContextMenuEventArgs : EventArgs
    {
        #region Private Members

        private DateTime _dateTime;
        private int _column;
        private ContextMenuStrip _menuStrip;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateTime">Date for column</param>
        /// <param name="Column">Column Index</param>
        /// <param name="Menu">ContextMenu</param>
        public ContextMenuEventArgs(DateTime dateTime, int Column, ContextMenuStrip Menu)
        {
            _dateTime = dateTime;
            _column = Column;
            _menuStrip = Menu;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Date/Time for current column
        /// </summary>
        public DateTime ColumnDateTime
        {
            get
            {
                return (_dateTime);
            }
        }

        /// <summary>
        /// Current Column Index
        /// </summary>
        public int Column
        {
            get
            {
                return (_column);
            }
        }

        /// <summary>
        /// ContextMenu to be shown
        /// </summary>
        public ContextMenuStrip Menu
        {
            get
            {
                return (_menuStrip);
            }
        }

        #endregion Properties
    }

    public delegate void ContextMenuEventHandler(object sender, ContextMenuEventArgs e);
}
