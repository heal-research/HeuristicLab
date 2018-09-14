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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("AdditiveMoveGenerator", "Base class for all additive move generators.")]
  [StorableClass]
  public abstract class AdditiveMoveGenerator : SingleSuccessorOperator, IAdditiveRealVectorMoveOperator, IMoveGenerator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public ILookupParameter<AdditiveMove> AdditiveMoveParameter {
      get { return (LookupParameter<AdditiveMove>)Parameters["AdditiveMove"]; }
    }
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected AdditiveMoveGenerator(bool deserializing) : base(deserializing) { }
    protected AdditiveMoveGenerator(AdditiveMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public AdditiveMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The real vector for which moves should be generated."));
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "A 2 column matrix specifying the lower and upper bound for each dimension. If there are less rows than dimension the bounds vector is cycled."));
    }

    public override IOperation Apply() {
      RealVector vector = RealVectorParameter.ActualValue;
      DoubleMatrix bounds = BoundsParameter.ActualValue;
      AdditiveMove[] moves = GenerateMoves(RandomParameter.ActualValue, vector, bounds);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(AdditiveMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);
      return base.Apply();
    }

    protected abstract AdditiveMove[] GenerateMoves(IRandom random, RealVector realVector, DoubleMatrix bounds);
  }
}
