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
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.Scheduling.JSSP {

  public class ScheduleTreeNode {
    private TimeSlot data;
    private ScheduleTreeNode left;
    private ScheduleTreeNode right;
    private ScheduleTreeNode parent;

    public ScheduleTreeNode(TimeSlot t) {
      data = t;
    }

    public ScheduleTreeNode Left {
      get { return left; }
      set { left = value; }
    }

    public ScheduleTreeNode Right {
      get { return right; }
      set { right = value; }
    }

    public ScheduleTreeNode Parent {
      get { return parent; }
      set { parent = value; }
    }

    public void SetLeft(ScheduleTreeNode child) {
      this.Left = child;
      child.Parent = this;
    }

    public void SetRight(ScheduleTreeNode child) {
      this.Right = child;
      child.Parent = this;
    }

    public TimeSlot Data {
      get { return data; }
      set { data = value; }
    }
  }

  public class ScheduleTree : StorableBase {
    private ScheduleTreeNode root;

    public ScheduleTreeNode Root {
      get { return root; }
    }

    private int timespan;
    public int Timespan {
      get { return timespan; }
    }

    public ScheduleTree(int timespan) {
      root = new ScheduleTreeNode(new TimeSlot(0, timespan));
      this.timespan = timespan;
    }

    public ScheduleTree() : this(10000) { }

    private int InsertOperation(Operation op) {
      return Insert(this.Root, op);
    }

    private int InsertOperation(TimeSlot t) {
      return Insert(this.Root, t.operation);
    }

    // ToDo: Rewrite code! (not efficient)
    public int Insert(Operation op) {
      foreach(ScheduleTreeNode node in this.InOrder) {
        int start = Math.Max(node.Data.start, op.Start);
        if((IsLeaf(node)) && (FitsIntoEmptySlot(node.Data, start, op.Duration))) {
          op.Start = start;
          return Insert(node, op);
        }
      }
      return -1;
    }

    private bool FitsIntoEmptySlot(TimeSlot slot, int start, int duration) {
      return ((slot.free >= duration) && (slot.start <= start) && (slot.end >= start + duration));
    }

    private void UpdateMaxFreeSlot(ScheduleTreeNode slot) {
      while(slot != null) {
        if(!IsLeaf(slot)) {
          int newMax = Math.Max(slot.Left.Data.maxFreeSlot, slot.Right.Data.maxFreeSlot);
          slot.Data.free = slot.Left.Data.free + slot.Right.Data.free;
          slot.Data.maxFreeSlot = newMax;
        }
        slot = slot.Parent;
      }
    }

    private int Insert(ScheduleTreeNode slot, Operation op) {
      if (op == null) {
        return -1;
      }
      int end = op.Start + op.Duration;;
      if(slot.Data.maxFreeSlot < op.Duration) {
        return -1;
      }
      if(IsLeaf(slot)) {
        // fits exactly into slot
        if((slot.Data.start == op.Start) && (slot.Data.end == end)) {
          slot.Data = new TimeSlot(op);
          UpdateMaxFreeSlot(slot);
          return op.Start;
        }
        // empty slot before op. 
        if(slot.Data.start < op.Start) {
          ScheduleTreeNode empty1 = new ScheduleTreeNode(new TimeSlot(slot.Data.start, op.Start));
          slot.SetLeft(empty1);
          if(slot.Data.end > end) {
            ScheduleTreeNode empty2 = new ScheduleTreeNode(new TimeSlot(op.Start, slot.Data.end));
            slot.SetRight(empty2);
            return Insert(empty2, op);
          } else {
            slot.SetRight(new ScheduleTreeNode(new TimeSlot(op)));
            UpdateMaxFreeSlot(slot);
            return op.Start;
          }
        }
        // empty after op. 
        if(slot.Data.end > end) {
          ScheduleTreeNode occupied = new ScheduleTreeNode(new TimeSlot(op));
          ScheduleTreeNode empty2 = new ScheduleTreeNode(new TimeSlot(end, slot.Data.end));
          slot.SetRight(empty2);
          slot.SetLeft(occupied);
          UpdateMaxFreeSlot(slot);
          return op.Start;
        }
      } else {
        if(slot.Left.Data.end < end) {
          return Insert(slot.Right, op);
        } else {
          return Insert(slot.Left, op);
        }
      }
      return -1;
    }

    public bool IsLeaf(ScheduleTreeNode slot) {
      return ((slot.Right == null) && (slot.Left == null));
    }

    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      foreach(ScheduleTreeNode node in this.InOrder) {
        if(IsLeaf(node)) {
          builder.Append(node.Data.ToString());
          builder.Append(";");
        }
      }
      return builder.ToString();
    }

    #region IEnumerable Members
    public IEnumerable<ScheduleTreeNode> InOrder {
      get { return ScanInOrder(root); }
    }

    IEnumerable<ScheduleTreeNode> ScanInOrder(ScheduleTreeNode root) {
      if(root.Left != null) {
        foreach(ScheduleTreeNode node in ScanInOrder(root.Left)) {
          yield return node;
        }
      }
      yield return root;
      if(root.Right != null) {
        foreach(ScheduleTreeNode node in ScanInOrder(root.Right)) {
          yield return node;
        }
      }
    }
    #endregion

    #region IStorable Members
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = this.ToString();
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      string[] tokens = node.InnerText.Split(';');
      for(int i = 0; i < tokens.Length - 1; i++) {
        TimeSlot t = new TimeSlot(tokens[i]);
        if(t.job > -1) {
          this.InsertOperation(t);
        }
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ScheduleTree clone = new ScheduleTree(timespan);
      clonedObjects.Add(Guid, clone);
      foreach (ScheduleTreeNode node in this.InOrder) {
        if (IsLeaf(node) && (node.Data.job > -1)) {
          clone.InsertOperation(node.Data);
        }
      }
      return clone;
    }

    #endregion
  }
}
