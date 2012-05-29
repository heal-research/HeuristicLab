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

using System.Globalization;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic_34.Tests {
  internal enum TokenSymbol { LPAR, RPAR, SYMB, NUMBER };
  internal class Token {
    public static readonly Token LPAR = Token.Parse("(");
    public static readonly Token RPAR = Token.Parse(")");

    public TokenSymbol Symbol { get; set; }
    public string StringValue { get; set; }
    public double DoubleValue { get; set; }
    public Token() { }

    public override bool Equals(object obj) {
      Token other = (obj as Token);
      if (other == null) return false;
      if (other.Symbol != Symbol) return false;
      return other.StringValue == this.StringValue;
    }

    public override int GetHashCode() {
      return Symbol.GetHashCode() & StringValue.GetHashCode();
    }

    public static Token Parse(string strToken) {
      strToken = strToken.Trim();
      Token t = new Token();
      t.StringValue = strToken.Trim();
      double temp;
      if (strToken == "") {
        t = null;
      } else if (strToken == "(") {
        t.Symbol = TokenSymbol.LPAR;
      } else if (strToken == ")") {
        t.Symbol = TokenSymbol.RPAR;
      } else if (double.TryParse(strToken, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out temp)) {
        t.Symbol = TokenSymbol.NUMBER;
        t.DoubleValue = double.Parse(strToken, CultureInfo.InvariantCulture.NumberFormat);
      } else {
        t.Symbol = TokenSymbol.SYMB;
        t.StringValue = t.StringValue.ToUpper();
      }
      return t;
    }
  }
}
