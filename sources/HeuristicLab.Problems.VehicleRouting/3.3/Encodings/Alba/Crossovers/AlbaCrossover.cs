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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaCrossover", "An operator which crosses two Alba VRP representations.")]
  [StorableClass]
  public sealed class AlbaCrossover : VRPCrossover {    
    public IValueLookupParameter<IPermutationCrossover> InnerCrossoverParameter {
      get { return (IValueLookupParameter<IPermutationCrossover>)Parameters["InnerCrossover"]; }
    }

    [StorableConstructor]
    private AlbaCrossover(bool deserializing) : base(deserializing) { }

    public AlbaCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationCrossover>("InnerCrossover", "The permutation crossover.", new EdgeRecombinationCrossover()));
    
      AlbaEncoding.RemoveUnusedParameters(Parameters);
    }

    private void Crossover() {
      //note - the inner crossover is called here and the result is converted to an alba representation
      //some refactoring should be done here in the future - the crossover operation should be called directly

      InnerCrossoverParameter.ActualValue.ParentsParameter.ActualName = ParentsParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        InnerCrossoverParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op);

      string childName = InnerCrossoverParameter.ActualValue.ChildParameter.ActualName;
      if (ExecutionContext.Scope.Variables.ContainsKey(childName)) {
        Permutation permutation = ExecutionContext.Scope.Variables[childName].Value as Permutation;
        ExecutionContext.Scope.Variables.Remove(childName);

        ChildParameter.ActualValue = new AlbaEncoding(permutation, Cities);
      } else
        ChildParameter.ActualValue = null;
    }

    public override IOperation Apply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];

        if (!(solution is AlbaEncoding)) {
          parents[i] = AlbaEncoding.ConvertFrom(solution, VehiclesParameter.ActualValue.Value);
        } else {
          parents[i] = solution;
        }
      }
      ParentsParameter.ActualValue = parents;

      Crossover();

      return base.Apply();
    }
  }
}
