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
using System.Text.RegularExpressions;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  public static class ShapeConstraintsParser {
    public static ShapeConstraints ParseConstraints(string text) {
      if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

      var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

      var sc = new ShapeConstraints();
      foreach (var line in lines) {
        var trimmedLine = line.Trim();
        if (trimmedLine == "") continue; // If it is a comment just continue without saving anything
        if (trimmedLine.StartsWith("#")) {
          // disabled constraints are commented
          var constraint = ParseConstaint(trimmedLine.TrimStart('#', ' '));
          sc.Add(constraint);
          sc.SetItemCheckedState(constraint, false);
        } else {
          var constraint = ParseConstaint(trimmedLine);
          sc.Add(constraint);
          sc.SetItemCheckedState(constraint, true);
        }
      }
      return sc;
    }

    public static ShapeConstraint ParseConstaint(string expr) {
      var trimmedLine = expr.TrimStart();
      if (trimmedLine.StartsWith("f")) {
        return ParseFunctionRangeConstraint(expr);
      } else if (trimmedLine.StartsWith("d") || trimmedLine.StartsWith("∂")) {
        return ParseDerivationConstraint(expr);
      } else {
        throw new
          ArgumentException($"Error at in definition {expr}. Constraints have to start with (f | d | ∂ | #)",
                            nameof(expr));
      }
    }

    // [124 .. 145]
    private const string intervalRegex = @"\s*[\[]" +
                                         @"\s*(?<lowerBound>[^\s;]*)" +
                                         @"\s*(\.{2}|\s+|;)" +
                                         @"\s*(?<upperBound>[^\s\]]*)" +
                                         @"\s*[\]]";

    private const string variableRegex = @"(['](?<varName>.*)[']|(?<varName>[^\s²³]+))\s*";
    private const string weightRegex = @"\s*(weight:\s*(?<weight>\S*))?";
    public static ShapeConstraint ParseFunctionRangeConstraint(string expr) {
      if (!expr.StartsWith("f")) throw new ArgumentException($"Invalid function range constraint {expr} (e.g. f in [1..2])");
      var start = "f".Length;
      var end = expr.Length;
      var targetConstraint = expr.Substring(start, end - start);


      // # Example for a target variable constraint:
      // f in [0 .. 100]
      // # Example for constraints on model parameters: 
      // df/d'x' in [0 .. 10]
      // ∂²f/∂'x'² in [-1 .. inf.]
      // df/d'x' in [0 .. 10] weight: 2.0
      // df / d'x' in [0..10], 'x' in [1 .. 3]
      // df / d'x' in [0..10], 'x' in [1 .. 3], y in [10..30] weight: 1.2

      var match = Regex.Match(targetConstraint,
                    @"\s*\bin\b" +
                    intervalRegex +
                    @"(" +
                      @"\s*,\s*" + variableRegex +
                      @"\s *\bin\b" +
                      intervalRegex +
                    @")*" +
                    weightRegex
                    );


      if (match.Success) {
        // if (match.Groups.Count < 19) throw new ArgumentException("The target-constraint is not complete.", nameof(expr));

        var lowerBound = ParseIntervalBounds(match.Groups["lowerBound"].Captures[0].Value);
        var upperBound = ParseIntervalBounds(match.Groups["upperBound"].Captures[0].Value);
        var interval = new Interval(lowerBound, upperBound);
        var weight = 1.0;

        if (match.Groups["weight"].Success && !string.IsNullOrWhiteSpace(match.Groups["weight"].Value))
          weight = ParseAndValidateDouble(match.Groups["weight"].Value);

        if (match.Groups["varName"].Success) {
          IntervalCollection regions = new IntervalCollection();
          // option variables found
          for (int idx = 0; idx < match.Groups["varName"].Captures.Count; ++idx) {
            KeyValuePair<string, Interval> region = ParseRegion(
              variable: match.Groups["varName"].Captures[idx].Value,
              lb: match.Groups["lowerBound"].Captures[idx + 1].Value,
              ub: match.Groups["upperBound"].Captures[idx + 1].Value);
            if (regions.GetReadonlyDictionary().All(r => r.Key != region.Key))
              regions.AddInterval(region.Key, region.Value);
            else
              throw new ArgumentException($"The constraint {expr} has multiple regions of the same variable.");
          }
          return new ShapeConstraint(interval, regions, weight);
        } else
          return new ShapeConstraint(interval, weight);
      } else
        throw new ArgumentException($"The target constraint {expr} is not valid.");
    }
    public static ShapeConstraint ParseDerivationConstraint(string expr) {
      var match = Regex.Match(expr,
                                @"([d∂])" +
                                @"(?<numDerivations>[²³]?)\s*" +
                                @"f\s*" +
                                @"(\/)\s*" +
                                @"([d∂])\s*" +
                                variableRegex +
                                @"(?<numDerivations>[²³]?)\s*\bin\b\s*" +
                                intervalRegex +
                                @"(" +
                                  @"\s*,\s*" + variableRegex +
                                  @"\s *\bin\b" +
                                  intervalRegex +
                                  @")*" +
                                weightRegex
                                );

      if (match.Success) {
        // if (match.Groups.Count < 26)
        //   throw new ArgumentException("The given derivation-constraint is not complete.", nameof(expr));

        var derivationVariable = match.Groups["varName"].Captures[0].Value.Trim();

        var enumeratorNumDeriv = match.Groups["numDerivations"].Captures[0].Value.Trim();
        var denominatorNumDeriv = match.Groups["numDerivations"].Captures[1].Value.Trim();
        if (enumeratorNumDeriv != "" || denominatorNumDeriv != "") {
          if (enumeratorNumDeriv == "" || denominatorNumDeriv == "")
            throw new ArgumentException($"Number of derivation has to be written on both sides in {expr}.");
          if (enumeratorNumDeriv != denominatorNumDeriv)
            throw new ArgumentException($"Derivation number is not equal on both sides in {expr}.");
        }

        var lowerBound = ParseIntervalBounds(match.Groups["lowerBound"].Captures[0].Value);
        var upperBound = ParseIntervalBounds(match.Groups["upperBound"].Captures[0].Value);
        var variable = derivationVariable;
        var numberOfDerivation = ParseDerivationCount(enumeratorNumDeriv);
        var interval = new Interval(lowerBound, upperBound);
        var weight = 1.0;

        if (match.Groups["weight"].Success && !string.IsNullOrWhiteSpace(match.Groups["weight"].Value))
          weight = ParseAndValidateDouble(match.Groups["weight"].Value);

        if (match.Groups["varName"].Captures.Count > 1) {
          IntervalCollection regions = new IntervalCollection();
          // option variables found
          for (int idx = 0; idx < match.Groups["varName"].Captures.Count - 1; ++idx) {
            KeyValuePair<string, Interval> region = ParseRegion(
              variable: match.Groups["varName"].Captures[idx + 1].Value,
              lb: match.Groups["lowerBound"].Captures[idx + 1].Value,
              ub: match.Groups["upperBound"].Captures[idx + 1].Value);
            if (regions.GetReadonlyDictionary().All(r => r.Key != region.Key))
              regions.AddInterval(region.Key, region.Value);
            else
              throw new ArgumentException($"The constraint {expr} has multiple regions of the same variable.");
          }
          return new ShapeConstraint(variable, numberOfDerivation, interval, regions, weight);
        } else
          return new ShapeConstraint(variable, numberOfDerivation, interval, weight);
      } else
        throw new ArgumentException($"The derivation constraint {expr} is not valid.");
    }


    private static KeyValuePair<string, Interval> ParseRegion(string variable, string lb, string ub) {
      var regionLb = ParseIntervalBounds(lb);
      var regionUb = ParseIntervalBounds(ub);
      return new KeyValuePair<string, Interval>(variable, new Interval(regionLb, regionUb));
    }

    private static double ParseIntervalBounds(string input) {
      input = input.ToLower();
      switch (input) {
        case "+inf.":
        case "inf.":
          return double.PositiveInfinity;
        case "-inf.":
          return double.NegativeInfinity;
        default: {
            return ParseAndValidateDouble(input);
          }
      }
    }

    private static double ParseAndValidateDouble(string input) {
      var valid = double.TryParse(input, out var value);
      if (!valid) {
        throw new ArgumentException($"Invalid value {input} (valid value format: \"" + "+inf. | inf. | -inf. | " + FormatPatterns.GetDoubleFormatPattern() + "\")");
      }

      return value;
    }

    private static int ParseDerivationCount(string input) {
      switch (input) {
        case "":
          return 1;
        case "²":
          return 2;
        case "³":
          return 3;
        default:
          int.TryParse(input, out var value);
          return value;
      }
    }
  }
}