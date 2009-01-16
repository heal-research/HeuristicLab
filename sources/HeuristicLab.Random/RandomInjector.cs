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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Random {
  /// <summary>
  /// Injects a random variable in a given scope.
  /// </summary>
  public class RandomInjector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RandomInjector"/> with three variable infos
    /// (<c>SetSeedRandomly</c>, <c>Seed</c> and <c>Random</c>.)
    /// </summary>
    public RandomInjector()
      : base() {
      AddVariableInfo(new VariableInfo("SetSeedRandomly", "Initialize random seed randomly", typeof(BoolData), VariableKind.In));
      GetVariableInfo("SetSeedRandomly").Local = true;
      AddVariable(new Variable("SetSeedRandomly", new BoolData(true)));

      AddVariableInfo(new VariableInfo("Seed", "Random seed", typeof(IntData), VariableKind.In));
      GetVariableInfo("Seed").Local = true;
      System.Random random = new System.Random();
      AddVariable(new Variable("Seed", new IntData(random.Next())));

      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.New));
    }

    /// <summary>
    /// Injects a new random variable in the given <paramref name="scope"/> .
    /// </summary>
    /// <param name="scope">The scope where to inject the variable.</param>
    /// <returns>null.</returns>
    public override IOperation Apply(IScope scope) {
      bool setRandomly = GetVariableValue<BoolData>("SetSeedRandomly", scope, true).Data;
      IntData seed = GetVariableValue<IntData>("Seed", scope, true);
      if (setRandomly) {
        System.Random random = new System.Random();
        seed.Data = random.Next();
      }
      MersenneTwister mersenneTwister = new MersenneTwister((uint)seed.Data);

      scope.AddVariable(new Variable(scope.TranslateName("Random"), mersenneTwister));
      return null;
    }
  }
}
