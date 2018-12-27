#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.QAPLIB {
  public class QAPLIBSolutionParser {
    public int Size { get; private set; }
    public int[] Assignment { get; private set; }
    public double Quality { get; private set; }
    public Exception Error { get; private set; }

    public QAPLIBSolutionParser() {
      Reset();
    }

    public void Reset() {
      Size = 0;
      Assignment = null;
      Quality = double.NaN;
      Error = null;
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the QAPLIB solution format.
    /// 
    /// The QAPLIB solution file format (.sln) is as follows:
    /// First line: Size of the permutation followed by a blank followed by the quality formatted using no thousands separator and if a fractional number with the "." as decimal symbol
    /// Remaining lines: The values of the permutation separated by blanks. Values must lie in the range [1;size of the permutation].
    /// </summary>
    /// <param name="file">The file to read data from.</param>
    /// <param name="valueAsLocation">The numbers can be interpreted either as facilities or locations.
    /// HeuristicLab always encodes the permutation such that the index denotes the facility and the value the location.
    /// If this parameter is true, then the permutation is expected to follow this encoding.
    /// If this parameter is false, the meaning of value and index will be swapped when reading the permutation.</param>
    /// <returns></returns>
    public bool Parse(string file, bool valueAsLocation) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        return Parse(stream, valueAsLocation);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the QAPLIB solution format.
    /// 
    /// The QAPLIB solution file format (.sln) is as follows:
    /// First line: Size of the permutation followed by a blank followed by the quality formatted using no thousands separator and if a fractional number with the "." as decimal symbol
    /// Remaining lines: The values of the permutation separated by blanks. Values must lie in the range [1;size of the permutation].
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    /// <param name="valueAsLocation">The numbers can be interpreted either as facilities or locations.
    /// HeuristicLab always encodes the permutation such that the index denotes the facility and the value the location.
    /// If this parameter is true, then the permutation is expected to follow this encoding.
    /// If this parameter is false, the meaning of value and index will be swapped when reading the permutation.</param>
    /// <returns>True if the file was successfully read or false otherwise.</returns>
    public bool Parse(Stream stream, bool valueAsLocation) {
      Error = null;
      try {
        StreamReader reader = new StreamReader(stream);
        char[] delim = new char[] { ' ', '\t', ',' }; // comma is added for nug30.sln which is the only file that separates the permutation with commas
        string[] firstline = reader.ReadLine().Split(delim, StringSplitOptions.RemoveEmptyEntries);
        Size = int.Parse(firstline[0]);
        Quality = double.Parse(firstline[1], CultureInfo.InvariantCulture.NumberFormat);
        Assignment = new int[Size];
        int read = 0;
        while (!reader.EndOfStream) {
          string valLine = reader.ReadLine();
          string[] vals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
          int start = 1;
          if (vals.Any(x => x == "0")) start = 0;
          if (vals.Length == 0) continue;
          for (int j = 0; j < vals.Length; j++) {
            if (valueAsLocation)
              Assignment[int.Parse(vals[j]) - start] = read++;
            else Assignment[read++] = int.Parse(vals[j]) - start;
          }
        }
        if (read < Size) Assignment = null;
        return true;
      } catch (Exception e) {
        Error = e;
        return false;
      }
    }
  }
}
