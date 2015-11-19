#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {

  [StorableClass]
  public class Calculator : IDeepCloneable {

    #region Fields & Properties

    private List<string> tokens;

    [Storable]
    public string Formula {
      get { return tokens == null ? string.Empty : string.Join(" ", tokens); }
      set { tokens = Tokenize(value).ToList(); }
    }

    private static readonly Regex TokenRegex =
      new Regex(@"""(\\""|[^""])*""|'(\\'|[^'])*'|[^\s]+");

    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    protected Calculator(bool deserializing) { }
    public Calculator() { }
    protected Calculator(Calculator original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      if (original.tokens != null)
        tokens = original.tokens.ToList();
    }
    public IDeepCloneable Clone(Cloner cloner) {
      return new Calculator(this, cloner);
    }
    public object Clone() {
      return Clone(new Cloner());
    }
    #endregion

    private static IEnumerable<string> Tokenize(string s) {
      return TokenRegex.Matches(s).Cast<Match>().Select(m => m.Value);
    }

    public IItem GetValue(IDictionary<string, IItem> variables) {
      var stack = new Stack<object>();
      int i = 0;
      try {
        for (; i < tokens.Count; i++) {
          var token = tokens[i];
          double d;
          if (TryParse(token, out d)) {
            stack.Push(d);
          } else if (token.StartsWith("\"")) {
            stack.Push(GetVariableValue(variables, token.Substring(1, token.Length - 2).Replace("\\\"", "\"")));
          } else if (token.StartsWith("'")) {
            stack.Push(token.Substring(1, token.Length - 2).Replace("\\'", "'"));
          } else {
            Apply(token, stack, variables);
          }
        }
      } catch (Exception x) {
        throw new Exception(string.Format(
          "Calculation of '{1}'{0}failed at token #{2}: {3} {0}current stack is: {0}{4}", Environment.NewLine,
          Formula, i, TokenWithContext(tokens, i, 3),
          string.Join(Environment.NewLine, stack.Select(AsString))),
          x);
      }
      if (stack.Count != 1)
        throw new Exception(
          string.Format("Invalid final evaluation stack size {0} (should be 1) in formula '{1}'",
          stack.Count, Formula));
      var result = stack.Pop();
      if (result is string) return new StringValue((string)result);
      if (result is int) return new IntValue((int)result);
      if (result is double) return new DoubleValue((double)result);
      if (result is bool) return new BoolValue((bool)result);
      return null;
    }

    private string TokenWithContext(List<string> tokens, int i, int context) {
      var sb = new StringBuilder();
      if (i > context + 1)
        sb.Append("... ");
      int prefix = Math.Max(0, i - context);
      sb.Append(string.Join(" ", tokens.GetRange(prefix, i - prefix)));
      sb.Append("   ---> ").Append(tokens[i]).Append(" <---   ");
      int postfix = Math.Min(tokens.Count, i + 1 + context);
      sb.Append(string.Join(" ", tokens.GetRange(i + 1, postfix - (i + 1))));
      if (postfix < tokens.Count)
        sb.Append(" ...");
      return sb.ToString();
    }


    private static void Apply(string token, Stack<object> stack, IDictionary<string, IItem> variables) {
      switch (token) {
        case "null": stack.Push(null); break;
        case "true": stack.Push(true); break;
        case "false": stack.Push(false); break;

        case "drop": stack.Pop(); break;
        case "dup": stack.Push(stack.Peek()); break;
        case "swap":
          var top = stack.Pop();
          var next = stack.Pop();
          stack.Push(top);
          stack.Push(next);
          break;

        case "log": Apply(stack, x => Math.Log(Convert.ToDouble(x))); break;
        case "+": Apply(stack, (x, y) => Convert.ToDouble(x) + Convert.ToDouble(y)); break;
        case "-": Apply(stack, (x, y) => Convert.ToDouble(x) - Convert.ToDouble(y)); break;
        case "*": Apply(stack, (x, y) => Convert.ToDouble(x) * Convert.ToDouble(y)); break;
        case "/": Apply(stack, (x, y) => Convert.ToDouble(x) / Convert.ToDouble(y)); break;
        case "^": Apply(stack, (x, y) => Math.Pow(Convert.ToDouble(x), Convert.ToDouble(y))); break;
        case "<": Apply(stack, (x, y) => Convert.ToDouble(x) < Convert.ToDouble(y)); break;
        case ">": Apply(stack, (x, y) => Convert.ToDouble(x) > Convert.ToDouble(y)); break;

        case "toint": Apply(stack, x => Convert.ToInt32(x)); break;
        case "todouble": Apply(stack, x => Convert.ToDouble(x)); break;

        case "[]": Apply(stack, (a, i) => GetArrayValueAtIndex(a, Convert.ToInt32(i))); break;

        case "==": Apply(stack, (x, y) => Equal(x, y)); break;
        case "not": Apply(stack, x => !Convert.ToBoolean(x)); break;
        case "isnull": Apply(stack, x => x == null); break;
        case "if": Apply(stack, (then, else_, cond) => Convert.ToBoolean(cond) ? then : else_); break;

        case "ismatch": Apply(stack, (s, p) => new Regex(Convert.ToString(p)).IsMatch(Convert.ToString(s))); break;
        case "rename": Apply(stack, (s, p, r) => new Regex(Convert.ToString(p)).Replace(Convert.ToString(s), Convert.ToString(r))); break;

        default: stack.Push(GetVariableValue(variables, token)); break;
      }
    }

    #region Auxiliary Functions

    #region IItem value conversion
    private static object GetIntValue(IItem value) {
      var v = value as IntValue;
      if (v != null) return (double)v.Value;
      return null;
    }

    private static object GetDoubleValue(IItem value) {
      var v = value as DoubleValue;
      if (v != null) return v.Value;
      return null;
    }

    private static object GetBoolValue(IItem value) {
      var v = value as BoolValue;
      if (v != null) return v.Value;
      return null;
    }

    private static object GetArrayValue(IItem value) {
      if (value is IntArray || value is DoubleArray || value is BoolArray || value is StringArray)
        return value;
      return null;
    }

    private static object GetVariableValue(IDictionary<string, IItem> variables, string name) {
      if (variables.ContainsKey(name)) {
        var item = variables[name];
        return
          GetIntValue(item) ??
          GetDoubleValue(item) ??
          GetBoolValue(item) ??
          GetArrayValue(item) ??
          item.ToString();
      }
      return null;
    }

    private static object GetArrayValueAtIndex(object array, int index) {
      if (array is IntArray)
        return ((IntArray)array)[index];
      if (array is DoubleArray)
        return ((DoubleArray)array)[index];
      if (array is BoolArray)
        return ((BoolArray)array)[index];
      if (array is StringArray)
        return ((StringArray)array)[index];
      throw new NotSupportedException(string.Format("Type {0} is not a supported array type", array.GetType().Name));
    }
    #endregion

    #region variadic equality
    private static bool Equal(object a, object b) { return EqualIntegerNumber(a, b) || EqualFloatingNumber(a, b) || EqualBool(a, b) || EqualString(a, b) || a == b; }
    private static bool EqualIntegerNumber(object a, object b) { return a is int && b is int && (int)a == (int)b; }
    private static bool EqualFloatingNumber(object a, object b) { return a is double && b is double && (double)a == (double)b; }
    private static bool EqualBool(object a, object b) { return a is bool && b is bool && (bool)a == (bool)b; }
    private static bool EqualString(object a, object b) { return a is string && b is string && ((string)a).Equals((string)b); }
    #endregion

    #region stack calculation
    private static void Apply(Stack<object> stack, Func<object, object> func) {
      if (stack.Count < 1)
        throw new InvalidOperationException("Stack is empty");
      var a = stack.Pop();
      try {
        stack.Push(func(a));
      } catch (Exception) {
        stack.Push(a);
        throw;
      }
    }

    private static void Apply(Stack<object> stack, Func<object, object, object> func) {
      if (stack.Count < 2)
        throw new InvalidOperationException("Stack contains less than two elements");
      var b = stack.Pop();
      var a = stack.Pop();
      try {
        stack.Push(func(a, b));
      } catch (Exception) {
        stack.Push(b);
        stack.Push(a);
        throw;
      }
    }

    private static void Apply(Stack<object> stack, Func<object, object, object, object> func) {
      if (stack.Count < 3)
        throw new InvalidOperationException("Stack contains less than three elements");
      var c = stack.Pop();
      var b = stack.Pop();
      var a = stack.Pop();
      try {
        stack.Push(func(a, b, c));
      } catch (Exception) {
        stack.Push(a);
        stack.Push(b);
        stack.Push(c);
        throw;
      }
    }
    #endregion

    private static bool TryParse(string token, out double d) {
      return double.TryParse(token,
                             NumberStyles.AllowDecimalPoint |
                             NumberStyles.AllowExponent |
                             NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d);
    }

    private static string AsString(object o) {
      if (o == null) return "null";
      if (o is string) return string.Format("'{0}'", o);
      return o.ToString();
    }

    #endregion
  }
}
