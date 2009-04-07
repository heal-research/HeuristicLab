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
using HeuristicLab.Data; 

namespace HeuristicLab.Scheduling.JSSP {
  public class TimeSlot {
    public int start;
    public int end;
    public int free;
    public int maxFreeSlot;
    public int job {
      get {
        if (operation != null) {
          return operation.Job;
        } else {
          return -1;
        }
      }
    } 
    public Operation operation;
    public List<TimeSlot> dependentSlots;
    public TimeSlot parent; 

    public TimeSlot(int start, int end) {
      this.start = start;
      this.end = end;
      free = end - start;
      maxFreeSlot = end - start;
      dependentSlots = new List<TimeSlot>(); 
    }

    public TimeSlot(Operation op) {
      operation = (Operation) op.Clone();
      start = op.Start;
      end = op.Start + op.Duration;
      free = 0;
      maxFreeSlot = 0;
      dependentSlots = new List<TimeSlot>(); 
    }

    public TimeSlot(string s) {
      string[] tokens = s.Split(',');
      start = int.Parse(tokens[1]);
      end = int.Parse(tokens[2]);
      free = int.Parse(tokens[3]);
      maxFreeSlot = int.Parse(tokens[4]);
      int job = int.Parse(tokens[0]);
      if (job != -1) {
        int opIndex = int.Parse(tokens[5]);
        int[] machines = new int[tokens.Length - 5];
        for(int i = 0; i < tokens.Length - 5; i++) {
          machines[i] = int.Parse(tokens[i]); 
        }
        operation = new Operation(job, start, end - start, opIndex, machines, null); 
      }
      dependentSlots = new List<TimeSlot>(); 
    }

    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      builder.Append(String.Format("{2},{0},{1},{3},{4}", start, end, job, free, maxFreeSlot));
      if(operation != null) {
        builder.Append(String.Format(",{0}", operation.OperationIndex)); 
        for (int i = 0; i < operation.Machines.Length; i++) {
          builder.Append(String.Format(",{0}", operation.Machines[i])); 
        }
      }
      return builder.ToString();
    }
  }
}
