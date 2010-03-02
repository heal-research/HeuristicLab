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
using HeuristicLab.Data;
using System.Xml;

namespace HeuristicLab.GP.StructureIdentification {
  public class Variable : Terminal {
    public const string WEIGHT = "Weight";
    public const string OFFSET = "SampleOffset";
    public const string VARIABLENAME = "Variable";

    private int minOffset;
    public int MinTimeOffset {
      get {
        return minOffset;
      }
      set {
        if (value != minOffset) {
          minOffset = value;
          SetupInitialization();
          SetupManipulation();
        }
      }
    }

    private int maxOffset;
    public int MaxTimeOffset {
      get {
        return maxOffset;
      }
      set {
        if (value != maxOffset) {
          maxOffset = value;
          SetupManipulation();
          SetupInitialization();
        }
      }
    }

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
      offsetRandomAdder.GetVariableInfo("MinValue").Local = true;
      offsetRandomAdder.AddVariable(new HeuristicLab.Core.Variable("MinValue", new DoubleData(minOffset)));
      offsetRandomAdder.GetVariableInfo("MaxValue").Local = true;
      offsetRandomAdder.AddVariable(new HeuristicLab.Core.Variable("MaxValue", new DoubleData(maxOffset + 1)));

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

    public override HeuristicLab.Core.IView CreateView() {
      return new VariableView(this);
    }

    #region persistence
    public override object Clone(System.Collections.Generic.IDictionary<System.Guid, object> clonedObjects) {
      Variable clone = (Variable)base.Clone(clonedObjects);
      clone.MaxTimeOffset = MaxTimeOffset;
      clone.MinTimeOffset = MinTimeOffset;
      return clone;
    }
    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, System.Collections.Generic.IDictionary<System.Guid, HeuristicLab.Core.IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      var minTimeOffsetAttr = document.CreateAttribute("MinTimeOffset");
      minTimeOffsetAttr.Value = MinTimeOffset.ToString();
      var maxTimeOffsetAttr = document.CreateAttribute("MaxTimeOffset");
      maxTimeOffsetAttr.Value = MaxTimeOffset.ToString();
      node.Attributes.Append(minTimeOffsetAttr);
      node.Attributes.Append(maxTimeOffsetAttr);
      return node;
    }
    public override void Populate(System.Xml.XmlNode node, System.Collections.Generic.IDictionary<System.Guid, HeuristicLab.Core.IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      if (node.Attributes["MinTimeOffset"] != null)
        MinTimeOffset = XmlConvert.ToInt32(node.Attributes["MinTimeOffset"].Value);
      else MinTimeOffset = 0;
      if (node.Attributes["MaxTimeOffset"] != null)
        MaxTimeOffset = XmlConvert.ToInt32(node.Attributes["MaxTimeOffset"].Value);
      else MaxTimeOffset = 0;
    }
    #endregion
  }
}
