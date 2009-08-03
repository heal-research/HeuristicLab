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

using HeuristicLab.GP.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.GP.StructureIdentification {
  public class Variable : Terminal {    
    public const string WEIGHT = "Weight";
    public const string OFFSET = "SampleOffset";
    public const string VARIABLENAME = "Variable";

    private int minOffset;
    private int maxOffset;

    public override string Description {
      get {
        return @"Variable reads a value from a dataset, multiplies that value with a given factor and returns the result.
The variable 'SampleOffset' can be used to read a value from previous or following rows.
The index of the row that is actually read is SampleIndex+SampleOffset).";
      }
    }


    public override IFunctionTree GetTreeNode() {
      return new VariableFunctionTree(this);
    }

    public Variable()
      : base() {
      SetupInitialization();
      SetupManipulation();
    }

    private void SetupInitialization() {
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformItemChooser variableRandomizer = new UniformItemChooser();
      variableRandomizer.GetVariableInfo("Value").ActualName = VARIABLENAME;
      variableRandomizer.GetVariableInfo("Values").ActualName = "InputVariables";
      variableRandomizer.Name = "Variable randomizer";
      NormalRandomizer weightRandomizer = new NormalRandomizer();
      weightRandomizer.Mu = 0.0;
      weightRandomizer.Sigma = 1.0;
      weightRandomizer.GetVariableInfo("Value").ActualName = WEIGHT;
      weightRandomizer.Name = "Weight Randomizer";
      UniformRandomizer offsetRandomizer = new UniformRandomizer();
      offsetRandomizer.Min = minOffset;
      offsetRandomizer.Max = maxOffset + 1;
      offsetRandomizer.GetVariableInfo("Value").ActualName = OFFSET;
      offsetRandomizer.Name = "Offset Randomizer";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(variableRandomizer);
      combinedOp.OperatorGraph.AddOperator(weightRandomizer);
      combinedOp.OperatorGraph.AddOperator(offsetRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(variableRandomizer);
      seq.AddSubOperator(weightRandomizer);
      seq.AddSubOperator(offsetRandomizer);
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
      NormalRandomAdder weightRandomAdder = new NormalRandomAdder();
      weightRandomAdder.Mu = 0.0;
      weightRandomAdder.Sigma = 1.0;
      weightRandomAdder.GetVariableInfo("Value").ActualName = WEIGHT;
      weightRandomAdder.Name = "Weight Adder";
      NormalRandomAdder offsetRandomAdder = new NormalRandomAdder();
      offsetRandomAdder.Mu = 0.0;
      offsetRandomAdder.Sigma = 1.0;
      offsetRandomAdder.GetVariableInfo("Value").ActualName = OFFSET;
      offsetRandomAdder.Name = "Offset Adder";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(variableRandomizer);
      combinedOp.OperatorGraph.AddOperator(weightRandomAdder);
      combinedOp.OperatorGraph.AddOperator(offsetRandomAdder);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(variableRandomizer);
      seq.AddSubOperator(weightRandomAdder);
      seq.AddSubOperator(offsetRandomAdder);
      Manipulator = combinedOp;
    }

    public void SetConstraints(int minSampleOffset, int maxSampleOffset) {
      this.minOffset = minSampleOffset;
      this.maxOffset = maxSampleOffset;
      SetupInitialization();
      SetupManipulation();
    }
  }
}
