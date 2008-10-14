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
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.GP.StructureIdentification {
  public sealed class Constant : FunctionBase {
    public const string VALUE = "Value";

    public override string Description {
      get { return "Returns the value of local variable 'Value'."; }
    }

    public Constant()
      : base() {
      AddVariableInfo(new VariableInfo(VALUE, "The constant value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo(VALUE).Local = true;

      ConstrainedDoubleData valueData = new ConstrainedDoubleData();
      // initialize a default range for the contant value
      valueData.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));
      HeuristicLab.Core.Variable value = new HeuristicLab.Core.Variable(VALUE, valueData);
      AddVariable(value);

      SetupInitialization();
      SetupManipulation();

      // constant can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    private void SetupInitialization() {
      // initialization operator
      AddVariableInfo(new VariableInfo(INITIALIZATION, "Initialization operator-graph for constants", typeof(IOperatorGraph), VariableKind.None));
      GetVariableInfo(INITIALIZATION).Local = false;
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor initSeq = new SequentialProcessor();
      UniformRandomizer randomizer = new UniformRandomizer();
      randomizer.Min = -20.0;
      randomizer.Max = 20.0;

      combinedOp.OperatorGraph.AddOperator(initSeq);
      combinedOp.OperatorGraph.AddOperator(randomizer);
      combinedOp.OperatorGraph.InitialOperator = initSeq;
      initSeq.AddSubOperator(randomizer);
      AddVariable(new HeuristicLab.Core.Variable(INITIALIZATION, combinedOp));
    }

    private void SetupManipulation() {
      // manipulation operator
      AddVariableInfo(new VariableInfo(MANIPULATION, "Manipulation operator-graph for constants", typeof(IOperatorGraph), VariableKind.None));
      GetVariableInfo(MANIPULATION).Local = false;
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor manipulationSeq = new SequentialProcessor();
      NormalRandomAdder valueAdder = new NormalRandomAdder();
      valueAdder.Mu = 0.0;
      valueAdder.Sigma = 0.1;

      combinedOp.OperatorGraph.AddOperator(manipulationSeq);
      combinedOp.OperatorGraph.AddOperator(valueAdder);
      combinedOp.OperatorGraph.InitialOperator = manipulationSeq;
      manipulationSeq.AddSubOperator(valueAdder);
      AddVariable(new HeuristicLab.Core.Variable(MANIPULATION, combinedOp));
    }
  }
}
