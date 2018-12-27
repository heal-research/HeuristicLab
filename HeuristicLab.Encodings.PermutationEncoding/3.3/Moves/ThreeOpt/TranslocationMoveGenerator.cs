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

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("TranslocationMoveGenerator", "Base class for move generators that produce translocation moves (3-opt).")]
  [StorableClass]
  public abstract class TranslocationMoveGenerator : SingleSuccessorOperator, IPermutationTranslocationMoveOperator, IMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (LookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected TranslocationMoveGenerator(bool deserializing) : base(deserializing) { }
    protected TranslocationMoveGenerator(TranslocationMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public TranslocationMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation for which moves should be generated."));
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    public override IOperation Apply() {
      Permutation p = PermutationParameter.ActualValue;
      TranslocationMove[] moves = GenerateMoves(p);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(TranslocationMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract TranslocationMove[] GenerateMoves(Permutation permutation);
  }
}
