#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;

namespace HeuristicLab.Problems.Instances.QAPLIB {
  public class QAPLIBParser {
    public int Size { get; private set; }
    public double[,] Distances { get; private set; }
    public double[,] Weights { get; private set; }

    public QAPLIBParser() {
      Reset();
    }

    public void Reset() {
      Size = 0;
      Distances = null;
      Weights = null;
    }

    public void Parse(string file) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        Parse(stream);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the QAPLIB format.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    /// <returns>True if the file was successfully read or false otherwise.</returns>
    public void Parse(Stream stream) {
      var reader = new StreamReader(stream);
      Size = int.Parse(reader.ReadLine());
      Distances = new double[Size, Size];
      Weights = new double[Size, Size];
      char[] delim = new char[] { ' ', '\t' };

      Weights = ParseMatrix(reader, delim);
      Distances = ParseMatrix(reader, delim);
    }

    private double[,] ParseMatrix(StreamReader reader, char[] delim) {
      int read = 0, k = 0;
      double[,] result = new double[Size, Size];
      while (k < Size) {
        if (reader.EndOfStream) throw new InvalidDataException("Reached end of stream while reading second matrix.");
        string valLine = reader.ReadLine();
        while (String.IsNullOrWhiteSpace(valLine)) valLine = reader.ReadLine();
        string[] vals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
        foreach (string val in vals) {
          result[k, read++] = double.Parse(val);
          if (read == Size) {
            read = 0;
            k++;
          }
        }
      }
      return result;
    }
  }
}
