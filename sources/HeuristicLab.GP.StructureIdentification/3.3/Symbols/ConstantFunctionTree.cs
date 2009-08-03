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
  public class ConstantFunctionTree : TerminalTreeNode {
    public double Value { get; set; }

    public ConstantFunctionTree(Constant constant) : base(constant){
    }

    protected ConstantFunctionTree(ConstantFunctionTree original) : base(original){
      Value = original.Value;
    }

    public override bool HasLocalParameters {
      get {
        return true;
      }
    }

    public override IOperation CreateShakingOperation(IScope scope) {
      Scope myVariableScope = new Scope();
      scope.AddSubScope(myVariableScope);
      myVariableScope.AddVariable(CreateValueVariable());
      return new AtomicOperation(Function.Manipulator, myVariableScope);
    }

    public override IOperation CreateInitOperation(IScope scope) {
      Scope myVariableScope = new Scope();
      scope.AddSubScope(myVariableScope);
      myVariableScope.AddVariable(CreateValueVariable());
      return new AtomicOperation(Function.Initializer, myVariableScope);
    }

    public override object Clone() {
      return new ConstantFunctionTree(this);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<System.Guid, IStorable> persistedObjects) {
      XmlNode node = document.CreateElement(name);
      XmlAttribute valueAttr = document.CreateAttribute("Value");
      valueAttr.Value = XmlConvert.ToString(Value);
      node.Attributes.Append(valueAttr);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<System.Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Value = XmlConvert.ToDouble(node.Attributes["Value"].Value);
    }

    private IVariable CreateValueVariable() {
      DoubleData data = new DoubleData(Value);
      data.Changed += (sender, args) => Value = data.Data;
      var variable = new HeuristicLab.Core.Variable(Constant.VALUE, data);
      variable.ValueChanged += (sender, args) => Value = ((DoubleData)variable.Value).Data;
      return variable;
    }
  }
}
