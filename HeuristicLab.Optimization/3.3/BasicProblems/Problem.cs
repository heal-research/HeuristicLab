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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class Problem<TSolution, TEvaluator> : HeuristicOptimizationProblem<TEvaluator, ISolutionCreator<TSolution>>, IProblemDefinition, IStorableContent
    where TSolution : class, ISolution
    where TEvaluator : class, IEvaluator {

    public string Filename { get; set; }

    protected IValueParameter<IEncoding<TSolution>> EncodingParameter {
      get { return (IValueParameter<IEncoding<TSolution>>)Parameters["Encoding"]; }
    }

    public IEncoding<TSolution> Encoding {
      get { return EncodingParameter.Value; }
      protected set {
        if (value == null) throw new ArgumentNullException("Encoding must not be null.");
        EncodingParameter.Value = value;
      }
    }

    protected override IEnumerable<IItem> GetOperators() {
      if (Encoding == null) return base.GetOperators();
      return base.GetOperators().Concat(Encoding.Operators);
    }
    public override IEnumerable<IParameterizedItem> ExecutionContextItems {
      get {
        if (Encoding == null) return base.ExecutionContextItems;
        return base.ExecutionContextItems.Concat(new[] { Encoding });
      }
    }

    protected Problem()
      : base() {
      Parameters.Add(new ValueParameter<IEncoding<TSolution>>("Encoding", "Describes the configuration of the encoding, what the variables are called, what type they are and their bounds if any."));
      if (Encoding != null) Parameterize();
      RegisterEvents();
    }

    protected Problem(Problem<TSolution, TEvaluator> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }

    [StorableConstructor]
    protected Problem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      EncodingParameter.ValueChanged += (o, e) => OnEncodingChanged();
      //var multiEncoding = Encoding as MultiEncoding;
      //if (multiEncoding != null) multiEncoding.EncodingsChanged += MultiEncodingOnEncodingsChanged;
    }

    protected virtual void OnEncodingChanged() {
      Parameterize();

      OnOperatorsChanged();
      OnReset();
    }

    private void Parameterize() {
      foreach (var op in Operators.OfType<IEncodingOperator<TSolution>>())
        op.EncodingParameter.ActualName = EncodingParameter.Name;

      //var multiEncoding = Encoding as MultiEncoding;
      //if (multiEncoding != null) multiEncoding.EncodingsChanged += MultiEncodingOnEncodingsChanged;
    }

    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      Encoding.SolutionCreator = SolutionCreator;
    }

    protected virtual void MultiEncodingOnEncodingsChanged(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
  }
}
