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
        "A combined operator contains a whole operator graph. It is useful for modularization to assemble complex operators out of simpler ones.\r\n\r\n" +
        "A combined operator can automatically inject its sub-operators into the scope it is applied on. Those operators can be extracted again in the contained operator graph by using an OperatorExtractor. So it is possible to parameterize a combined operator with custom operators. To activate sub-operator injection take a look at the local variables InjectSubOperators and SubOperatorNames.";
      myOperatorGraph = new OperatorGraph();
      AddVariableInfo(new VariableInfo("InjectSubOperators", "true if the sub-operators of this combined operator should be injected into the scope", typeof(BoolData), VariableKind.In));
      GetVariableInfo("InjectSubOperators").Local = true;
      AddVariable(new Variable("InjectSubOperators", new BoolData(false)));
      AddVariableInfo(new VariableInfo("SubOperatorNames", "Variable names for injecting the sub-operators", typeof(ItemList), VariableKind.In));
      GetVariableInfo("SubOperatorNames").Local = true;
      ItemList subOperatorNames = new ItemList();
      subOperatorNames.ItemType = typeof(StringData);
      AddVariable(new Variable("SubOperatorNames", subOperatorNames));
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
        bool inject = GetVariableValue<BoolData>("InjectSubOperators", scope, false).Data;
        if (inject) {
          ItemList names = GetVariableValue<ItemList>("SubOperatorNames", scope, false);
          for (int i = 0; i < SubOperators.Count; i++) {
            if (scope.GetVariable(names[i].ToString()) != null)
              scope.RemoveVariable(names[i].ToString());
            scope.AddVariable(new Variable(names[i].ToString(), SubOperators[i]));
          }
        }
        return new AtomicOperation(OperatorGraph.InitialOperator, scope);
      } else {
        return null;
      }
    }
    public override void Reset() {
      OperatorGraph.Reset();
      base.Reset();
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
