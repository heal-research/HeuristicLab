#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;
using HeuristicLab.Parameters;
using System.Collections.Generic;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("LambdaInterchangeMoveGenerator", "Generates lambda interchange moves from a given Alba VRP encoding.")]
  [StorableClass]
  public abstract class LambdaInterchangeMoveGenerator : AlbaMoveOperator, IExhaustiveMoveGenerator, IAlbaLambdaInterchangeMoveOperator {
    #region IAlbaLambdaInterchangeMoveOperator Members

    public ILookupParameter<LambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<LambdaInterchangeMove>)Parameters["LambdaInterchangeMove"]; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public IValueParameter<IntValue> LambdaParameter {
      get { return (IValueParameter<IntValue>)Parameters["Lambda"]; }
    }

    #endregion

    [StorableConstructor]
    protected LambdaInterchangeMoveGenerator(bool deserializing) : base(deserializing) { }

    public LambdaInterchangeMoveGenerator()
      : base() {
        Parameters.Add(new LookupParameter<LambdaInterchangeMove>("LambdaInterchangeMove", "The moves that should be generated in subscopes."));
        Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
        Parameters.Add(new ValueParameter<IntValue>("Lambda", "The lambda value.", new IntValue(1)));
    }

    protected abstract LambdaInterchangeMove[] GenerateMoves(AlbaEncoding individual, int lambda);

    public override IOperation Apply() {
      IOperation next = base.Apply();

      AlbaEncoding individual = VRPToursParameter.ActualValue as AlbaEncoding;
      LambdaInterchangeMove[] moves = GenerateMoves(individual, LambdaParameter.Value.Value);
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
