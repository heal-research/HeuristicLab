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


using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
namespace HeuristicLab.GP.StructureIdentification.Networks {
  public sealed class OpenParameter : Terminal {
    public const string OFFSET = "SampleOffset";
    public const string VARIABLENAME = "Variable";

    private int minOffset;
    private int maxOffset;

    public override string Description {
      get {
        return @"";
      }
    }

    public OpenParameter()
      : base() {
      SetupInitialization();
      SetupManipulation();
    }

    public override HeuristicLab.GP.Interfaces.IFunctionTree GetTreeNode() {
      return new OpenParameterFunctionTree(this);
    }

    private void SetupInitialization() {
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      UniformRandomizer offsetRandomizer = new UniformRandomizer();
      offsetRandomizer.Min = minOffset;
      offsetRandomizer.Max = maxOffset + 1;
      offsetRandomizer.GetVariableInfo("Value").ActualName = OFFSET;
      offsetRandomizer.Name = "Offset Randomizer";

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(offsetRandomizer);
      combinedOp.OperatorGraph.InitialOperator = seq;
      seq.AddSubOperator(offsetRandomizer);
      Initializer = combinedOp;
    }

    private void SetupManipulation() {
      // manipulation operator
      CombinedOperator combinedOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      NormalRandomAdder offsetRandomAdder = new NormalRandomAdder();
      offsetRandomAdder.Mu = 0.0;
      offsetRandomAdder.Sigma = 1.0;
      offsetRandomAdder.GetVariableInfo("Value").ActualName = OFFSET;
      offsetRandomAdder.Name = "Offset Adder";
      offsetRandomAdder.GetVariableInfo("MinValue").Local = true;
      offsetRandomAdder.AddVariable(new HeuristicLab.Core.Variable("MinValue", new DoubleData(minOffset)));
      offsetRandomAdder.GetVariableInfo("MaxValue").Local = true;
      offsetRandomAdder.AddVariable(new HeuristicLab.Core.Variable("MaxValue", new DoubleData(maxOffset + 1)));

      combinedOp.OperatorGraph.AddOperator(seq);
      combinedOp.OperatorGraph.AddOperator(offsetRandomAdder);
      combinedOp.OperatorGraph.InitialOperator = seq;
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
