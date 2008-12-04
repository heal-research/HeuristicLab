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
using HeuristicLab.Random;

namespace HeuristicLab.ES {
  /// <summary>
  /// Mutates the endogenous strategy parameters.
  /// </summary>
  public class SelfAdaptiveMutationStrengthAdjuster : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Mutates the endogenous strategy parameters"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SelfAdaptiveMutationStrengthAdjuster"/> with four 
    /// variable infos (<c>Random</c>, <c>StrategyVector</c>, <c>GeneralLearningRate</c> and
    /// <c>LearningRate</c>).
    /// </summary>
    public SelfAdaptiveMutationStrengthAdjuster()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("StrategyVector", "Vector containing the endogenous strategy parameters", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("GeneralLearningRate", "The general learning rate will scale all mutations. It's influence is calculated as: e^(GeneralLearningRate*N(0,1))", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LearningRate", "Learning parameter defines the strength of the adaption of each component in the object parameter vector", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Mutates the endogenous strategy parameters.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBase.Apply"/> of base class <see cref="OperatorBase"/>.</remarks>
    /// <param name="scope">The current scope to mutate.</param>
    /// <inheritdoc select="returns"/>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      double[] strategyParams = GetVariableValue<DoubleArrayData>("StrategyVector", scope, false).Data;
      double tau = GetVariableValue<DoubleData>("LearningRate", scope, true).Data;
      double tau0 = GetVariableValue<DoubleData>("GeneralLearningRate", scope, true).Data;

      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0);
      double generalMultiplier = Math.Exp(tau0 * N.NextDouble());
      for (int i = 0; i < strategyParams.Length; i++) {
        strategyParams[i] *= generalMultiplier * Math.Exp(tau * N.NextDouble());
      }
      return base.Apply(scope);
    }
  }
}
