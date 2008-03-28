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
  public abstract class RealVectorSelfAdaptiveMultiCrossoverBase : MultiCrossoverBase {
    public RealVectorSelfAdaptiveMultiCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("StrategyVector", "Endogenous strategy parameter vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

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

    protected abstract void Cross(IScope scope, IRandom random, IList<double[]> parents, IList<double[]> strategyParametersList, out double[] childIndividual, out double[] strategyParameters);
  }
}
