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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Class to inject local variables into the scope.
  /// </summary>
  public class VariableInjector : OperatorBase {

    private Dictionary<IVariable, IVariableInfo> variableVariableInfoTable;    
    private Dictionary<IVariableInfo, IVariable> variableInfoVariableTable;

    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableInjector"/>.
    /// </summary>
    public VariableInjector()
      : base() {
      variableVariableInfoTable = new Dictionary<IVariable, IVariableInfo>();
      variableInfoVariableTable = new Dictionary<IVariableInfo, IVariable>();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone performed with <see cref="Auxiliary.Clone"/> of helper class
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="VariableInjector"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      VariableInjector clone = new VariableInjector();
      clonedObjects.Add(Guid, clone);
      clone.Name = Name;
      foreach (IVariable variable in Variables)
        clone.AddVariable((IVariable)Auxiliary.Clone(variable, clonedObjects));
      return clone;
    }

    /// <summary>
    /// Adds the specified <paramref name="variable"/> to the current instance to get injected.
    /// </summary>
    /// <param name="variable">The variable to add.</param>
    /// <remarks>Calls private method <see cref="CreateVariableInfo"/> and <see cref="OperatorBase.AddVariable"/>
    /// of base class <see cref="OperatorBase"/>.</remarks>
    public override void AddVariable(IVariable variable) {
      base.AddVariable(variable);
      CreateVariableInfo(variable);
    }

    /// <summary>
    /// Removes a variable with the specified <paramref name="name"/> from the current injector.
    /// </summary>
    /// <remarks>Calls private method <see cref="DeleteVariableInfo"/> and <see cref="OperatorBase.RemoveVariable"/>
    /// of base class <see cref="OperatorBase"/>.</remarks>
    /// <param name="name">The name of the </param>
    public override void RemoveVariable(string name) {
      DeleteVariableInfo(name);
      base.RemoveVariable(name);
    }

    /// <summary>
    /// Adds the specified variables to the given <paramref name="scope"/> (and removes them first,
    /// if they already exist in the current scope).
    /// </summary>
    /// <param name="scope">The scope where to inject the variables.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      foreach (IVariable variable in Variables) {
        if (scope.GetVariable(variable.Name) != null)
          scope.RemoveVariable(variable.Name);
        scope.AddVariable((IVariable)variable.Clone());
      }
      return null;
    }

    [Storable]
    private KeyValuePair<Dictionary<IVariableInfo, IVariable>, Dictionary<IVariable, IVariableInfo>> VariableMappingPersistence {
      get {
        return new KeyValuePair<Dictionary<IVariableInfo, IVariable>, Dictionary<IVariable, IVariableInfo>>(
          variableInfoVariableTable, variableVariableInfoTable);
      }
      set {
        variableInfoVariableTable = value.Key;
        variableVariableInfoTable = value.Value;
        foreach (var pair in variableInfoVariableTable) {
          pair.Key.ActualNameChanged += new EventHandler(VariableInfo_ActualNameChanged);
          pair.Value.NameChanged += new EventHandler(Variable_NameChanged);          
        }
      }
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
  }
}
