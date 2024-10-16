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
    /// Team View Count - Retrieves number of columns to be shown
    /// </summary>
    public class TeamViewCountEventArgs : EventArgs
    {
        #region Private Members

        private int _columnCount;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="count">Number of columns to show</param>
        public TeamViewCountEventArgs(int count)
        {
            _columnCount = count;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Determines how many columns should be shown in TeamView mode
        /// </summary>
        public int Count
        {
            get
            {
                return (_columnCount);
            }

            set
            {
                _columnCount = value < 0 ? 0 : value;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// Retrieves header text when in TeamView mode
    /// </summary>
    public class TeamViewGetEventArgs : EventArgs
    {
        #region Private Members

        private string _header = "Value Not Set";
        private int _index;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Column who's header text is sought</param>
        public TeamViewGetEventArgs(int index)
        {
            _index = index;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Column Index for header text
        /// </summary>
        public int Index
        {
            get
            {
                return (_index);
            }
        }

        /// <summary>
        /// Headertext to be displayed
        /// </summary>
        public string HeaderText
        {
            get
            {
                return (_header);
            }

            set
            {
                _header = value;
            }
        }

        #endregion Properties
    }

    public delegate void MultiGetEventHandler(object sender, TeamViewGetEventArgs e);
    public delegate void MultiCountEventHandler(object sender, TeamViewCountEventArgs e);
}
