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
using System.Linq;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeatureSelectionInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Feature Selection Problems"; }
    }
    public override string Description {
      get { return "A set of artificial feature selection benchmark problems"; }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }
    public int Seed { get; private set; }

    public FeatureSelectionInstanceProvider() : base() {
      Seed = (int)DateTime.Now.Ticks;
    }

    public FeatureSelectionInstanceProvider(int seed) : base() {
      Seed = seed;
    }


    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var sizes = new int[] { 50, 100, 200 };
      var pp = new double[] { 0.1, 0.25, 0.5 };
      var noiseRatios = new double[] { 0.01, 0.05, 0.1, 0.2 };
      var rand = new MersenneTwister((uint)Seed); // use fixed seed for deterministic problem generation

      return (from size in sizes
              from p in pp
              from noiseRatio in noiseRatios
              let instanceSeed = rand.Next()
              let mt = new MersenneTwister((uint)instanceSeed)
              let xGenerator = new NormalDistributedRandom(mt, 0, 1)
              let weightGenerator = new UniformDistributedRandom(mt, 0, 10)
              select new FeatureSelection(size, p, noiseRatio, xGenerator, weightGenerator))
              .Cast<IDataDescriptor>()
              .ToList();
    }

    public override IRegressionProblemData LoadData(IDataDescriptor descriptor) {
      var featureSelectionDescriptor = descriptor as FeatureSelection;
      if (featureSelectionDescriptor == null) throw new ArgumentException("FeatureSelectionInstanceProvider expects an FeatureSelection data descriptor.");
      // base call generates a regression problem data
      var regProblemData = base.LoadData(featureSelectionDescriptor);
      var problemData =
        new FeatureSelectionRegressionProblemData(
          regProblemData.Dataset, regProblemData.AllowedInputVariables, regProblemData.TargetVariable,
          featureSelectionDescriptor.SelectedFeatures, featureSelectionDescriptor.Weights,
          featureSelectionDescriptor.OptimalRSquared);

      // copy values from regProblemData to feature selection problem data
      problemData.Name = regProblemData.Name;
      problemData.Description = regProblemData.Description;
      problemData.TrainingPartition.Start = regProblemData.TrainingPartition.Start;
      problemData.TrainingPartition.End = regProblemData.TrainingPartition.End;
      problemData.TestPartition.Start = regProblemData.TestPartition.Start;
      problemData.TestPartition.End = regProblemData.TestPartition.End;

      return problemData;
    }
  }
}
