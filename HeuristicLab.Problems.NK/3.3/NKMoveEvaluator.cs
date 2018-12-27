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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.NK {

  [Item("NKMoveEvaluator", "A base class for operators which evaluate moves on the NK Landscape.")]
  [StorableClass]
  public abstract class NKMoveEvaluator : SingleSuccessorOperator, INKMoveEvaluator, IBinaryVectorMoveOperator {

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    public ILookupParameter<BoolMatrix> GeneInteractionsParameter {
      get { return (ILookupParameter<BoolMatrix>)Parameters["GeneInteractions"]; }
    }
    public ILookupParameter<IntValue> InteractionSeedParameter {
      get { return (ILookupParameter<IntValue>)Parameters["InteractionSeed"]; }
    }
    public ILookupParameter<DoubleArray> WeightsParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Weights"]; }
    }
    public ILookupParameter<IntValue> QParameter { get { return (ILookupParameter<IntValue>)Parameters["Q"]; } }
    public ILookupParameter<DoubleValue> PParameter { get { return (ILookupParameter<DoubleValue>)Parameters["P"]; } }

    [StorableConstructor]
    protected NKMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected NKMoveEvaluator(NKMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected NKMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a OneMax solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a OneMax solution."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The solution as BinaryVector."));
      Parameters.Add(new LookupParameter<BoolMatrix>("GeneInteractions", "Matrix indicating gene interactions."));
      Parameters.Add(new LookupParameter<IntValue>("InteractionSeed", "Seed used for randomly generting gene interactions."));
      Parameters.Add(new LookupParameter<DoubleArray>("Weights", "The weights for the component functions. If shorter, will be repeated."));
      Parameters.Add(new LookupParameter<IntValue>("Q", "Number of allowed fitness values in the (virutal) random table, or zero"));
      Parameters.Add(new LookupParameter<DoubleValue>("P", "Probability of any entry in the (virtual) random table being zero"));
    }
  }
}
