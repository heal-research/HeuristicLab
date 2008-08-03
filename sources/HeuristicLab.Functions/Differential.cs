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
using System.Diagnostics;
using HeuristicLab.Data;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.Functions {
  public sealed class Differential : FunctionBase {

    public const string WEIGHT = "Weight";
    public const string OFFSET = "SampleOffset";
    public const string INDEX = "Variable";

    public override string Description {
      get {
        return @"Differential returns the difference between the value of a variable at t and t-1. The weight is a coefficient that is multiplied to the the difference.";
      }
    }

    public Differential()
      : base() {
      AddVariableInfo(new VariableInfo(INDEX, "Index of the variable in the dataset representing this feature", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(INDEX).Local = true;
      AddVariableInfo(new VariableInfo(WEIGHT, "Weight is multiplied to the feature value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo(WEIGHT).Local = true;
      AddVariableInfo(new VariableInfo(OFFSET, "SampleOffset is added to the sample index", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(OFFSET).Local = true;

      ConstrainedDoubleData weight = new ConstrainedDoubleData();
      // initialize a totally arbitrary range for the weight = [-20.0, 20.0]
      weight.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));
      AddVariable(new HeuristicLab.Core.Variable(WEIGHT, weight));

      ConstrainedIntData variable = new ConstrainedIntData();
      AddVariable(new HeuristicLab.Core.Variable(INDEX, variable));

      ConstrainedIntData sampleOffset = new ConstrainedIntData();
      // initialize a sample offset for static models
      IntBoundedConstraint offsetConstraint = new IntBoundedConstraint(0, 0);
      offsetConstraint.LowerBoundIncluded = true;
      offsetConstraint.UpperBoundIncluded = true;
      sampleOffset.AddConstraint(offsetConstraint);
      AddVariable(new HeuristicLab.Core.Variable(OFFSET, sampleOffset));

      SetupInitialization();
      SetupManipulation();

      // variable can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    private void SetupInitialization() {
      AddVariableInfo(new VariableInfo(INITIALIZATION, "Initialization operator for differentials", typeof(CombinedOperator), VariableKind.None));
      GetVariableInfo(INITIALIZATION).Local = false;
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformRandomizer indexRandomizer = new UniformRandomizer();
      indexRandomizer.Min = 0;
      indexRandomizer.Max = 10;
      indexRandomizer.GetVariableInfo("Value").ActualName = INDEX;
      indexRandomizer.Name = "Index Randomizer";
      NormalRandomizer weightRandomizer = new NormalRandomizer();
      weightRandomizer.Mu = 1.0;
      weightRandomizer.Sigma = 1.0;
      weightRandomizer.GetVariableInfo("Value").ActualName = WEIGHT;
      weightRandomizer.Name = "Weight Randomizer";
      UniformRandomizer offsetRandomizer = new UniformRandomizer();
      offsetRandomizer.Min = 0.0;
      offsetRandomizer.Max = 1.0;
      offsetRandomizer.GetVariableInfo("Value").ActualName = OFFSET;
      offsetRandomizer.Name = "Offset Randomizer";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(indexRandomizer);
      combinedOp.OperatorGraph.AddOperator(weightRandomizer);
      combinedOp.OperatorGraph.AddOperator(offsetRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(indexRandomizer);
      seq.AddSubOperator(weightRandomizer);
      seq.AddSubOperator(offsetRandomizer);
      AddVariable(new HeuristicLab.Core.Variable(INITIALIZATION, combinedOp));
    }

    private void SetupManipulation() {
      // manipulation operator
      AddVariableInfo(new VariableInfo(MANIPULATION, "Manipulation operator for differentials", typeof(CombinedOperator), VariableKind.None));
      GetVariableInfo(MANIPULATION).Local = false;
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformRandomizer indexRandomizer = new UniformRandomizer();
      indexRandomizer.Min = 0;
      indexRandomizer.Max = 10;
      indexRandomizer.GetVariableInfo("Value").ActualName = INDEX;
      indexRandomizer.Name = "Index Randomizer";
      NormalRandomAdder weightRandomAdder = new NormalRandomAdder();
      weightRandomAdder.Mu = 0.0;
      weightRandomAdder.Sigma = 0.1;
      weightRandomAdder.GetVariableInfo("Value").ActualName = WEIGHT;
      weightRandomAdder.Name = "Weight Adder";
      NormalRandomAdder offsetRandomAdder = new NormalRandomAdder();
      offsetRandomAdder.Mu = 0.0;
      offsetRandomAdder.Sigma = 1.0;
      offsetRandomAdder.GetVariableInfo("Value").ActualName = OFFSET;
      offsetRandomAdder.Name = "Offset Adder";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(indexRandomizer);
      combinedOp.OperatorGraph.AddOperator(weightRandomAdder);
      combinedOp.OperatorGraph.AddOperator(offsetRandomAdder);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(indexRandomizer);
      seq.AddSubOperator(weightRandomAdder);
      seq.AddSubOperator(offsetRandomAdder);
      AddVariable(new HeuristicLab.Core.Variable(MANIPULATION, combinedOp));
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
