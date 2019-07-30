#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("A6293516-C146-469D-B248-31B866A1D94F")]
  public sealed class RegressionTreeParameters : Item {
    [Storable]
    private readonly ISplitter splitter;
    [Storable]
    private readonly IPruning pruning;
    [Storable]
    private readonly ILeafModel leafModel;
    [Storable]
    private readonly int minLeafSize;
    [Storable]
    private readonly IRegressionProblemData problemData;
    [Storable]
    private readonly IRandom random;

    public ISplitter Splitter {
      get { return splitter; }
    }
    public IPruning Pruning {
      get { return pruning; }
    }
    public ILeafModel LeafModel {
      get { return leafModel; }
    }
    public int MinLeafSize {
      get { return minLeafSize; }
    }
    private IRegressionProblemData ProblemData {
      get { return problemData; }
    }
    public IRandom Random {
      get { return random; }
    }
    public IEnumerable<string> AllowedInputVariables {
      get { return ProblemData.AllowedInputVariables; }
    }
    public string TargetVariable {
      get { return ProblemData.TargetVariable; }
    }
    public IDataset Data {
      get { return ProblemData.Dataset; }
    }

    #region Constructors & Cloning
    [StorableConstructor]
    private RegressionTreeParameters(StorableConstructorFlag _) : base(_) { }
    private RegressionTreeParameters(RegressionTreeParameters original, Cloner cloner) : base(original, cloner) {
      problemData = cloner.Clone(original.problemData);
      random = cloner.Clone(original.random);
      leafModel = cloner.Clone(original.leafModel);
      splitter = cloner.Clone(original.splitter);
      pruning = cloner.Clone(original.pruning);
      minLeafSize = original.minLeafSize;
    }

    public RegressionTreeParameters(IPruning pruning, int minleafSize, ILeafModel leafModel,
      IRegressionProblemData problemData, IRandom random, ISplitter splitter) {
      this.problemData = problemData;
      this.random = random;
      this.leafModel = leafModel;
      this.splitter = splitter;
      this.pruning = pruning;
      minLeafSize = Math.Max(pruning.MinLeafSize(problemData, leafModel), Math.Max(minleafSize, leafModel.MinLeafSize(problemData)));
    }
    public RegressionTreeParameters(ILeafModel modeltype, IRegressionProblemData problemData, IRandom random) {
      this.problemData = problemData;
      this.random = random;
      leafModel = modeltype;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionTreeParameters(this, cloner);
    }
    #endregion
  }
}