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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using System.Xml;

namespace HeuristicLab.GP.StructureIdentification {
  public class VariableFunctionTree : TerminalTreeNode {
    public double Weight { get; set; }
    public string VariableName { get; set; }
    public int SampleOffset { get; set; }

    public VariableFunctionTree(Variable variable)
      : base(variable) {
    }

    protected VariableFunctionTree(VariableFunctionTree original)
      : base(original) {
      Weight = original.Weight;
      VariableName = original.VariableName;
      SampleOffset = original.SampleOffset;
    }

    public override bool HasLocalParameters {
      get {
        return true;
      }
    }

    public override IOperation CreateShakingOperation(IScope scope) {
      Scope myVariableScope = new Scope();
      scope.AddSubScope(myVariableScope);
      myVariableScope.AddVariable(CreateWeightVariable());
      myVariableScope.AddVariable(CreateVariableNameVariable());
      myVariableScope.AddVariable(CreateSampleOffsetVariable());
      return new AtomicOperation(Function.Manipulator, myVariableScope);
    }

    public override IOperation CreateInitOperation(IScope scope) {
      Scope myVariableScope = new Scope();
      scope.AddSubScope(myVariableScope);
      myVariableScope.AddVariable(CreateWeightVariable());
      myVariableScope.AddVariable(CreateVariableNameVariable());
      myVariableScope.AddVariable(CreateSampleOffsetVariable());
      return new AtomicOperation(Function.Initializer, myVariableScope);
    }

    public override object Clone() {
      return new VariableFunctionTree(this);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<System.Guid, IStorable> persistedObjects) {
      XmlNode node = document.CreateElement(name);
      XmlAttribute weightAttr = document.CreateAttribute("Weight");
      weightAttr.Value = XmlConvert.ToString(Weight);
      XmlAttribute variableAttr = document.CreateAttribute("Variable");
      variableAttr.Value = VariableName;
      XmlAttribute sampleOffsetAttr = document.CreateAttribute("SampleOffset");
      sampleOffsetAttr.Value = XmlConvert.ToString(SampleOffset);
      node.Attributes.Append(weightAttr);
      node.Attributes.Append(sampleOffsetAttr);
      node.Attributes.Append(variableAttr);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<System.Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Weight = XmlConvert.ToDouble(node.Attributes["Weight"].Value);
      SampleOffset = XmlConvert.ToInt32(node.Attributes["SampleOffset"].Value);
      VariableName = node.Attributes["Variable"].Value;
    }

    private IVariable CreateSampleOffsetVariable() {
      IntData data = new IntData(SampleOffset);
      data.Changed += (sender, args) => SampleOffset = data.Data;
      var variable = new HeuristicLab.Core.Variable(Variable.OFFSET, data);
      variable.ValueChanged += (sender, args) => SampleOffset = ((IntData)variable.Value).Data;
      return variable;
    }

    private IVariable CreateVariableNameVariable() {
      StringData data = new StringData(VariableName);
      data.Changed += (sender, args) => VariableName = data.Data;
      var variable = new HeuristicLab.Core.Variable(Variable.VARIABLENAME, data);
      variable.ValueChanged += (sender, args) => VariableName = ((StringData)variable.Value).Data;
      return variable;
    }

    private IVariable CreateWeightVariable() {
      DoubleData data = new DoubleData(Weight);
      data.Changed += (sender, args) => Weight = data.Data;
      var variable = new HeuristicLab.Core.Variable(Variable.WEIGHT, data);
      variable.ValueChanged += (sender, args) => Weight = ((DoubleData)variable.Value).Data;
      return variable;
    }
  }
}
