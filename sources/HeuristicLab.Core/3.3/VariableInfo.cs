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
  /// Class for storing meta-information about variables/parameters of an operator.
  /// </summary>
  public class VariableInfo : ItemBase, IVariableInfo {

    [Storable]
    private string myActualName;
    /// <summary>
    /// Gets or sets the actual name of the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="OnActualNameChanged"/> in the setter.</remarks>
    public string ActualName {
      get { return myActualName; }
      set {
        if (myActualName != value) {
          myActualName = value;
          OnActualNameChanged();
        }
      }
    }

    [Storable]
    private string myFormalName;
    /// <summary>
    /// Gets the formal name of the current instance.
    /// </summary>
    public string FormalName {
      get { return myFormalName; }
    }

    [Storable]
    private string myDescription;
    /// <summary>
    /// Gets the description of the current instance.
    /// </summary>
    public string Description {
      get { return myDescription; }
    }

    [Storable]
    private Type myDataType;
    /// <summary>
    /// Gets the data type of the parameter.
    /// </summary>
    public Type DataType {
      get { return myDataType; }
    }

    [Storable]
    private VariableKind myKind;
    /// <summary>
    /// Gets the kind of the parameter (input parameter, output parameter,...).
    /// </summary>
    public VariableKind Kind {
      get { return myKind; }
    }

    [Storable]
    private bool myLocal;
    /// <summary>
    /// Gets or sets a boolean value, whether the variable is a local one. 
    /// </summary>
    /// <remarks>Calls <see cref="OnLocalChanged"/> in the setter.</remarks>
    public bool Local {
      get { return myLocal; }
      set {
        if (myLocal != value) {
          myLocal = value;
          OnLocalChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableInfo"/> with actual and formal name "Anonymous",
    /// no description, a <c>null</c> data type and the <c>local</c> flag set to <c>false</c>. The type of 
    /// the variable is an input parameter.
    /// </summary>
    public VariableInfo() {
      myActualName = "Anonymous";
      myFormalName = "Anonymous";
      myDescription = "";
      myDataType = null;
      myKind = VariableKind.In;
      myLocal = false;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableInfo"/> with the given parameters.
    /// </summary>
    /// <remarks>Calls <see cref="VariableInfo()"/>.<br/>
    /// The formal name is assigned to the actual name, too.</remarks>
    /// <param name="formalName">The formal name of the current instance.</param>
    /// <param name="description">The description of the current instance.</param>
    /// <param name="dataType">The data type of the parameter.</param>
    /// <param name="kind">The type of the parameter.</param>
    public VariableInfo(string formalName, string description, Type dataType, VariableKind kind)
      : this() {
      myActualName = formalName;
      myFormalName = formalName;
      myDescription = description;
      myDataType = dataType;
      myKind = kind;
    }

    /// <summary>
    /// Creates a new instance of <see cref="VariableInfoView"/> to represent the current instance
    /// visually.
    /// </summary>
    /// <returns>The created view as <see cref="VariableInfoView"/>.</returns>
    public override IView CreateView() {
      return new VariableInfoView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="VariableInfo"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      VariableInfo clone = new VariableInfo();
      clonedObjects.Add(Guid, clone);
      clone.myActualName = ActualName;
      clone.myFormalName = FormalName;
      clone.myDescription = Description;
      clone.myDataType = DataType;
      clone.myKind = Kind;
      clone.myLocal = Local;
      return clone;
    }

    /// <inheritdoc/>
    public event EventHandler ActualNameChanged;
    /// <summary>
    /// Fires a new <c>ActualNameChanged</c> event.
    /// </summary>
    protected virtual void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
    }
    /// <inheritdoc />
    public event EventHandler LocalChanged;
    /// <summary>
    /// Fires a new <c>LocalChanged</c> event.
    /// </summary>
    protected virtual void OnLocalChanged() {
      if (LocalChanged != null)
        LocalChanged(this, new EventArgs());
    }
  }
}
