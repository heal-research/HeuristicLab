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
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FriedmanRandomFunctionInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Friedman Random Functions"; }
    }
    public override string Description {
      get { return "A set of regression benchmark instances as described by Friedman in the Greedy Function Approximation paper"; }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com"); }
    }
    public override string ReferencePublication {
      get { return "Friedman, Jerome H. 'Greedy function approximation: a gradient boosting machine.' Annals of statistics (2001): 1189-1232."; }
    }
    public int Seed { get; private set; }

    public FriedmanRandomFunctionInstanceProvider() : base() {
    }

    public FriedmanRandomFunctionInstanceProvider(int seed) : base() {
      Seed = seed;
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var numVariables = new int[] { 10, 25, 50, 100 };
      var noiseRatios = new double[] { 0.01, 0.05, 0.1 };
      var rand = new MersenneTwister((uint)Seed); // use fixed seed for deterministic problem generation
      return (from size in numVariables
              from noiseRatio in noiseRatios
              select new FriedmanRandomFunction(size, noiseRatio, new MersenneTwister((uint)rand.Next())))
              .Cast<IDataDescriptor>()
              .ToList();
    }

    public override IRegressionProblemData LoadData(IDataDescriptor descriptor) {
      var frfDescriptor = descriptor as FriedmanRandomFunction;
      if (frfDescriptor == null) throw new ArgumentException("FriedmanRandomFunctionInstanceProvider expects an FriedmanRandomFunction data descriptor.");
      // base call generates a regression problem data
      var problemData = base.LoadData(frfDescriptor);
      return problemData;
    }
  }
}
