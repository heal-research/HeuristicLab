#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class VariableNetworkInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Variable Network Instances"; }
    }
    public override string Description {
      get { return "A set of regression benchmark instances for variable network analysis"; }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var numVariables = new int[] { 10, 20, 50, 100 };
      var noiseRatios = new double[] { 0.01, 0.05, 0.1 };
      var rand = new System.Random(1234); // use fixed seed for deterministic problem generation
      return (from size in numVariables
              from noiseRatio in noiseRatios
              select new VariableNetwork(size, noiseRatio, new MersenneTwister((uint)rand.Next())))
              .Cast<IDataDescriptor>()
              .ToList();
    }

    public override IRegressionProblemData LoadData(IDataDescriptor descriptor) {
      var varNetwork = descriptor as VariableNetwork;
      if (varNetwork == null) throw new ArgumentException("VariableNetworkInstanceProvider expects an VariableNetwork data descriptor.");
      // base call generates a regression problem data
      var regProblemData = base.LoadData(varNetwork);
      var problemData =
        new RegressionProblemData(
          regProblemData.Dataset, regProblemData.AllowedInputVariables, regProblemData.TargetVariable);

      // copy values from regProblemData to feature selection problem data
      problemData.Name = regProblemData.Name;
      problemData.Description = varNetwork.Description + Environment.NewLine + varNetwork.NetworkDefinition;
      problemData.TrainingPartition.Start = regProblemData.TrainingPartition.Start;
      problemData.TrainingPartition.End = regProblemData.TrainingPartition.End;
      problemData.TestPartition.Start = regProblemData.TestPartition.Start;
      problemData.TestPartition.End = regProblemData.TestPartition.End;

      return problemData;
    }
  }
}
