#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;

namespace HeuristicLab.Problems.Instances.DIMACS {
  public class Parser {
    public int Nodes { get; private set; }
    public int Edges { get; private set; }
    public ICollection<Tuple<int, int>> AdjacencyList { get { return edges; } }
    private HashSet<Tuple<int, int>> edges;

    public Parser() {
      Reset();
    }

    public void Reset() {
      Nodes = 0;
      Edges = 0;
      edges = new HashSet<Tuple<int, int>>();
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
      char[] delim = new char[] { ' ', '\t' };
      var reader = new StreamReader(stream);
      var line = reader.ReadLine().Trim();
      // skip comments
      while (line.StartsWith("c", StringComparison.InvariantCultureIgnoreCase)) line = reader.ReadLine().Trim();

      // p edge NODES EDGES
      var split = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
      Nodes = int.Parse(split[2]);
      do {
        line = reader.ReadLine();
        if (string.IsNullOrEmpty(line)) break;
        // e XX YY
        split = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);
        var src = int.Parse(split[1]);
        var tgt = int.Parse(split[2]);
        Tuple<int, int> e = null;
        if (src < tgt) e = Tuple.Create(src, tgt);
        else if (src > tgt) e = Tuple.Create(tgt, src);
        else continue; // src == tgt
        if (edges.Add(e)) Edges++;
      } while (!reader.EndOfStream);
    }
  }
}
