#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class KornsInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Korns Benchmark Problems"; }
    }
    public override string Description {
      get { return ""; }
    }
    public override Uri WebLink {
      get { return new Uri("http://www.gpbenchmarks.org/wiki/index.php?title=Problem_Classification#Korns"); }
    }
    public override string ReferencePublication {
      get { return "McDermott et al., 2012 \"Genetic Programming Needs Better Benchmarks\", in Proc. of GECCO 2012."; }
    }
    public int Seed { get; private set; }
    public KornsInstanceProvider() : this((int)System.DateTime.Now.Ticks) { }
    public KornsInstanceProvider(int seed) : base() {
      Seed = seed;
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<IDataDescriptor> descriptorList = new List<IDataDescriptor>();
      var rand = new MersenneTwister((uint)Seed);
      descriptorList.Add(new KornsFunctionOne(rand.Next()));
      descriptorList.Add(new KornsFunctionTwo(rand.Next()));
      descriptorList.Add(new KornsFunctionThree(rand.Next()));
      descriptorList.Add(new KornsFunctionFour(rand.Next()));
      descriptorList.Add(new KornsFunctionFive(rand.Next()));
      descriptorList.Add(new KornsFunctionSix(rand.Next()));
      descriptorList.Add(new KornsFunctionSeven(rand.Next()));
      descriptorList.Add(new KornsFunctionEight(rand.Next()));
      descriptorList.Add(new KornsFunctionNine(rand.Next()));
      descriptorList.Add(new KornsFunctionTen(rand.Next()));
      descriptorList.Add(new KornsFunctionEleven(rand.Next()));
      descriptorList.Add(new KornsFunctionTwelve(rand.Next()));
      descriptorList.Add(new KornsFunctionThirteen(rand.Next()));
      descriptorList.Add(new KornsFunctionFourteen(rand.Next()));
      descriptorList.Add(new KornsFunctionFifteen(rand.Next()));
      return descriptorList;
    }
  }
}
