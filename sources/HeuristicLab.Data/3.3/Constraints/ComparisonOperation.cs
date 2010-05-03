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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Data {
  [StorableClass]
  public sealed class ComparisonOperation {
    public static readonly ComparisonOperation Equal = new ComparisonOperation(0, "Equal");
    public static readonly ComparisonOperation NotEqual = new ComparisonOperation(1, "Not equal");
    public static readonly ComparisonOperation Lesser = new ComparisonOperation(2, "Lesser");
    public static readonly ComparisonOperation LesserOrEqual = new ComparisonOperation(3, "Lesser or equal");
    public static readonly ComparisonOperation Greater = new ComparisonOperation(4, "Greater");
    public static readonly ComparisonOperation GreaterOrEqual = new ComparisonOperation(5, "Greater or equal");
    public static readonly ComparisonOperation IsTypeOf = new ComparisonOperation(6, "Is type of");
    public static readonly ComparisonOperation IsNotTypeOf = new ComparisonOperation(7, "Is not type of");

    [Storable]
    private int value;
    [Storable]
    private string name;
    [StorableConstructor]
    private ComparisonOperation() {
      value = -1;
      name = "empty";
    }
    private ComparisonOperation(int value, string name) {
      this.value = value;
      this.name = name;
    }

    public override string ToString() {
      return name;
    }

    public override bool Equals(object obj) {
      if (obj is ComparisonOperation)
        return this == (ComparisonOperation)obj;

      return false;
    }
    public override int GetHashCode() {
      return value;
    }
    public static bool operator ==(ComparisonOperation co1, ComparisonOperation co2) {
      return co1.value == co2.value;
    }
    public static bool operator !=(ComparisonOperation co1, ComparisonOperation co2) {
      return !(co1 == co2);
    }
  }
}