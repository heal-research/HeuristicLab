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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An operator that evaluates an expression.
  /// </summary>
  [Item("ExpressionCalculator", "An operator that evaluates an expression.")]
  [StorableClass]
  public class ExpressionCalculator : ValuesCollector {
    [Storable]
    public Calculator Calculator { get; set; }

    #region Parameter Properties
    public IValueLookupParameter<StringValue> ExpressionParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters["Expression"]; }
    }
    public IValueLookupParameter<IItem> ExpressionResultParameter {
      get { return (IValueLookupParameter<IItem>)Parameters["ExpressionResult"]; }
    }
    #endregion

    #region Properties
    private string Formula {
      get { return ExpressionParameter.Value.Value; }
    }
    private IItem ExpressionResult {
      set { ExpressionResultParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected ExpressionCalculator(bool deserializing) : base(deserializing) { }
    protected ExpressionCalculator(ExpressionCalculator original, Cloner cloner)
      : base(original, cloner) {
      this.Calculator = cloner.Clone(original.Calculator);
    }
    public ExpressionCalculator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IItem>("ExpressionResult", "The result of the evaluated expression."));
      Parameters.Add(new ValueLookupParameter<StringValue>("Expression",
@"RPN formula for new value in postfix notation.

This can contain the following elements:

literals:
  numbers, true, false, null and strings in single quotes
variables (run parameters or results):
  unquoted or in double quotes if they contain special characters or whitespace
mathematical functions:
  +, -, *, /, ^ (power), log
predicates:
  ==, <, >, isnull, not
stack manipulation:
  drop swap dup
string matching:
  <string> <pattern> ismatch
string replacing:
  <string> <pattern> <replacement> rename
conditionals:
  <then> <else> <condition> if

If the final value is null, the result variable is removed if it exists."));

      Calculator = new Calculator();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExpressionCalculator(this, cloner);
    }

    public override IOperation Apply() {
      Calculator.Formula = Formula;
      var variables = new Dictionary<string, IItem>();
      foreach (var collectedValue in CollectedValues)
        variables[collectedValue.Name] = collectedValue.ActualValue;
      ExpressionResult = Calculator.GetValue(variables);
      return base.Apply();
    }
  }
}