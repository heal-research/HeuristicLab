#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// An operator that tracks the best symbolic expression trees
  /// </summary>
  [Item("BestSymbolicExpressionTreeAnalyzer", "An operator that tracks the best symbolic expression trees")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class BestSymbolicExpressionTreeAnalyzer : SingleSuccessorOperator, ISymbolicExpressionTreeAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ResultsParameterName = "Results";
    private const string QualityParameterName = "Quality";
    private const string BestTreeQualityParameterName = "BestTreeQuality";
    private const string MaximizationParameterName = "Maximization";

    #region Parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestTreeQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestTreeQualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    #endregion

    [StorableConstructor]
    private BestSymbolicExpressionTreeAnalyzer(bool deserializing) : base(deserializing) { }
    private BestSymbolicExpressionTreeAnalyzer(BestSymbolicExpressionTreeAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSymbolicExpressionTreeAnalyzer(this, cloner);
    }
    public BestSymbolicExpressionTreeAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose length should be calculated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The quality value of a tree."));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The order relation for qualities of trees."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestTreeQualityParameterName, "The quality of the best tree so far."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The results collection where the best symbolic expression tree should be stored."));

      SymbolicExpressionTreeParameter.Hidden = true;
      QualityParameter.Hidden = true;
      MaximizationParameter.Hidden = true;
      ResultsParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public override IOperation Apply() {
      var qualities = QualityParameter.ActualValue;
      var trees = SymbolicExpressionTreeParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      int bestIdx = 0;
      double bestQuality = qualities[bestIdx].Value;
      if (maximization) {
        for (int i = 1; i < qualities.Length; i++)
          if (qualities[i].Value > bestQuality) {
            bestIdx = i;
            bestQuality = qualities[i].Value;
          }
      } else {
        for (int i = 1; i < qualities.Length; i++)
          if (qualities[i].Value < bestQuality) {
            bestIdx = i;
            bestQuality = qualities[i].Value;
          }
      }
      var bestTree = trees[bestIdx];

      var res = ResultsParameter.ActualValue;
      if (res.ContainsKey("Best tree")) {
        var prevBestQuality = (DoubleValue)BestTreeQualityParameter.ActualValue;
        if ((maximization && prevBestQuality.Value < bestQuality) ||
            (!maximization && prevBestQuality.Value > bestQuality)) {
          res["Best tree"].Value = bestTree;
          prevBestQuality.Value = bestQuality;
        }
      } else {
        BestTreeQualityParameter.ActualValue = new DoubleValue(bestQuality);
        res.Add(new Result("Best tree", bestTree));
      }

      return base.Apply();
    }
  }
}
