#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.BinPacking3D {
  internal class RandomDataDescriptor : IDataDescriptor {
    public string Name { get; private set; }
    public string Description { get; private set; }

    internal int NumItems { get; private set; }
    internal int Seed { get; private set; }

    internal RandomDataDescriptor(string name, string description, int numItems, int @class, int seed) {
      this.Name = name;
      this.Description = description;
      this.NumItems = numItems;
      this.Seed = seed;
    }
  }
}
