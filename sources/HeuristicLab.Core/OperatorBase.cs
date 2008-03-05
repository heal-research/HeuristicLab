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

namespace HeuristicLab.Core {
  public abstract class OperatorBase : ConstrainedItemBase, IOperator {
    private string myName;
    public string Name {
      get { return myName; }
      set {
        if (myName != value) {
          myName = value;
          OnNameChanged();
        }
      }
    }
    public virtual string Description {
      get { return "No operator description available."; }
    }

    private bool myCanceled;
    public bool Canceled {
      get { return myCanceled; }
    }
    private bool myBreakpoint;
    public bool Breakpoint {
      get { return myBreakpoint; }
      set {
        if (value != myBreakpoint) {
          myBreakpoint = value;
          OnBreakpointChanged();
        }
      }
    }

    private List<IOperator> mySubOperators;
    public virtual IList<IOperator> SubOperators {
      get { return mySubOperators.AsReadOnly(); }
    }
    private Dictionary<string, IVariableInfo> myVariableInfos;
    public virtual ICollection<IVariableInfo> VariableInfos {
      get { return myVariableInfos.Values; }
    }
    private Dictionary<string, IVariable> myVariables;
    public virtual ICollection<IVariable> Variables {
      get { return myVariables.Values; }
    }

    protected OperatorBase() {
      myName = this.GetType().Name;
      myCanceled = false;
      myBreakpoint = false;
      mySubOperators = new List<IOperator>();
      myVariableInfos = new Dictionary<string, IVariableInfo>();
      myVariables = new Dictionary<string, IVariable>();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorBase clone = (OperatorBase)base.Clone(clonedObjects);
      clone.myName = Name;
      clone.mySubOperators.Clear();
      for (int i = 0; i < SubOperators.Count; i++)
        clone.AddSubOperator((IOperator)Auxiliary.Clone(SubOperators[i], clonedObjects));
      clone.myVariableInfos.Clear();
      foreach (IVariableInfo variableInfo in myVariableInfos.Values)
        clone.AddVariableInfo((IVariableInfo)Auxiliary.Clone(variableInfo, clonedObjects));
      clone.myVariables.Clear();
      foreach (IVariable variable in myVariables.Values)
        clone.AddVariable((IVariable)Auxiliary.Clone(variable, clonedObjects));
      return clone;
    }

    public override IView CreateView() {
      return new OperatorBaseView(this);
    }

    #region SubOperator Methods
    public virtual void AddSubOperator(IOperator subOperator) {
      mySubOperators.Add(subOperator);
      OnSubOperatorAdded(subOperator, mySubOperators.Count - 1);
    }
    public virtual bool TryAddSubOperator(IOperator subOperator) {
      mySubOperators.Add(subOperator);
      if (IsValid()) {
        OnSubOperatorAdded(subOperator, mySubOperators.Count - 1);
        return true;
      } else {
        mySubOperators.RemoveAt(mySubOperators.Count - 1);
        return false;
      }
    }
    public virtual bool TryAddSubOperator(IOperator subOperator, out ICollection<IConstraint> violatedConstraints) {
      mySubOperators.Add(subOperator);
      if (IsValid(out violatedConstraints)) {
        OnSubOperatorAdded(subOperator, mySubOperators.Count - 1);
        return true;
      } else {
        mySubOperators.RemoveAt(mySubOperators.Count - 1);
        return false;
      }
    }
    public virtual void AddSubOperator(IOperator subOperator, int index) {
      mySubOperators.Insert(index, subOperator);
      OnSubOperatorAdded(subOperator, index);
    }
    public virtual bool TryAddSubOperator(IOperator subOperator, int index) {
      mySubOperators.Insert(index, subOperator);
      if (IsValid()) {
        OnSubOperatorAdded(subOperator, index);
        return true;
      } else {
        mySubOperators.RemoveAt(index);
        return false;
      }
    }
    public virtual bool TryAddSubOperator(IOperator subOperator, int index, out ICollection<IConstraint> violatedConstraints) {
      mySubOperators.Insert(index, subOperator);
      if (IsValid(out violatedConstraints)) {
        OnSubOperatorAdded(subOperator, index);
        return true;
      } else {
        mySubOperators.RemoveAt(index);
        return false;
      }
    }
    public virtual void RemoveSubOperator(int index) {
      IOperator op = mySubOperators[index];
      mySubOperators.RemoveAt(index);
      OnSubOperatorRemoved(op, index);
    }
    public virtual bool TryRemoveSubOperator(int index) {
      IOperator op = mySubOperators[index];
      mySubOperators.RemoveAt(index);
      if (IsValid()) {
        OnSubOperatorRemoved(op, index);
        return true;
      } else {
        mySubOperators.Insert(index, op);
        return false;
      }
    }
    public virtual bool TryRemoveSubOperator(int index, out ICollection<IConstraint> violatedConstraints) {
      IOperator op = mySubOperators[index];
      mySubOperators.RemoveAt(index);
      if (IsValid(out violatedConstraints)) {
        OnSubOperatorRemoved(op, index);
        return true;
      } else {
        mySubOperators.Insert(index, op);
        return false;
      }
    }
    #endregion

