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
using System.Collections.Generic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class VariousInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Various Benchmark Problems"; }
    }
    public override string Description {
      get { return ""; }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac/hl/core/wiki/AdditionalMaterial"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }
    public int Seed { get; private set; }

    public VariousInstanceProvider() : this((int)DateTime.Now.Ticks) { }

    public VariousInstanceProvider(int seed) : base() {
      Seed = seed;
    }
    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<IDataDescriptor> descriptorList = new List<IDataDescriptor>();
      var rand = new MersenneTwister((uint)Seed);
      descriptorList.Add(new BreimanOne(rand.Next()));
      descriptorList.Add(new FriedmanOne(rand.Next()));
      descriptorList.Add(new FriedmanTwo(rand.Next()));
      descriptorList.Add(new PolyTen(rand.Next()));
      descriptorList.Add(new SpatialCoevolution(rand.Next()));
      return descriptorList;
    }
  }
}
