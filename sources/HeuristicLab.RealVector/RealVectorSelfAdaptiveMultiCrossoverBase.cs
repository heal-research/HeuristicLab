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
using HeuristicLab.Evolutionary;

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Base class for self adaptive multiple crossovers for real vectors.
  /// </summary>
  public abstract class RealVectorSelfAdaptiveMultiCrossoverBase : MultiCrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="RealVectorSelfAdaptiveMultiCrossoverBase"/> with two
    /// variable infos (<c>RealVector</c> and <c>StrategyVector</c>).
    /// </summary>
    public RealVectorSelfAdaptiveMultiCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("StrategyVector", "Endogenous strategy parameter vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

    /// <summary>
    /// Performs a self adaptive multiple crossover on the given list of <paramref name="parents"/>.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents of which to perform the crossover.</param>
    /// <param name="child">The newly generated real vector, resulting from the self adaptive multiple
    /// crossover.</param>
    protected sealed override void Cross(IScope scope, IRandom random, IScope[] parents, IScope child) {
      IList<double[]> parentsList = new List<double[]>(parents.Length);
      IList<double[]> strategyParametersList = new List<double[]>(parents.Length);

      for (int i = 0; i < parents.Length; i++) {
        parentsList.Add(parents[i].GetVariableValue<DoubleArrayData>("RealVector", false).Data);
        strategyParametersList.Add(parents[i].GetVariableValue<DoubleArrayData>("StrategyVector", false).Data);
      }

      double[] childIndividual, strategyParameters;
      Cross(scope, random, parentsList, strategyParametersList, out childIndividual, out strategyParameters);
      child.AddVariable(new Variable(child.TranslateName("RealVector"), new DoubleArrayData(childIndividual)));
      child.AddVariable(new Variable(child.TranslateName("StrategyVector"), new DoubleArrayData(strategyParameters)));
    }

    /// <summary>
    /// Performs a self adaptive multiple crossover on the given list of <paramref name="parents"/>.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents of which to perform the crossover.</param>
    /// <param name="strategyParametersList">The strategy parameter list.</param>
    /// <param name="childIndividual">Output parameter; the created child.</param>
    /// <param name="strategyParameters">Output parameter; endogenous strategy parameters.</param>
    protected abstract void Cross(IScope scope, IRandom random, IList<double[]> parents, IList<double[]> strategyParametersList, out double[] childIndividual, out double[] strategyParameters);
  }
}
