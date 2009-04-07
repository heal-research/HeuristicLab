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
using System.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Scheduling.JSSP {
  public class Operation : ItemBase, IComparable {

    #region Data
    private IntData start;
    public int Start {
      get { return start.Data; }
      set {
        start.Data = value;
        OnChanged();
      }
    }

    private IntData duration;
    public int Duration {
      get { return duration.Data; }
      set {
        duration.Data = value;
        OnChanged();
      }
    }

    private IntData job;
    public int Job {
      get { return job.Data; }
      set {
        job.Data = value;
        OnChanged();
      }
    }

    private IntData operationIndex;
    public int OperationIndex {
      get { return operationIndex.Data; }
      set { operationIndex.Data = value; }
    }

    private IntArrayData machines;
    public int[] Machines {
      get { return machines.Data; }
      set {
        machines.Data = value;
        OnChanged();
      }
    }

    private ItemList predecessors;
    public ItemList Predecessors {
      get { return predecessors; }
      set { predecessors = value; }
    }
    #endregion

    public Operation()
      : this(-1, 0, 0, -1, new int[0], null) {
    }

    public Operation(int job, int earliestStart, int duration, int operationIndex, int[] machines, int[] predecessors) {
      this.start = new IntData(earliestStart);
      this.duration = new IntData(duration);
      this.job = new IntData(job);
      this.machines = new IntArrayData(machines);
      this.predecessors = new ItemList();
      if(predecessors != null) {
        for(int i = 0; i < predecessors.Length; i++) {
          this.predecessors.Add(new IntData(predecessors[i]));
        }
      }
      this.operationIndex = new IntData(operationIndex);
    }

    public override string ToString() {
      if(this.Job == -1) {
        return "";
      }
      return (String.Format("|{2},{3}:[m:{0} d:{1}]", machines.Data[0], duration, job, operationIndex));
    }

    #region IComparable Members

    public int CompareTo(object obj) {
      if(obj is Operation) {
        Operation bin = (Operation)obj;
        return (start.CompareTo(bin.start));
      } else { throw new ArgumentException("Object is not an Operation"); }
    }

    #endregion

    #region IViewable Members

    public override IView CreateView() {
      return new OperationView(this);
    }

    #endregion

    #region IStorable Members

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Operation clone = new Operation();
      clonedObjects.Add(Guid, clone);
      clone.duration = (IntData)Auxiliary.Clone(duration, clonedObjects);
      clone.start = (IntData)Auxiliary.Clone(start, clonedObjects);
      clone.operationIndex = (IntData)Auxiliary.Clone(operationIndex, clonedObjects);
      clone.job = (IntData)Auxiliary.Clone(job, clonedObjects);
      clone.machines = (IntArrayData)Auxiliary.Clone(machines, clonedObjects);
      clone.predecessors = (ItemList)Auxiliary.Clone(predecessors, clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Job", job, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("OperationIndex", operationIndex, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Start", start, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Duration", duration, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Predecessors", predecessors, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Machines", machines, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      job = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Job"), restoredObjects);
      operationIndex = (IntData)PersistenceManager.Restore(node.SelectSingleNode("OperationIndex"), restoredObjects);
      start = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Start"), restoredObjects);
      duration = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Duration"), restoredObjects);
      predecessors = (ItemList)PersistenceManager.Restore(node.SelectSingleNode("Predecessors"), restoredObjects);
      machines = (IntArrayData)PersistenceManager.Restore(node.SelectSingleNode("Machines"), restoredObjects);
    }

    #endregion
  }
}
