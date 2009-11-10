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
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent an operator (e.g. GreaterThanComparator,...), 
  /// a basic instruction of an algorithm.
  /// </summary>
  public interface IOperator : IConstrainedItem {
    /// <summary>
    /// Gets or sets the name of the current instance.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Gets or sets the description of the current instance.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets information whether the current operator has been canceled.
    /// </summary>
    bool Canceled { get; }
    /// <summary>
    /// Gets or sets a boolean value whether the engine should stop here during the run.
    /// </summary>
    bool Breakpoint { get; set; }

    /// <summary>
    /// Gets a list of all sub operators.
    /// </summary>
    IList<IOperator> SubOperators { get; }
    /// <summary>
    /// Gets a collection of all variable (parameter) infos.
    /// </summary>
    ICollection<IVariableInfo> VariableInfos { get; }
    /// <summary>
    /// Gets a collection of all variables of the current operator.
    /// </summary>
    ICollection<IVariable> Variables { get; }

    /// <summary>
    /// Adds the given sub operator to the current instance.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    void AddSubOperator(IOperator op);
    /// <summary>
    /// Adds the given sub operator to the current instance if all constraints can be fulfilled.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    /// <returns><c>true</c> if the operator could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddSubOperator(IOperator op);
    /// <summary>
    /// Adds the given sub operator to the current instance if all constraints can be fulfilled.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could not be
    /// fulfilled.</param>
    /// <returns><c>true</c> if the operator could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddSubOperator(IOperator op, out ICollection<IConstraint> violatedConstraints);
    /// <summary>
    /// Adds the given sub operator at a the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    /// <param name="index">The position where to add the operator.</param>
    void AddSubOperator(IOperator op, int index);
    /// <summary>
    /// Adds the given operator at the specified <paramref name="index"/> to the current instance 
    /// if all constraints can be fulfilled.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    /// <param name="index">The position where to add the operator.</param>
    /// <returns><c>true</c> if the operator could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddSubOperator(IOperator op, int index);
    /// <summary>
    /// Adds the given operator at the specified <paramref name="index"/> to the current instance 
    /// if all constraints can be fulfilled.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    /// <param name="index">The position where to add the operator.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could not be
    /// fulfilled.</param>
    /// <returns><c>true</c> if the operator could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddSubOperator(IOperator op, int index, out ICollection<IConstraint> violatedConstraints);
    /// <summary>
    /// Removes a sub operator at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The position where to delete the operator.</param>
    void RemoveSubOperator(int index);
    /// <summary>
    /// Removes a sub operator at the specified <paramref name="index"/> if all constraint can be fulfilled.
    /// </summary>
    /// <param name="index">The position where to delete the operator.</param>
    /// <returns><c>true</c> if the operator could be deleted without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryRemoveSubOperator(int index);
    /// <summary>
    /// Deletes the operator at the specified <paramref name="index"/>  
    /// if all constraints can be fulfilled.
    /// </summary>
    /// <param name="index">The position where to delete the operator.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could not be
    /// fulfilled.</param>
    /// <returns><c>true</c> if the operator could be deleted without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryRemoveSubOperator(int index, out ICollection<IConstraint> violatedConstraints);

    /// <summary>
    /// Gets the variable info with the given <paramref name="formalName"/>.
    /// </summary>
    /// <param name="formalName">The formal name of the variable info.</param>
    /// <returns>The variable info with the specified formal name.</returns>
    IVariableInfo GetVariableInfo(string formalName);
    /// <summary>
    /// Adds the specified variable info to the current instance.
    /// </summary>
    /// <param name="variableInfo">The variable info to add.</param>
    void AddVariableInfo(IVariableInfo variableInfo);
    /// <summary>
    /// Adds the specified variable info to the current instance, if all constraints can be fulfilled.
    /// </summary>
    /// <param name="variableInfo">The variable info to add.</param>
    /// <returns><c>true</c> if the variable info could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddVariableInfo(IVariableInfo variableInfo);
    /// <summary>
    /// Adds the specified variable info to the current instance, if all constraints can be fulfilled.
    /// </summary>
    /// <param name="variableInfo">The variable info to add.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could not be
    /// fulfilled.</param>
    /// <returns><c>true</c> if the variable info could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddVariableInfo(IVariableInfo variableInfo, out ICollection<IConstraint> violatedConstraints);
    /// <summary>
    /// Removes the variable info with the given formal name.
    /// </summary>
    /// <param name="formalName">The formal name of the variable info to remove.</param>
    void RemoveVariableInfo(string formalName);
    /// <summary>
    /// Deletes the variable info with the given formal name, 
    /// if all constraints can be fulfilled.
    /// </summary>
    /// <param name="formalName">The formal name of the variable info to remove.</param>
    /// <returns><c>true</c> if the variable info could be deleted without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryRemoveVariableInfo(string formalName);
    /// <summary>
    /// Deletes the variable info with the given formal name, 
    /// if all constraints can be fulfilled.
    /// </summary>
    /// <param name="formalName">The formal name of the variable info to remove.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could not be
    /// fulfilled.</param>
    /// <returns><c>true</c> if the variable info could be deleted without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryRemoveVariableInfo(string formalName, out ICollection<IConstraint> violatedConstraints);

    /// <summary>
    /// Gets a variable with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The variable with the specified name.</returns>
    IVariable GetVariable(string name);
    /// <summary>
    /// Adds the specified <paramref name="variable"/> to the current instance.
    /// </summary>
    /// <param name="variable">The variable to add.</param>
    void AddVariable(IVariable variable);
    /// <summary>
    /// Adds the specified <paramref name="variable"/> to the current instance if all constraints can
    /// be fulfilled.
    /// </summary>
    /// <param name="variable">The variable to add.</param>
    /// <returns><c>true</c> if the variable could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddVariable(IVariable variable);
    /// <summary>
    /// Adds the specified <paramref name="variable"/> to the current instance if all constraints can
    /// be fulfilled.
    /// </summary>
    /// <param name="variable">The variable to add.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could 
    /// not be fulfillled.</param>
    /// <returns><c>true</c> if the variable could be added without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryAddVariable(IVariable variable, out ICollection<IConstraint> violatedConstraints);
    /// <summary>
    /// Deletes the variable with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the variable to delete.</param>
    void RemoveVariable(string name);
    /// <summary>
    /// Deletes the variable with the specified <paramref name="name"/> if all constraints can be 
    /// fulfilled.
    /// </summary>
    /// <param name="name">The name of the variable to remove.</param>
    /// <returns><c>true</c> if the variable could be deleted without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryRemoveVariable(string name);
    /// <summary>
    /// Deletes the variable with the specified <paramref name="name"/> if all constraints can be 
    /// fulfilled.
    /// </summary>
    /// <param name="name">The name of the variable to remove.</param>
    /// <param name="violatedConstraints">Output parameter; contains all constraints that could 
    /// not be fulfilled.</param>
    /// <returns><c>true</c> if the variable could be deleted without violating constraints,
    /// <c>false</c> otherwise.</returns>
    bool TryRemoveVariable(string name, out ICollection<IConstraint> violatedConstraints);
    
    /// <inheritdoc cref="GetVariableValue(string, HeuristicLab.Core.IScope, bool)"/>
    /// <typeparam name="T">The type of the value that is searched.</typeparam>       
    T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup) where T : class, IItem;
    /// <inheritdoc cref="GetVariableValue(string, HeuristicLab.Core.IScope, bool, bool)"/>
    /// <typeparam name="T">The type of the value that is searched.</typeparam>
    T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup, bool throwOnError) where T : class, IItem;
    /// <inheritdoc cref="GetVariableValue(System.String, IScope, bool, bool)" 
    /// select="summary"/>
    /// <param name="formalName">The formal name of the variable info whose variable value is searched.</param>
    /// <param name="scope">The scope where to look for the variable.</param>
    /// <param name="recursiveLookup">Boolean value, whether also the parent scopes shall be searched if
    /// the variable is not found in the specified <paramref name="scope"/>.</param>
    /// <returns>The value of the searched variable or null if it is not found.</returns>
    IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup);
    /// <summary>
    /// Gets the value of the variable in the specified <paramref name="scope"/>
    /// whose variable(parameter) info has the specified <paramref name="formalName"/>.
    /// </summary>
    /// <param name="formalName">The formal name of the variable info whose variable value is searched.</param>
    /// <param name="scope">The scope where to look for the variable.</param>
    /// <param name="recursiveLookup">Boolean value, whether also the parent scopes shall be searched if
    /// the variable is not found in the specified <paramref name="scope"/>.</param>
    /// <param name="throwOnError">Boolean value, whether an exception shall be thrown, if the variable 
    /// cannot be found or just <c>null</c> shall be returned.</param>
    /// <returns>The value of the searched variable (or null if the variable is not 
    /// found and <paramref name="throwOnError"/> is set to false).</returns>
    IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup, bool throwOnError);

    /// <summary>
    /// Executes the current instance on the specified <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to execute the current instance.</param>
    /// <returns>The next operation.</returns>
    IOperation Execute(IScope scope);
    /// <summary>
    /// Aborts the current operator.
    /// </summary>
    void Abort();

    /// <summary>
    /// Occurs when the name of the operator was changed.
    /// </summary>
    event EventHandler NameChanged;
    /// <summary>
    /// Occurs when the breakpoint flag of the current instance was changed.
    /// </summary>
    event EventHandler BreakpointChanged; 
    /// <summary>
    /// Occurs when a sub operator has been added.
    /// </summary>
    event EventHandler<EventArgs<IOperator, int>> SubOperatorAdded;
    /// <summary>
    /// Occurs when a sub operator has been deleted.
    /// </summary>
    event EventHandler<EventArgs<IOperator, int>> SubOperatorRemoved;
    /// <summary>
    /// Occurs when a variable info has been added.
    /// </summary>
    event EventHandler<EventArgs<IVariableInfo>> VariableInfoAdded;
    /// <summary>
    /// Occurs when a variable info has been deleted.
    /// </summary>
    event EventHandler<EventArgs<IVariableInfo>> VariableInfoRemoved;
    /// <summary>
    /// Occurs when a variable has been added.
    /// </summary>
    event EventHandler<EventArgs<IVariable>> VariableAdded;
    /// <summary>
    /// Occurs when a variable has been deleted.
    /// </summary>
    event EventHandler<EventArgs<IVariable>> VariableRemoved;
    /// <summary>
    /// Occurs when the current instance is executed.
    /// </summary>
    event EventHandler Executed;
  }
}
