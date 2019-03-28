#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      get { return "A set of regression benchmark instances for variable network analysis. The data for these instances are randomly generated as described in the reference publication."; }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com"); }
    }
    public override string ReferencePublication {
      get { return "G. Kronberger, B. Burlacu, M. Kommenda, S. Winkler, M. Affenzeller. Measures for the Evaluation and Comparison of Graphical Model Structures. to appear in Computer Aided Systems Theory - EUROCAST 2017, Springer 2018"; }
    }
    public int Seed { get; private set; }

    public VariableNetworkInstanceProvider() : this((int)DateTime.Now.Ticks) { }
    public VariableNetworkInstanceProvider(int seed) : base() {
      Seed = seed;
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var numVariables = new int[] { 10, 20, 50, 100 };
      var noiseRatios = new double[] { 0, 0.01, 0.05, 0.1, 0.2 };
      var rand = new MersenneTwister((uint)Seed); // use fixed seed for deterministic problem generation
      var lr = (from size in numVariables
                from noiseRatio in noiseRatios
                select new LinearVariableNetwork(size, noiseRatio, new MersenneTwister((uint)rand.Next())))
                .Cast<IDataDescriptor>()
                .ToList();
      var gp = (from size in numVariables
                from noiseRatio in noiseRatios
                select new GaussianProcessVariableNetwork(size, noiseRatio, new MersenneTwister((uint)rand.Next())))
                .Cast<IDataDescriptor>()
                .ToList();
      return lr.Concat(gp);
    }

    public override IRegressionProblemData LoadData(IDataDescriptor descriptor) {
      var varNetwork = descriptor as VariableNetwork;
      if (varNetwork == null) throw new ArgumentException("VariableNetworkInstanceProvider expects an VariableNetwork data descriptor.");
      // base call generates a regression problem data
      var problemData = base.LoadData(varNetwork);
      problemData.Description = varNetwork.Description + Environment.NewLine + varNetwork.NetworkDefinition;
      return problemData;
    }
  }
}
