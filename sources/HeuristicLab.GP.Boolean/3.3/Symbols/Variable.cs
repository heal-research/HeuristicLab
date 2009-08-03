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
using HeuristicLab.Data;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.GP.Boolean {
  public sealed class Variable : Terminal {
    public const string VARIABLENAME = "Variable";

    public Variable()
      : base() {
      SetupInitialization();
      SetupManipulation();
    }

    public override HeuristicLab.GP.Interfaces.IFunctionTree GetTreeNode() {
      return new VariableFunctionTree(this);
    }

    private void SetupInitialization() {
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformItemChooser variableRandomizer = new UniformItemChooser();
      variableRandomizer.GetVariableInfo("Value").ActualName = VARIABLENAME;
      variableRandomizer.GetVariableInfo("Values").ActualName = "InputVariables";
      variableRandomizer.Name = "Variable randomizer";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(variableRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(variableRandomizer);
      Initializer = combinedOp;
    }

    private void SetupManipulation() {
      // manipulation operator
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformItemChooser variableRandomizer = new UniformItemChooser();
      variableRandomizer.GetVariableInfo("Value").ActualName = VARIABLENAME;
      variableRandomizer.GetVariableInfo("Values").ActualName = "InputVariables";
      variableRandomizer.Name = "Variable randomizer";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(variableRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(variableRandomizer);
      Manipulator = combinedOp;
    }
  }
}
