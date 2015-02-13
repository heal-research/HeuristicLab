#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("MultiVRPMoveTabuChecker", "Checks if a VRP move is tabu.")]
  [StorableClass]
  public class MultiVRPMoveTabuChecker : SingleSuccessorOperator, IMultiVRPMoveOperator, ITabuChecker, IGeneralVRPOperator {
    public ILookupParameter VRPMoveParameter {
      get { return (ILookupParameter)Parameters["VRPMove"]; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public ILookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (ILookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ILookupParameter<BoolValue> MoveTabuParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["MoveTabu"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ValueParameter<BoolValue> UseAspirationCriterionParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseAspirationCriterion"]; }
    }

    public BoolValue UseAspirationCriterion {
      get { return UseAspirationCriterionParameter.Value; }
      set { UseAspirationCriterionParameter.Value = value; }
    }

    [StorableConstructor]
    protected MultiVRPMoveTabuChecker(bool deserializing) : base(deserializing) { }

    public MultiVRPMoveTabuChecker()
      : base() {
      Parameters.Add(new LookupParameter<IVRPMove>("VRPMove", "The move."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours considered in the move."));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));

      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPMoveTabuChecker(this, cloner);
    }

    protected MultiVRPMoveTabuChecker(MultiVRPMoveTabuChecker original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      IVRPMove move = VRPMoveParameter.ActualValue as IVRPMove;

      ITabuChecker tabuChecker;
      if (UseAspirationCriterion.Value)
        tabuChecker = move.GetSoftTabuChecker();
      else
        tabuChecker = move.GetTabuChecker();

      (tabuChecker as IVRPMoveOperator).VRPMoveParameter.ActualName = VRPMoveParameter.Name;

      OperationCollection next = new OperationCollection(base.Apply());
      next.Insert(0, ExecutionContext.CreateOperation(tabuChecker));

      return next;
    }
  }
}
