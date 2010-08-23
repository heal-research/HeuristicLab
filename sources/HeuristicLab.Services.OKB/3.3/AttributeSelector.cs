#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Runtime.Serialization;
using HeuristicLab.Services.OKB.AttributeSelection;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Data capsule for describing data base fields and
  /// restrictions. This is the basis for building complex queries.
  /// </summary>
  [DataContract]
  public class AttributeSelector : IAttributeSelector {

    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    /// <value>The name of the table.</value>
    [DataMember]
    public string TableName { get; set; }

    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    [DataMember]
    public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is hidden.
    /// </summary>
    /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
    [DataMember]
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets or sets the min value.
    /// </summary>
    /// <value>The min value.</value>
    [DataMember]
    public object MinValue { get; set; }

    /// <summary>
    /// Gets or sets the max value.
    /// </summary>
    /// <value>The max value.</value>
    [DataMember]
    public object MaxValue { get; set; }

    /// <summary>
    /// Gets or sets the allowed values.
    /// </summary>
    /// <value>The allowed values.</value>
    [DataMember]
    public ICollection<object> AllowedValues { get; set; }

    /// <summary>
    /// Gets or sets the name of the data type.
    /// </summary>
    /// <value>The name of the data type.</value>
    [DataMember]
    public string DataTypeName { get; set; }

    /// <summary>
    /// Gets an actual implementations linked to the database.
    /// </summary>
    /// <param name="okb">The okb data context.</param>
    /// <returns>A <see cref="RunAttributeSelector"/></returns>
    public RunAttributeSelector GetImpl(OKBDataContext okb) {
      return new RunAttributeSelector(okb, TableName, FieldName) {
        IsHidden = IsHidden,
        MinValue = MinValue,
        MaxValue = MaxValue,
        AllowedValues = AllowedValues,
      };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSelector"/> class.
    /// </summary>
    public AttributeSelector() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSelector"/> class.
    /// </summary>
    /// <param name="selector">The selector.</param>
    public AttributeSelector(RunAttributeSelector selector) {
      TableName = selector.TableName;
      FieldName = selector.FieldName;
      IsHidden = selector.IsHidden;
      MinValue = selector.MinValue;
      MaxValue = selector.MaxValue;
      AllowedValues = selector.AllowedValues;
      DataTypeName = selector.DataType.FullName;
    }
  }
}
