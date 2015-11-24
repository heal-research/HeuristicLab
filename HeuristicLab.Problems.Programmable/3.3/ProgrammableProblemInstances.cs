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
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Programmable {
  #region single-objective
  [Item("Binary Vector Problem (single-objective)", "Represents a binary vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveBinaryVectorProgrammableProblem : SingleObjectiveProgrammableProblem<BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private SingleObjectiveBinaryVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveBinaryVectorProgrammableProblem(SingleObjectiveBinaryVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveBinaryVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "BinaryVector");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveBinaryVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Integer Vector Problem (single-objective)", "Represents an integer vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 101)]
  [StorableClass]
  public sealed class SingleObjectiveIntegerVectorProgrammableProblem : SingleObjectiveProgrammableProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private SingleObjectiveIntegerVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveIntegerVectorProgrammableProblem(SingleObjectiveIntegerVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveIntegerVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "IntegerVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveIntegerVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Real Vector Problem (single-objective)", "Represents a real vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 102)]
  [StorableClass]
  public sealed class SingleObjectiveRealVectorProgrammableProblem : SingleObjectiveProgrammableProblem<RealVectorEncoding, RealVector> {

    [StorableConstructor]
    private SingleObjectiveRealVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveRealVectorProgrammableProblem(SingleObjectiveRealVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveRealVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "RealVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveRealVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Permutation Problem (single-objective)", "Represents a permutation single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 103)]
  [StorableClass]
  public sealed class SingleObjectivePermutationProgrammableProblem : SingleObjectiveProgrammableProblem<PermutationEncoding, Permutation> {

    [StorableConstructor]
    private SingleObjectivePermutationProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectivePermutationProgrammableProblem(SingleObjectivePermutationProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectivePermutationProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.PermutationEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "PermutationEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "Permutation");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectivePermutationProgrammableProblem(this, cloner);
    }
  }

  [Item("Symbolic Expression Tree Problem (single-objective)", "Represents a symbolic expression tree single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 104)]
  [StorableClass]
  public sealed class SingleObjectiveSymbolicExpressionTreeProgrammableProblem : SingleObjectiveProgrammableProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {

    [StorableConstructor]
    private SingleObjectiveSymbolicExpressionTreeProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveSymbolicExpressionTreeProgrammableProblem(SingleObjectiveSymbolicExpressionTreeProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveSymbolicExpressionTreeProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.SymbolicExpressionTreeEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "SymbolicExpressionTreeEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "ISymbolicExpressionTree");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveSymbolicExpressionTreeProgrammableProblem(this, cloner);
    }
  }

  [Item("Linear Linkage Problem (single-objective)", "Represents a linear linkage single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 105)]
  [StorableClass]
  public sealed class SingleObjectiveLinearLinkageProgrammableProblem : SingleObjectiveProgrammableProblem<LinearLinkageEncoding, LinearLinkage> {

    [StorableConstructor]
    private SingleObjectiveLinearLinkageProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveLinearLinkageProgrammableProblem(SingleObjectiveLinearLinkageProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveLinearLinkageProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.LinearLinkageEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "LinearLinkageEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "LinearLinkage");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveLinearLinkageProgrammableProblem(this, cloner);
    }
  }

  [Item("Combined Encoding Problem (single-objective)", "Represents a combined encoding single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsSingleObjective, Priority = 1000)]
  [StorableClass]
  public sealed class SingleObjectiveCombinedEncodingProgrammableProblem : SingleObjectiveProgrammableProblem<CombinedEncoding, CombinedSolution> {

    [StorableConstructor]
    private SingleObjectiveCombinedEncodingProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveCombinedEncodingProgrammableProblem(SingleObjectiveCombinedEncodingProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveCombinedEncodingProgrammableProblem()
      : base() {
      ProblemScript.Code = ScriptTemplates.SingleObjectiveCombinedEncodingProblem_Template;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveCombinedEncodingProgrammableProblem(this, cloner);
    }
  }
  #endregion

  #region multi-objective
  [Item("Binary Vector Problem (multi-objective)", "Represents a binary vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveBinaryVectorProgrammableProblem : MultiObjectiveProgrammableProblem<BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private MultiObjectiveBinaryVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveBinaryVectorProgrammableProblem(MultiObjectiveBinaryVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveBinaryVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "BinaryVector");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveBinaryVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Integer Vector Problem (multi-objective)", "Represents an integer vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 101)]
  [StorableClass]
  public sealed class MultiObjectiveIntegerVectorProgrammableProblem : MultiObjectiveProgrammableProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private MultiObjectiveIntegerVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveIntegerVectorProgrammableProblem(MultiObjectiveIntegerVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveIntegerVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "IntegerVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "IntegerVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveIntegerVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Real Vector Problem (multi-objective)", "Represents a real vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 102)]
  [StorableClass]
  public sealed class MultiObjectiveRealVectorProgrammableProblem : MultiObjectiveProgrammableProblem<RealVectorEncoding, RealVector> {

    [StorableConstructor]
    private MultiObjectiveRealVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveRealVectorProgrammableProblem(MultiObjectiveRealVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveRealVectorProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "RealVectorEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "RealVector");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveRealVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Permutation Problem (multi-objective)", "Represents a permutation multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 103)]
  [StorableClass]
  public sealed class MultiObjectivePermutationProgrammableProblem : MultiObjectiveProgrammableProblem<PermutationEncoding, Permutation> {

    [StorableConstructor]
    private MultiObjectivePermutationProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectivePermutationProgrammableProblem(MultiObjectivePermutationProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectivePermutationProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.PermutationEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "PermutationEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "Permutation");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectivePermutationProgrammableProblem(this, cloner);
    }
  }

  [Item("Symbolic Expression Tree Programmable Problem (multi-objective)", "Represents a symbolic expression tree multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 104)]
  [StorableClass]
  public sealed class MultiObjectiveSymbolicExpressionTreeProgrammableProblem : MultiObjectiveProgrammableProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {

    [StorableConstructor]
    private MultiObjectiveSymbolicExpressionTreeProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveSymbolicExpressionTreeProgrammableProblem(MultiObjectiveSymbolicExpressionTreeProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveSymbolicExpressionTreeProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.SymbolicExpressionTreeEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "SymbolicExpressionTreeEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "ISymbolicExpressionTree");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveSymbolicExpressionTreeProgrammableProblem(this, cloner);
    }
  }

  [Item("Linear Linkage Programmable Problem (multi-objective)", "Represents a linear linkage multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 105)]
  [StorableClass]
  public sealed class MultiObjectiveLinearLinkageProgrammableProblem : MultiObjectiveProgrammableProblem<LinearLinkageEncoding, LinearLinkage> {

    [StorableConstructor]
    private MultiObjectiveLinearLinkageProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveLinearLinkageProgrammableProblem(MultiObjectiveLinearLinkageProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveLinearLinkageProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.LinearLinkageEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "LinearLinkageEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "LinearLinkage");
      ProblemScript.Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveLinearLinkageProgrammableProblem(this, cloner);
    }
  }

  [Item("Combined Encoding Problem (multi-objective)", "Represents a combined encoding multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.ProgrammableProblemsMultiObjective, Priority = 1000)]
  [StorableClass]
  public sealed class MultiObjectiveCombinedEncodingProgrammableProblem : MultiObjectiveProgrammableProblem<CombinedEncoding, CombinedSolution> {

    [StorableConstructor]
    private MultiObjectiveCombinedEncodingProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveCombinedEncodingProgrammableProblem(MultiObjectiveCombinedEncodingProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public MultiObjectiveCombinedEncodingProgrammableProblem()
      : base() {
      ProblemScript.Code = ScriptTemplates.MultiObjectiveCombinedEncodingProblem_Template;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveCombinedEncodingProgrammableProblem(this, cloner);
    }
  }
  #endregion
}
