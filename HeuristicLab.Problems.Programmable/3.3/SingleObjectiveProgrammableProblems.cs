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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Programmable {
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
  public sealed class SingleObjectiveMultiSolutionProgrammableProblem : SingleObjectiveProgrammableProblem<MultiEncoding, MultiSolution> {

    [StorableConstructor]
    private SingleObjectiveMultiSolutionProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveMultiSolutionProgrammableProblem(SingleObjectiveMultiSolutionProgrammableProblem original, Cloner cloner) : base(original, cloner) { }

    public SingleObjectiveMultiSolutionProgrammableProblem()
      : base() {
      var codeTemplate = ScriptTemplates.CompiledSingleObjectiveProblemDefinition_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, "HeuristicLab.Encodings.BinaryVectorEncoding");
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, "MultiEncoding");
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, "MultiSolution");
      ProblemScript.Code = codeTemplate;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveMultiSolutionProgrammableProblem(this, cloner);
    }
  }
  //TODO
  /*
  [Item("Integer Vector Programmable Problem (single-objective)", "Represents an integer vector single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveIntegerVectorProgrammableProblem : SingleObjectiveProgrammableProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private SingleObjectiveIntegerVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveIntegerVectorProgrammableProblem(SingleObjectiveIntegerVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveIntegerVectorProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.IntegerVectorEncoding", "IntegerVectorEncoding", "IntegerVector")) { }

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
      : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.RealVectorEncoding", "RealVectorEncoding", "RealVector")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveRealVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Permutation Programmable Problem (single-objective)", "Represents a permutation single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectivePermutationProgrammableProblem : SingleObjectiveProgrammableProblem<PermutationEncoding, Permutation> {

    [StorableConstructor]
    private SingleObjectivePermutationProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectivePermutationProgrammableProblem(SingleObjectivePermutationProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectivePermutationProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.PermutationEncoding", "PermutationEncoding", "Permutation")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectivePermutationProgrammableProblem(this, cloner);
    }
  }

  [Item("Symbolic Expression Tree Programmable Problem (single-objective)", "Represents a symbolic expression tree single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveSymbolicExpressionTreeProgrammableProblem : SingleObjectiveProgrammableProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree> {

    [StorableConstructor]
    private SingleObjectiveSymbolicExpressionTreeProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveSymbolicExpressionTreeProgrammableProblem(SingleObjectiveSymbolicExpressionTreeProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveSymbolicExpressionTreeProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "SymbolicExpressionTreeEncoding", "SymbolicExpressionTree")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveSymbolicExpressionTreeProgrammableProblem(this, cloner);
    }
  }

  [Item("Linear Linkage Programmable Problem (single-objective)", "Represents a linear linkage single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class SingleObjectiveLinearLinkageProgrammableProblem : SingleObjectiveProgrammableProblem<LinearLinkageEncoding, LinearLinkage> {

    [StorableConstructor]
    private SingleObjectiveLinearLinkageProgrammableProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveLinearLinkageProgrammableProblem(SingleObjectiveLinearLinkageProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveLinearLinkageProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledSingleObjectiveProblemDefinition, "HeuristicLab.Encodings.LinearLinkageEncoding", "LinearLinkageEncoding", "LinearLinkage")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveLinearLinkageProgrammableProblem(this, cloner);
    }
  }*/
}
