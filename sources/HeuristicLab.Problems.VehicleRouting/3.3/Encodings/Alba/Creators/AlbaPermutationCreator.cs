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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaPermutationCreator", "An operator which creates a new alba VRP representation.")]
  [StorableClass]
  public sealed class AlbaPermutationCreator : VRPCreator, IStochasticOperator {
    public IValueLookupParameter<IPermutationCreator> PermutationCreatorParameter {
      get { return (IValueLookupParameter<IPermutationCreator>)Parameters["PermutationCreatorParameter"]; }
    }

    #region IStochasticOperator Members
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion
    
    public AlbaPermutationCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<IPermutationCreator>("PermutationCreatorParameter", "The permutation creator.", new RandomPermutationCreator()));

      ParameterizeSolutionCreator();

      Initialize();
    }

    [StorableConstructor]
    private AlbaPermutationCreator(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      CitiesParameter.ValueChanged += new EventHandler(CitiesParameter_ValueChanged);
      if(CitiesParameter.Value != null)
        CitiesParameter.Value.ValueChanged += new EventHandler(CitiesValue_ValueChanged); 
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      AlbaPermutationCreator clone = (AlbaPermutationCreator)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    void CitiesParameter_ValueChanged(object sender, EventArgs e) {
      CitiesParameter.Value.ValueChanged += new EventHandler(CitiesValue_ValueChanged);
      
      ParameterizeSolutionCreator();
    }

    void CitiesValue_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }

    private void ParameterizeSolutionCreator() {
      PermutationCreatorParameter.Value.LengthParameter.Value = CitiesParameter.Value;
      PermutationCreatorParameter.Value.PermutationTypeParameter.Value = new PermutationType(PermutationTypes.RelativeUndirected);
    }

    public override IOperation Apply() {
      PermutationCreatorParameter.Value.LengthParameter.Value = new IntValue(CitiesParameter.Value.Value + VehiclesParameter.ActualValue.Value - 1);

      IPermutationCreator creator = PermutationCreatorParameter.ActualValue;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(creator);
      op.Operator.Execute((IExecutionContext)op);

      Permutation permutation = ExecutionContext.Scope.Variables["Permutation"].Value as Permutation;
      ExecutionContext.Scope.Variables.Remove("Permutation");

      VRPSolutionParameter.ActualValue = new AlbaEncoding(permutation, CitiesParameter.ActualValue.Value);

      return base.Apply();
    }
  }
}
