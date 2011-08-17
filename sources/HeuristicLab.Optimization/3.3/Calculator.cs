using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Globalization;

namespace HeuristicLab.Optimization {  
    
  [StorableClass]
  public class Calculator : IDeepCloneable {

    #region Fields & Properties
    
    private List<string> tokens;

    [Storable]
    public string Formula {
      get { return string.Join(" ", tokens); }
      set { tokens = Tokenize(value).ToList(); }
    }

    private static readonly Regex TokenRegex = new Regex(@"[a-zA-Z0-9._]+|""([^""]|\"")+""|[-+*/^]|log");    

    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    protected Calculator(bool deserializing) { }
    public Calculator() { }
    public Calculator(Calculator original, Cloner cloner) {
      this.tokens = original.tokens.ToList();
    }
    public IDeepCloneable Clone(Cloner cloner) {
      return new Calculator(this, cloner);
    }
    public object Clone() {
      return Clone(new Cloner());
    }
    #endregion

    public IEnumerable<string> Tokenize(string s) {
      return TokenRegex.Matches(s).Cast<Match>().Select(m => m.Value);
    }

    private double GetVariableValue(IDictionary<string, IItem> variables, string name) {
      if (variables.ContainsKey(name)) {
        var item = variables[name];
        var intValue = item as IntValue;
        if (intValue != null) {
          return intValue.Value;
        } else {
          var doubleValue = item as DoubleValue;
          if (doubleValue != null)
            return doubleValue.Value;
          else
            throw new InvalidOperationException("Non numerical argument");
        }
      } else {
        throw new InvalidOperationException(string.Format("variable \"{0}\" not found", name));
      }
    }

    public IItem GetValue(IDictionary<string, IItem> variables) {
      var stack = new Stack<double>();
      Action<Func<double, double, double>> binf = op => {
        var b = stack.Pop();
        stack.Push(op(stack.Pop(), b));
      };
      try {
        foreach (var token in tokens) {
          double d;
          if (double.TryParse(token,
                NumberStyles.AllowDecimalPoint |
              NumberStyles.AllowExponent |
              NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d)) {
            stack.Push(d);
          } else if (token.StartsWith("\"")) {
            stack.Push(GetVariableValue(variables, token.Substring(1, token.Length - 2).Replace(@"\""", @"""")));
          } else {
            switch (token) {
              case "log": stack.Push(Math.Log(stack.Pop())); break;
              case "+": binf((x, y) => x + y); break;
              case "-": binf((x, y) => x - y); break;
              case "*": binf((x, y) => x * y); break;
              case "/": binf((x, y) => x / y); break;
              case "^": binf(Math.Pow); break;
              default: stack.Push(GetVariableValue(variables, token)); break;
            }
          }
        }
      } catch (InvalidOperationException x) {
        return new StringValue(string.Format("Calculation Failed: {0}", x.Message));
      }
      if (stack.Count != 1)
        return new StringValue("Invalid final evaluation stack size != 1");
      return new DoubleValue(stack.Pop());      
    }    
  }
}
