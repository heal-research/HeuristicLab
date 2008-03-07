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
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  public class CombinedOperator : OperatorBase {
    private string myDescription;
    public override string Description {
      get { return myDescription; }
    }
    private IOperatorGraph myOperatorGraph;
    public IOperatorGraph OperatorGraph {
      get { return myOperatorGraph; }
    }

    public CombinedOperator()
      : base() {
      myDescription =
        @"A combined operator contains a whole operator graph. It is useful for modularization to assemble complex operators out of simpler ones.

A combined operator automatically inject its sub-operators into the scope it is applied on. Thereby the names of the sub-operators are used as variable names. Those operators can be extracted again in the contained operator graph by using an OperatorExtractor. So it is possible to parameterize a combined operator with custom operators.";
      myOperatorGraph = new OperatorGraph();
    }

    public void SetDescription(string description) {
      if (description == null)
        throw new NullReferenceException("description must not be null");

      if (description != myDescription) {
        myDescription = description;
        OnDescriptionChanged();
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      CombinedOperator clone = (CombinedOperator)base.Clone(clonedObjects);
      clone.myDescription = Description;
      clone.myOperatorGraph = (IOperatorGraph)Auxiliary.Clone(OperatorGraph, clonedObjects);
      return clone;
    }

    public override IOperation Apply(IScope scope) {
      if (OperatorGraph.InitialOperator != null) {
        if ((scope.SubScopes.Count != 1) || (scope.SubScopes[0].Name != Guid.ToString())) {
          PreCall(scope);
          CompositeOperation next = new CompositeOperation();
          next.AddOperation(new AtomicOperation(OperatorGraph.InitialOperator, scope.SubScopes[0]));
          next.AddOperation(new AtomicOperation(this, scope));
          return next;
        } else {
          PostCall(scope);
        }
     }
      return null;
    }
    private void PreCall(IScope scope) {
      // create temporary scope and inject variables
      IScope temp = new Scope(Guid.ToString());

      while (scope.SubScopes.Count > 0) {
        IScope child = scope.SubScopes[0];
        scope.RemoveSubScope(child);
        temp.AddSubScope(child);
      }
      scope.AddSubScope(temp);

      for (int i = 0; i < SubOperators.Count; i++) {
        if (scope.GetVariable(SubOperators[i].Name) != null)
          scope.RemoveVariable(SubOperators[i].Name);
        scope.AddVariable(new Variable(SubOperators[i].Name, SubOperators[i]));
      }

      foreach (IVariableInfo variableInfo in VariableInfos) {
        if ((variableInfo.Kind & VariableKind.In) == VariableKind.In) {
          IItem value = GetVariableValue(variableInfo.FormalName, scope, true, true);
          temp.AddVariable(new Variable(variableInfo.FormalName, value));
        }
      }
    }
    private void PostCall(IScope scope) {
      // remove temporary scope and write back variables
      IScope temp = scope.SubScopes[0];

      scope.RemoveSubScope(temp);
      while (temp.SubScopes.Count > 0) {
        IScope child = temp.SubScopes[0];
        temp.RemoveSubScope(child);
        scope.AddSubScope(child);
      }

      foreach (IVariableInfo variableInfo in VariableInfos) {
        if (((variableInfo.Kind & VariableKind.Out) == VariableKind.Out) ||
            ((variableInfo.Kind & VariableKind.New) == VariableKind.New)) {
          IItem value = temp.GetVariableValue(variableInfo.FormalName, false, true);
          if (scope.GetVariable(variableInfo.ActualName) != null)
            scope.RemoveVariable(variableInfo.ActualName);
          scope.AddVariable(new Variable(variableInfo.ActualName, value));
        }
        if ((variableInfo.Kind & VariableKind.Deleted) == VariableKind.Deleted)
          scope.RemoveVariable(variableInfo.ActualName);
        temp.RemoveVariable(variableInfo.FormalName);
      }

      foreach (IVariable variable in temp.Variables) {
        if (scope.GetVariable(variable.Name) != null)
          scope.RemoveVariable(variable.Name);
        scope.AddVariable(variable);
      }
    }

    public override IView CreateView() {
      return new CombinedOperatorView(this);
    }

    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      if (DescriptionChanged != null)
        DescriptionChanged(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode descriptionNode = document.CreateNode(XmlNodeType.Element, "Description", null);
      descriptionNode.InnerText = myDescription;
      node.AppendChild(descriptionNode);
      node.AppendChild(PersistenceManager.Persist("OperatorGraph", OperatorGraph, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode descriptionNode = node.SelectSingleNode("Description");
      if (descriptionNode != null) myDescription = descriptionNode.InnerText;
      myOperatorGraph = (IOperatorGraph)PersistenceManager.Restore(node.SelectSingleNode("OperatorGraph"), restoredObjects);
    }
    #endregion
  }
}
