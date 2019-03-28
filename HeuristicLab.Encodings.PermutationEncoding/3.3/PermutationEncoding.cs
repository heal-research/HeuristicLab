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
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("PermutationEncoding", "Describes a permutation encoding.")]
  [StorableType("E30E7507-44BA-4021-8F56-C3FC5569A6FE")]
  public sealed class PermutationEncoding : Encoding<Permutation> {
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

    [Storable]
    private IFixedValueParameter<PermutationType> permutationTypeParameter;
    public IFixedValueParameter<PermutationType> PermutationTypeParameter {
      get { return permutationTypeParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Permutation type parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Permutation type parameter value must not be null.");
        if (permutationTypeParameter == value) return;

        if (permutationTypeParameter != null) Parameters.Remove(permutationTypeParameter);
        permutationTypeParameter = value;
        Parameters.Add(permutationTypeParameter);
        OnPermutationTypeParameterChanged();
      }
    }
    #endregion

    public int Length {
      get { return LengthParameter.Value.Value; }
      set { LengthParameter.Value.Value = value; }
    }

    public PermutationTypes Type {
      get { return PermutationTypeParameter.Value.Value; }
      set { PermutationTypeParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private PermutationEncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
      DiscoverOperators();
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new PermutationEncoding(this, cloner); }
    private PermutationEncoding(PermutationEncoding original, Cloner cloner)
      : base(original, cloner) {
      lengthParameter = cloner.Clone(original.lengthParameter);
      permutationTypeParameter = cloner.Clone(original.permutationTypeParameter);
      RegisterParameterEvents();
    }


    public PermutationEncoding() : this("Permutation", 10, PermutationTypes.Absolute) { }
    public PermutationEncoding(string name) : this(name, 10, PermutationTypes.Absolute) { }
    public PermutationEncoding(int length) : this("Permutation", length, PermutationTypes.Absolute) { }
    public PermutationEncoding(string name, int length, PermutationTypes type)
      : base(name) {
      lengthParameter = new FixedValueParameter<IntValue>(Name + ".Length", new IntValue(length));
      permutationTypeParameter = new FixedValueParameter<PermutationType>(Name + ".Type", new PermutationType(type));
      Parameters.Add(lengthParameter);
      Parameters.Add(permutationTypeParameter);

      SolutionCreator = new RandomPermutationCreator();
      RegisterParameterEvents();
      DiscoverOperators();
    }

    private void OnLengthParameterChanged() {
      RegisterLengthParameterEvents();
      ConfigureOperators(Operators);
    }

    private void OnPermutationTypeParameterChanged() {
      RegisterPermutationTypeParameterEvents();
      ConfigureOperators(Operators);
    }

    private void RegisterParameterEvents() {
      RegisterLengthParameterEvents();
      RegisterPermutationTypeParameterEvents();
    }
    private void RegisterLengthParameterEvents() {
      LengthParameter.Value.ValueChanged += (o, s) => ConfigureOperators(Operators);
    }
    private void RegisterPermutationTypeParameterEvents() {
      PermutationTypeParameter.Value.ValueChanged += (o, s) => ConfigureOperators(Operators);
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static PermutationEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IPermutationOperator),
          typeof (IPermutationCreator),
          typeof (IPermutationCrossover),
          typeof (IPermutationManipulator),
          typeof (IPermutationMultiNeighborhoodShakingOperator),
          typeof (IPermutationMoveOperator),
          typeof (IPermutationInversionMoveOperator),
          typeof (IPermutationScrambleMoveOperator),
          typeof (IPermutationSwap2MoveOperator),                    
          typeof (IPermutationTranslocationMoveOperator),
          typeof (IPermutationLocalImprovementOperator),
          typeof (IPermutationSolutionOperator),
          typeof (IPermutationSolutionsOperator),
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IPermutationOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      ConfigureCreators(operators.OfType<IPermutationCreator>());
      ConfigureCrossovers(operators.OfType<IPermutationCrossover>());
      ConfigureManipulators(operators.OfType<IPermutationManipulator>());
      ConfigureShakingOperators(operators.OfType<IPermutationMultiNeighborhoodShakingOperator>());
      ConfigureMoveOperators(operators.OfType<IPermutationMoveOperator>());
      ConfigureInversionMoveOperators(operators.OfType<IPermutationInversionMoveOperator>());
      ConfigureScrambleMoveOperators(operators.OfType<IPermutationScrambleMoveOperator>());
      ConfigureSwap2MoveOperators(operators.OfType<IPermutationSwap2MoveOperator>());
      ConfigureTranslocationMoveOperators(operators.OfType<IPermutationTranslocationMoveOperator>());
      ConfigureLocalImprovementOperators(operators.OfType<IPermutationLocalImprovementOperator>());
      ConfigureSolutionOperators(operators.OfType<IPermutationSolutionOperator>());
      ConfigureSolutionsOperators(operators.OfType<IPermutationSolutionsOperator>());
    }

    #region specific operator wiring
    private void ConfigureCreators(IEnumerable<IPermutationCreator> creators) {
      foreach (var creator in creators) {
        creator.LengthParameter.ActualName = LengthParameter.Name;
        creator.PermutationTypeParameter.Value.Value = Type;
      }
    }
    private void ConfigureCrossovers(IEnumerable<IPermutationCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ChildParameter.ActualName = Name;
        crossover.ParentsParameter.ActualName = Name;
      }
    }
    private void ConfigureManipulators(IEnumerable<IPermutationManipulator> manipulators) {
      // IPermutationManipulator does not contain additional parameters (already contained in IPermutationSolutionOperator)
    }
    private void ConfigureShakingOperators(IEnumerable<IPermutationMultiNeighborhoodShakingOperator> shakingOperators) {
      foreach (var shakingOperator in shakingOperators) {
        shakingOperator.PermutationParameter.ActualName = Name;
      }
    }
    private void ConfigureMoveOperators(IEnumerable<IPermutationMoveOperator> moveOperators) {
      foreach (var moveOperator in moveOperators) {
        moveOperator.PermutationParameter.ActualName = Name;
      }
    }
    private void ConfigureInversionMoveOperators(IEnumerable<IPermutationInversionMoveOperator> inversionMoveOperators) {
      foreach (var inversionMoveOperator in inversionMoveOperators) {
        inversionMoveOperator.InversionMoveParameter.ActualName = Name + ".InversionMove";
      }
    }
    private void ConfigureScrambleMoveOperators(IEnumerable<IPermutationScrambleMoveOperator> scrambleMoveOperators) {
      foreach (var scrambleMoveOperator in scrambleMoveOperators) {
        scrambleMoveOperator.ScrambleMoveParameter.ActualName = Name + ".ScrambleMove";
      }
    }
    private void ConfigureSwap2MoveOperators(IEnumerable<IPermutationSwap2MoveOperator> swap2MoveOperators) {
      foreach (var swap2MoveOperator in swap2MoveOperators) {
        swap2MoveOperator.Swap2MoveParameter.ActualName = Name + ".Swap2Move";
      }
    }
    private void ConfigureTranslocationMoveOperators(IEnumerable<IPermutationTranslocationMoveOperator> translocationMoveOperators) {
      foreach (var translocationMoveOperator in translocationMoveOperators) {
        translocationMoveOperator.TranslocationMoveParameter.ActualName = Name + ".TranslocationMove";
      }
    }
    private void ConfigureLocalImprovementOperators(IEnumerable<IPermutationLocalImprovementOperator> localImprovementOperators) {
      // IPermutationLocalImprovementOperator does not contain additional parameters (already contained in IPermutationSolutionOperator)
    }
    private void ConfigureSolutionOperators(IEnumerable<IPermutationSolutionOperator> solutionOperators) {
      foreach (var solutionOperator in solutionOperators) {
        solutionOperator.PermutationParameter.ActualName = Name;
      }
    }
    private void ConfigureSolutionsOperators(IEnumerable<IPermutationSolutionsOperator> solutionsOperators) {
      foreach (var solutionsOperator in solutionsOperators) {
        solutionsOperator.PermutationsParameter.ActualName = Name;
      }
    }
    #endregion
  }
}
