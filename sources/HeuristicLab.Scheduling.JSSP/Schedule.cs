#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Scheduling.JSSP {
  public class Schedule : ItemBase {

    private ScheduleTree[] schedule;

    public Schedule(int nrOfMachines, int timeSpan)
      : base() {
      schedule = new ScheduleTree[nrOfMachines];
      for(int i = 0; i < nrOfMachines; i++) {
        schedule[i] = new ScheduleTree(timeSpan);
      }
    }

    public Schedule() : this(0, 100) { }

    public int Machines {
      get { return schedule.Length; }
    }

    public ScheduleTree GetMachineSchedule(int machineIndex) {
      return schedule[machineIndex];
    }

    // return value: operation completion time
    public int ScheduleOperation(Operation op) {
      int machine = op.Machines[0];
      return schedule[machine].Insert(op);
    }

    public override string ToString() {
      string s = "";
      for(int i = 0; i < this.Machines; i++) {
        s += schedule[i].ToString();
        s += "\n";
      }
      return s;
    }

    #region IStorable Members

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute machineAttribute = document.CreateAttribute("Machines");
      machineAttribute.Value = Machines.ToString();
      node.Attributes.Append(machineAttribute);
      for(int i = 0; i < Machines; i++) {
        node.AppendChild(PersistenceManager.Persist("Machine" + i.ToString(), schedule[i], document, persistedObjects));
      }
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      int machines = int.Parse(node.Attributes["Machines"].Value);
      schedule = new ScheduleTree[machines];
      for(int i = 0; i < machines; i++) {
        schedule[i] = (ScheduleTree)PersistenceManager.Restore(node.SelectSingleNode("Machine" + i.ToString()), restoredObjects);
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      int timespan = 0;
      if ((schedule != null) && (schedule.Length > 0)) {
        timespan = schedule[0].Timespan;
      }
      Schedule clone = new Schedule(this.Machines, timespan);
      clonedObjects.Add(Guid, clone);
      clone.schedule = new ScheduleTree[this.Machines];
      for(int i = 0; i < this.Machines; i++) {
        clone.schedule[i] = (ScheduleTree)Auxiliary.Clone(schedule[i], clonedObjects);
      }
      return clone;
    }

    #endregion
  }
}
