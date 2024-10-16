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
    /// Tooltip Event Arguments
    /// </summary>
    public class TooltipEventArgs : EventArgs
    {
        #region Private Members

        private DateTime _dateTime;
        private int _column;
        private bool _baloon;
        private string _title;
        private string _text;
        private bool _isHeader;
        private ToolTipIcon _icon;
        private Appointment _appointment;

        private bool _allowShow;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appointment">Appointment if the cursor is over an appointment</param>
        /// <param name="dateTime">Date/Time where the cursor is</param>
        /// <param name="column">Column which the cursor is over</param>
        /// <param name="isHeader">Indicates wether the cursor is over the header</param>
        public TooltipEventArgs(Appointment appointment, DateTime dateTime, int column, bool isHeader)
        {
            _dateTime = dateTime;
            _column = column;
            _allowShow = true;
            _appointment = appointment;
            _baloon = false;
            _icon = ToolTipIcon.None;
            _title = String.Empty;
            _text = String.Empty;
            _isHeader = isHeader;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Date time for current cell where cursor resides
        /// </summary>
        public DateTime CurrentCellDateTime
        {
            get
            {
                return (_dateTime);
            }
        }

        /// <summary>
        /// Column where cursor resides
        /// </summary>
        public int Column
        {
            get
            {
                return (_column);
            }
        }

        /// <summary>
        /// Indicates wether the cursor is over the header
        /// </summary>
        public bool IsHeader
        {
            get
            {
                return (_isHeader);
            }
        }

        /// <summary>
        /// Determines if balloon type hint is shown
        /// </summary>
        public bool ShowBalloon
        {
            get
            {
                return (_baloon);
            }

            set
            {
                _baloon = value;
            }
        }

        /// <summary>
        /// Title of tooltip
        /// </summary>
        public string Title
        {
            get
            {
                return (_title);
            }

            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// Tooltip text
        /// </summary>
        public string Text
        {
            get
            {
                return (_text);
            }

            set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Icon to be shown with tooltip
        /// </summary>
        public ToolTipIcon Icon
        {
            get
            {
                return (_icon);
            }

            set
            {
                _icon = value;
            }
        }

        /// <summary>
        /// Appointment cursor is over, otherwise null
        /// </summary>
        public Appointment Appointment
        {
            get
            {
                return (_appointment);
            }
        }

        /// <summary>
        /// Determines if a tooltip is shown
        /// </summary>
        public bool AllowShow
        {
            get
            {
                return (_allowShow);
            }

            set
            {
                _allowShow = value;
            }
        }

        #endregion Properties
    }

    public delegate void TooltipEventHandler(object sender, TooltipEventArgs e);
}
