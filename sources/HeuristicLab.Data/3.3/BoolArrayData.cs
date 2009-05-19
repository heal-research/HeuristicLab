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

namespace HeuristicLab.Data {
  /// <summary>
  /// An array consisting of boolean values.
  /// </summary>
  [EmptyStorableClass]
  public class BoolArrayData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the boolean elements of the array.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase.Data"/> of base class <see cref="ArrayDataBase"/>. 
    /// No own data storage present.</remarks>
    public new bool[] Data {
      get { return (bool[])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoolArrayData"/> class.
    /// </summary>
    public BoolArrayData() {
      Data = new bool[0];
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolArrayData"/> class. 
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The boolean array the instance should represent.</param>
    public BoolArrayData(bool[] data) {
      Data = data;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BoolArrayDataView"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="BoolArrayDataView"/>.</returns>
    public override IView CreateView() {
      return new BoolArrayDataView(this);
    }
  }
}
