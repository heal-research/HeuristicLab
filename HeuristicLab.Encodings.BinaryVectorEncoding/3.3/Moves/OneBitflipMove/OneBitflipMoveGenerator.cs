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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("OneBitflipMoveGenerator", "Base class for all one bitflip move generators.")]
  [StorableType("600CE405-5ADC-471A-966B-27965190AF73")]
  public abstract class OneBitflipMoveGenerator : SingleSuccessorOperator, IOneBitflipMoveOperator, IMoveGenerator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (LookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected OneBitflipMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected OneBitflipMoveGenerator(OneBitflipMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public OneBitflipMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The BinaryVector for which moves should be generated."));
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    public override IOperation Apply() {
      BinaryVector v = BinaryVectorParameter.ActualValue;
      OneBitflipMove[] moves = GenerateMoves(v);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(OneBitflipMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract OneBitflipMove[] GenerateMoves(BinaryVector binaryVector);
  }
}
