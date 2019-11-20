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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("72638F28-11DD-440D-B7A2-79E16E0EFB83")]
  public abstract class SolutionContext : Item {

    [Storable]
    private readonly Dictionary<string, object> data = new Dictionary<string, object>();

    [Storable]
    public IEncodedSolution EncodedSolution { get; private set; }

    [Storable]
    public IEvaluationResult EvaluationResult { get; protected set; }

    public bool IsEvaluated => EvaluationResult != null;

    protected SolutionContext(IEncodedSolution encodedSolution) : base() {
      EncodedSolution = encodedSolution ?? throw new ArgumentNullException("encodedSolution");
    }

    protected SolutionContext(IEncodedSolution encodedSolution, IEvaluationResult evaluationResult) : this(encodedSolution) {
      EvaluationResult = evaluationResult ?? throw new ArgumentNullException("evaluationResult");
    }

    [StorableConstructor]
    protected SolutionContext(StorableConstructorFlag _) : base(_) { }

    public SolutionContext(SolutionContext original, Cloner cloner) : base(original, cloner) {
      //TODO clone data dictionary
      EncodedSolution = cloner.Clone(original.EncodedSolution);
      EvaluationResult = cloner.Clone(original.EvaluationResult);
    }

    public void SetAdditionalData(string identifier, object o) {
      data[identifier] = o;
    }
    public object GetAdditionalData(string identifier) {
      return data[identifier];
    }
  }

  [StorableType("DF6DA9C9-7EF4-4DC3-9855-6C43BDEDD735")]
  public class SingleObjectiveSolutionContext : SolutionContext, ISingleObjectiveSolutionContext {
    public new ISingleObjectiveEvaluationResult EvaluationResult { get; set; }

    public SingleObjectiveSolutionContext(IEncodedSolution encodedSolution) : base(encodedSolution) { }


    public SingleObjectiveSolutionContext(IEncodedSolution encodedSolution, IEvaluationResult evaluationResult) : base(encodedSolution, evaluationResult) { }

    [StorableConstructor]
    public SingleObjectiveSolutionContext(StorableConstructorFlag _) : base(_) {    }

    public SingleObjectiveSolutionContext(SingleObjectiveSolutionContext original, Cloner cloner) : base(original, cloner) { }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveSolutionContext(this, cloner);
    }
  }

  [StorableType("929868B3-8994-4D75-B363-CCF9C51410F9")]
  public class MultiObjectiveSolutionContext : SolutionContext, IMultiObjectiveSolutionContext {
    public new IMultiObjectiveEvaluationResult EvaluationResult { get; set; }

    public MultiObjectiveSolutionContext(IEncodedSolution encodedSolution) : base(encodedSolution) { }


    public MultiObjectiveSolutionContext(IEncodedSolution encodedSolution, IEvaluationResult evaluationResult) : base(encodedSolution, evaluationResult) { }

    [StorableConstructor]
    public MultiObjectiveSolutionContext(StorableConstructorFlag _) : base(_) { }

    public MultiObjectiveSolutionContext(MultiObjectiveSolutionContext original, Cloner cloner) : base(original, cloner) { }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveSolutionContext(this, cloner);
    }
  }
}
