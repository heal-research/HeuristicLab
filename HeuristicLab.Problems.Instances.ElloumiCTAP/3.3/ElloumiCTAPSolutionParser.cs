#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.IO;

namespace HeuristicLab.Problems.Instances.ElloumiCTAP {
  public class ElloumiCTAPSolutionParser {
    public int[] Assignment { get; private set; }
    public double Quality { get; private set; }
    public Exception Error { get; private set; }

    public ElloumiCTAPSolutionParser() {
      Reset();
    }

    public void Reset() {
      Assignment = null;
      Quality = double.NaN;
      Error = null;
    }

    public bool Parse(string file, int tasks) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        return Parse(stream, tasks);
      }
    }

    public bool Parse(Stream stream, int tasks) {
      Error = null;
      try {
        StreamReader reader = new StreamReader(stream);
        char[] delim = new char[] { ' ', '\t' };

        string line = Continue(reader);
        Assignment = new int[tasks];
        for (int i = 0; i < tasks; i++) {
          string[] assign = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
          Assignment[int.Parse(assign[0]) - 1] = int.Parse(assign[1]) - 1;
          line = Continue(reader);
        }

        Quality = double.Parse(line.Trim(), CultureInfo.InvariantCulture.NumberFormat);

        return true;
      } catch (Exception e) {
        Error = e;
        return false;
      }
    }

    private static string Continue(StreamReader reader) {
      string line = String.Empty;
      while (String.IsNullOrEmpty(line) && !reader.EndOfStream) {
        line = reader.ReadLine();
      }
      return line;
    }
  }
}
