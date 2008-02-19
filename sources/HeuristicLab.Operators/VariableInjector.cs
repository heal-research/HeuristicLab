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
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Operators {
  public class VariableInjector : OperatorBase {
    private Dictionary<IVariable, IVariableInfo> variableVariableInfoTable;
    private Dictionary<IVariableInfo, IVariable> variableInfoVariableTable;

    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public VariableInjector()
      : base() {
      variableVariableInfoTable = new Dictionary<IVariable, IVariableInfo>();
      variableInfoVariableTable = new Dictionary<IVariableInfo, IVariable>();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      VariableInjector clone = new VariableInjector();
      clonedObjects.Add(Guid, clone);
      clone.Name = Name;
      foreach (IVariable variable in Variables)
        clone.AddVariable((IVariable)Auxiliary.Clone(variable, clonedObjects));
      return clone;
    }

    public override void AddVariable(IVariable variable) {
      base.AddVariable(variable);
      CreateVariableInfo(variable);
    }

    public override void RemoveVariable(string name) {
      DeleteVariableInfo(name);
      base.RemoveVariable(name);
    }

    public override IOperation Apply(IScope scope) {
      foreach (IVariable variable in Variables) {
        if (scope.GetVariable(variable.Name) != null)
          scope.RemoveVariable(variable.Name);
        scope.AddVariable((IVariable)variable.Clone());
      }
      return null;
    }

    public override IView CreateView() {
      return new VariableInjectorView(this);
    }

    private void CreateVariableInfo(IVariable variable) {
      IVariableInfo info = new VariableInfo(Guid.NewGuid().ToString(), "Injected variable", variable.Value.GetType(), VariableKind.New);
      info.ActualName = variable.Name;
      AddVariableInfo(info);
      variableVariableInfoTable.Add(variable, info);
      variableInfoVariableTable.Add(info, variable);
      info.ActualNameChanged += new EventHandler(VariableInfo_ActualNameChanged);
      variable.NameChanged += new EventHandler(Variable_NameChanged);
    }
    private void DeleteVariableInfo(string name) {
      IVariable variable = GetVariable(name);
      if (variable != null) {
        IVariableInfo info = variableVariableInfoTable[variable];
        RemoveVariableInfo(info.FormalName);
        variableVariableInfoTable.Remove(variable);
        variableInfoVariableTable.Remove(info);
        info.ActualNameChanged -= new EventHandler(VariableInfo_ActualNameChanged);
        variable.NameChanged -= new EventHandler(Variable_NameChanged);
      }
    }

    #region VariableInfo and Variable Events
    private void VariableInfo_ActualNameChanged(object sender, EventArgs e) {
      IVariableInfo info = (IVariableInfo)sender;
      IVariable variable = variableInfoVariableTable[info];
      variable.Name = info.ActualName;
    }
    private void Variable_NameChanged(object sender, EventArgs e) {
      IVariable variable = (IVariable)sender;
      IVariableInfo info = variableVariableInfoTable[variable];
      info.ActualName = variable.Name;
    }
    #endregion

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      // variable infos should not be persisted
      XmlNode infosNode = node.SelectSingleNode("VariableInfos");
      infosNode.RemoveAll();
      return node;
    }
    #endregion
  }
}
