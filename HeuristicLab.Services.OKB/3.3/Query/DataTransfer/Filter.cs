#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Runtime.Serialization;

namespace HeuristicLab.Services.OKB.Query.DataTransfer {
  [DataContract]
  [KnownType(typeof(OrdinalComparisonDateTimeFilter))]
  [KnownType(typeof(OrdinalComparisonIntFilter))]
  [KnownType(typeof(OrdinalComparisonLongFilter))]
  [KnownType(typeof(StringComparisonFilter))]
  [KnownType(typeof(StringComparisonAvailableValuesFilter))]
  [KnownType(typeof(NameEqualityComparisonBoolFilter))]
  [KnownType(typeof(NameEqualityComparisonByteArrayFilter))]
  [KnownType(typeof(NameOrdinalComparisonFloatFilter))]
  [KnownType(typeof(NameOrdinalComparisonDoubleFilter))]
  [KnownType(typeof(NameOrdinalComparisonPercentFilter))]
  [KnownType(typeof(NameOrdinalComparisonIntFilter))]
  [KnownType(typeof(NameOrdinalComparisonLongFilter))]
  [KnownType(typeof(NameOrdinalComparisonTimeSpanFilter))]
  [KnownType(typeof(NameStringComparisonFilter))]
  [KnownType(typeof(NameStringComparisonAvailableValuesFilter))]
  [KnownType(typeof(CombinedFilter))]
  public abstract class Filter {
    [DataMember]
    public string FilterTypeName { get; protected set; }
    [DataMember]
    public string Label { get; protected set; }
  }
}
