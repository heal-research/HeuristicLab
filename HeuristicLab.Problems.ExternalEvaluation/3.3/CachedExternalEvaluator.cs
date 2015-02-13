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

using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {

  [Item("CachedExternalEvaluationValuesCollector", "Creates a solution message, and communicates it via the driver to receive a quality message, also keeps a cache of previous evaluation results.")]
  [StorableClass]
  public class CachedExternalEvaluator : ExternalEvaluator {

    #region Parameters
    public ValueLookupParameter<EvaluationCache> CacheParameter {
      get { return (ValueLookupParameter<EvaluationCache>)Parameters["Cache"]; }
    }
    #endregion

    #region Parameter Values
    protected DoubleValue Quality {
      get { return QualityParameter.ActualValue; }
      set { QualityParameter.ActualValue = value; }
    }
    protected EvaluationCache Cache {
      get { return CacheParameter.ActualValue; }
      set { CacheParameter.ActualValue = value; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected CachedExternalEvaluator(bool deserializing) : base(deserializing) { }
    protected CachedExternalEvaluator(CachedExternalEvaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CachedExternalEvaluator(this, cloner);
    }
    public CachedExternalEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<EvaluationCache>("Cache", "Cache of previously evaluated solutions"));
    }
    #endregion

    public override IOperation Apply() {
      if (Cache == null)
        return base.Apply();

      if (Quality == null) Quality = new DoubleValue(0);
      Quality.Value = Cache.GetValue(BuildSolutionMessage(), m => EvaluateOnNextAvailableClient(m).Quality);
      if (Successor != null)
        return ExecutionContext.CreateOperation(Successor);
      else
        return null;
    }
  }
}
