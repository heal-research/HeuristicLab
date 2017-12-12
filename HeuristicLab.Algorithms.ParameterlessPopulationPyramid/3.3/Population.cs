#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [StorableClass]
  public class Population : DeepCloneable {
    [Storable]
    public List<BinaryVector> Solutions {
      get;
      private set;
    }
    [Storable]
    public LinkageTree Tree {
      get;
      private set;
    }


    [StorableConstructor]
    protected Population(bool deserializing) : base() { }


    protected Population(Population original, Cloner cloner) : base(original, cloner) {
      Solutions = original.Solutions.Select(cloner.Clone).ToList();
      Tree = cloner.Clone(original.Tree);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Population(this, cloner);
    }

    public Population(int length, IRandom rand) {
      Solutions = new List<BinaryVector>();
      Tree = new LinkageTree(length, rand);
    }
    public void Add(BinaryVector solution) {
      Solutions.Add(solution);
      Tree.Add(solution);
    }
  }
}
