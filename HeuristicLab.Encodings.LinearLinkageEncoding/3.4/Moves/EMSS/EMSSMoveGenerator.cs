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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("EMSSMoveGenerator", "Base class for all EMSS move generators (extract, merge, shift, and split).")]
  [StorableClass]
  public abstract class EMSSMoveGenerator : SingleSuccessorOperator, ILinearLinkageEMSSMoveOperator, IMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }
    public ILookupParameter<EMSSMove> EMSSMoveParameter {
      get { return (LookupParameter<EMSSMove>)Parameters["EMSSMove"]; }
    }

    [StorableConstructor]
    protected EMSSMoveGenerator(bool deserializing) : base(deserializing) { }
    protected EMSSMoveGenerator(EMSSMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public EMSSMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The LLE solution for which moves should be generated."));
      Parameters.Add(new LookupParameter<EMSSMove>("EMSSMove", "The moves that should be generated in subscopes."));
    }

    public override IOperation Apply() {
      var lle = LLEParameter.ActualValue;
      var moves = GenerateMoves(lle);
      var moveScopes = new Scope[moves.Length];
      for (var i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(EMSSMoveParameter.ActualName, moves[i]));
      }
      ExecutionContext.Scope.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract EMSSMove[] GenerateMoves(LinearLinkage lle);
  }
}
