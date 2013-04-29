#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Services.OKB.RunCreation.DataTransfer {
  [DataContract]
  [KnownType(typeof(BoolValue))]
  [KnownType(typeof(IntValue))]
  [KnownType(typeof(LongValue))]
  [KnownType(typeof(FloatValue))]
  [KnownType(typeof(DoubleValue))]
  [KnownType(typeof(StringValue))]
  [KnownType(typeof(BinaryValue))]
  [KnownType(typeof(PercentValue))]
  [KnownType(typeof(TimeSpanValue))]
  public abstract class Value {
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public DataType DataType { get; set; }
  }
}
