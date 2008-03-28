#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  public class SelfAdaptiveIntermediateMultiCrossover : RealVectorSelfAdaptiveMultiCrossoverBase {
    public override string Description {
      get {
        return @"This creates a new offspring by computing the centroid of the parents. It will also use the same strategy to combine the endogenous strategy parameter vector.";
      }
    }

    public static void Apply(IList<double[]> parents, IList<double[]> strategyParametersList, out double[] childIndividual, out double[] strategyParameters) {
      childIndividual = IntermediateMultiCrossover.Apply(parents);
      strategyParameters = IntermediateMultiCrossover.Apply(strategyParametersList);
    }

    protected override void Cross(IScope scope, IRandom random, IList<double[]> parents, IList<double[]> strategyParametersList, out double[] childIndividual, out double[] strategyParameters) {
      Apply(parents, strategyParametersList, out childIndividual, out strategyParameters);
    }
  }
}
