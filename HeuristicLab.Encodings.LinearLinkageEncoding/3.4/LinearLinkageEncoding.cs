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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Linear Linkage Encoding", "Describes a linear linkage (LLE) encoding.")]
  [StorableClass]
  public sealed class LinearLinkageEncoding : Encoding<ILinearLinkageCreator> {
    #region encoding parameters
    [Storable]
    private IFixedValueParameter<IntValue> lengthParameter;
    public IFixedValueParameter<IntValue> LengthParameter {
      get { return lengthParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Length parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Length parameter value must not be null.");
        if (lengthParameter == value) return;

        if (lengthParameter != null) Parameters.Remove(lengthParameter);
        lengthParameter = value;
        Parameters.Add(lengthParameter);
        OnLengthParameterChanged();
      }
    }
    #endregion

    public int Length {
      get { return LengthParameter.Value.Value; }
      set { LengthParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private LinearLinkageEncoding(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
      DiscoverOperators();
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new LinearLinkageEncoding(this, cloner); }
    private LinearLinkageEncoding(LinearLinkageEncoding original, Cloner cloner)
      : base(original, cloner) {
      lengthParameter = cloner.Clone(original.lengthParameter);
      RegisterParameterEvents();
    }


    public LinearLinkageEncoding() : this("LLE", 10) { }
    public LinearLinkageEncoding(string name) : this(name, 10) { }
    public LinearLinkageEncoding(int length) : this("LLE", length) { }
    public LinearLinkageEncoding(string name, int length)
      : base(name) {
      lengthParameter = new FixedValueParameter<IntValue>(Name + ".Length", new IntValue(length));
      Parameters.Add(lengthParameter);

      SolutionCreator = new RandomLinearLinkageCreator();
      RegisterParameterEvents();
      DiscoverOperators();
    }

    private void OnLengthParameterChanged() {
      RegisterLengthParameterEvents();
      ConfigureOperators(Operators);
    }

    private void RegisterParameterEvents() {
      RegisterLengthParameterEvents();
    }
    private void RegisterLengthParameterEvents() {
      LengthParameter.Value.ValueChanged += (o, s) => ConfigureOperators(Operators);
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static LinearLinkageEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (ILinearLinkageOperator),
          typeof (ILinearLinkageCreator),
          typeof (ILinearLinkageCrossover),
          typeof (ILinearLinkageManipulator),
          typeof (ILinearLinkageShakingOperator),
          typeof (ILinearLinkageMoveOperator)
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(ILinearLinkageOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IOperator> operators) {
      ConfigureCreators(operators.OfType<ILinearLinkageCreator>());
      ConfigureCrossovers(operators.OfType<ILinearLinkageCrossover>());
      ConfigureManipulators(operators.OfType<ILinearLinkageManipulator>());
      ConfigureShakingOperators(operators.OfType<ILinearLinkageShakingOperator>());
      ConfigureMoveOperators(operators.OfType<ILinearLinkageMoveOperator>());
      ConfigureSwap2MoveOperators(operators.OfType<ILinearLinkageSwap2MoveOperator>());
    }

    #region specific operator wiring
    private void ConfigureCreators(IEnumerable<ILinearLinkageCreator> creators) {
      foreach (var creator in creators) {
        creator.LengthParameter.ActualName = LengthParameter.Name;
        creator.LLEParameter.ActualName = Name;
      }
    }
    private void ConfigureCrossovers(IEnumerable<ILinearLinkageCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ChildParameter.ActualName = Name;
        crossover.ParentsParameter.ActualName = Name;
      }
    }
    private void ConfigureManipulators(IEnumerable<ILinearLinkageManipulator> manipulators) {
      foreach (var manipulator in manipulators) {
        manipulator.LLEParameter.ActualName = Name;
      }
    }
    private void ConfigureShakingOperators(IEnumerable<ILinearLinkageShakingOperator> shakingOperators) {
      foreach (var shakingOperator in shakingOperators) {
        shakingOperator.LLEParameter.ActualName = Name;
      }
    }
    private void ConfigureMoveOperators(IEnumerable<ILinearLinkageMoveOperator> moveOperators) {
      foreach (var moveOperator in moveOperators) {
        moveOperator.LLEParameter.ActualName = Name;
      }
    }
    private void ConfigureSwap2MoveOperators(IEnumerable<ILinearLinkageSwap2MoveOperator> swap2MoveOperators) {
      foreach (var swap2MoveOperator in swap2MoveOperators) {
        swap2MoveOperator.Swap2MoveParameter.ActualName = Name + ".Swap2Move";
      }
    }
    #endregion
  }

  public static class IndividualExtensionMethods {
    public static LinearLinkage LinearLinkage(this Individual individual) {
      var encoding = individual.GetEncoding<LinearLinkageEncoding>();
      return individual.LinearLinkage(encoding.Name);
    }
    public static LinearLinkage LinearLinkage(this Individual individual, string name) {
      return (LinearLinkage)individual[name];
    }
  }
}
