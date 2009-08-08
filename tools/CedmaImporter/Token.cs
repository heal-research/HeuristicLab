using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Modeling.Database;
using HeuristicLab.Modeling.Database.SQLServerCompact;
using HeuristicLab.GP;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP.StructureIdentification;
using System.Diagnostics;

namespace CedmaImporter {
  public enum TokenSymbol { LPAR, RPAR, SYMB, NUMBER };
  public class Token {
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
      } else if (double.TryParse(strToken, out temp)) {
        t.Symbol = TokenSymbol.NUMBER;
        t.DoubleValue = double.Parse(strToken);
      } else {
        t.Symbol = TokenSymbol.SYMB;
      }
      return t;
    }
  }
}
