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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Scheduling.JSSP;
using HeuristicLab.Charting.Grid; 

namespace HeuristicLab.Charting.Gantt {
  public partial class GanttControl : Form {
    public GanttControl() {
      InitializeComponent();
      int timespan = 50;
      Schedule sched = new Schedule(3, timespan);
      JSSPParser parser = new JSSPParser("C:\\Documents and Settings\\Monika Kofler\\Desktop\\ft3.jssp");
      parser.Parse();
      ItemList ops = parser.Operations;
      while (ops.Count > 0) {
        Operation op = getNextToSchedule(ops);
        if(op != null) {
          sched.ScheduleOperation(op);
          UpdatePredecessors(ops, op); 
        }
      }
      Ganttchart g = new Ganttchart(sched, 0, 0, 5, 5);
      // g.DragDropEnabled = false; 
      gridChartControl1.Chart = g; 
    }

    private void UpdatePredecessors(ItemList ops, Operation scheduled) {
      int job = scheduled.Job;
      int opIndex = scheduled.OperationIndex;
      foreach(Operation op in ops) {
        if((op.Job == job) && (op.Predecessors.Count > 0) && (((IntData)op.Predecessors[0]).Data == opIndex)) {
          op.Predecessors.Clear();
          op.Start = scheduled.Start + scheduled.Duration; 
        }
      }
    }

    private Operation getNextToSchedule(ItemList ops) {
      Operation op = null;
      for(int i = 0; i < ops.Count; i++) {
        if(((Operation)ops[i]).Predecessors.Count == 0) {
          op = (Operation)ops[i];
          ops.RemoveAt(i);
          return op;
        }
      }
      return op; 
    }
  }
}
