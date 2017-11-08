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

namespace HeuristicLab.Problems.BinPacking3D {
  public class ThreeDInstanceParser {
    public PackingShape Bin { get; private set; }
    public List<PackingItem> Items { get; private set; }

    public void Parse(Stream stream) {
      var reader = new StreamReader(stream);
      var length = GetNextInteger(reader);
      var width = GetNextInteger(reader);
      var height = GetNextInteger(reader);
      var maxWeight = GetNextInteger(reader);
      Bin = new PackingShape(width, height, length);
      Items = new List<PackingItem>();
      while (true) {
        try {
          var id = GetNextInteger(reader);
          var pieces = GetNextInteger(reader);
          length = GetNextInteger(reader);
          width = GetNextInteger(reader);
          height = GetNextInteger(reader);
          var weight = GetNextInteger(reader);
          var stack = GetNextInteger(reader);
          var material = GetNextInteger(reader);
          var rotate = GetNextInteger(reader);
          var tilt = GetNextInteger(reader);
          for (var i = 0; i < pieces; i++) {
            Items.Add(new PackingItem(width, height, length, Bin, weight, material));
          }
        } catch (InvalidOperationException) {
          break;
        }
      }
    }

    private int GetNextInteger(StreamReader reader) {
      var next = reader.Read();
      var builder = new StringBuilder();
      while (next >= 0 && !char.IsDigit((char)next)) next = reader.Read();
      if (next == -1) throw new InvalidOperationException("No further integer available");
      while (char.IsDigit((char)next)) {
        builder.Append((char)next);
        next = reader.Read();
        if (next == -1) break;
      }
      return int.Parse(builder.ToString());
    }
  }
}
