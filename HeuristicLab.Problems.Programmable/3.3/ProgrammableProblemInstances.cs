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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Programmable {
  #region single-objective
  [Item("Binary Vector Programmable Problem (single-objective)", "Represents a binary vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveBinaryVectorProgrammableProblem : SingleObjectiveProgrammableProblem<BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private SingleObjectiveBinaryVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveBinaryVectorProgrammableProblem(SingleObjectiveBinaryVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveBinaryVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "BinaryVector");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveBinaryVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Multi Solution Programmable Problem (single-objective)", "Represents a multi solution single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveMultiSolutionProgrammableProblem : SingleObjectiveProgrammableProblem<MultiEncoding, CombinedSolution> {

    [StorableConstructor]
    private SingleObjectiveMultiSolutionProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveMultiSolutionProgrammableProblem(SingleObjectiveMultiSolutionProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveMultiSolutionProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "MultiEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "CombinedSolution");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveMultiSolutionProgrammableProblem(this, cloner);
    }
  }

  [Item("Integer Vector Programmable Problem (single-objective)", "Represents an integer vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveIntegerVectorProgrammableProblem : SingleObjectiveProgrammableProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private SingleObjectiveIntegerVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveIntegerVectorProgrammableProblem(SingleObjectiveIntegerVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveIntegerVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "IntegerVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveIntegerVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Real Vector Programmable Problem (single-objective)", "Represents a real vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveRealVectorProgrammableProblem : SingleObjectiveProgrammableProblem<RealVectorEncoding, RealVector> {

    [StorableConstructor]
    private SingleObjectiveRealVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveRealVectorProgrammableProblem(SingleObjectiveRealVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveRealVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "RealVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveRealVectorProgrammableProblem(this, cloner);
    }
  }

  //[Item("Permutation Programmable Problem (single-objective)", "Represents a permutation single-objective problem that can be programmed with a script.")]
  //[Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  //[StorableClass]
  //public sealed class SingleObjectivePermutationProgrammableProblem : SingleObjectiveProgrammableProblem<PermutationEncoding, Permutation> {

  //  [StorableConstructor]
  //  private SingleObjectivePermutationProgrammableProblem(bool deserializing) : base(deserializing) { }
  //  private SingleObjectivePermutationProgrammableProblem(SingleObjectivePermutationProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
  //  public SingleObjectivePermutationProgrammableProblem()
  //    : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.PermutationEncoding", "PermutationEncoding", "Permutation")) { }

  //  public override IDeepCloneable Clone(Cloner cloner) {
  //    return new SingleObjectivePermutationProgrammableProblem(this, cloner);
  //  }
  //}

  //[Item("Symbolic Expression Tree Programmable Problem (single-objective)", "Represents a symbolic expression tree single-objective problem that can be programmed with a script.")]
  //[Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  //[StorableClass]
  //public sealed class SingleObjectiveSymbolicExpressionTreeProgrammableProblem : SingleObjectiveProgrammableProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree> {

  //  [StorableConstructor]
  //  private SingleObjectiveSymbolicExpressionTreeProgrammableProblem(bool deserializing) : base(deserializing) { }
  //  private SingleObjectiveSymbolicExpressionTreeProgrammableProblem(SingleObjectiveSymbolicExpressionTreeProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
  //  public SingleObjectiveSymbolicExpressionTreeProgrammableProblem()
  //    : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "SymbolicExpressionTreeEncoding", "SymbolicExpressionTree")) { }

  //  public override IDeepCloneable Clone(Cloner cloner) {
  //    return new SingleObjectiveSymbolicExpressionTreeProgrammableProblem(this, cloner);
  //  }
  //}

  //[Item("Linear Linkage Programmable Problem (single-objective)", "Represents a linear linkage single-objective problem that can be programmed with a script.")]
  //[Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  //[StorableClass]
  //public sealed class SingleObjectiveLinearLinkageProgrammableProblem : SingleObjectiveProgrammableProblem<LinearLinkageEncoding, LinearLinkage> {

  //  [StorableConstructor]
  //  private SingleObjectiveLinearLinkageProgrammableProblem(bool deserializing) : base(deserializing) { }
  //  private SingleObjectiveLinearLinkageProgrammableProblem(SingleObjectiveLinearLinkageProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
  //  public SingleObjectiveLinearLinkageProgrammableProblem()
  //    : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.LinearLinkageEncoding", "LinearLinkageEncoding", "LinearLinkage")) { }

  //  public override IDeepCloneable Clone(Cloner cloner) {
  //    return new SingleObjectiveLinearLinkageProgrammableProblem(this, cloner);
  //  }
  //}
  #endregion

  #region multi-objective
  [Item("Binary Vector Programmable Problem (multi-objective)", "Represents a binary vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveBinaryVectorProgrammableProblem : MultiObjectiveProgrammableProblem<BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private MultiObjectiveBinaryVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveBinaryVectorProgrammableProblem(MultiObjectiveBinaryVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveBinaryVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledMultiObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "BinaryVector");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveBinaryVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Multi Solution Programmable Problem (multi-objective)", "Represents a multi solution multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveMultiSolutionProgrammableProblem : MultiObjectiveProgrammableProblem<MultiEncoding, CombinedSolution> {

    [StorableConstructor]
    private MultiObjectiveMultiSolutionProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveMultiSolutionProgrammableProblem(MultiObjectiveMultiSolutionProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveMultiSolutionProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "MultiEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "CombinedSolution");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveMultiSolutionProgrammableProblem(this, cloner);
    }
  }

  [Item("Integer Vector Programmable Problem (multi-objective)", "Represents an integer vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveIntegerVectorProgrammableProblem : MultiObjectiveProgrammableProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private MultiObjectiveIntegerVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveIntegerVectorProgrammableProblem(MultiObjectiveIntegerVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveIntegerVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "IntegerVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveIntegerVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Real Vector Programmable Problem (multi-objective)", "Represents a real vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveRealVectorProgrammableProblem : MultiObjectiveProgrammableProblem<RealVectorEncoding, RealVector> {

    [StorableConstructor]
    private MultiObjectiveRealVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveRealVectorProgrammableProblem(MultiObjectiveRealVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveRealVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "RealVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveRealVectorProgrammableProblem(this, cloner);
    }
  }

  //[Item("Permutation Programmable Problem (multi-objective)", "Represents a permutation multi-objective problem that can be programmed with a script.")]
  //[Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  //[StorableClass]
  //public sealed class MultiObjectivePermutationProgrammableProblem : MultiObjectiveProgrammableProblem<PermutationEncoding, Permutation> {

  //  [StorableConstructor]
  //  private MultiObjectivePermutationProgrammableProblem(bool deserializing) : base(deserializing) { }
  //  private MultiObjectivePermutationProgrammableProblem(MultiObjectivePermutationProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
  //  public MultiObjectivePermutationProgrammableProblem()
  //    : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.PermutationEncoding", "PermutationEncoding", "Permutation")) { }

  //  public override IDeepCloneable Clone(Cloner cloner) {
  //    return new MultiObjectivePermutationProgrammableProblem(this, cloner);
  //  }
  //}

  //[Item("Symbolic Expression Tree Programmable Problem (multi-objective)", "Represents a symbolic expression tree multi-objective problem that can be programmed with a script.")]
  //[Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  //[StorableClass]
  //public sealed class MultiObjectiveSymbolicExpressionTreeProgrammableProblem : MultiObjectiveProgrammableProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree> {

  //  [StorableConstructor]
  //  private MultiObjectiveSymbolicExpressionTreeProgrammableProblem(bool deserializing) : base(deserializing) { }
  //  private MultiObjectiveSymbolicExpressionTreeProgrammableProblem(MultiObjectiveSymbolicExpressionTreeProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
  //  public MultiObjectiveSymbolicExpressionTreeProgrammableProblem()
  //    : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "SymbolicExpressionTreeEncoding", "SymbolicExpressionTree")) { }

  //  public override IDeepCloneable Clone(Cloner cloner) {
  //    return new MultiObjectiveSymbolicExpressionTreeProgrammableProblem(this, cloner);
  //  }
  //}

  //[Item("Linear Linkage Programmable Problem (multi-objective)", "Represents a linear linkage multi-objective problem that can be programmed with a script.")]
  //[Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  //[StorableClass]
  //public sealed class MultiObjectiveLinearLinkageProgrammableProblem : MultiObjectiveProgrammableProblem<LinearLinkageEncoding, LinearLinkage> {

  //  [StorableConstructor]
  //  private MultiObjectiveLinearLinkageProgrammableProblem(bool deserializing) : base(deserializing) { }
  //  private MultiObjectiveLinearLinkageProgrammableProblem(MultiObjectiveLinearLinkageProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
  //  public MultiObjectiveLinearLinkageProgrammableProblem()
  //    : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.LinearLinkageEncoding", "LinearLinkageEncoding", "LinearLinkage")) { }

  //  public override IDeepCloneable Clone(Cloner cloner) {
  //    return new MultiObjectiveLinearLinkageProgrammableProblem(this, cloner);
  //  }
  //}
  #endregion
}
