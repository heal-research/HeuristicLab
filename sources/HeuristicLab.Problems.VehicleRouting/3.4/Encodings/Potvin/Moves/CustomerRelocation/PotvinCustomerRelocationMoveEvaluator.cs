#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Common;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationMoveEvaluator", "Evaluates a customer relocation move for a VRP representation. ")]
  [StorableClass]
  public sealed class PotvinCustomerRelocationMoveEvaluator : PotvinMoveEvaluator, IPotvinCustomerRelocationMoveOperator {
    public ILookupParameter<PotvinCustomerRelocationMove> CustomerRelocationMoveParameter {
      get { return (ILookupParameter<PotvinCustomerRelocationMove>)Parameters["PotvinCustomerRelocationMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return CustomerRelocationMoveParameter; }
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
    private PotvinCustomerRelocationMoveEvaluator(bool deserializing) : base(deserializing) { }

    public PotvinCustomerRelocationMoveEvaluator()
      : base() {
       Parameters.Add(new LookupParameter<PotvinCustomerRelocationMove>("PotvinCustomerRelocationMove", "The move that should be evaluated."));

       Parameters.Add(new LookupParameter<VariableCollection>("Memories", "The TS memory collection."));
       Parameters.Add(new ValueParameter<StringValue>("AdditionFrequencyMemoryKey", "The key that is used for the addition frequency in the TS memory.", new StringValue("AdditionFrequency")));
       Parameters.Add(new ValueParameter<DoubleValue>("Lambda", "The lambda parameter.", new DoubleValue(0.015)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationMoveEvaluator(this, cloner);
    }

    private PotvinCustomerRelocationMoveEvaluator(PotvinCustomerRelocationMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() {
      PotvinCustomerRelocationMove move = CustomerRelocationMoveParameter.ActualValue;

      PotvinEncoding newSolution = CustomerRelocationMoveParameter.ActualValue.Individual.Clone() as PotvinEncoding;
      PotvinCustomerRelocationMoveMaker.Apply(newSolution, move, ProblemInstance);

      UpdateEvaluation(newSolution);

      //Apply memory, only if move is worse
      if (MoveQualityParameter.ActualValue.Value >= QualityParameter.ActualValue.Value) {
        VariableCollection memory = MemoriesParameter.ActualValue;
        string key = AdditionFrequencyMemoryKeyParameter.Value.Value;

        if (memory != null && memory.ContainsKey(key)) {
          ItemDictionary<PotvinCustomerRelocationMoveAttribute, IntValue> additionFrequency =
               memory[key].Value as ItemDictionary<PotvinCustomerRelocationMoveAttribute, IntValue>;
          PotvinCustomerRelocationMoveAttribute attr = new PotvinCustomerRelocationMoveAttribute(0, move.Tour, move.City);
          if (additionFrequency.ContainsKey(attr)) {
            int frequency = additionFrequency[attr].Value;
            double quality = MoveQualityParameter.ActualValue.Value;

            MoveQualityParameter.ActualValue.Value +=
              LambdaParameter.Value.Value * quality * frequency;
          }
        }
      }
    } 
  }
}
