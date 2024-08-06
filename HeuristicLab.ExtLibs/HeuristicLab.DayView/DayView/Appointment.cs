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
using System.Drawing.Drawing2D;

namespace Calendar
{
    /// <summary>
    /// Appointment Object
    /// </summary>
    public class Appointment
    {
        #region Private Members

        private int _column;
        private object _object;
        private Int64 _id;
        private int _layer;
        private string _group;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _locked = false;
        private Color _color;
        private Color _textColor = Color.Black;
        private bool _drawBorder = false;
        private string _title = "";
        private bool _allDayEvent = false;
        private Color _borderColor;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Appointment()
        {
            UseCustomHatchStyle = false;
            _color = Color.White;
            _borderColor = Color.Blue;
            _title = "New Appointment";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Column Index where appointment resides (TeamView mode)
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
        /// User defined object associated with the Calendar Appointment
        /// </summary>
        public object Object
        {
            get
            {
                return (_object);
            }

            set
            {
                _object = value;
            }
        }

        /// <summary>
        /// User defined Int64 associated with the appointemtn
        /// </summary>
        public Int64 ID
        {
            get
            {
                return (_id);
            }

            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Appointment Layer
        /// </summary>
        public int Layer
        {
            get 
            { 
                return (_layer); 
            }
            
            set 
            { 
                _layer = value; 
            }
        }

        /// <summary>
        /// Appointment Group
        /// </summary>
        public string Group
        {
            get 
            { 
                return (_group); 
            }

            set 
            { 
                _group = value; 
            }
        }

        /// <summary>
        /// Appointment Start Date/Time
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return (_startDate);
            }

            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnStartDateChanged();
                }

            }
        }

        /// <summary>
        /// End Date/Time of appointment
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return (_endDate);
            }

            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnEndDateChanged();
                }
            }
        }

        /// <summary>
        /// Determines wether the appointment is Locked
        /// </summary>
        public bool Locked
        {
            get
            {
                return (_locked);
            }

            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    OnLockedChanged();
                }
            }
        }

        /// <summary>
        /// Determines wether a custom brush is used when drawing appointment or not
        /// </summary>
        public bool UseCustomHatchStyle { get; set; }

        /// <summary>
        /// Custom brush to use during the drawing of an appointment
        /// </summary>
        public HatchStyle CustomHatch { get; set; }

        /// <summary>
        /// Determines the Color of the appointment
        /// </summary>
        public Color Color
        {
            get
            {
                return (_color);
            }

            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnColorChanged();
                }
            }
        }

        /// <summary>
        /// Color of the text within the appointment
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (_textColor);
            }

            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnTextColorChanged();
                }
            }
        }

        /// <summary>
        /// Determines wether the border is drawn or not
        /// </summary>
        public bool DrawBorder
        {
            get
            {
                return (_drawBorder);
            }

            set
            {
                _drawBorder = value;
            }
        }

        /// <summary>
        /// Border Color for appointment
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return (_borderColor);
            }

            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    OnBorderColorChanged();
                }
            }
        }

        /// <summary>
        /// Appointment Title
        /// </summary>
        public string Title
        {
            get
            {
                return (_title);
            }

            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnTitleChanged();
                }
            }
        }

        /// <summary>
        /// Determines wether the appointment is an all day event
        /// </summary>
        public bool AllDayEvent
        {
            get
            {
                return (_allDayEvent);
            }

            set
            {
                if (_allDayEvent != value)
                {
                    _allDayEvent = value;
                    OnAllDayEventChanged();
                }
            }
        }

        #endregion Properties

        #region Virtual Methods

        protected virtual void OnStartDateChanged()
        {
        }

        protected virtual void OnEndDateChanged()
        {
        }

        protected virtual void OnLockedChanged()
        {
        }

        protected virtual void OnTextColorChanged()
        {
        }

        protected virtual void OnBorderColorChanged()
        {
        }

        protected virtual void OnAllDayEventChanged()
        {
        }

        protected virtual void OnTitleChanged()
        {
        }

        protected virtual void OnColorChanged()
        {
        }

        #endregion Virtual Methods

        #region Internal Members

        #region Internal Methods

        /// <summary>
        /// Used internally to determine wether the appointment conflicts with another
        /// </summary>
        internal int conflictCount;

        /// <summary>
        /// Used internally to determine wether the appointment has moved
        /// </summary>
        internal bool AppointmentMoved = false;

        #endregion Internal Methods

        #endregion Internal Members

        #region Overridden Methods

#if DEBUG
        public override string ToString()
        {
            return (String.Format("Appointment: {0}; conflict: {1}", Title, conflictCount));
        }
#endif

        #endregion Overridden Methods
    }
}
