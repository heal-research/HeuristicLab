#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Data {
  [Item("PercentArray", "Represents an array of double values in percent.")]
  [StorableType("8ED7F07C-1DE7-4782-90C0-50395627F8A0")]
  public class PercentArray : DoubleArray {
    [StorableConstructor]
    protected PercentArray(StorableConstructorFlag _) : base(_) { }
    protected PercentArray(PercentArray original, Cloner cloner)
      : base(original, cloner) {
    }
    public PercentArray() : base() { }
    public PercentArray(int length) : base(length) { }
    public PercentArray(double[] elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PercentArray(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (array.Length > 0) {
        sb.Append(array[0].ToString("#0.#################### %"));  // percent format
        for (int i = 1; i < array.Length; i++)
          sb.Append(";").Append(array[i].ToString("#0.#################### %"));  // percent format
      }
      sb.Append("]");
      return sb.ToString();
    }

    protected override bool Validate(string value, out string errorMessage) {
      value = value.Replace("%", " ");
      return base.Validate(value, out errorMessage);
    }
    protected override string GetValue(int index) {
      return this[index].ToString("#0.#################### %");  // percent format
    }
    protected override bool SetValue(string value, int index) {
      bool percent = value.Contains("%");
      value = value.Replace("%", " ");
      double val;
      if (double.TryParse(value, out val)) {
        if (percent) {
          if (!(val).IsAlmost(this[index] * 100.0))
            this[index] = val == 0 ? 0 : val / 100.0;
        } else {
          this[index] = val;
        }
        return true;
      } else {
        return false;
      }
    }
  }
}
