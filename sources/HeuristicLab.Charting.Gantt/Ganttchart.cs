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
using HeuristicLab.Scheduling.JSSP; 
using HeuristicLab.Charting.Grid;
using System.Drawing; 

namespace HeuristicLab.Charting.Gantt {
  public class Ganttchart : Gridchart {
    private Schedule schedule;
    public Schedule Schedule {
      get { return schedule; }
      set { schedule = value; DisplaySchedule(); }
    }

    private List<Color> colors; 

    public Ganttchart(Schedule s, PointD lowerLeft, PointD upperRight) 
      : base(lowerLeft, upperRight) {
      schedule = s;
      colors = new List<Color>(); 
      DisplaySchedule(); 
    }

    public Ganttchart(Schedule s, double x1, double y1, double x2, double y2)
      : base(x1, y1, x2, y2) {
      schedule = s;
      colors = new List<Color>();
      DisplaySchedule();   
    }

    private void DisplaySchedule() {
      Random rand = new Random();
      if((schedule != null) && (schedule.Machines > 0)) {
        // delete all primitives except the grid 
        Grid.Grid grid = (Grid.Grid) Group.Primitives[Group.Primitives.Count - 1];
        Group.Clear();
        Group.Add(grid); 

        this.Redim(schedule.Machines, schedule.GetMachineSchedule(0).Timespan);
        for(int i = 0; i < schedule.Machines; i++) {
          ScheduleTree tree = schedule.GetMachineSchedule(i);
          foreach(ScheduleTreeNode node in tree.InOrder) {
            if(tree.IsLeaf(node) && (node.Data.job != -1)) {
              OperationCell s = new OperationCell(this);
              s.Value = node.Data.job;
              s.SetSpan(1, node.Data.end - node.Data.start);
              while(node.Data.job >= colors.Count) {
                colors.Add(Color.Empty);
              }
              if(colors[node.Data.job].IsEmpty) {
                colors[node.Data.job] = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
              }
              s.Brush = new SolidBrush(colors[node.Data.job]);
              this[i, node.Data.start] = s;
            }
          }
        }
      }
    }
  }
}
