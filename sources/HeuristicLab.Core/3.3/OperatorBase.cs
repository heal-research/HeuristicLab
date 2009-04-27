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
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// The base class for all operators.
  /// </summary>
  public abstract class OperatorBase : ConstrainedItemBase, IOperator {

    [Storable]
    private string myName;
    /// <summary>
    /// Gets or sets the name of the operator.
    /// </summary>
    /// <remarks>Calls <see cref="OnNameChanged"/> in the setter.</remarks>
    public string Name {
      get { return myName; }
      set {
        if (myName != value) {
          myName = value;
          OnNameChanged();
        }
      }
    }
    /// <summary>
    /// Gets the description of the current operator.
    /// </summary>
    /// <remarks>Returns "No operator description available" if the method is not overriden.</remarks>
    public virtual string Description {
      get { return "No operator description available."; }
    }
    /// <summary>
    /// Flag whether the current instance has been canceled.
    /// </summary>
    protected bool myCanceled;
    /// <inheritdoc/>
    public bool Canceled {
      get { return myCanceled; }
    }

    [Storable]
    private bool myBreakpoint;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnBreakpointChanged"/> in the setter.</remarks>
    public bool Breakpoint {
      get { return myBreakpoint; }
      set {
        if (value != myBreakpoint) {
          myBreakpoint = value;
          OnBreakpointChanged();
        }
      }
    }

    [Storable]
    private List<IOperator> mySubOperators;
    /// <summary>
    /// Gets a list of all suboperators.
    /// <note type="caution"> Returns the suboperators read-only!</note>
    /// </summary>
    public virtual IList<IOperator> SubOperators {
      get { return mySubOperators.AsReadOnly(); }
    }

    [Storable]
    private Dictionary<string, IVariableInfo> myVariableInfos;
    /// <inheritdoc/>
    public virtual ICollection<IVariableInfo> VariableInfos {
      get { return myVariableInfos.Values; }
    }

    [Storable]
    private Dictionary<string, IVariable> myVariables;
    /// <inheritdoc/>
    public virtual ICollection<IVariable> Variables {
      get { return myVariables.Values; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBase"/> setting the breakpoint flag and 
    /// the canceled flag to <c>false</c> and the name of the operator to the type name. 
    /// </summary>
    protected OperatorBase() {
      myName = this.GetType().Name;
      myCanceled = false;
      myBreakpoint = false;
      mySubOperators = new List<IOperator>();
      myVariableInfos = new Dictionary<string, IVariableInfo>();
      myVariables = new Dictionary<string, IVariable>();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Clones also sub operators, variables and variable infos.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorBase"/>.</returns>
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

    /// <summary>
    /// Creates a new instance of <see cref="OperatorBaseView"/> to represent the current operator
    /// visually.
    /// </summary>
    /// <returns>The created view as <see cref="OperatorBaseView"/>.</returns>
    public override IView CreateView() {
      return new OperatorBaseView(this);
    }

    #region SubOperator Methods
    /// <inheritdoc cref="HeuristicLab.Core.IOperator.AddSubOperator(HeuristicLab.Core.IOperator)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
    public virtual void AddSubOperator(IOperator subOperator) {
      mySubOperators.Add(subOperator);
      OnSubOperatorAdded(subOperator, mySubOperators.Count - 1);
    }
    /// <inheritdoc cref="IOperator.TryAddSubOperator(HeuristicLab.Core.IOperator)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
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
    /// <inheritdoc cref="HeuristicLab.Core.IOperator.TryAddSubOperator(HeuristicLab.Core.IOperator, 
    /// out System.Collections.Generic.ICollection&lt;HeuristicLab.Core.IConstraint&gt;)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
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
    /// <inheritdoc cref="HeuristicLab.Core.IOperator.AddSubOperator(HeuristicLab.Core.IOperator, int)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
    public virtual void AddSubOperator(IOperator subOperator, int index) {
      mySubOperators.Insert(index, subOperator);
      OnSubOperatorAdded(subOperator, index);
    }
    /// <inheritdoc cref="IOperator.TryAddSubOperator(HeuristicLab.Core.IOperator, int)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
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
    /// <inheritdoc cref="IOperator.TryAddSubOperator(HeuristicLab.Core.IOperator, int, out
    /// System.Collections.Generic.ICollection&lt;HeuristicLab.Core.IConstraint&gt;)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnSubOperatorRemoved"/>.</remarks>
    public virtual void RemoveSubOperator(int index) {
      IOperator op = mySubOperators[index];
      mySubOperators.RemoveAt(index);
      OnSubOperatorRemoved(op, index);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnSubOperatorRemoved"/>.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnSubOperatorRemoved"/>.</remarks>
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
    /// <inheritdoc/>
    public virtual IVariableInfo GetVariableInfo(string formalName) {
      IVariableInfo info;
      if (myVariableInfos.TryGetValue(formalName, out info))
        return info;
      else
        return null;
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableInfoAdded"/>.</remarks>
    public virtual void AddVariableInfo(IVariableInfo variableInfo) {
      myVariableInfos.Add(variableInfo.FormalName, variableInfo);
      OnVariableInfoAdded(variableInfo);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableInfoAdded"/>.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableInfoAdded"/>.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableInfoRemoved"/>.</remarks>
    public virtual void RemoveVariableInfo(string formalName) {
      IVariableInfo variableInfo;
      if (myVariableInfos.TryGetValue(formalName, out variableInfo)) {
        myVariableInfos.Remove(formalName);
        OnVariableInfoRemoved(variableInfo);
      }
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableInfoRemoved"/>.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableInfoRemoved"/>.</remarks>
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
    /// <inheritdoc/>
    public virtual IVariable GetVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable))
        return variable;
      else
        return null;
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableAdded"/> and adds <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
    public virtual void AddVariable(IVariable variable) {
      myVariables.Add(variable.Name, variable);
      variable.NameChanging += new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
      variable.NameChanged += new EventHandler(Variable_NameChanged);
      OnVariableAdded(variable);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableAdded"/> and adds <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableAdded"/> and adds <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableRemoved"/> and removes <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
    public virtual void RemoveVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        variable.NameChanging -= new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
        variable.NameChanged -= new EventHandler(Variable_NameChanged);
        myVariables.Remove(name);
        OnVariableRemoved(variable);
      }
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableRemoved"/> and removes <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
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
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableRemoved"/> and removes <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
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
    /// <inheritdoc cref="IOperator.GetVariableValue&lt;T&gt;(string, HeuristicLab.Core.IScope, bool)"/>
    ///  <remarks>Calls <see cref="GetVariableValue&lt;T&gt;(string, HeuristicLab.Core.IScope, bool, bool)"/> 
    /// with <c>throwOnError</c> set to <c>false</c>.</remarks>
    public T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup) where T : class, IItem {
      return GetVariableValue<T>(formalName, scope, recursiveLookup, true);
    }
    /// <inheritdoc cref="IOperator.GetVariableValue&lt;T&gt;(string, HeuristicLab.Core.IScope, bool, bool)"/>
    /// <remarks>Calls 
    /// <see cref="GetVariableValue(string, HeuristicLab.Core.IScope, bool, bool)"/>.</remarks>
    public T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup, bool throwOnError) where T : class, IItem {
      return (T)GetVariableValue(formalName, scope, recursiveLookup, throwOnError);
    }
    /// <inheritdoc cref="IOperator.GetVariableValue(string, HeuristicLab.Core.IScope, bool)"/>
    /// <remarks>Calls <see cref="GetVariableValue(string, HeuristicLab.Core.IScope, bool, bool)"/> 
    /// with <c>throwOnError</c> set to <c>false</c>.</remarks>
    public IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup) {
      return GetVariableValue(formalName, scope, recursiveLookup, true);
    }
    /// <inheritdoc cref="IOperator.GetVariableValue(string, HeuristicLab.Core.IScope, bool, bool)"/>
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
        return scope.GetVariableValue(formalName, recursiveLookup, throwOnError);
      }
    }
    #endregion
    /// <inheritdoc/>
    public virtual IOperation Execute(IScope scope) {
      myCanceled = false;

      foreach (IVariableInfo variableInfo in VariableInfos)
        scope.AddAlias(variableInfo.FormalName, variableInfo.ActualName);

      IOperation next = Apply(scope);

      foreach (IVariableInfo variableInfo in VariableInfos)
        scope.RemoveAlias(variableInfo.FormalName);

      OnExecuted();
      return next;
    }
    /// <inheritdoc/>
    /// <remarks>Sets property <see cref="Canceled"/> to <c>true</c>.</remarks>
    public virtual void Abort() {
      myCanceled = true;
    }
    /// <summary>
    /// Performs the current operator on the specified <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to execute the operator</param>
    /// <returns><c>null</c>.</returns>
    public virtual IOperation Apply(IScope scope) {
      return null;
    }
    /// <inheritdoc/>
    public event EventHandler NameChanged;
    /// <summary>
    /// Fires a new <c>NameChanged</c> event.
    /// </summary>
    protected virtual void OnNameChanged() {
      if (NameChanged != null) {
        NameChanged(this, new EventArgs());
      }
    }
    /// <inheritdoc/>
    public event EventHandler BreakpointChanged;
    /// <summary>
    /// Fires a new <c>BreakpointChanged</c> event.
    /// </summary>
    protected virtual void OnBreakpointChanged() {
      if (BreakpointChanged != null) {
        BreakpointChanged(this, new EventArgs());
      }
    }
    /// <inheritdoc/>
    public event EventHandler<OperatorIndexEventArgs> SubOperatorAdded;
    /// <summary>
    /// Fires a new <c>SubOperatorAdded</c> event.
    /// </summary>
    /// <param name="subOperator">The sub operator that has been added.</param>
    /// <param name="index">The position where the operator has been added.</param>
    protected virtual void OnSubOperatorAdded(IOperator subOperator, int index) {
      if (SubOperatorAdded != null)
        SubOperatorAdded(this, new OperatorIndexEventArgs(subOperator, index));
    }
    /// <inheritdoc/>
    public event EventHandler<OperatorIndexEventArgs> SubOperatorRemoved;
    /// <summary>
    /// Fires a new <c>SubOperatorRemoved</c> event.
    /// </summary>
    /// <param name="subOperator">The sub operator that has been removed.</param>
    /// <param name="index">The position where the operator has been removed.</param>
    protected virtual void OnSubOperatorRemoved(IOperator subOperator, int index) {
      if (SubOperatorRemoved != null)
        SubOperatorRemoved(this, new OperatorIndexEventArgs(subOperator, index));
    }
    /// <inheritdoc/>
    public event EventHandler<VariableInfoEventArgs> VariableInfoAdded;
    /// <summary>
    /// Fires a new <c>VariableInfoAdded</c> event.
    /// </summary>
    /// <param name="variableInfo">The variable info that has been added.</param>
    protected virtual void OnVariableInfoAdded(IVariableInfo variableInfo) {
      if (VariableInfoAdded != null)
        VariableInfoAdded(this, new VariableInfoEventArgs(variableInfo));
    }
    /// <inheritdoc/>
    public event EventHandler<VariableInfoEventArgs> VariableInfoRemoved;
    /// <summary>
    /// Fires a new <c>VariableInfoRemoved</c> event.
    /// </summary>
    /// <param name="variableInfo">The variable info that has been removed.</param>
    protected virtual void OnVariableInfoRemoved(IVariableInfo variableInfo) {
      if (VariableInfoRemoved != null)
        VariableInfoRemoved(this, new VariableInfoEventArgs(variableInfo));
    }
    /// <inheritdoc/>
    public event EventHandler<VariableEventArgs> VariableAdded;
    /// <summary>
    /// Fires a new <c>VariableAdded</c> event.
    /// </summary>
    /// <param name="variable">The variable that has been added.</param>
    protected virtual void OnVariableAdded(IVariable variable) {
      if (VariableAdded != null)
        VariableAdded(this, new VariableEventArgs(variable));
    }
    /// <inheritdoc/>
    public event EventHandler<VariableEventArgs> VariableRemoved;
    /// <summary>
    /// Fires a new <c>VariableRemoved</c> event.
    /// </summary>
    /// <param name="variable">The variable that has been removed</param>
    protected virtual void OnVariableRemoved(IVariable variable) {
      if (VariableRemoved != null)
        VariableRemoved(this, new VariableEventArgs(variable));
    }
    /// <inheritdoc/>
    public event EventHandler Executed;
    /// <summary>
    /// Fires a new <c>Executed</c> event.
    /// </summary>
    protected virtual void OnExecuted() {
      if (Executed != null) {
        Executed(this, new EventArgs());
      }
    }
  }
}
