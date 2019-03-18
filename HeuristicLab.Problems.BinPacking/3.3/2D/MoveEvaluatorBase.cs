#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.BinPacking2D {
  [StorableClass]
  public abstract class MoveEvaluatorBase<TSol, TMove> : SingleSuccessorOperator,
    ISingleObjectiveMoveEvaluator, ISingleObjectiveMoveOperator, IOperator<TSol>
    where TSol : class, IItem
    where TMove : class, IItem {
    public ILookupParameter<TSol> EncodedSolutionParameter {
      get { return (ILookupParameter<TSol>)Parameters["EncodedSolution"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<ReadOnlyItemList<PackingItem>> ItemsParameter {
      get { return (LookupParameter<ReadOnlyItemList<PackingItem>>)Parameters["Items"]; }
    }
    public ILookupParameter<PackingShape> BinShapeParameter {
      get { return (LookupParameter<PackingShape>)Parameters["BinShape"]; }
    }
    public ILookupParameter<IDecoder<TSol>> DecoderParameter {
      get { return (ILookupParameter<IDecoder<TSol>>)Parameters["Decoder"]; }
    }
    public ILookupParameter<IEvaluator> SolutionEvaluatorParameter {
      get { return (ILookupParameter<IEvaluator>)Parameters["SolutionEvaluator"]; }
    }
    public ILookupParameter<TMove> MoveParameter {
      get { return (ILookupParameter<TMove>)Parameters["Move"]; }
    }


    [StorableConstructor]
    protected MoveEvaluatorBase(bool deserializing) : base(deserializing) { }
    protected MoveEvaluatorBase(MoveEvaluatorBase<TSol, TMove> original, Cloner cloner) : base(original, cloner) { }
    protected MoveEvaluatorBase()
      : base() {
      Parameters.Add(new LookupParameter<TSol>("EncodedSolution", "The encoded solution candidate to evaluate"));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a packing solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a packing solution."));
      Parameters.Add(new LookupParameter<ReadOnlyItemList<PackingItem>>("Items", "Packing-item data taken from the bin-packing problem-instance."));
      Parameters.Add(new LookupParameter<PackingShape>("BinShape", "Packing-bin data taken from the bin-packing problem-instance."));
      Parameters.Add(new LookupParameter<IDecoder<TSol>>("Decoder", "The decoding operator that is used to calculate a packing plan from the used representation."));
      Parameters.Add(new LookupParameter<IEvaluator>("SolutionEvaluator", "The actual packing plan evaluation operator."));
      Parameters.Add(new LookupParameter<TMove>("Move", "The move to evaluate."));
    }

    public override IOperation Apply() {
      var move = MoveParameter.ActualValue;
      var encSolCandidate = EncodedSolutionParameter.ActualValue;
      var binShape = BinShapeParameter.ActualValue;
      var items = ItemsParameter.ActualValue;


      double moveQuality = EvaluateMove(encSolCandidate, move, binShape, items);

      if (MoveQualityParameter.ActualValue == null)
        MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      else
        MoveQualityParameter.ActualValue.Value = moveQuality;
      return base.Apply();
    }

    public abstract double EvaluateMove(TSol encodedSolutionCandidate, TMove move, PackingShape binShape, ReadOnlyItemList<PackingItem> items);
  }
}
