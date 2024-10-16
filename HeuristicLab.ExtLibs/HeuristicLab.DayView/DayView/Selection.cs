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

namespace Calendar
{
    /// <summary>
    /// Enum to determine the type of selection
    /// </summary>
    public enum SelectionType
    {
        DateRange,
        Appointment,
        None
    }

    /// <summary>
    /// Enum to determine the selection mode
    /// </summary>
    internal enum SelectionMode
    {
        ResizeTop,
        ResizeBottom,
        ResizeLeft,
        ResizeRight,
        Move,
        None
    }

}
