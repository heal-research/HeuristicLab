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

namespace HeuristicLab.Assignment.QAP {
  public class QAPParser {
    public int facilities;
    public double[,] distances;
    public double[,] weights; 
    
    public QAPParser() { 
    }

    public bool Parse(string file) {
      try {
        StreamReader reader = new StreamReader(file);
        facilities = int.Parse(reader.ReadLine());
        distances = new double[facilities, facilities];
        weights = new double[facilities, facilities];
        reader.ReadLine();
        char[] delim = new char[] {' '};
        for (int i = 0; i < facilities; i++) {
          string valLine = reader.ReadLine();
          string[] vals = new string[facilities]; 
          string[] partVals =  valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries); 
          partVals.CopyTo(vals, 0);
          int index = partVals.Length; 
          while (index < facilities) {
            valLine = reader.ReadLine();
            partVals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            partVals.CopyTo(vals, index);
            index += partVals.Length; 
          }
          for (int j = 0; j < facilities; j++) {
            distances[i, j] = double.Parse(vals[j]);
          }
        }
        reader.ReadLine();
        int read = 0;
        int k = 0;
        while (!reader.EndOfStream) {
          string valLine = reader.ReadLine();
          string[] vals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
          for (int j = 0; j < vals.Length; j++) {
            if (read + j == facilities) {
              read = 0;
              k++;
            }
            weights[k, read + j] = double.Parse(vals[j]);
          }
          read += vals.Length; 
        }
        return true; 
      } catch (Exception) { 
        // not good
        return false; 
      }

    }
  }
}
