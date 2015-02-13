#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Core {
  [StorableClass]
  public class ConstraintOperation {
    public static readonly ConstraintOperation Equal = new ConstraintOperation(0, "Equal");
    public static readonly ConstraintOperation NotEqual = new ConstraintOperation(1, "Not equal");
    public static readonly ConstraintOperation Less = new ConstraintOperation(2, "Less than");
    public static readonly ConstraintOperation LessOrEqual = new ConstraintOperation(3, "Less than or equal");
    public static readonly ConstraintOperation Greater = new ConstraintOperation(4, "Greater than");
    public static readonly ConstraintOperation GreaterOrEqual = new ConstraintOperation(5, "Greater than or equal");
    public static readonly ConstraintOperation IsTypeCompatible = new ConstraintOperation(6, "Is type compatible to");
    public static readonly ConstraintOperation IsTypeNotCompatible = new ConstraintOperation(7, "Is type not compatible to");

    [Storable]
    private int value;
    [Storable]
    private string name;

    [StorableConstructor]
    protected ConstraintOperation(bool deserializing) { }
    protected ConstraintOperation(int value, string name) {
      this.value = value;
      this.name = name;
    }

    public override string ToString() {
      return name;
    }

    public override bool Equals(object obj) {
      if (obj is ConstraintOperation)
        return this == (ConstraintOperation)obj;

      return false;
    }
    public override int GetHashCode() {
      return value;
    }
    public static bool operator ==(ConstraintOperation co1, ConstraintOperation co2) {
      if (object.ReferenceEquals(co1, co2))
        return true;

      if ((object)co1 == null || (object)co2 == null)
        return false;

      return co1.value == co2.value;
    }
    public static bool operator !=(ConstraintOperation co1, ConstraintOperation co2) {
      return !(co1 == co2);
    }
  }
}