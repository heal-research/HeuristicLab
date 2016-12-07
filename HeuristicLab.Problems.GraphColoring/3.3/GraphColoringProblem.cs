#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.GraphColoring {
  [Item("Graph Coloring Problem (GCP)", "Attempts to find a coloring using a minimal number of colors that doesn't produce a conflict.")]
  [StorableClass]
  public sealed class GraphColoringProblem : SingleObjectiveBasicProblem<LinearLinkageEncoding> {

    public override bool Maximization {
      get { return false; }
    }

    private IValueParameter<BoolMatrix> adjacencyMatrixParameter;
    public IValueParameter<BoolMatrix> AdjacencyMatrixParameter {
      get { return adjacencyMatrixParameter; }
    }

    [StorableConstructor]
    private GraphColoringProblem(bool deserializing) : base(deserializing) { }
    private GraphColoringProblem(GraphColoringProblem original, Cloner cloner)
      : base(original, cloner) {
      adjacencyMatrixParameter = cloner.Clone(original.adjacencyMatrixParameter);
    }
    public GraphColoringProblem() {
      Encoding = new LinearLinkageEncoding("lle") { Length = 32 };
      Parameters.Add(adjacencyMatrixParameter = new ValueParameter<BoolMatrix>("AdjacencyMatrix", "The matrix that describes whether two nodes are linked."));
      
      var rand = new System.Random(0);
      var matrix = new BoolMatrix(32, 32);
      for (var i = 0; i < 31; i++) {
        for (var j = i + 1; j < 32; j++) {
          matrix[i, j] = rand.NextDouble() < 0.2;
          matrix[j, i] = matrix[i, j];
        }
      }
      AdjacencyMatrixParameter.Value = matrix;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GraphColoringProblem(this, cloner);
    }

    public override double Evaluate(Individual individual, IRandom random) {
      var matrix = adjacencyMatrixParameter.Value;
      var lle = individual.LinearLinkage("lle");
      var groups = lle.GetGroups();
      var conflicts = 0;
      var colors = 0;
      foreach (var g in groups) {
        colors++;
        for (var i = 0; i < g.Count - 1; i++) {
          for (var j = i + 1; j < g.Count; j++) {
            if (matrix[g[i], g[j]]) conflicts++; // both nodes are adjacent and have the same color (are in the same group)
          }
        }
      }

      var mag = Math.Pow(10, -(int)Math.Ceiling(Math.Log10(matrix.Rows)));
      return conflicts + colors * mag;
    }
  }
}
