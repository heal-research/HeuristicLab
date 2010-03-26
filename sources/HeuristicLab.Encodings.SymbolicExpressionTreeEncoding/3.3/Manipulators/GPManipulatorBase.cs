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

using HeuristicLab.Core;
using HeuristicLab.Random;
using HeuristicLab.Data;

namespace HeuristicLab.Encodings.SymbolicExpressionTree {
  public abstract class GPManipulatorBase : OperatorBase {
    public GPManipulatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Uniform random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionLibrary", "The operator library containing all available operators", typeof(FunctionLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeHeight", "The maximal allowed height of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTreeSize", "The maximal allowed size (number of nodes) of the tree", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to crossover", typeof(IGeneticProgrammingModel), VariableKind.In | VariableKind.New));
    }

    internal abstract IOperation Manipulate(MersenneTwister random, IGeneticProgrammingModel gpModel, FunctionLibrary library, int maxSize, int maxHeight, IScope scope);

    public override IOperation Apply(IScope scope) {
      FunctionLibrary funLibrary = GetVariableValue<FunctionLibrary>("FunctionLibrary", scope, true);
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, false);
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      int maxTreeSize = GetVariableValue<IntData>("MaxTreeSize", scope, true).Data;
      int maxTreeHeight = GetVariableValue<IntData>("MaxTreeHeight", scope, true).Data;

      return Manipulate(random, gpModel, funLibrary, maxTreeSize, maxTreeHeight, scope);
    }
  }
}
