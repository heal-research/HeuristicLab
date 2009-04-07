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

namespace HeuristicLab.GP.Boolean {
  public sealed class Variable : FunctionBase {
    public const string INDEX = "Variable";

    private int minIndex;
    private int maxIndex;

    public override string Description {
      get { return ""; }
    }

    public Variable()
      : base() {
      AddVariableInfo(new VariableInfo(INDEX, "Index of the variable in the dataset representing this feature", typeof(ConstrainedIntData), VariableKind.None));
      GetVariableInfo(INDEX).Local = true;
      AddVariableInfo(new VariableInfo(INITIALIZATION, "Initialization operator for variables", typeof(CombinedOperator), VariableKind.None));
      GetVariableInfo(INITIALIZATION).Local = false;
      AddVariableInfo(new VariableInfo(MANIPULATION, "Manipulation operator for variables", typeof(CombinedOperator), VariableKind.None));
      GetVariableInfo(MANIPULATION).Local = false;

      AddConstraint(new NumberOfSubOperatorsConstraint(0, 0));

      ConstrainedIntData variable = new ConstrainedIntData();
      AddVariable(new HeuristicLab.Core.Variable(INDEX, variable));
      minIndex = 0; maxIndex = 100;

      SetupInitialization();
      SetupManipulation();

    }

    private void SetupInitialization() {
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformRandomizer indexRandomizer = new UniformRandomizer();
      indexRandomizer.Min = minIndex;
      indexRandomizer.Max = maxIndex + 1; // uniform randomizer generates numbers in the range [min, max[
      indexRandomizer.GetVariableInfo("Value").ActualName = INDEX;
      indexRandomizer.Name = "Index Randomizer";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(indexRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(indexRandomizer);
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

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(indexRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(indexRandomizer);
      HeuristicLab.Core.IVariable manipulationOp = GetVariable(MANIPULATION);
      if(manipulationOp == null) {
        AddVariable(new HeuristicLab.Core.Variable(MANIPULATION, combinedOp));
      } else {
        manipulationOp.Value = combinedOp;
      }
    }

    public void SetConstraints(int[] allowedIndexes) {
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
  }
}
