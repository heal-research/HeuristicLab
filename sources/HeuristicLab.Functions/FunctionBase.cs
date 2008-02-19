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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public abstract class FunctionBase : OperatorBase, IFunction {
    private List<IFunction> subFunctions;
    // instance subfunctions
    public IList<IFunction> SubFunctions {
      get {
        return subFunctions;
      }
    }

    // instance variables
    private List<IVariable> variables;
    public ICollection<IVariable> LocalVariables {
      get { return variables.AsReadOnly(); }
    }

    // reference to the 'type' of the function
    private FunctionBase metaObject;
    public IFunction MetaObject {
      get { return metaObject; }
    }

    public FunctionBase() {
      metaObject = this; // (FunctionBase)Activator.CreateInstance(this.GetType());
      AddVariableInfo(new VariableInfo("Dataset", "Dataset from which to read samples", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SampleIndex", "Gives the row index from which to read the sample", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Result", "The result of the evaluation of the function", typeof(DoubleData), VariableKind.Out));

      subFunctions = new List<IFunction>();
      variables = new List<IVariable>();
    }

    public FunctionBase(FunctionBase source, IDictionary<Guid, object> clonedObjects)
      : base() {
      this.metaObject = source.metaObject;
      variables = new List<IVariable>();
      subFunctions = new List<IFunction>();
      foreach (IFunction subFunction in source.SubFunctions) {
        subFunctions.Add((IFunction)Auxiliary.Clone(subFunction, clonedObjects));
      }
      foreach (IVariable variable in source.variables) {
        variables.Add((IVariable)Auxiliary.Clone(variable, clonedObjects));
      }
    }

    public abstract double Evaluate(Dataset dataset, int sampleIndex);

    public override IOperation Apply(IScope scope) {
      DoubleData result = this.GetVariableValue<DoubleData>("Result", scope, false);
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      IntData sampleIndex = GetVariableValue<IntData>("SampleIndex", scope, true);
      result.Data = Evaluate(dataset, sampleIndex.Data);
      return null;
    }

    public virtual void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }

    public override void AddSubOperator(IOperator subOperator) {
      subFunctions.Add((IFunction)subOperator);
    }

    public override bool TryAddSubOperator(IOperator subOperator) {
      subFunctions.Add((IFunction)subOperator);
      bool valid = IsValid();
      if (!valid) {
        subFunctions.RemoveAt(subFunctions.Count - 1);
      }

      return valid;
    }

    public override bool TryAddSubOperator(IOperator subOperator, int index) {
      subFunctions.Insert(index, (IFunction)subOperator);
      bool valid = IsValid();
      if (!valid) {
        subFunctions.RemoveAt(index);
      }
      return valid;
    }

    public override bool TryAddSubOperator(IOperator subOperator, int index, out ICollection<IConstraint> violatedConstraints) {
      subFunctions.Insert(index, (IFunction)subOperator);
      bool valid = IsValid(out violatedConstraints);
      if (!valid) {
        subFunctions.RemoveAt(index);
      }
      return valid;
    }

    public override bool TryAddSubOperator(IOperator subOperator, out ICollection<IConstraint> violatedConstraints) {
      subFunctions.Add((IFunction)subOperator);
      bool valid = IsValid(out violatedConstraints);
      if (!valid) {
        subFunctions.RemoveAt(subFunctions.Count - 1);
      }

      return valid;
    }

    public override void AddSubOperator(IOperator subOperator, int index) {
      subFunctions.Insert(index, (IFunction)subOperator);
    }

    public override void RemoveSubOperator(int index) {
      if (index >= subFunctions.Count) throw new InvalidOperationException();
      subFunctions.RemoveAt(index);
    }

    public override IList<IOperator> SubOperators {
      get { return subFunctions.ConvertAll(f => (IOperator)f); }
    }

    public override ICollection<IVariable> Variables {
      get {
        List<IVariable> mergedVariables = new List<IVariable>(variables);
        if (this == metaObject) {
          foreach (IVariable variable in base.Variables) {
            if (!IsLocalVariable(variable.Name)) {
              mergedVariables.Add(variable);
            }
          }
        } else {
          foreach (IVariable variable in metaObject.Variables) {
            if (!IsLocalVariable(variable.Name)) {
              mergedVariables.Add(variable);
            }
          }
        }
        return mergedVariables;
      }
    }

    private bool IsLocalVariable(string name) {
      foreach (IVariable variable in variables) {
        if (variable.Name == name) return true;
      }
      return false;
    }


    public override bool TryRemoveSubOperator(int index) {
      IFunction removedFunction = subFunctions[index];
      subFunctions.RemoveAt(index);
      bool valid = IsValid();
      if (!valid) {
        subFunctions.Insert(index, removedFunction);
      }

      return valid;
    }

    public override bool TryRemoveSubOperator(int index, out ICollection<IConstraint> violatedConstraints) {
      IFunction removedFunction = subFunctions[index];
      subFunctions.RemoveAt(index);
      bool valid = IsValid(out violatedConstraints);
      if (!valid) {
        subFunctions.Insert(index, removedFunction);
      }

      return valid;
    }

    public override void AddVariable(IVariable variable) {
      if (metaObject == this) {
        base.AddVariable(variable);
      } else {
        metaObject.AddVariable(variable);
      }
    }

    public override IVariable GetVariable(string name) {
      foreach (IVariable variable in variables) {
        if (variable.Name == name) return variable;
      }
      if (metaObject == this) {
        return base.GetVariable(name);
      } else {
        return metaObject.GetVariable(name);
      }
    }

    public void AddLocalVariable(IVariable variable) {
      variables.Add(variable);
    }

    public override void RemoveVariable(string name) {
      foreach (IVariable variable in variables) {
        if (variable.Name == name) {
          variables.Remove(variable);
          return;
        }
      }
      if (metaObject == this) {
        base.RemoveVariable(name);
      } else {
        metaObject.RemoveVariable(name);
      }
    }

    public override IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup, bool throwOnError) {
      foreach (IVariable variable in Variables) {
        if (variable.Name == formalName) {
          return variable.Value;
        }
      }
      return metaObject.GetVariableValue(formalName, scope, recursiveLookup, throwOnError);
    }

    public override ICollection<IVariableInfo> VariableInfos {
      get {
        if (metaObject == this) {
          return base.VariableInfos;
        } else {
          return metaObject.VariableInfos;
        }
      }
    }

    public override void AddVariableInfo(IVariableInfo variableInfo) {
      if (metaObject == this) {
        base.AddVariableInfo(variableInfo);
      } else {
        metaObject.AddVariableInfo(variableInfo);
      }
    }

    public override void RemoveVariableInfo(string formalName) {
      if (metaObject == this) {
        base.RemoveVariableInfo(formalName);
      } else {
        metaObject.RemoveVariableInfo(formalName);
      }
    }

    public override ICollection<IConstraint> Constraints {
      get {
        if (metaObject == this) {
          return base.Constraints;
        } else {
          return metaObject.Constraints;
        }
      }
    }

    public override void AddConstraint(IConstraint constraint) {
      if (metaObject == this) {
        base.AddConstraint(constraint);
      } else {
        metaObject.AddConstraint(constraint);
      }
    }
    public override void RemoveConstraint(IConstraint constraint) {
      if (metaObject == this) {
        base.RemoveConstraint(constraint);
      } else {
        metaObject.RemoveConstraint(constraint);
      }
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      if (metaObject != this) {
        XmlNode functionTemplateNode = document.CreateElement("FunctionTemplate");
        functionTemplateNode.AppendChild(PersistenceManager.Persist(metaObject, document, persistedObjects));
        node.AppendChild(functionTemplateNode);
      } 

      // don't need to persist the sub-functions because OperatorBase.GetXmlNode already persisted the sub-operators

      // persist local variables
      XmlNode variablesNode = document.CreateNode(XmlNodeType.Element, "LocalVariables", null);
      foreach (IVariable variable in variables) {
        variablesNode.AppendChild(PersistenceManager.Persist(variable, document, persistedObjects));
      }
      node.AppendChild(variablesNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode functionTemplateNode = node.SelectSingleNode("FunctionTemplate");
      if (functionTemplateNode != null) {
        metaObject = (FunctionBase)PersistenceManager.Restore(functionTemplateNode.ChildNodes[0], restoredObjects);
      } else {
        metaObject = this;
      }
      // don't need to explicitly load the sub-functions because that has already been done in OperatorBase.Populate()

      // load local variables
      XmlNode variablesNode = node.SelectSingleNode("LocalVariables");
      
      // remove the variables that have been added in a constructor
      variables.Clear();
      // load the persisted variables
      foreach (XmlNode variableNode in variablesNode.ChildNodes)
        variables.Add((IVariable)PersistenceManager.Restore(variableNode, restoredObjects));
    }

    public override IView CreateView() {
      return new FunctionView(this);
    }
  }
}
