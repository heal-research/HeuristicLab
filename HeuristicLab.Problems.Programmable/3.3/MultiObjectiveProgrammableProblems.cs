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

  //TODO
  /*
  [Item("Integer Vector Programmable Problem (multi-objective)", "Represents an integer vector multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveIntegerVectorProgrammableProblem : MultiObjectiveProgrammableProblem<IntegerVectorEncoding, IntegerVector> {

    [StorableConstructor]
    private MultiObjectiveIntegerVectorProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveIntegerVectorProgrammableProblem(MultiObjectiveIntegerVectorProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveIntegerVectorProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.IntegerVectorEncoding", "IntegerVectorEncoding", "IntegerVector")) { }

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
      : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.RealVectorEncoding", "RealVectorEncoding", "RealVector")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveRealVectorProgrammableProblem(this, cloner);
    }
  }

  [Item("Permutation Programmable Problem (multi-objective)", "Represents a permutation multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectivePermutationProgrammableProblem : MultiObjectiveProgrammableProblem<PermutationEncoding, Permutation> {

    [StorableConstructor]
    private MultiObjectivePermutationProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectivePermutationProgrammableProblem(MultiObjectivePermutationProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectivePermutationProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.PermutationEncoding", "PermutationEncoding", "Permutation")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectivePermutationProgrammableProblem(this, cloner);
    }
  }

  [Item("Symbolic Expression Tree Programmable Problem (multi-objective)", "Represents a symbolic expression tree multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveSymbolicExpressionTreeProgrammableProblem : MultiObjectiveProgrammableProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree> {

    [StorableConstructor]
    private MultiObjectiveSymbolicExpressionTreeProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveSymbolicExpressionTreeProgrammableProblem(MultiObjectiveSymbolicExpressionTreeProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveSymbolicExpressionTreeProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "SymbolicExpressionTreeEncoding", "SymbolicExpressionTree")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveSymbolicExpressionTreeProgrammableProblem(this, cloner);
    }
  }

  [Item("Linear Linkage Programmable Problem (multi-objective)", "Represents a linear linkage multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 100)]
  [StorableClass]
  public sealed class MultiObjectiveLinearLinkageProgrammableProblem : MultiObjectiveProgrammableProblem<LinearLinkageEncoding, LinearLinkage> {

    [StorableConstructor]
    private MultiObjectiveLinearLinkageProgrammableProblem(bool deserializing) : base(deserializing) { }
    private MultiObjectiveLinearLinkageProgrammableProblem(MultiObjectiveLinearLinkageProgrammableProblem original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveLinearLinkageProgrammableProblem()
      : base(string.Format(ScriptTemplates.CompiledMultiObjectiveProblemDefinition, "HeuristicLab.Encodings.LinearLinkageEncoding", "LinearLinkageEncoding", "LinearLinkage")) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveLinearLinkageProgrammableProblem(this, cloner);
    }
  }*/
}
