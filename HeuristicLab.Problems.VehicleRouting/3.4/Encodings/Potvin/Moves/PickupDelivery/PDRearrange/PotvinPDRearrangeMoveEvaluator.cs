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
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDRearrangeMoveEvaluator", "Evaluates a rearrange move for a PDP representation. ")]
  [StorableType("4B60C6C0-7D1E-49E8-B1FF-240A5A52CD62")]
  public sealed class PotvinPDRearrangeMoveEvaluator : PotvinMoveEvaluator, IPotvinPDRearrangeMoveOperator {
    public ILookupParameter<PotvinPDRearrangeMove> PDRearrangeMoveParameter {
      get { return (ILookupParameter<PotvinPDRearrangeMove>)Parameters["PotvinPDRearrangeMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return PDRearrangeMoveParameter; }
    }

    public ILookupParameter<VariableCollection> MemoriesParameter {
      get { return (ILookupParameter<VariableCollection>)Parameters["Memories"]; }
    }

    public IValueParameter<StringValue> AdditionFrequencyMemoryKeyParameter {
      get { return (IValueParameter<StringValue>)Parameters["AdditionFrequencyMemoryKey"]; }
    }

    public IValueParameter<DoubleValue> LambdaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Lambda"]; }
    }

    [StorableConstructor]
    private PotvinPDRearrangeMoveEvaluator(StorableConstructorFlag _) : base(_) { }

    public PotvinPDRearrangeMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinPDRearrangeMove>("PotvinPDRearrangeMove", "The move that should be evaluated."));

      Parameters.Add(new LookupParameter<VariableCollection>("Memories", "The TS memory collection."));
      Parameters.Add(new ValueParameter<StringValue>("AdditionFrequencyMemoryKey", "The key that is used for the addition frequency in the TS memory.", new StringValue("AdditionFrequency")));
      Parameters.Add(new ValueParameter<DoubleValue>("Lambda", "The lambda parameter.", new DoubleValue(0.015)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDRearrangeMoveEvaluator(this, cloner);
    }

    private PotvinPDRearrangeMoveEvaluator(PotvinPDRearrangeMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() {
      PotvinPDRearrangeMove move = PDRearrangeMoveParameter.ActualValue;

      PotvinEncoding newSolution = PDRearrangeMoveParameter.ActualValue.Individual.Clone() as PotvinEncoding;
      PotvinPDRearrangeMoveMaker.Apply(newSolution, move, ProblemInstance);

      UpdateEvaluation(newSolution);
    }
  }
}
