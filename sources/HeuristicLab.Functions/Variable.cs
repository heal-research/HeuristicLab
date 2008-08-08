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
using HeuristicLab.Random;
using HeuristicLab.Operators;

namespace HeuristicLab.Functions {
  public class Variable : FunctionBase {

    public const string WEIGHT = "Weight";
    public const string OFFSET = "SampleOffset";
    public const string INDEX = "Variable";

    private int minIndex;
    private int maxIndex;
    private int minOffset;
    private int maxOffset;

    public override string Description {
      get {
        return @"Variable reads a value from a dataset, multiplies that value with a given factor and returns the result.
The variable 'SampleOffset' can be used to read a value from previous or following rows.
The index of the row that is actually read is SampleIndex+SampleOffset).";
      }
    }

    public Variable()
      : base() {
      AddVariableInfo(new VariableInfo(INDEX, "Index of the variable in the dataset representing this feature", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(INDEX).Local = true;
      AddVariableInfo(new VariableInfo(WEIGHT, "Weight is multiplied to the feature value", typeof(ConstrainedDoubleData), VariableKind.None));
      GetVariableInfo(WEIGHT).Local = true;
      AddVariableInfo(new VariableInfo(OFFSET, "SampleOffset is added to the sample index", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(OFFSET).Local = true;
      AddVariableInfo(new VariableInfo(INITIALIZATION, "Initialization operator for variables", typeof(CombinedOperator), VariableKind.None));
      GetVariableInfo(INITIALIZATION).Local = false;
      AddVariableInfo(new VariableInfo(MANIPULATION, "Manipulation operator for variables", typeof(CombinedOperator), VariableKind.None));
      GetVariableInfo(MANIPULATION).Local = false;

      ConstrainedDoubleData weight = new ConstrainedDoubleData();
      // initialize a totally arbitrary range for the weight = [-20.0, 20.0]
      weight.AddConstraint(new DoubleBoundedConstraint(-20.0, 20.0));
      AddVariable(new HeuristicLab.Core.Variable(WEIGHT, weight));

      ConstrainedIntData variable = new ConstrainedIntData();
      AddVariable(new HeuristicLab.Core.Variable(INDEX, variable));
      minIndex = 0; maxIndex = 100;

      ConstrainedIntData sampleOffset = new ConstrainedIntData();
      AddVariable(new HeuristicLab.Core.Variable(OFFSET, sampleOffset));

      SetupInitialization();
      SetupManipulation();

      // variable can't have suboperators
      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));
    }

    private void SetupInitialization() {
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformRandomizer indexRandomizer = new UniformRandomizer();
      indexRandomizer.Min = minIndex;
      indexRandomizer.Max = maxIndex + 1; // uniform randomizer generates numbers in the range [min, max[
      indexRandomizer.GetVariableInfo("Value").ActualName = INDEX;
      indexRandomizer.Name = "Index Randomizer";
      NormalRandomizer weightRandomizer = new NormalRandomizer();
      weightRandomizer.Mu = 1.0;
      weightRandomizer.Sigma = 1.0;
      weightRandomizer.GetVariableInfo("Value").ActualName = WEIGHT;
      weightRandomizer.Name = "Weight Randomizer";
      UniformRandomizer offsetRandomizer = new UniformRandomizer();
      offsetRandomizer.Min = minOffset;
      offsetRandomizer.Max = maxOffset + 1;
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
      HeuristicLab.Core.IVariable initOp = GetVariable(INITIALIZATION);
      if(initOp == null) {
        AddVariable(new HeuristicLab.Core.Variable(INITIALIZATION, combinedOp));
      } else {
        initOp.Value = combinedOp;
      }
    }

    private void SetupManipulation() {
      // manipulation operator
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformRandomizer indexRandomizer = new UniformRandomizer();
      indexRandomizer.Min = minIndex;
      indexRandomizer.Max = maxIndex + 1;
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
      HeuristicLab.Core.IVariable manipulationOp = GetVariable(MANIPULATION);
      if(manipulationOp == null) {
        AddVariable(new HeuristicLab.Core.Variable(MANIPULATION, combinedOp));
      } else {
        manipulationOp.Value = combinedOp;
      }
    }

    public void SetConstraints(int[] allowedIndexes, int minSampleOffset, int maxSampleOffset) {
      ConstrainedIntData offset = GetVariableValue<ConstrainedIntData>(OFFSET, null, false);
      IntBoundedConstraint rangeConstraint = new IntBoundedConstraint();
      this.minOffset = minSampleOffset;
      this.maxOffset = maxSampleOffset;
      rangeConstraint.LowerBound = minSampleOffset;
      rangeConstraint.LowerBoundEnabled = true;
      rangeConstraint.LowerBoundIncluded = true;
      rangeConstraint.UpperBound = maxSampleOffset;
      rangeConstraint.UpperBoundEnabled = true;
      rangeConstraint.UpperBoundIncluded = true;
      offset.AddConstraint(rangeConstraint);

      ConstrainedIntData index = GetVariableValue<ConstrainedIntData>(INDEX, null, false);
      Array.Sort(allowedIndexes);
      minIndex = allowedIndexes[0]; maxIndex = allowedIndexes[allowedIndexes.Length - 1];
      List<IConstraint> constraints = new List<IConstraint>();
      int start = allowedIndexes[0];
      int prev = start;
      for(int i = 1; i < allowedIndexes.Length; i++) {
        if(allowedIndexes[i] != prev + 1) {
          IntBoundedConstraint lastRange = new IntBoundedConstraint();
          lastRange.LowerBound = start;
          lastRange.LowerBoundEnabled = true;
          lastRange.LowerBoundIncluded = true;
          lastRange.UpperBound = prev;
          lastRange.UpperBoundEnabled = true;
          lastRange.UpperBoundIncluded = true;
          constraints.Add(lastRange);
          start = allowedIndexes[i];
          prev = start;
        }
        prev = allowedIndexes[i];
      }
      IntBoundedConstraint range = new IntBoundedConstraint();
      range.LowerBound = start;
      range.LowerBoundEnabled = true;
      range.LowerBoundIncluded = true;
      range.UpperBound = prev;
      range.UpperBoundEnabled = true;
      range.UpperBoundIncluded = true;
      constraints.Add(range);
      if(constraints.Count > 1) {
        OrConstraint or = new OrConstraint();
        foreach(IConstraint c in constraints) or.Clauses.Add(c);
        index.AddConstraint(or);
      } else {
        index.AddConstraint(constraints[0]);
      }

      SetupInitialization();
      SetupManipulation();
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
