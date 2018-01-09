#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for single objective symbolic data analysis analyzers.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisSingleObjectiveAnalyzer : SymbolicDataAnalysisAnalyzer, ISymbolicDataAnalysisSingleObjectiveAnalyzer {
    private const string QualityParameterName = "Quality";
    private const string MaximizationParameterName = "Maximization";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";

    #region parameter properties
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public ILookupParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    #endregion
    #region properties
    public ItemArray<DoubleValue> Quality {
      get { return QualityParameter.ActualValue; }
    }
    public BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectiveAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisSingleObjectiveAnalyzer(SymbolicDataAnalysisSingleObjectiveAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisSingleObjectiveAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The qualities of the trees that should be analyzed."));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The direction of optimization."));
      Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (Parameters.ContainsKey(ApplyLinearScalingParameterName) && !(Parameters[ApplyLinearScalingParameterName] is LookupParameter<BoolValue>))
        Parameters.Remove(ApplyLinearScalingParameterName);
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName))
        Parameters.Add(new LookupParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the individual should be linearly scaled before evaluating."));
    }
  }
}