    #region VariableInfo Methods
    public virtual IVariableInfo GetVariableInfo(string formalName) {
      IVariableInfo info;
      if (myVariableInfos.TryGetValue(formalName, out info))
        return info;
      else
        return null;
    }
    public virtual void AddVariableInfo(IVariableInfo variableInfo) {
      myVariableInfos.Add(variableInfo.FormalName, variableInfo);
      OnVariableInfoAdded(variableInfo);
    }
    public virtual bool TryAddVariableInfo(IVariableInfo variableInfo) {
      myVariableInfos.Add(variableInfo.FormalName, variableInfo);
      if (IsValid()) {
        OnVariableInfoAdded(variableInfo);
        return true;
      } else {
        myVariableInfos.Remove(variableInfo.FormalName);
        return false;
      }
    }
    public virtual bool TryAddVariableInfo(IVariableInfo variableInfo, out ICollection<IConstraint> violatedConstraints) {
      myVariableInfos.Add(variableInfo.FormalName, variableInfo);
      if (IsValid(out violatedConstraints)) {
        OnVariableInfoAdded(variableInfo);
        return true;
      } else {
        myVariableInfos.Remove(variableInfo.FormalName);
        return false;
      }
    }
    public virtual void RemoveVariableInfo(string formalName) {
      IVariableInfo variableInfo;
      if (myVariableInfos.TryGetValue(formalName, out variableInfo)) {
        myVariableInfos.Remove(formalName);
        OnVariableInfoRemoved(variableInfo);
      }
    }
    public virtual bool TryRemoveVariableInfo(string formalName) {
      IVariableInfo variableInfo;
      if (myVariableInfos.TryGetValue(formalName, out variableInfo)) {
        myVariableInfos.Remove(formalName);
        if (IsValid()) {
          OnVariableInfoRemoved(variableInfo);
          return true;
        } else {
          myVariableInfos.Add(formalName, variableInfo);
          return false;
        }
      }
      return true;
    }
    public virtual bool TryRemoveVariableInfo(string formalName, out ICollection<IConstraint> violatedConstraints) {
      IVariableInfo variableInfo;
      if (myVariableInfos.TryGetValue(formalName, out variableInfo)) {
        myVariableInfos.Remove(formalName);
        if (IsValid(out violatedConstraints)) {
          OnVariableInfoRemoved(variableInfo);
          return true;
        } else {
          myVariableInfos.Add(formalName, variableInfo);
          return false;
        }
      }
      violatedConstraints = new List<IConstraint>();
      return true;
    }
    #endregion

