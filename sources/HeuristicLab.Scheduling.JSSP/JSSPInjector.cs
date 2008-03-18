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
using HeuristicLab.Operators;
using HeuristicLab.Data;

namespace HeuristicLab.Scheduling.JSSP {
  public class JSSPInjector : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    private ItemList operations; 
    public ItemList Operations {
      get { return operations; }
      set { operations = value; }
    }

    private IntData machines;
    public int NrOfMachines {
      get { return machines.Data; }
      set { machines.Data = value; }
    }
    private IntData jobs;
    public int NrOfJobs {
      get { return jobs.Data; }
      set { jobs.Data = value; }
    }

    public JSSPInjector()
      : base() {
      operations = new ItemList();
      machines = new IntData();
      jobs = new IntData(); 
      AddVariableInfo(new VariableInfo("Machines", "Number of machines", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo("Jobs", "Number of jobs", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo("Operations", "List of JSSP operations", typeof(ItemList), VariableKind.New));
    }

    public override IView CreateView() {
      return new JSSPInjectorView(this);
    }

    public override IOperation Apply(IScope scope) {
      scope.AddVariable(new Variable(scope.TranslateName("Machines"), machines.Clone() as IntData));
      scope.AddVariable(new Variable(scope.TranslateName("Jobs"), jobs.Clone() as IntData));
      scope.AddVariable(new Variable(scope.TranslateName("Operations"), (ItemList)operations.Clone()));
      return base.Apply(scope);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      JSSPInjector clone = (JSSPInjector)base.Clone(clonedObjects);
      clone.operations = (ItemList)Auxiliary.Clone(operations, clonedObjects);
      clone.jobs = (IntData)Auxiliary.Clone(jobs, clonedObjects);
      clone.machines = (IntData)Auxiliary.Clone(machines, clonedObjects); 
      return clone;
    }

    #region IStorable Members
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Machings", machines, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Jobs", jobs, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Operations", operations, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      operations = (ItemList)PersistenceManager.Restore(node.SelectSingleNode("Operations"), restoredObjects);
      jobs = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Jobs"), restoredObjects);
      machines = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Machines"), restoredObjects); 
    }
    #endregion
  }
}
