#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Globalization;
using System.Text;

namespace HeuristicLab.Data {
  public static class FormatPatterns {
    public static string GetBoolFormatPattern() {
      StringBuilder sb = new StringBuilder();
      sb.Append(bool.TrueString).Append(" | ").Append(bool.FalseString);
      return sb.ToString();
    }
    public static string GetDoubleFormatPattern() {
      StringBuilder sb = new StringBuilder();
      NumberFormatInfo nf = CultureInfo.CurrentCulture.NumberFormat;
      sb.Append(nf.PositiveInfinitySymbol).Append(" | ");
      sb.Append(nf.NegativeInfinitySymbol).Append(" | ");
      sb.Append(nf.NaNSymbol).Append(" | ");
      sb.Append("[").Append(nf.PositiveSign).Append(" | ").Append(nf.NegativeSign).Append("]");
      sb.Append("digits[").Append(nf.NumberDecimalSeparator).Append("[digits]]");
      sb.Append("[(e | E)");
      sb.Append("[").Append(nf.PositiveSign).Append(" | ").Append(nf.NegativeSign).Append("]");
      sb.Append("digits]");
      return sb.ToString();
    }
    public static string GetIntFormatPattern() {
      StringBuilder sb = new StringBuilder();
      NumberFormatInfo nf = CultureInfo.CurrentCulture.NumberFormat;
      sb.Append("[").Append(nf.PositiveSign).Append(" | ").Append(nf.NegativeSign).Append("]");
      sb.Append("digits");
      return sb.ToString();
    }
    public static string GetTimeSpanFormatPattern() {
      return "[-](d | d.hh:mm[:ss[.ff]] | hh:mm[:ss[.ff]])";
    }
  }
}
