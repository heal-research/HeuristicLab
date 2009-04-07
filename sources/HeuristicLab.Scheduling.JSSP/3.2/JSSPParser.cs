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
using System.IO;
using HeuristicLab.Core;
using HeuristicLab.Data; 

namespace HeuristicLab.Scheduling.JSSP {
  public class JSSPParser {
    private StreamReader source;
    private int nrOfJobs;
    public int NrOfJobs {
      get { return nrOfJobs; }
    }

    private int nrOfMachines;
    public int NrOfMachines {
      get { return nrOfMachines; }
    }

    private int[,] machineSequence; // each row: machine sequence per job
    public int[,] MachineSequence {
      get { return machineSequence; }
    }

    private int[,] processingTime;  // each row: processing time per job and machine
    public int[,] ProcessingTime {
      get { return processingTime; }
    }

    private ItemList operations;
    public ItemList Operations {
      get { return operations; }
    }

    public JSSPParser(String path) {
      if(!path.EndsWith(".jssp"))
        throw new ArgumentException("Input file name has to be in JSSP format (*.jssp)");
      source = new StreamReader(path);
    }

    private ItemList getOperations() {
      ItemList ops = new ItemList();
      for(int i = 0; i < NrOfJobs; i++) {
        for(int j = 0; j < NrOfMachines; j++) {
          List<int> opMachines = new List<int>();
          opMachines.Add(MachineSequence[i, j]);
          List<int> opPredecessors = new List<int>();
          if(j > 0) {
            opPredecessors.Add(j - 1);
          }
          Operation op = new Operation(i, 0, ProcessingTime[i, MachineSequence[i, j]], j, opMachines.ToArray(), opPredecessors.ToArray());
          ops.Add(op);
        }
      }
      return ops;
    }

    // File structure of *.jssp files: 
    // #jobs #machines
    // one line for each job; start count from zero
    // machineNumber processingTime machineNumber processingTime ....
    public void Parse() {
      string str = source.ReadLine();
      string[] tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
      if(tokens.Length != 2)
        throw new InvalidDataException("Invalid problem specification!");
      nrOfJobs = Int32.Parse(tokens[0]);
      nrOfMachines = Int32.Parse(tokens[1]);
      // Console.WriteLine("nrOfMachines: {0}, nrOfJobs: {1}", nrOfMachines, nrOfJobs);
      machineSequence = new int[nrOfJobs, nrOfMachines];
      processingTime = new int[nrOfJobs, nrOfMachines];
      for(int i = 0; i < nrOfJobs; i++) {
        str = source.ReadLine();
        tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        for(int j = 0; j < nrOfMachines; j++) {
          machineSequence[i, j] = Int32.Parse(tokens[2 * j]);
          processingTime[i, machineSequence[i, j]] = Int32.Parse(tokens[2 * j + 1]);
        }
      }
      operations = getOperations(); 
    }
  }
}
