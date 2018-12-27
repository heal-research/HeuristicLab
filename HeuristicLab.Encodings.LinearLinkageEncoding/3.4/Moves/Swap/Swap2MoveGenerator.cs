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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Swap2MoveGenerator", "Base class for all swap-2 move generators.")]
  [StorableClass]
  public abstract class Swap2MoveGenerator : SingleSuccessorOperator, ILinearLinkageSwap2MoveOperator, IMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (LookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }

    [StorableConstructor]
    protected Swap2MoveGenerator(bool deserializing) : base(deserializing) { }
    protected Swap2MoveGenerator(Swap2MoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public Swap2MoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The LLE solution for which moves should be generated."));
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The moves that should be generated in subscopes."));
    }

    public override IOperation Apply() {
      var lle = LLEParameter.ActualValue;
      var moves = GenerateMoves(lle);
      var moveScopes = new Scope[moves.Length];
      for (var i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(Swap2MoveParameter.ActualName, moves[i]));
      }
      ExecutionContext.Scope.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract Swap2Move[] GenerateMoves(LinearLinkage lle);
  }
}
