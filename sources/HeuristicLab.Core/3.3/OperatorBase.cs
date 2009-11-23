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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// The base class for all operators.
  /// </summary>
  public abstract class OperatorBase : ItemBase, IOperator {

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
    
    private Dictionary<string, IVariable> myVariables;
    /// <inheritdoc/>    
    public virtual ICollection<IVariable> Variables {
      get { return myVariables.Values; }
    }

    [Storable(Name="Variables")]
    private List<IVariable> VariablePersistence {
      get { return new List<IVariable>(myVariables.Values); }
      set {
        myVariables.Clear();
        foreach (IVariable var in value) {
          AddVariable(var);
        }
      }
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

    #region SubOperator Methods
    /// <inheritdoc cref="HeuristicLab.Core.IOperator.AddSubOperator(HeuristicLab.Core.IOperator)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
    public virtual void AddSubOperator(IOperator subOperator) {
      mySubOperators.Add(subOperator);
      OnSubOperatorAdded(subOperator, mySubOperators.Count - 1);
    }
    /// <inheritdoc cref="HeuristicLab.Core.IOperator.AddSubOperator(HeuristicLab.Core.IOperator, int)"/>
    /// <param name="subOperator">The sub operator to add.</param>
    /// <remarks>Calls <see cref="OnSubOperatorAdded"/>.</remarks>
    public virtual void AddSubOperator(IOperator subOperator, int index) {
      mySubOperators.Insert(index, subOperator);
      OnSubOperatorAdded(subOperator, index);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnSubOperatorRemoved"/>.</remarks>
    public virtual void RemoveSubOperator(int index) {
      IOperator op = mySubOperators[index];
      mySubOperators.RemoveAt(index);
      OnSubOperatorRemoved(op, index);
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
    /// <remarks>Calls <see cref="OnVariableInfoRemoved"/>.</remarks>
    public virtual void RemoveVariableInfo(string formalName) {
      IVariableInfo variableInfo;
      if (myVariableInfos.TryGetValue(formalName, out variableInfo)) {
        myVariableInfos.Remove(formalName);
        OnVariableInfoRemoved(variableInfo);
      }
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
      variable.NameChanging += new EventHandler<CancelEventArgs<string>>(Variable_NameChanging);
      variable.NameChanged += new EventHandler(Variable_NameChanged);
      OnVariableAdded(variable);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnVariableRemoved"/> and removes <c>NameChanging</c> and <c>NameChanged</c>
    /// event handlers.</remarks>
    public virtual void RemoveVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        variable.NameChanging -= new EventHandler<CancelEventArgs<string>>(Variable_NameChanging);
        variable.NameChanged -= new EventHandler(Variable_NameChanged);
        myVariables.Remove(name);
        OnVariableRemoved(variable);
      }
    }
    private void Variable_NameChanging(object sender, CancelEventArgs<string> e) {
      e.Cancel = myVariables.ContainsKey(e.Value);
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
    public event EventHandler<EventArgs<IOperator, int>> SubOperatorAdded;
    /// <summary>
    /// Fires a new <c>SubOperatorAdded</c> event.
    /// </summary>
    /// <param name="subOperator">The sub operator that has been added.</param>
    /// <param name="index">The position where the operator has been added.</param>
    protected virtual void OnSubOperatorAdded(IOperator subOperator, int index) {
      if (SubOperatorAdded != null)
        SubOperatorAdded(this, new EventArgs<IOperator, int>(subOperator, index));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IOperator, int>> SubOperatorRemoved;
    /// <summary>
    /// Fires a new <c>SubOperatorRemoved</c> event.
    /// </summary>
    /// <param name="subOperator">The sub operator that has been removed.</param>
    /// <param name="index">The position where the operator has been removed.</param>
    protected virtual void OnSubOperatorRemoved(IOperator subOperator, int index) {
      if (SubOperatorRemoved != null)
        SubOperatorRemoved(this, new EventArgs<IOperator, int>(subOperator, index));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IVariableInfo>> VariableInfoAdded;
    /// <summary>
    /// Fires a new <c>VariableInfoAdded</c> event.
    /// </summary>
    /// <param name="variableInfo">The variable info that has been added.</param>
    protected virtual void OnVariableInfoAdded(IVariableInfo variableInfo) {
      if (VariableInfoAdded != null)
        VariableInfoAdded(this, new EventArgs<IVariableInfo>(variableInfo));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IVariableInfo>> VariableInfoRemoved;
    /// <summary>
    /// Fires a new <c>VariableInfoRemoved</c> event.
    /// </summary>
    /// <param name="variableInfo">The variable info that has been removed.</param>
    protected virtual void OnVariableInfoRemoved(IVariableInfo variableInfo) {
      if (VariableInfoRemoved != null)
        VariableInfoRemoved(this, new EventArgs<IVariableInfo>(variableInfo));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IVariable>> VariableAdded;
    /// <summary>
    /// Fires a new <c>VariableAdded</c> event.
    /// </summary>
    /// <param name="variable">The variable that has been added.</param>
    protected virtual void OnVariableAdded(IVariable variable) {
      if (VariableAdded != null)
        VariableAdded(this, new EventArgs<IVariable>(variable));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IVariable>> VariableRemoved;
    /// <summary>
    /// Fires a new <c>VariableRemoved</c> event.
    /// </summary>
    /// <param name="variable">The variable that has been removed</param>
    protected virtual void OnVariableRemoved(IVariable variable) {
      if (VariableRemoved != null)
        VariableRemoved(this, new EventArgs<IVariable>(variable));
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
