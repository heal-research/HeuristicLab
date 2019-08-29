#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;

namespace HeuristicLab.Problems.Instances.Scheduling {
  public class JSSPORLIBParser {
    public string Name { get; set; }
    public string Description { get; set; }
    public int Jobs { get; set; }
    public int Resources { get; set; }
    public double[,] ProcessingTimes { get; set; }
    public int[,] Demands { get; set; }
    public double[] DueDates { get; set; }

    public JSSPORLIBParser() {
      Reset();
    }

    public void Reset() {
      Name = Description = String.Empty;
      Jobs = Resources = 0;
      ProcessingTimes = null;
      Demands = null;
    }

    public void Parse(string file) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        Parse(stream);
      }
    }

    public void Export(string file) {
      using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write)) {
        Export(stream);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the JSSP ORLIB format.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    public void Parse(Stream stream) {
      using (var reader = new StreamReader(stream, Encoding.UTF8, true, 4092, true)) {
        Name = reader.ReadLine().Trim();
        Description = reader.ReadLine().Trim();
        var delim = new char[] {' ', '\t'};

        var info = reader.ReadLine().Split(delim, StringSplitOptions.RemoveEmptyEntries);
        Jobs = int.Parse(info[0]);
        Resources = int.Parse(info[1]);
        ProcessingTimes = new double[Jobs, Resources];
        Demands = new int[Jobs, Resources];

        for (int k = 0; k < Jobs; k++) {
          if (reader.EndOfStream) throw new InvalidDataException("Unexpected End of Stream.");
          var valLine = reader.ReadLine();
          while (String.IsNullOrWhiteSpace(valLine)) valLine = reader.ReadLine();
          var vals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
          if (vals.Length > 2 * Resources) {
            if (DueDates == null) DueDates = new double[Jobs];
            DueDates[k] = double.Parse(vals.Last(), CultureInfo.InvariantCulture.NumberFormat);
          }

          for (int i = 0; i < Resources; i++) {
            Demands[k, i] = int.Parse(vals[2 * i]);
            ProcessingTimes[k, i] = double.Parse(vals[2 * i + 1], CultureInfo.InvariantCulture.NumberFormat);
          }
        }
      }
    }

    /// <summary>
    /// Writes to the given stream data which is expected to be in the JSSP ORLIB format.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to write data to.</param>
    public void Export(Stream stream) {
      using (var writer = new StreamWriter(stream, Encoding.UTF8, 4092, true)) {
        writer.WriteLine(Name);
        writer.WriteLine(Description);
        writer.WriteLine(Jobs.ToString(CultureInfo.InvariantCulture.NumberFormat) + " " + Resources.ToString(CultureInfo.InvariantCulture.NumberFormat));
        for (int i = 0; i < Jobs; i++) {
          for (int j = 0; j < Resources; j++) {
            writer.Write(Demands[i, j] + " " + ProcessingTimes[i, j] + " ");
          }
          if (DueDates != null) writer.Write(DueDates[i].ToString("r", CultureInfo.InvariantCulture.NumberFormat));
          writer.WriteLine();
        }
        writer.Flush();
      }
    }

  }
}
