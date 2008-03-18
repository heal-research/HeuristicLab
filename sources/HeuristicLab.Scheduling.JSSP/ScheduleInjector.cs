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
using HeuristicLab.Data;

namespace HeuristicLab.Scheduling.JSSP {
  public class ScheduleInjector : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    private Schedule schedule;
    public Schedule Schedule {
      get { return schedule; }
    }

    public ScheduleInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Machines", "Number of machines", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Timespan", "Desired timespan for the schedule", typeof(IntData), VariableKind.In));
      GetVariableInfo("Timespan").Local = true;
      AddVariable(new Variable("Timespan", new IntData(10000)));
      AddVariableInfo(new VariableInfo("Schedule", "Schedule", typeof(Schedule), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      IntData machines = GetVariableValue<IntData>("Machines", scope, true);
      IntData timespan = GetVariableValue<IntData>("Timespan", scope, true);
      schedule = new Schedule(machines.Data, timespan.Data);
      scope.AddVariable(new Variable(scope.TranslateName("Schedule"), schedule));
      return null;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ScheduleInjector clone = (ScheduleInjector)base.Clone(clonedObjects);
      if(schedule != null) {
        clone.schedule = (Schedule)Auxiliary.Clone(schedule, clonedObjects);
      }
      return clone;
    }

    #region IStorable Members
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      if(schedule != null) {
        node.AppendChild(PersistenceManager.Persist("Schedule", schedule, document, persistedObjects));
      }
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode scheduleNode = node.SelectSingleNode("Schedule");
      if(scheduleNode != null) {
        schedule = (Schedule)PersistenceManager.Restore(scheduleNode, restoredObjects);
      }
    }
    #endregion

  }
}
