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
  public class KeijzerInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Keijzer Benchmark Problems"; }
    }
    public override string Description {
      get { return ""; }
    }
    public override Uri WebLink {
      get { return new Uri("http://www.gpbenchmarks.org/wiki/index.php?title=Problem_Classification#Keijzer"); }
    }
    public override string ReferencePublication {
      get { return "McDermott et al., 2012 \"Genetic Programming Needs Better Benchmarks\", in Proc. of GECCO 2012."; }
    }
    public int Seed { get; private set; }

    public KeijzerInstanceProvider() : this((int)System.DateTime.Now.Ticks) {
    }
    public KeijzerInstanceProvider(int seed) : base() {
      Seed = seed;
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<IDataDescriptor> descriptorList = new List<IDataDescriptor>();
      var rand = new MersenneTwister((uint)Seed);
      descriptorList.Add(new KeijzerFunctionOne());
      descriptorList.Add(new KeijzerFunctionTwo());
      descriptorList.Add(new KeijzerFunctionThree());
      descriptorList.Add(new KeijzerFunctionFour());
      descriptorList.Add(new KeijzerFunctionFive(rand.Next()));
      descriptorList.Add(new KeijzerFunctionSix());
      descriptorList.Add(new KeijzerFunctionSeven());
      descriptorList.Add(new KeijzerFunctionEight());
      descriptorList.Add(new KeijzerFunctionNine());
      descriptorList.Add(new KeijzerFunctionTen(rand.Next()));
      descriptorList.Add(new KeijzerFunctionEleven(rand.Next()));
      descriptorList.Add(new KeijzerFunctionTwelve(rand.Next()));
      descriptorList.Add(new KeijzerFunctionThirteen(rand.Next()));
      descriptorList.Add(new KeijzerFunctionFourteen(rand.Next()));
      descriptorList.Add(new KeijzerFunctionFifteen(rand.Next()));
      return descriptorList;
    }
  }
}
