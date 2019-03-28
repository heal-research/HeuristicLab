#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("Single-objective MoveGenerator", "Calls the GetNeighbors method of the problem definition to obtain the moves.")]
  [StorableType("CB37E7D8-EAC3-4061-9D39-20538CD1064D")]
  public class SingleObjectiveMoveGenerator<TSolution> : SingleSuccessorOperator, INeighborBasedOperator<TSolution>, IMultiMoveGenerator, IStochasticOperator, ISingleObjectiveMoveOperator
  where TSolution : class, ISolution {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public ILookupParameter<IEncoding<TSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<TSolution>>)Parameters["Encoding"]; }
    }

    public Func<TSolution, IRandom, IEnumerable<TSolution>> GetNeighborsFunc { get; set; }

    [StorableConstructor]
    protected SingleObjectiveMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveMoveGenerator(SingleObjectiveMoveGenerator<TSolution> original, Cloner cloner)
      : base(original, cloner) { }
    public SingleObjectiveMoveGenerator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to sample."));
      Parameters.Add(new LookupParameter<IEncoding<TSolution>>("Encoding", "An item that holds the problem's encoding."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveMoveGenerator<TSolution>(this, cloner);
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var sampleSize = SampleSizeParameter.ActualValue.Value;
      var encoding = EncodingParameter.ActualValue;
      var solution = ScopeUtil.GetSolution(ExecutionContext.Scope, encoding);
      var nbhood = GetNeighborsFunc(solution, random).Take(sampleSize).ToList();

      var moveScopes = new Scope[nbhood.Count];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString(CultureInfo.InvariantCulture.NumberFormat));
        ScopeUtil.CopySolutionToScope(moveScopes[i], encoding, nbhood[i]);
      }
      ExecutionContext.Scope.SubScopes.AddRange(moveScopes);

      return base.Apply();
    }
  }
}
