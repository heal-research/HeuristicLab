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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Dynamic {
  [Item("GenericAlgorithmTracker", "TODO")]
  [StorableType("F2080EAF-CF5B-4661-9D1C-DC932738365A")]
  public class GenericAlgorithmTracker<TSol, TState> 
    : ParameterizedNamedItem, IMultiObjectiveDynamicProblemTracker<TSol,TState>, ISingleObjectiveDynamicProblemTracker<TSol,TState> 
    where TSol: IItem 
    where TState: IDynamicProblemState<TState>, IProblem, new()
  {
    public const string AlgorithmParameterName = "Analyis Algorithm";
    public const string HideAlgorithmParameterName = "Hide Algorithm";
    public string ResultCollectionName => (Algorithm?.Name ?? "") + "_Results";
    public const string VersionResultName = "Version";

    public IValueParameter<IAlgorithm> AlgorithmParameter => (IValueParameter<IAlgorithm>)Parameters[AlgorithmParameterName];

    public IFixedValueParameter<BoolValue> HideAlgorithmParameter =>
      (IFixedValueParameter<BoolValue>)Parameters[HideAlgorithmParameterName];
    public IAlgorithm Algorithm => AlgorithmParameter.Value;
    public bool HideAlgorithm => HideAlgorithmParameter.Value.Value;

    #region Constructor & Cloning
    public GenericAlgorithmTracker() {
      Parameters.Add(new ValueParameter<IAlgorithm>(AlgorithmParameterName, ""));
      Parameters.Add(new FixedValueParameter<BoolValue>(HideAlgorithmParameterName, new BoolValue(false)));
      AlgorithmParameter.ValueChanged += OnAlgChanged;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AlgorithmParameter.ValueChanged += OnAlgChanged;
      if (!Parameters.ContainsKey(HideAlgorithmParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(HideAlgorithmParameterName, new BoolValue(false)));
    }

    private void OnAlgChanged(object sender, EventArgs e) {
      Algorithm.Problem = new TState();
    }

    protected GenericAlgorithmTracker(GenericAlgorithmTracker<TSol, TState> original, Cloner cloner) : base(original, cloner) { }

    protected GenericAlgorithmTracker(StorableConstructorFlag _) : base(_) { }

    public void OnEvaluation(TSol solution, double[] quality, long version, long time) { }

    public void OnEvaluation(TSol solution, double quality, long version, long time) {}

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GenericAlgorithmTracker<TSol, TState>(this, cloner);
    }
    #endregion

    public void OnEvaluation(TSol solution, object quality, long version, long time) { }

    public void OnEpochChange(TState data, long version, long time) {
      var alg = HideAlgorithm? (IAlgorithm)Algorithm.Clone(): Algorithm;
      ((TState)alg.Problem).MergeFrom((TState)data.Clone());
      alg.Prepare(HideAlgorithm);
      alg.Start();
      alg.Runs.Last().Name = $"Epoch {version} {Algorithm.Name}";
      alg.Runs.Last().Parameters[VersionResultName] = new IntValue((int)version);
      if(HideAlgorithm) Algorithm.Runs.Add(alg.Runs.Last());
    }

    public void OnAnalyze(ResultCollection results) {
      IResult t;
      RunCollection subResults;

      if (!results.TryGetValue(ResultCollectionName, out t))
        results.Add(new Result(ResultCollectionName, subResults = new RunCollection()));
      else
        subResults = (RunCollection)t.Value;

      subResults.AddRange(Algorithm.Runs);
      Algorithm.Runs.Clear();
    }

    public void Reset() {
      Algorithm?.Runs?.Clear();
    }
  }
}
