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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  public static class ShapeConstraintsParser {

    private enum Symbol {
      Number, Text, F, In,
      Derivative, Power2, Power3,
      Frac, NegInf, PosInf,
      IntervalSep, Colon, Comment,
      OpenBracket, CloseBracket, Comma,
      Weight, Where, With, EOF
    }

    private class LexAnalyser {
      private string Text { get; set; }
      private int CurIndex { get; set; }
      private int CurRow { get; set; }
      private int CurCol { get; set; }
      public object CurValue { get; private set; }

      public LexAnalyser(string textToParse) {
        Text = textToParse
          .Replace("\r\n", "\n")
          .Replace("\r", "\n")
           //.Replace("\n", " ")
          .Replace("\t", " ")
          .Trim();
        CurIndex = 0;
      }

      public bool Next(Symbol symbol) {
        SkipWhitespace();
        switch (symbol) {
          case Symbol.In: return ReadToken("in");
          case Symbol.Frac: return ReadToken("/");
          case Symbol.Comment: return ReadToken("#");
          case Symbol.Comma: return ReadToken(",");
          case Symbol.IntervalSep: return ReadToken(new string[] { "..", "," });
          case Symbol.OpenBracket: return ReadToken("[");
          case Symbol.CloseBracket: return ReadToken("]");
          case Symbol.Colon: return ReadToken(":");
          case Symbol.F: return ReadToken("f", () => CurValue = "f");
          case Symbol.Derivative: return ReadToken(new string[] { "d", "∂" }, caseSensitiv: true);
          case Symbol.Power2: return ReadToken("²");
          case Symbol.Power3: return ReadToken("³");
          case Symbol.NegInf: return ReadToken("-inf.", () => CurValue = double.NegativeInfinity);
          case Symbol.PosInf: return ReadToken(new string[] { "inf.", "+inf." }, () => CurValue = double.PositiveInfinity);
          case Symbol.Where: return ReadToken("where");
          case Symbol.With: return ReadToken("with");
          case Symbol.Weight: return ReadToken("weight");
          case Symbol.Number: return ReadText(c => "1234567890.e-".Contains(c), str => CurValue = ParseNumber(str));
          case Symbol.Text: return ReadText(c => char.IsLetterOrDigit(c), str => CurValue = str);
          case Symbol.EOF: return CurIndex >= Text.Length;
          default: throw new NotSupportedException($"Symbol '{symbol}' is not supported!");
        }
      }

      public double ParseNumber(string str) => 
        double.Parse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

      public void ExpectNext(params Symbol[] symbols) {
        if (!symbols.Any(x => Next(x)))
          throw new ArgumentException($"Expected symbol(s) '{string.Join(",", symbols)}' at {CurRow}:{CurCol}");
      }

      private void SkipWhitespace() {
        while (CurIndex < Text.Length && (Text[CurIndex] == ' ' || Text[CurIndex] == '\n')) {
          if (Text[CurIndex] == '\n') {
            CurRow++;
            CurCol = 0;
          }
          CurIndex++;
        }
      }

      private bool ReadToken(string[] tokens, Action ifSucess = null, bool caseSensitiv = false) => tokens.Any(x => ReadToken(x, ifSucess, caseSensitiv));
      private bool ReadToken(string token, Action ifSucess = null, bool caseSensitiv = false) {
        token = token.ToLower();
        if (CurIndex < Text.Length && 
           (caseSensitiv ? Text.Substring(CurIndex, token.Length) : Text.Substring(CurIndex, token.Length).ToLower()) == token) {
          CurIndex += token.Length;
          CurCol += token.Length;
          ifSucess?.Invoke();
          return true;
        }
        return false;
      }

      private bool ReadText(Func<char, bool> func, Action<string> ifSuccess = null, bool caseSensitiv = false) {
        StringBuilder builder = new StringBuilder();
        while (CurIndex < Text.Length && func(caseSensitiv ? Text[CurIndex] : char.ToLowerInvariant(Text[CurIndex]))) {
          builder.Append(Text[CurIndex++]);
          CurCol++;
        }
        var str = builder.ToString();
        var res = string.IsNullOrEmpty(str);
        if (!res)
          ifSuccess?.Invoke(str);
        return !res;
      }
    }

    /*
      ShapeConstraintList = { ShapeConstraint } .
      ShapeConstraint = ['#'] Shape 'in' Interval [ 'where' RegionList ] [ 'with' Weight ] .
      Shape = Func | ('d' Func '/' 'd' Variable) | ('d²' Func '/' 'd' Variable '²') | ('d³' Func '/' 'd' Variable '³') .
      Func = 'f' | text .
      Interval = '[' (number | '-Inf.') ('..' | ',') (number | 'Inf.') ']' .
      Variable = text .
      RegionList = Region { ',' Region } .
      Region = Variable 'in' Interval .
      Weight = 'weight' ':' number .
    */

    // ShapeConstraintList = { ShapeConstraint } .
    public static ShapeConstraints ParseConstraints(string text) {
      if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

      var lex = new LexAnalyser(text);
      var sc = new ShapeConstraints();
      do {
        ParseShapeConstraint(lex, sc);
      } while (!lex.Next(Symbol.EOF));
      return sc;
    }

    // ShapeConstraint = ['#'] Shape 'in' Interval [ 'where' RegionList ] [ 'with' Weight ] .
    private static void ParseShapeConstraint(LexAnalyser lex, ShapeConstraints collection) {
      IntervalCollection regions = null;
      double weight = 1;
      bool enabled = !lex.Next(Symbol.Comment);
      (string func, string variable, int derivation) = ParseShape(lex);
      lex.ExpectNext(Symbol.In);
      var interval = ParseInterval(lex);
      if (lex.Next(Symbol.Where))
        regions = ParseRegionList(lex);
      if (lex.Next(Symbol.With))
        weight = ParseWeight(lex);

      ShapeConstraint constraint = regions == null ?
        new ShapeConstraint(variable, derivation, interval, weight) :
        new ShapeConstraint(variable, derivation, interval, regions, weight);
      collection.Add(constraint);
      collection.SetItemCheckedState(constraint, enabled);
    }

    // Shape = Func | ('d' Func '/' 'd' Variable) | ('d²' Func '/' 'd' Variable '²') | ('d³' Func '/' 'd' Variable '³') .
    private static (string, string, int) ParseShape(LexAnalyser lex) {
      int derivation = 0;
      string func = null;
      string variable = null;
      if (lex.Next(Symbol.Derivative)) {
        derivation = 1;
        if (lex.Next(Symbol.Power2))
          derivation = 2;
        else if (lex.Next(Symbol.Power3))
          derivation = 3;
        func = ParseFunc(lex);
        lex.ExpectNext(Symbol.Frac);
        lex.ExpectNext(Symbol.Derivative);
        variable = ParseVariable(lex);
        if (derivation == 2)
          lex.ExpectNext(Symbol.Power2);
        else if (derivation == 3)
          lex.ExpectNext(Symbol.Power3);
      } else {
        func = ParseFunc(lex);
      }
      return (func, variable, derivation);
    }

    // Func = 'f' | text .
    private static string ParseFunc(LexAnalyser lex) {
      lex.ExpectNext(Symbol.F, Symbol.Text);
      return lex.CurValue?.ToString();
    }

    // Interval = '[' (number | '-Inf.') ('..' | ',') (number | 'Inf.') ']' .
    private static Interval ParseInterval(LexAnalyser lex) {
      double lowerBound = 0;
      double upperBound = 0;
      lex.ExpectNext(Symbol.OpenBracket);
      // lower bound
      if (lex.Next(Symbol.NegInf) || lex.Next(Symbol.Number))
        lowerBound = (double)lex.CurValue;
      lex.ExpectNext(Symbol.IntervalSep);
      // upper bound
      if (lex.Next(Symbol.PosInf) || lex.Next(Symbol.Number))
        upperBound = (double)lex.CurValue;
      lex.ExpectNext(Symbol.CloseBracket);
      return new Interval(lowerBound, upperBound);
    }

    // Variable = text .
    private static string ParseVariable(LexAnalyser lex) {
      lex.ExpectNext(Symbol.Text);
      return (string)lex.CurValue;
    }

    // RegionList = Region { ',' Region } .
    private static IntervalCollection ParseRegionList(LexAnalyser lex) {
      var intervalCollection = new IntervalCollection();
      do {
        ParseRegion(lex, intervalCollection);
      } while (lex.Next(Symbol.Comma));
      return intervalCollection;
    }

    // Region = Variable 'in' Interval .
    private static void ParseRegion(LexAnalyser lex, IntervalCollection collection) {
      var variable = ParseVariable(lex);
      lex.ExpectNext(Symbol.In);
      var interval = ParseInterval(lex);
      collection.AddInterval(variable, interval);
    }

    // Weight = 'weight' ':' number .
    private static double ParseWeight(LexAnalyser lex) {
      lex.ExpectNext(Symbol.Weight);
      lex.ExpectNext(Symbol.Colon);
      lex.ExpectNext(Symbol.Number);
      return (double)lex.CurValue;
    }
  }
}