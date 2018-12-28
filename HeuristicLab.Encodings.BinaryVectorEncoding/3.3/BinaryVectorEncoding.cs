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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("BinaryVectorEncoding", "Describes a binary vector encoding.")]
  [StorableType("889C5E1A-3FBF-4AB3-AB2E-199A781503B5")]
  public sealed class BinaryVectorEncoding : Encoding<IBinaryVectorCreator> {
    #region Encoding Parameters
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
    private BinaryVectorEncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
      DiscoverOperators();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new BinaryVectorEncoding(this, cloner); }
    private BinaryVectorEncoding(BinaryVectorEncoding original, Cloner cloner)
      : base(original, cloner) {
      lengthParameter = cloner.Clone(original.lengthParameter);
      RegisterParameterEvents();
    }

    public BinaryVectorEncoding() : this("BinaryVector", 10) { }
    public BinaryVectorEncoding(string name) : this(name, 10) { }
    public BinaryVectorEncoding(int length) : this("BinaryVector", length) { }
    public BinaryVectorEncoding(string name, int length)
      : base(name) {
      lengthParameter = new FixedValueParameter<IntValue>(Name + ".Length", new IntValue(length));
      Parameters.Add(lengthParameter);

      SolutionCreator = new RandomBinaryVectorCreator();
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
      LengthParameter.ValueChanged += (o, s) => ConfigureOperators(Operators);
      LengthParameter.Value.ValueChanged += (o, s) => ConfigureOperators(Operators);
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static BinaryVectorEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
        typeof (IBinaryVectorOperator),
        typeof (IBinaryVectorCreator),
        typeof (IBinaryVectorCrossover),
        typeof (IBinaryVectorManipulator),
        typeof (IBinaryVectorMoveOperator),
        typeof (IBinaryVectorMultiNeighborhoodShakingOperator),
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IBinaryVectorOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IOperator> operators) {
      ConfigureCreators(operators.OfType<IBinaryVectorCreator>());
      ConfigureCrossovers(operators.OfType<IBinaryVectorCrossover>());
      ConfigureManipulators(operators.OfType<IBinaryVectorManipulator>());
      ConfigureMoveOperators(operators.OfType<IBinaryVectorMoveOperator>());
      ConfigureBitFlipMoveOperators(operators.OfType<IOneBitflipMoveOperator>());
      ConfigureShakingOperators(operators.OfType<IBinaryVectorMultiNeighborhoodShakingOperator>());
    }

    #region Specific Operator Wiring
    private void ConfigureCreators(IEnumerable<IBinaryVectorCreator> creators) {
      foreach (var creator in creators) {
        creator.BinaryVectorParameter.ActualName = Name;
        creator.LengthParameter.ActualName = LengthParameter.Name;
      }
    }
    private void ConfigureCrossovers(IEnumerable<IBinaryVectorCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ParentsParameter.ActualName = Name;
        crossover.ChildParameter.ActualName = Name;
      }
    }
    private void ConfigureManipulators(IEnumerable<IBinaryVectorManipulator> manipulators) {
      foreach (var manipulator in manipulators) {
        manipulator.BinaryVectorParameter.ActualName = Name;
      }
    }
    private void ConfigureMoveOperators(IEnumerable<IBinaryVectorMoveOperator> moveOperators) {
      foreach (var moveOperator in moveOperators) {
        moveOperator.BinaryVectorParameter.ActualName = Name;
      }
    }
    private void ConfigureBitFlipMoveOperators(IEnumerable<IOneBitflipMoveOperator> oneBitflipMoveOperators) {
      foreach (var oneBitFlipMoveOperator in oneBitflipMoveOperators) {
        oneBitFlipMoveOperator.OneBitflipMoveParameter.ActualName = Name + "_OneBitFlipMove";
      }
    }
    private void ConfigureShakingOperators(IEnumerable<IBinaryVectorMultiNeighborhoodShakingOperator> shakingOperators) {
      foreach (var shakingOperator in shakingOperators) {
        shakingOperator.BinaryVectorParameter.ActualName = Name;
      }
    }
    #endregion
  }

  public static class IndividualExtensionMethods {
    public static BinaryVector BinaryVector(this Individual individual) {
      var encoding = individual.GetEncoding<BinaryVectorEncoding>();
      return individual.BinaryVector(encoding.Name);
    }

    public static BinaryVector BinaryVector(this Individual individual, string name) {
      return (BinaryVector)individual[name];
    }
  }
}
