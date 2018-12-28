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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaLambdaInterchangeMoveGenerator", "Generates lambda interchange moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("4BE6784E-663A-4B06-9E8A-52502A1967CC")]
  public abstract class AlbaLambdaInterchangeMoveGenerator : AlbaMoveGenerator, IExhaustiveMoveGenerator, IAlbaLambdaInterchangeMoveOperator {
    #region IAlbaLambdaInterchangeMoveOperator Members

    public ILookupParameter<AlbaLambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<AlbaLambdaInterchangeMove>)Parameters["AlbaLambdaInterchangeMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return LambdaInterchangeMoveParameter; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IValueParameter<IntValue> LambdaParameter {
      get { return (IValueParameter<IntValue>)Parameters["Lambda"]; }
    }

    #endregion

    [StorableConstructor]
    protected AlbaLambdaInterchangeMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public AlbaLambdaInterchangeMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<AlbaLambdaInterchangeMove>("AlbaLambdaInterchangeMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
      Parameters.Add(new ValueParameter<IntValue>("Lambda", "The lambda value.", new IntValue(1)));
    }

    protected AlbaLambdaInterchangeMoveGenerator(AlbaLambdaInterchangeMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract AlbaLambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, IVRPProblemInstance problemInstance, int lambda);

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      AlbaEncoding individual = VRPToursParameter.ActualValue as AlbaEncoding;
      AlbaLambdaInterchangeMove[] moves = GenerateMoves(individual, ProblemInstanceParameter.ActualValue, LambdaParameter.Value.Value);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(LambdaInterchangeMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);

      return next;
    }
  }
}
