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
using System.Text;
using System.Data;

namespace Calendar
{
    public class DataBoundAppointment : Appointment
    {
        public DataBoundAppointment(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            this.row = row;
        }

        DataRow row;

        public DataRow Row
        {
            get
            {
                return row;
            }
        }

        protected override void OnStartDateChanged()
        {
            base.OnStartDateChanged();

            row["StartDate"] = this.StartDate;
        }

        protected override void OnEndDateChanged()
        {
            base.OnEndDateChanged();

            row["EndDate"] = this.EndDate;
        }

        protected override void OnLockedChanged()
        {
            base.OnLockedChanged();

            row["Locked"] = this.Locked;
        }

        protected override void OnTitleChanged()
        {
            base.OnTitleChanged();

            row["Title"] = this.Title;
        }

        protected override void OnColorChanged()
        {
            base.OnColorChanged();

            row["Color"] = this.Color.ToArgb();
        }

        protected override void OnBorderColorChanged()
        {
            base.OnBorderColorChanged();

            row["BorderColor"] = this.BorderColor.ToArgb();
        }

        protected override void OnTextColorChanged()
        {
            base.OnTextColorChanged();

            row["TextColor"] = this.TextColor.ToArgb();
        }
    }
}
