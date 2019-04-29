#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.ExternalEvaluation {
  #region single-objective
  [Item("Binary Vector External Evaluation Problem (single-objective)", "Represents a binary vector single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 100)]
  [StorableType("4ea0ded8-4451-4011-b88e-4d0680721b01")]
  public sealed class SingleObjectiveBinaryVectorExternalEvaluationProblem : ExternalEvaluationProblem<BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private SingleObjectiveBinaryVectorExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveBinaryVectorExternalEvaluationProblem(SingleObjectiveBinaryVectorExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveBinaryVectorExternalEvaluationProblem()
      : base(new BinaryVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveBinaryVectorExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Integer Vector External Evaluation Problem (single-objective)", "Represents an integer vector single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 101)]
  [StorableType("46465e8c-11d8-4d02-8c45-de41a08db7fa")]
  public sealed class SingleObjectiveIntegerVectorExternalEvaluationProblem : ExternalEvaluationProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private SingleObjectiveIntegerVectorExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveIntegerVectorExternalEvaluationProblem(SingleObjectiveIntegerVectorExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveIntegerVectorExternalEvaluationProblem()
      : base(new IntegerVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveIntegerVectorExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Real Vector External Evaluation Problem (single-objective)", "Represents a real vector single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 102)]
  [StorableType("637f091f-6601-494e-bafb-2a8ea474210c")]
  public sealed class SingleObjectiveRealVectorExternalEvaluationProblem : ExternalEvaluationProblem<RealVectorEncoding, RealVector> {

    [StorableConstructor]
    private SingleObjectiveRealVectorExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveRealVectorExternalEvaluationProblem(SingleObjectiveRealVectorExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveRealVectorExternalEvaluationProblem()
      : base(new RealVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveRealVectorExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Permutation External Evaluation Problem (single-objective)", "Represents a permutation single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 103)]
  [StorableType("ad9d45f8-b97e-49a7-b3d2-487d9a2cbdf9")]
  public sealed class SingleObjectivePermutationExternalEvaluationProblem : ExternalEvaluationProblem<PermutationEncoding, Permutation> {

    [StorableConstructor]
    private SingleObjectivePermutationExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectivePermutationExternalEvaluationProblem(SingleObjectivePermutationExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectivePermutationExternalEvaluationProblem()
      : base(new PermutationEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the permutation.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectivePermutationExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Symbolic Expression Tree External Evaluation Problem (single-objective)", "Represents a symbolic expression tree single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 104)]
  [StorableType("9b3ee4a8-7076-4edd-ae7e-4188bc49aaa3")]
  public sealed class SingleObjectiveSymbolicExpressionTreeExternalEvaluationProblem : ExternalEvaluationProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {

    [StorableConstructor]
    private SingleObjectiveSymbolicExpressionTreeExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveSymbolicExpressionTreeExternalEvaluationProblem(SingleObjectiveSymbolicExpressionTreeExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveSymbolicExpressionTreeExternalEvaluationProblem()
      : base(new SymbolicExpressionTreeEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("TreeLength", "The total amount of nodes.", new IntValue(50));
      Parameters.Add(lengthParameter);
      Encoding.TreeLengthParameter = lengthParameter;
      var depthParameter = new FixedValueParameter<IntValue>("TreeDepth", "The depth of the tree.", new IntValue(10));
      Parameters.Add(depthParameter);
      Encoding.TreeDepthParameter = depthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveSymbolicExpressionTreeExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Linear Linkage External Evaluation Problem (single-objective)", "Represents a linear linkage single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 105)]
  [StorableType("945a35d9-89a8-4423-9ea0-21829ac68887")]
  public sealed class SingleObjectiveLinearLinkageExternalEvaluationProblem : ExternalEvaluationProblem<LinearLinkageEncoding, LinearLinkage> {

    [StorableConstructor]
    private SingleObjectiveLinearLinkageExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveLinearLinkageExternalEvaluationProblem(SingleObjectiveLinearLinkageExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveLinearLinkageExternalEvaluationProblem()
      : base(new LinearLinkageEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveLinearLinkageExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Combined Encoding External Evaluation Problem (single-objective)", "Represents a combined encoding single-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsSingleObjective, Priority = 1000)]
  [StorableType("0effb975-c1ff-4485-afc9-5f4cf30ac62b")]
  public sealed class SingleObjectiveCombinedEncodingExternalEvaluationProblem : ExternalEvaluationProblem<CombinedEncoding, CombinedSolution> {

    [StorableConstructor]
    private SingleObjectiveCombinedEncodingExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveCombinedEncodingExternalEvaluationProblem(SingleObjectiveCombinedEncodingExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveCombinedEncodingExternalEvaluationProblem() : base(new CombinedEncoding()) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveCombinedEncodingExternalEvaluationProblem(this, cloner);
    }
  }
  #endregion

  #region multi-objective
  [Item("Binary Vector External Evaluation Problem (multi-objective)", "Represents a binary vector multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 100)]
  [StorableType("f14c7e88-b74d-4cad-ae55-83daf7b4c288")]
  public sealed class MultiObjectiveBinaryVectorExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private MultiObjectiveBinaryVectorExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveBinaryVectorExternalEvaluationProblem(MultiObjectiveBinaryVectorExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveBinaryVectorExternalEvaluationProblem()
      : base(new BinaryVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveBinaryVectorExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Integer Vector External Evaluation Problem (multi-objective)", "Represents an integer vector multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 101)]
  [StorableType("90a82c2f-6c37-4ffd-8495-bee278c583d3")]
  public sealed class MultiObjectiveIntegerVectorExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private MultiObjectiveIntegerVectorExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveIntegerVectorExternalEvaluationProblem(MultiObjectiveIntegerVectorExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveIntegerVectorExternalEvaluationProblem()
      : base(new IntegerVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveIntegerVectorExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Real Vector External Evaluation Problem (multi-objective)", "Represents a real vector multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 102)]
  [StorableType("38e1d068-d569-48c5-bad6-cbdd685b7c6b")]
  public sealed class MultiObjectiveRealVectorExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<RealVectorEncoding, RealVector> {

    [StorableConstructor]
    private MultiObjectiveRealVectorExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveRealVectorExternalEvaluationProblem(MultiObjectiveRealVectorExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveRealVectorExternalEvaluationProblem()
      : base(new RealVectorEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveRealVectorExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Permutation External Evaluation Problem (multi-objective)", "Represents a permutation multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 103)]
  [StorableType("f1b265b0-ac7c-4c36-b346-5b3f2c37694b")]
  public sealed class MultiObjectivePermutationExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<PermutationEncoding, Permutation> {

    [StorableConstructor]
    private MultiObjectivePermutationExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectivePermutationExternalEvaluationProblem(MultiObjectivePermutationExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectivePermutationExternalEvaluationProblem()
      : base(new PermutationEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the permutation.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectivePermutationExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Symbolic Expression Tree External Evaluation Problem (multi-objective)", "Represents a symbolic expression tree multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 104)]
  [StorableType("fb6834e2-2d56-4711-a3f8-5e0ab55cd040")]
  public sealed class MultiObjectiveSymbolicExpressionTreeExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {

    [StorableConstructor]
    private MultiObjectiveSymbolicExpressionTreeExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveSymbolicExpressionTreeExternalEvaluationProblem(MultiObjectiveSymbolicExpressionTreeExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveSymbolicExpressionTreeExternalEvaluationProblem()
      : base(new SymbolicExpressionTreeEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("TreeLength", "The total amount of nodes.", new IntValue(50));
      Parameters.Add(lengthParameter);
      Encoding.TreeLengthParameter = lengthParameter;
      var depthParameter = new FixedValueParameter<IntValue>("TreeDepth", "The depth of the tree.", new IntValue(10));
      Parameters.Add(depthParameter);
      Encoding.TreeDepthParameter = depthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveSymbolicExpressionTreeExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Linear Linkage External Evaluation Problem (multi-objective)", "Represents a linear linkage multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 105)]
  [StorableType("ed0c1129-651d-465f-87b0-f412f3e3b3d1")]
  public sealed class MultiObjectiveLinearLinkageExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<LinearLinkageEncoding, LinearLinkage> {

    [StorableConstructor]
    private MultiObjectiveLinearLinkageExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveLinearLinkageExternalEvaluationProblem(MultiObjectiveLinearLinkageExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveLinearLinkageExternalEvaluationProblem()
      : base(new LinearLinkageEncoding()) {
      var lengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the vector.", new IntValue(10));
      Parameters.Add(lengthParameter);
      Encoding.LengthParameter = lengthParameter;
      // TODO: Add and parameterize additional operators, 
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveLinearLinkageExternalEvaluationProblem(this, cloner);
    }
  }

  [Item("Combined Encoding External Evaluation Problem (multi-objective)", "Represents a combined encoding multi-objective problem that is evaluated by a separate process.")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblemsMultiObjective, Priority = 1000)]
  [StorableType("5f136869-1750-4d96-ba7e-b25edd2bcab1")]
  public sealed class MultiObjectiveCombinedEncodingExternalEvaluationProblem : MultiObjectiveExternalEvaluationProblem<CombinedEncoding, CombinedSolution> {

    [StorableConstructor]
    private MultiObjectiveCombinedEncodingExternalEvaluationProblem(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveCombinedEncodingExternalEvaluationProblem(MultiObjectiveCombinedEncodingExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveCombinedEncodingExternalEvaluationProblem() : base(new CombinedEncoding()) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveCombinedEncodingExternalEvaluationProblem(this, cloner);
    }
  }
  #endregion
}
