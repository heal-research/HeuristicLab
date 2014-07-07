#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ElloumiCTAPParser {
    public int Tasks { get; private set; }
    public int Processors { get; private set; }
    public double[,] ExecutionCosts { get; private set; }
    public double[,] CommunicationCosts { get; private set; }
    public double[] MemoryRequirements { get; private set; }
    public double[] MemoryCapacities { get; private set; }

    public ElloumiCTAPParser() {
      Reset();
    }

    public void Reset() {
      Tasks = 0;
      Processors = 0;
      ExecutionCosts = null;
      CommunicationCosts = null;
      MemoryRequirements = null;
      MemoryCapacities = null;
    }

    public void Parse(string file) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        Parse(stream);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the Elloumi's CTAP format.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    /// <returns>True if the file was successfully read or false otherwise.</returns>
    public void Parse(Stream stream) {
      var separator = new char[] { ' ', '\t' };
      var reader = new StreamReader(stream);
      string line = Continue(reader);

      Tasks = int.Parse(line);

      line = Continue(reader);

      Processors = int.Parse(line);

      line = Continue(reader);

      ExecutionCosts = new double[Processors, Tasks];
      for (int i = 0; i < Processors; i++) {
        string[] costsPerTask = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < Tasks; j++)
          ExecutionCosts[i, j] = double.Parse(costsPerTask[j], CultureInfo.InvariantCulture.NumberFormat);
        line = Continue(reader);
      }

      CommunicationCosts = new double[Tasks, Tasks];
      for (int i = 0; i < Tasks - 1; i++) {
        string[] costsTaskToTasks = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        for (int j = i + 1; j < Tasks; j++) {
          CommunicationCosts[i, j] = double.Parse(costsTaskToTasks[j - i - 1], CultureInfo.InvariantCulture.NumberFormat);
          CommunicationCosts[j, i] = CommunicationCosts[i, j];
        }
        line = Continue(reader);
      }

      MemoryRequirements = new double[Tasks];
      string[] requirementsPerTask = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < Tasks; i++)
        MemoryRequirements[i] = double.Parse(requirementsPerTask[i], CultureInfo.InvariantCulture.NumberFormat);

      line = Continue(reader);

      MemoryCapacities = new double[Processors];
      string[] capacitiesPerProcessor = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < Processors; i++)
        MemoryCapacities[i] = double.Parse(capacitiesPerProcessor[i], CultureInfo.InvariantCulture.NumberFormat);

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
