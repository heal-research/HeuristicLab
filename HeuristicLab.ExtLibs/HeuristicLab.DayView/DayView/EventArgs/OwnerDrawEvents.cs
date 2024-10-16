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

namespace Calendar
{
    /// <summary>
    /// After Draw Header Event Arguments
    /// </summary>
    public class AfterDrawHeaderEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="g">Graphics object used for drawing</param>
        /// <param name="rect">Rect object for header</param>
        /// <param name="column">Header Column</param>
        /// <param name="date">Date of Header</param>
        public AfterDrawHeaderEventArgs(Graphics g, Rectangle rect, int column, DateTime date)
        {
            Graphics = g;
            Rectangle = rect;
            Column = column;
            Date = date;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Graphics object used for drawing
        /// </summary>
        public Graphics Graphics { get; private set; }

        /// <summary>
        /// Rectangle object for header
        /// </summary>
        public Rectangle Rectangle { get; private set; }

        /// <summary>
        /// Column Index
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Date of header
        /// </summary>
        public DateTime Date { get; private set; }

        #endregion Properties
    }

    public delegate void AfterDrawHeaderEventHandler(object sender, AfterDrawHeaderEventArgs e);
}
