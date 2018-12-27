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
  [Item("ScrambleMoveGenerator", "Base class for all scramble move generators.")]
  [StorableClass]
  public abstract class ScrambleMoveGenerator : SingleSuccessorOperator, IPermutationScrambleMoveOperator, IMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<ScrambleMove> ScrambleMoveParameter {
      get { return (LookupParameter<ScrambleMove>)Parameters["ScrambleMove"]; }
    }

    [StorableConstructor]
    protected ScrambleMoveGenerator(bool deserializing) : base(deserializing) { }
    protected ScrambleMoveGenerator(ScrambleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ScrambleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation for which moves should be generated."));
      Parameters.Add(new LookupParameter<ScrambleMove>("ScrambleMove", "The moves that should be generated in subscopes."));
    }

    public override IOperation Apply() {
      IScope scope = ExecutionContext.Scope;
      Permutation p = PermutationParameter.ActualValue;
      ScrambleMove[] moves = GenerateMoves(p);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(ScrambleMoveParameter.ActualName, moves[i]));
      }
      scope.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract ScrambleMove[] GenerateMoves(Permutation permutation);
  }
}
