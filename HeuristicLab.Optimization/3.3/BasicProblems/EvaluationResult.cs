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

using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {

  //TODO maybe change EvaluationResult from class to struct
  //TODO implement indexer instead of get/set additional data
  [StorableType("3E4F5781-0DDB-4B7A-99E2-89C3F1028EB1")]
  public abstract class EvaluationResult : Item, IEvaluationResult {

    [Storable]
    private readonly Dictionary<string, object> data = new Dictionary<string, object>();

    protected EvaluationResult() : base() { }


    [StorableConstructor]
    protected EvaluationResult(StorableConstructorFlag _) : base(_) { }

    protected EvaluationResult(EvaluationResult original, Cloner cloner) : base(original, cloner) {
      //TODO clone data dictionary
    }


    public void SetAdditionalData(string identifier, object o) {
      data[identifier] = o;
    }
    public object GetAdditionalData(string identifier) {
      return data[identifier];
    }

  }

  [StorableType("1468E570-64D1-43A5-8B0A-B7821BFAE708")]
  public class SingleObjectiveEvaluationResult : EvaluationResult, ISingleObjectiveEvaluationResult {
    [Storable]
    public double Quality { get; private set; }

    public SingleObjectiveEvaluationResult(double quality) {
      Quality = quality;
    }

    [StorableConstructor]
    protected SingleObjectiveEvaluationResult(StorableConstructorFlag _) : base(_) { }

    public SingleObjectiveEvaluationResult(SingleObjectiveEvaluationResult original, Cloner cloner) : base(original, cloner) {
      Quality = original.Quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveEvaluationResult(this, cloner);
    }
  }

  [StorableType("6D2B628D-E3B0-4C0A-8A2E-61BC8F38439B")]
  public class MultiObjectiveEvaluationResult : EvaluationResult, IMultiObjectiveEvaluationResult {
    [Storable]
    public double[] Quality { get; private set; }

    public MultiObjectiveEvaluationResult(double[] quality) {
      Quality = quality;
    }

    [StorableConstructor]
    protected MultiObjectiveEvaluationResult(StorableConstructorFlag _) : base(_) { }

    public MultiObjectiveEvaluationResult(MultiObjectiveEvaluationResult original, Cloner cloner) : base(original, cloner) {
      Quality = original.Quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveEvaluationResult(this, cloner);
    }
  }
}
