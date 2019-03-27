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

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HeuristicLab.Clients.OKB.RunCreation {
  public partial class Value {
    public virtual string GetValue() {
      return string.Empty;
    }

    public static Value Create(string name, string strValue, string type) {
      switch (type) {
        case "BoolValue": return new BoolValue() {
          Name = name,
          Value = bool.Parse(strValue)
        };
        case "IntValue": return new IntValue() {
          Name = name,
          Value = int.Parse(strValue, CultureInfo.CurrentCulture.NumberFormat)
        };
        case "LongValue": return new LongValue() {
          Name = name,
          Value = long.Parse(strValue, CultureInfo.CurrentCulture.NumberFormat)
        };
        case "FloatValue": return new FloatValue() {
          Name = name,
          Value = float.Parse(strValue, CultureInfo.CurrentCulture.NumberFormat)
        };
        case "DoubleValue": return new DoubleValue() {
          Name = name,
          Value = double.Parse(strValue, CultureInfo.CurrentCulture.NumberFormat)
        };
        case "PercentValue": return new PercentValue() {
          Name = name,
          Value = double.Parse(Regex.Match(strValue, "[0-9.,]+").Value, CultureInfo.CurrentCulture.NumberFormat) / 100.0
        };
        case "StringValue": return new StringValue() {
          Name = name,
          Value = strValue
        };
        case "TimeSpanValue": return new TimeSpanValue() {
          Name = name,
          Value = long.Parse(strValue, CultureInfo.CurrentCulture.NumberFormat)
        };
        default: throw new ArgumentException(string.Format("type {0} is unknown", type), "type");
      }
    }
  }
}
