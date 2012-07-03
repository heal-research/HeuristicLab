#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationMoveMaker", "Peforms the customer relocation move on a given VRP encoding and updates the quality.")]
  [StorableClass]
  public class PotvinCustomerRelocationMoveMaker : PotvinMoveMaker, IPotvinCustomerRelocationMoveOperator, IMoveMaker {
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

    [StorableConstructor]
    protected PotvinCustomerRelocationMoveMaker(bool deserializing) : base(deserializing) { }

    public PotvinCustomerRelocationMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinCustomerRelocationMove>("PotvinCustomerRelocationMove", "The moves that should be made."));

      Parameters.Add(new LookupParameter<VariableCollection>("Memories", "The TS memory collection."));
      Parameters.Add(new ValueParameter<StringValue>("AdditionFrequencyMemoryKey", "The key that is used for the addition frequency in the TS memory.", new StringValue("AdditionFrequency")));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationMoveMaker(this, cloner);
    }

    protected PotvinCustomerRelocationMoveMaker(PotvinCustomerRelocationMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(PotvinEncoding solution, PotvinCustomerRelocationMove move, IVRPProblemInstance problemInstance) {
      if (move.Tour >= solution.Tours.Count)
        solution.Tours.Add(new Tour());
      Tour tour = solution.Tours[move.Tour];

      Tour oldTour = solution.Tours.Find(t => t.Stops.Contains(move.City));
      oldTour.Stops.Remove(move.City);
      /*if (oldTour.Stops.Count == 0)
        solution.Tours.Remove(oldTour);*/

      int place = solution.FindBestInsertionPlace(tour, move.City);
      tour.Stops.Insert(place, move.City);

      solution.Repair();
    }

    protected override void PerformMove() {
      PotvinCustomerRelocationMove move = CustomerRelocationMoveParameter.ActualValue;

      PotvinEncoding newSolution = move.Individual.Clone() as PotvinEncoding;
      Apply(newSolution, move, ProblemInstance);
      newSolution.Repair();
      VRPToursParameter.ActualValue = newSolution;

      //reset move quality
      VRPEvaluation eval = ProblemInstance.Evaluate(newSolution);
      MoveQualityParameter.ActualValue.Value = eval.Quality;

      //update memory
      VariableCollection memory = MemoriesParameter.ActualValue;
      string key = AdditionFrequencyMemoryKeyParameter.Value.Value;

      if (memory != null) {
        if (!memory.ContainsKey(key)) {
          memory.Add(new Variable(key,
              new ItemDictionary<PotvinCustomerRelocationMoveAttribute, IntValue>()));
        }
        ItemDictionary<PotvinCustomerRelocationMoveAttribute, IntValue> additionFrequency =
          memory[key].Value as ItemDictionary<PotvinCustomerRelocationMoveAttribute, IntValue>;

        PotvinCustomerRelocationMoveAttribute attr = new PotvinCustomerRelocationMoveAttribute(0, move.Tour, move.City);
        if (!additionFrequency.ContainsKey(attr))
          additionFrequency[attr] = new IntValue(0);

        additionFrequency[attr].Value++;
      }
    }
  }
}
