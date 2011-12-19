#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Drawing;
using System.Xml.Serialization;

namespace HeuristicLab.Clients.Hive.Administrator.Views {

  public class HiveAppointment : Calendar.Appointment {
    private Guid recurringId = Guid.Empty;

    [XmlIgnore]
    public bool Changed { get; set; }

    public HiveAppointment()
      : base() {
      Changed = false;
    }

    [XmlIgnore]
    public new int Layer {
      get { return base.Layer; }
      set { base.Layer = value; }
    }

    [XmlIgnore]
    public new string Group {
      get { return base.Group; }
      set { base.Group = value; }
    }

    [XmlAttribute("Locked")]
    public new bool Locked {
      get { return base.Locked; }
      set { base.Locked = value; }
    }

    [XmlIgnore]
    public new Color Color {
      get { return base.Color; }
      set { base.Color = value; }
    }

    [XmlIgnore]
    public new Color TextColor {
      get { return base.TextColor; }
      set { base.TextColor = value; }
    }

    [XmlIgnore]
    public new Color BorderColor {
      get { return base.BorderColor; }
      set { base.BorderColor = value; }
    }

    [XmlIgnore]
    public new bool DrawBorder {
      get { return base.DrawBorder; }
      set { base.DrawBorder = value; }
    }

    [XmlAttribute("AllDayEvent")]
    public new bool AllDayEvent {
      get { return base.AllDayEvent; }
      set { base.AllDayEvent = value; Changed = true; }
    }

    [XmlAttribute("Recurring")]
    public new bool Recurring {
      get { return base.Recurring; }
      set { base.Recurring = value; Changed = true; }
    }

    [XmlAttribute("RecurringId")]
    public Guid RecurringId {
      get { return recurringId; }
      set { recurringId = value; Changed = true; }
    }

    [XmlAttribute("EndDate")]
    public new DateTime EndDate {
      get { return base.EndDate; }
      set { base.EndDate = value; Changed = true; }
    }

    [XmlAttribute("StartDate")]
    public new DateTime StartDate {
      get { return base.StartDate; }
      set { base.StartDate = value; Changed = true; }
    }

    [XmlIgnore]
    public new int AppointmentId {
      get { return base.AppointmentId; }
      set { base.AppointmentId = value; }
    }

    [XmlElement("Subject")]
    public new string Subject {
      get { return base.Subject; }
      set { base.Subject = value; }
    }

    [XmlIgnore]
    public new string Location {
      get { return base.Location; }
      set { base.Location = value; }
    }

    [XmlIgnore]
    public new string Note {
      get { return base.Note; }
      set { base.Note = value; }
    }

    [XmlIgnore]
    public bool Deleted { get; set; }

    [XmlIgnore]
    public Guid Id { get; set; }
  }
}
