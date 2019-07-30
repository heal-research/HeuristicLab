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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("InversionMoveGenerator", "Base class for all inversion (2-opt) move generators.")]
  [StorableType("FB20D0B5-4A65-4718-9A92-90C421034BCF")]
  public abstract class InversionMoveGenerator : SingleSuccessorOperator, IPermutationInversionMoveOperator, IMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (LookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected InversionMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected InversionMoveGenerator(InversionMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public InversionMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation for which moves should be generated."));
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    public override IOperation Apply() {
      Permutation p = PermutationParameter.ActualValue;
      InversionMove[] moves = GenerateMoves(p);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(InversionMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract InversionMove[] GenerateMoves(Permutation permutation);
  }
}