    #region Variable Methods
    public virtual IVariable GetVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable))
        return variable;
      else
        return null;
    }
    public virtual void AddVariable(IVariable variable) {
      myVariables.Add(variable.Name, variable);
      variable.NameChanging += new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
      variable.NameChanged += new EventHandler(Variable_NameChanged);
      OnVariableAdded(variable);
    }
    public virtual bool TryAddVariable(IVariable variable) {
      myVariables.Add(variable.Name, variable);
      if (IsValid()) {
        variable.NameChanging += new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
        variable.NameChanged += new EventHandler(Variable_NameChanged);
        OnVariableAdded(variable);
        return true;
      } else {
        myVariableInfos.Remove(variable.Name);
        return false;
      }
    }
    public virtual bool TryAddVariable(IVariable variable, out ICollection<IConstraint> violatedConstraints) {
      myVariables.Add(variable.Name, variable);
      if (IsValid(out violatedConstraints)) {
        variable.NameChanging += new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
        variable.NameChanged += new EventHandler(Variable_NameChanged);
        OnVariableAdded(variable);
        return true;
      } else {
        myVariableInfos.Remove(variable.Name);
        return false;
      }
    }
    public virtual void RemoveVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        variable.NameChanging -= new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
        variable.NameChanged -= new EventHandler(Variable_NameChanged);
        myVariables.Remove(name);
        OnVariableRemoved(variable);
      }
    }
    public virtual bool TryRemoveVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        myVariables.Remove(name);
        if (IsValid()) {
          variable.NameChanging -= new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
          variable.NameChanged -= new EventHandler(Variable_NameChanged);
          OnVariableRemoved(variable);
          return true;
        } else {
          myVariables.Add(name, variable);
          return false;
        }
      }
      return true;
    }
    public virtual bool TryRemoveVariable(string name, out ICollection<IConstraint> violatedConstraints) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        myVariables.Remove(name);
        if (IsValid(out violatedConstraints)) {
          variable.NameChanging -= new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
          variable.NameChanged -= new EventHandler(Variable_NameChanged);
          OnVariableRemoved(variable);
          return true;
        } else {
          myVariables.Add(name, variable);
          return false;
        }
      }
      violatedConstraints = new List<IConstraint>();
      return true;
    }
    private void Variable_NameChanging(object sender, NameChangingEventArgs e) {
      e.Cancel = myVariables.ContainsKey(e.Name);
    }
    private void Variable_NameChanged(object sender, EventArgs e) {
      IVariable variable = (IVariable)sender;
      string oldName = null;
      foreach (KeyValuePair<string, IVariable> element in myVariables) {
        if (element.Value == variable)
          oldName = element.Key;
      }
      myVariables.Remove(oldName);
      myVariables.Add(variable.Name, variable);
    }
    public T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup) where T : class, IItem {
      return GetVariableValue<T>(formalName, scope, recursiveLookup, true);
    }
    public T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup, bool throwOnError) where T : class, IItem {
      return (T)GetVariableValue(formalName, scope, recursiveLookup, throwOnError);
    }
    public IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup) {
      return GetVariableValue(formalName, scope, recursiveLookup, true);
    }
    public virtual IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup, bool throwOnError) {
      IVariableInfo info = GetVariableInfo(formalName);
      if (info.Local) {
        IVariable variable;
        if (myVariables.TryGetValue(info.ActualName, out variable))
          return variable.Value;
        else {
          if (throwOnError)
            throw new ArgumentException("Variable " + info.ActualName + " not found");
          else
            return null;
        }
      } else {
        return scope.GetVariableValue(info.ActualName, recursiveLookup, throwOnError);
      }
    }
    #endregion

    public virtual IOperation Execute(IScope scope) {
      myCanceled = false;
      IOperation next = Apply(scope);
      OnExecuted();
      return next;
    }
    public virtual void Abort() {
      myCanceled = true;
    }

    public virtual IOperation Apply(IScope scope) {
      return null;
    }

    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      if (NameChanged != null) {
        NameChanged(this, new EventArgs());
      }
    }
    public event EventHandler BreakpointChanged;
    protected virtual void OnBreakpointChanged() {
      if (BreakpointChanged != null) {
        BreakpointChanged(this, new EventArgs());
      }
    }
    public event EventHandler<OperatorIndexEventArgs> SubOperatorAdded;
    protected virtual void OnSubOperatorAdded(IOperator subOperator, int index) {
      if (SubOperatorAdded != null)
        SubOperatorAdded(this, new OperatorIndexEventArgs(subOperator, index));
    }
    public event EventHandler<OperatorIndexEventArgs> SubOperatorRemoved;
    protected virtual void OnSubOperatorRemoved(IOperator subOperator, int index) {
      if (SubOperatorRemoved != null)
        SubOperatorRemoved(this, new OperatorIndexEventArgs(subOperator, index));
    }
    public event EventHandler<VariableInfoEventArgs> VariableInfoAdded;
    protected virtual void OnVariableInfoAdded(IVariableInfo variableInfo) {
      if (VariableInfoAdded != null)
        VariableInfoAdded(this, new VariableInfoEventArgs(variableInfo));
    }
    public event EventHandler<VariableInfoEventArgs> VariableInfoRemoved;
    protected virtual void OnVariableInfoRemoved(IVariableInfo variableInfo) {
      if (VariableInfoRemoved != null)
        VariableInfoRemoved(this, new VariableInfoEventArgs(variableInfo));
    }
    public event EventHandler<VariableEventArgs> VariableAdded;
    protected virtual void OnVariableAdded(IVariable variable) {
      if (VariableAdded != null)
        VariableAdded(this, new VariableEventArgs(variable));
    }
    public event EventHandler<VariableEventArgs> VariableRemoved;
    protected virtual void OnVariableRemoved(IVariable variable) {
      if (VariableRemoved != null)
        VariableRemoved(this, new VariableEventArgs(variable));
    }
    public event EventHandler Executed;
    protected virtual void OnExecuted() {
      if (Executed != null) {
        Executed(this, new EventArgs());
      }
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nameAttribute = document.CreateAttribute("Name");
      nameAttribute.Value = Name;
      node.Attributes.Append(nameAttribute);
      if (Breakpoint) {
        XmlAttribute breakpointAttribute = document.CreateAttribute("Breakpoint");
        breakpointAttribute.Value = Breakpoint.ToString();
        node.Attributes.Append(breakpointAttribute);
      }
      XmlNode subOperatorsNode = document.CreateNode(XmlNodeType.Element, "SubOperators", null);
      for (int i = 0; i < SubOperators.Count; i++)
        subOperatorsNode.AppendChild(PersistenceManager.Persist(SubOperators[i], document, persistedObjects));
      node.AppendChild(subOperatorsNode);
      XmlNode infosNode = document.CreateNode(XmlNodeType.Element, "VariableInfos", null);
      foreach (IVariableInfo info in myVariableInfos.Values)
        infosNode.AppendChild(PersistenceManager.Persist(info, document, persistedObjects));
      node.AppendChild(infosNode);
      XmlNode variablesNode = document.CreateNode(XmlNodeType.Element, "Variables", null);
      foreach (IVariable variable in myVariables.Values)
        variablesNode.AppendChild(PersistenceManager.Persist(variable, document, persistedObjects));
      node.AppendChild(variablesNode);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myName = node.Attributes["Name"].Value;
      if (node.Attributes["Breakpoint"] != null)
        myBreakpoint = bool.Parse(node.Attributes["Breakpoint"].Value);
      XmlNode subOperatorsNode = node.SelectSingleNode("SubOperators");
      for (int i = 0; i < subOperatorsNode.ChildNodes.Count; i++)
        AddSubOperator((IOperator)PersistenceManager.Restore(subOperatorsNode.ChildNodes[i], restoredObjects));
      XmlNode infosNode = node.SelectSingleNode("VariableInfos");
      myVariableInfos.Clear();
      foreach (XmlNode infoNode in infosNode.ChildNodes)
        AddVariableInfo((IVariableInfo)PersistenceManager.Restore(infoNode, restoredObjects));
      XmlNode variablesNode = node.SelectSingleNode("Variables");
      myVariables.Clear();
      foreach (XmlNode variableNode in variablesNode.ChildNodes)
        AddVariable((IVariable)PersistenceManager.Restore(variableNode, restoredObjects));
    }
    #endregion
  }
}
