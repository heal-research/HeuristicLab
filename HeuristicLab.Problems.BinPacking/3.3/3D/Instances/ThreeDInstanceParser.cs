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
using System.Text;

namespace HeuristicLab.Problems.BinPacking3D.Instances {
  public class ThreeDInstanceParser {
    public PackingShape Bin { get; private set; }
    public List<PackingItem> Items { get; private set; }

    public ThreeDInstanceParser() {
      Bin = new PackingShape();
      Items = new List<PackingItem>();
    }

    /// <summary>
    /// Parses a given stream and puts the parsed data into the properties
    /// </summary>
    /// <param name="stream"></param>
    public void Parse(Stream stream) {
      Items.Clear();
      try {
        using (var reader = new StreamReader(stream)) {
          ParseBin(reader);
          while (stream.CanRead) {
            var id = GetNextInteger(reader);
            var pieces = GetNextInteger(reader);
            var length = GetNextInteger(reader);
            var width = GetNextInteger(reader);
            var height = GetNextInteger(reader);
            var weight = GetNextInteger(reader);
            var stack = GetNextInteger(reader);
            var material = GetNextInteger(reader);
            var rotate = GetNextInteger(reader);
            var tilt = GetNextInteger(reader);
            for (var i = 0; i < pieces; i++) {
              PackingItem item = new PackingItem(width, height, length, Bin, weight, material);
              Items.Add(item);
            }
          }
        }
      } catch (InvalidOperationException) { }
    }
    
    /// <summary>
    /// Parses the bin and puts it into the Bin property
    /// </summary>
    /// <param name="reader"></param>
    private void ParseBin(StreamReader reader) {
      var depth = GetNextInteger(reader);
      var width = GetNextInteger(reader);
      var height = GetNextInteger(reader);
      var maxWeight = GetNextInteger(reader);

      Bin.Depth = depth;
      Bin.Width = width;
      Bin.Height = height;
    }

    /// <summary>
    /// Reads the next integer form the given stream. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <param name="reader"></param>
    /// <returns>Returns the next integer which has been read from the stream.</returns>
    private int GetNextInteger(StreamReader reader) {
      var next = reader.Read();
      var builder = new StringBuilder();
      while (next >= 0 && !char.IsDigit((char)next))
        next = reader.Read();
      if (next == -1)
        throw new InvalidOperationException("No further integer available");
      while (char.IsDigit((char)next)) {
        builder.Append((char)next);
        next = reader.Read();
        if (next == -1)
          break;
      }
      return int.Parse(builder.ToString());
    }
  }
}
