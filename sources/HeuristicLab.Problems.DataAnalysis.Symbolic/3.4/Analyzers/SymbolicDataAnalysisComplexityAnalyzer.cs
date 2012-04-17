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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Calculates the complexity of all trees in the population.
  /// </summary>
  [Item("SymbolicDataAnalysisComplexityAnalyzer", "Calculates the complexity of all trees in the population.")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisComplexityAnalyzer : SymbolicDataAnalysisAnalyzer {
    private const string ComplexityParameterName = "Complexity";
    private const string WeightsParameterName = "Weights";
    
    public override bool EnabledByDefault {
      get { return false; }
    }

    #region parameter properties
    public IScopeTreeLookupParameter<DoubleValue> ComplexityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[ComplexityParameterName]; }
    }
    public IValueParameter<ItemDictionary<StringValue, DoubleValue>> WeightsParameter {
      get { return (IValueParameter<ItemDictionary<StringValue, DoubleValue>>)Parameters[WeightsParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicDataAnalysisComplexityAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisComplexityAnalyzer(SymbolicDataAnalysisComplexityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisComplexityAnalyzer()
      : base() {
      var defaultWeights = new Dictionary<string, double>()
                             {
                                { "StartSymbol", 0.0},
                                { "ProgramRootSymbol", 0.0},
                                { "Addition", 1.0 },
                                { "AiryA", 10.0 },
                                { "AiryB", 10.0 },
                                { "And", 2.0 },
                                { "Average", 2.0 },
                                { "Bessel", 10.0 },
                                { "Constant", 1.0 },
                                { "Cosine", 5.0 },
                                { "CosineIntegral", 10.0 },
                                { "Dawson", 10.0 },
                                { "Derivative", 5.0 },
                                { "Division", 2.0 },
                                { "Erf", 10.0 },
                                { "Exponential", 5.0 },
                                { "ExponentialIntegralEi", 10.0 },
                                { "FresnelCosineIntegral", 10.0 },
                                { "FresnelSineIntegral", 10.0 },
                                { "Gamma", 10.0 },
                                { "GreaterThan", 2.0 },
                                { "HyperbolicCosineIntegral", 10.0 },
                                { "HyperbolicSineIntegral", 10.0 },
                                { "IfThenElse", 3.0 },
                                { "Integral", 5.0},
                                { "LaggedVariable", 2.0 },
                                { "LessThan", 2.0 },
                                { "Logarithm", 5.0 },
                                { "Multiplication", 2.0 },
                                { "Norm", 10.0 },
                                { "Not", 2.0 },
                                { "Or", 1.0 },
                                { "Power", 5.0 },
                                { "Psi", 10.0 },
                                { "Root", 5.0 },
                                { "Sine", 5.0 },
                                { "SineIntegral", 10.0 },
                                { "Square", 2.0 },
                                { "SquareRoot", 2.0 },
                                { "Subtraction", 1.0 },
                                { "Tangent", 3.0 },
                                { "TimeLag", 3.0 },
                                { "Variable", 1.0 },
                                { "Variable Condition", 5.0 },
                             };

      var defaultWeightsTable = new ItemDictionary<StringValue, DoubleValue>();
      foreach (var p in defaultWeights) {
        defaultWeightsTable.Add(new StringValue(p.Key), new DoubleValue(p.Value));
      }
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(ComplexityParameterName, "The complexity of the tree."));
      Parameters.Add(new ValueParameter<ItemDictionary<StringValue, DoubleValue>>(WeightsParameterName, "A table with complexity weights for each symbol.", defaultWeightsTable));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisComplexityAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<ISymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      var weightsTable = WeightsParameter.Value;
      var weights = new Dictionary<string, double>();
      foreach (var p in weightsTable)
        weights.Add(p.Key.Value, p.Value.Value);
      var complexities = from t in expressions
                         select CalculateComplexity(t, weights);
      ComplexityParameter.ActualValue = new ItemArray<DoubleValue>(complexities.Select(x => new DoubleValue(x)).ToArray());
      return base.Apply();
    }

    private double CalculateComplexity(ISymbolicExpressionTree t, Dictionary<string, double> weights) {
      double c = 0.0;
      foreach (var n in t.Root.IterateNodesPrefix()) {
        if (!weights.ContainsKey(n.Symbol.Name)) throw new ArgumentException("Weight for symbol " + n.Symbol.Name + " is not defined.");
        c += weights[n.Symbol.Name];
      }
      return c;
    }
  }
}
