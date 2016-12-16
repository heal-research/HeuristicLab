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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.GraphColoring {
  public enum FitnessFunction { Prioritized, Penalized }
  [Item("Graph Coloring Problem (GCP)", "Attempts to find a coloring using a minimal number of colors that doesn't produce a conflict.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 135)]
  [StorableClass]
  public sealed class GraphColoringProblem : SingleObjectiveBasicProblem<LinearLinkageEncoding>, IProblemInstanceConsumer<GCPData> {

    public override bool Maximization {
      get { return false; }
    }

    [Storable]
    private IValueParameter<IntMatrix> adjacencyListParameter;
    public IValueParameter<IntMatrix> AdjacencyListParameter {
      get { return adjacencyListParameter; }
    }
    [Storable]
    private IValueParameter<EnumValue<FitnessFunction>> fitnessFunctionParameter;
    public IValueParameter<EnumValue<FitnessFunction>> FitnessFunctionParameter {
      get { return fitnessFunctionParameter; }
    }
    public FitnessFunction FitnessFunction {
      get { return fitnessFunctionParameter.Value.Value; }
      set { fitnessFunctionParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private GraphColoringProblem(bool deserializing) : base(deserializing) { }
    private GraphColoringProblem(GraphColoringProblem original, Cloner cloner)
      : base(original, cloner) {
      adjacencyListParameter = cloner.Clone(original.adjacencyListParameter);
      fitnessFunctionParameter = cloner.Clone(original.fitnessFunctionParameter);
      RegisterEventHandlers();
    }
    public GraphColoringProblem() {
      Encoding = new LinearLinkageEncoding("lle");
      Parameters.Add(adjacencyListParameter = new ValueParameter<IntMatrix>("Adjacency List", "The adjacency list that describes the (symmetric) edges in the graph with nodes from 0 to N-1."));
      Parameters.Add(fitnessFunctionParameter = new ValueParameter<EnumValue<FitnessFunction>>("Fitness Function", "The function to use for evaluating the quality of a solution.", new EnumValue<FitnessFunction>(FitnessFunction.Penalized)));

      var imat = new IntMatrix(defaultInstance.Length, 2);
      for (var i = 0; i < defaultInstance.Length; i++) {
        imat[i, 0] = defaultInstance[i].Item1 - 1;
        imat[i, 1] = defaultInstance[i].Item2 - 1;
      }
      Encoding.Length = defaultInstanceNodes;
      AdjacencyListParameter.Value = imat;
      BestKnownQualityParameter.Value = null;

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GraphColoringProblem(this, cloner);
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();

      Parameterize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      fitnessFunctionParameter.ValueChanged += FitnessFunctionParameterOnValueChanged;
      fitnessFunctionParameter.Value.ValueChanged += FitnessFunctionOnValueChanged;
    }

    private void FitnessFunctionParameterOnValueChanged(object sender, EventArgs eventArgs) {
      fitnessFunctionParameter.Value.ValueChanged += FitnessFunctionOnValueChanged;
      OnReset();
    }

    private void FitnessFunctionOnValueChanged(object sender, EventArgs eventArgs) {
      BestKnownQualityParameter.Value = null;
      OnReset();
    }

    public override double Evaluate(Individual individual, IRandom random) {
      var adjList = adjacencyListParameter.Value;
      var llee = individual.LinearLinkage(Encoding.Name).ToEndLinks(); // LLE-e encoding uses the highest indexed member as group number

      switch (FitnessFunction) {
        case FitnessFunction.Prioritized: {
            var colors = llee.Distinct().Count();
            var conflicts = CalculateConflicts(llee);
            // number of conflicts is the integer part of the quality
            // number of colors constitutes the fractional part
            // the number of fractional digits is defined by the maximum number of possible colors (each node its own color)
            var mag = Math.Pow(10, -(int)Math.Ceiling(Math.Log10(llee.Length)));
            // the value is e.g. 4.03 for 4 conflicts with 3 colors (and less than 100 nodes)
            return conflicts + colors * mag;
          }
        case FitnessFunction.Penalized: {
            // Fitness function from
            // David S. Johnson, Cecilia R. Aragon, Lyle A. McGeoch, and Catherine Schevon. 1991.
            // Optimization by simulated annealing: An experimental evaluation; part II, graph coloring and number partitioning.
            // Operations Research 39(3), pp. 378–406.
            // All local optima of this function correspond to legal colorings.
            // We need to calculate conflicts and nodes per color
            var colors = llee.GroupBy(x => x).ToDictionary(x => x.Key, x => new EvaluationHelper() { ColorCount = x.Count() });
            for (var r = 0; r < adjList.Rows; r++) {
              var color1 = llee[adjList[r, 0]];
              var color2 = llee[adjList[r, 1]];
              if (color1 == color2) colors[color1].ConflictCount++;
            }
            return 2 * colors.Sum(x => x.Value.ColorCount * x.Value.ConflictCount) - colors.Sum(x => x.Value.ColorCount * x.Value.ColorCount);
          }
        default: throw new InvalidOperationException(string.Format("Unknown fitness function {0}.", FitnessFunction));
      }
    }

    private class EvaluationHelper {
      public int ColorCount { get; set; }
      public int ConflictCount { get; set; }
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      var best = Maximization ? orderedIndividuals.Last().Individual.LinearLinkage(Encoding.Name) : orderedIndividuals.First().Individual.LinearLinkage(Encoding.Name);
        
      var lle = best.ToEndLinks();
      var colors = lle.Distinct().Count();
      var conflicts = CalculateConflicts(lle);

      IResult res;
      if (!results.TryGetValue("Best Solution Colors", out res)) {
        res = new Result("Best Solution Colors", new IntValue(colors));
        results.Add(res);
      } else ((IntValue)res.Value).Value = colors;
      if (!results.TryGetValue("Best Solution Conflicts", out res)) {
        res = new Result("Best Solution Conflicts", new IntValue(conflicts));
        results.Add(res);
      } else ((IntValue)res.Value).Value = conflicts;
    }

    private int CalculateConflicts(int[] llee) {
      var adjList = AdjacencyListParameter.Value;
      var conflicts = 0;
      for (var r = 0; r < adjList.Rows; r++) {
        if (llee[adjList[r, 0]] == llee[adjList[r, 1]]) conflicts++; // both nodes are adjacent and have the same color (are in the same group)
      }
      return conflicts;
    }

    public void Load(GCPData data) {
      Encoding.Length = data.Nodes;
      AdjacencyListParameter.Value = new IntMatrix(data.Adjacencies);
      if (data.BestKnownColors.HasValue && FitnessFunction == FitnessFunction.Prioritized) {
        var mag = Math.Pow(10, -(int)Math.Ceiling(Math.Log10(data.Nodes)));
        // the value is e.g. 0.051 for 0 conflicts with 51 colors (and less than 1000 nodes)
        BestKnownQuality = data.BestKnownColors.Value * mag;
      } else if (data.BestKnownColoring != null && FitnessFunction == FitnessFunction.Prioritized) {
        var mag = Math.Pow(10, -(int)Math.Ceiling(Math.Log10(data.Nodes)));
        var colors = data.BestKnownColoring.Distinct().Count();
        BestKnownQuality = colors * mag;
      } else if (data.BestKnownColoring != null && FitnessFunction == FitnessFunction.Penalized) {
        var colors = data.BestKnownColoring.GroupBy(x => x).Select(x => x.Count());
        BestKnownQuality = -colors.Sum(x => x * x);
      } else BestKnownQualityParameter.Value = null;
      Name = data.Name;
      Description = data.Description;
      OnReset();
    }

    private void InitializeOperators() {
      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
    }

    private void Parameterize() {
      foreach (var simCalc in Operators.OfType<ISolutionSimilarityCalculator>()) {
        simCalc.SolutionVariableName = Encoding.Name;
        simCalc.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    #region Default Instance (myciel6.col)
    private static readonly int defaultInstanceNodes = 95;
    private static readonly Tuple<int, int>[] defaultInstance = {
Tuple.Create(1, 2),
Tuple.Create(1, 4),
Tuple.Create(1, 7),
Tuple.Create(1, 9),
Tuple.Create(1, 13),
Tuple.Create(1, 15),
Tuple.Create(1, 18),
Tuple.Create(1, 20),
Tuple.Create(1, 25),
Tuple.Create(1, 27),
Tuple.Create(1, 30),
Tuple.Create(1, 32),
Tuple.Create(1, 36),
Tuple.Create(1, 38),
Tuple.Create(1, 41),
Tuple.Create(1, 43),
Tuple.Create(1, 49),
Tuple.Create(1, 51),
Tuple.Create(1, 54),
Tuple.Create(1, 56),
Tuple.Create(1, 60),
Tuple.Create(1, 62),
Tuple.Create(1, 65),
Tuple.Create(1, 67),
Tuple.Create(1, 72),
Tuple.Create(1, 74),
Tuple.Create(1, 77),
Tuple.Create(1, 79),
Tuple.Create(1, 83),
Tuple.Create(1, 85),
Tuple.Create(1, 88),
Tuple.Create(1, 90),
Tuple.Create(2, 3),
Tuple.Create(2, 6),
Tuple.Create(2, 8),
Tuple.Create(2, 12),
Tuple.Create(2, 14),
Tuple.Create(2, 17),
Tuple.Create(2, 19),
Tuple.Create(2, 24),
Tuple.Create(2, 26),
Tuple.Create(2, 29),
Tuple.Create(2, 31),
Tuple.Create(2, 35),
Tuple.Create(2, 37),
Tuple.Create(2, 40),
Tuple.Create(2, 42),
Tuple.Create(2, 48),
Tuple.Create(2, 50),
Tuple.Create(2, 53),
Tuple.Create(2, 55),
Tuple.Create(2, 59),
Tuple.Create(2, 61),
Tuple.Create(2, 64),
Tuple.Create(2, 66),
Tuple.Create(2, 71),
Tuple.Create(2, 73),
Tuple.Create(2, 76),
Tuple.Create(2, 78),
Tuple.Create(2, 82),
Tuple.Create(2, 84),
Tuple.Create(2, 87),
Tuple.Create(2, 89),
Tuple.Create(3, 5),
Tuple.Create(3, 7),
Tuple.Create(3, 10),
Tuple.Create(3, 13),
Tuple.Create(3, 16),
Tuple.Create(3, 18),
Tuple.Create(3, 21),
Tuple.Create(3, 25),
Tuple.Create(3, 28),
Tuple.Create(3, 30),
Tuple.Create(3, 33),
Tuple.Create(3, 36),
Tuple.Create(3, 39),
Tuple.Create(3, 41),
Tuple.Create(3, 44),
Tuple.Create(3, 49),
Tuple.Create(3, 52),
Tuple.Create(3, 54),
Tuple.Create(3, 57),
Tuple.Create(3, 60),
Tuple.Create(3, 63),
Tuple.Create(3, 65),
Tuple.Create(3, 68),
Tuple.Create(3, 72),
Tuple.Create(3, 75),
Tuple.Create(3, 77),
Tuple.Create(3, 80),
Tuple.Create(3, 83),
Tuple.Create(3, 86),
Tuple.Create(3, 88),
Tuple.Create(3, 91),
Tuple.Create(4, 5),
Tuple.Create(4, 6),
Tuple.Create(4, 10),
Tuple.Create(4, 12),
Tuple.Create(4, 16),
Tuple.Create(4, 17),
Tuple.Create(4, 21),
Tuple.Create(4, 24),
Tuple.Create(4, 28),
Tuple.Create(4, 29),
Tuple.Create(4, 33),
Tuple.Create(4, 35),
Tuple.Create(4, 39),
Tuple.Create(4, 40),
Tuple.Create(4, 44),
Tuple.Create(4, 48),
Tuple.Create(4, 52),
Tuple.Create(4, 53),
Tuple.Create(4, 57),
Tuple.Create(4, 59),
Tuple.Create(4, 63),
Tuple.Create(4, 64),
Tuple.Create(4, 68),
Tuple.Create(4, 71),
Tuple.Create(4, 75),
Tuple.Create(4, 76),
Tuple.Create(4, 80),
Tuple.Create(4, 82),
Tuple.Create(4, 86),
Tuple.Create(4, 87),
Tuple.Create(4, 91),
Tuple.Create(5, 8),
Tuple.Create(5, 9),
Tuple.Create(5, 14),
Tuple.Create(5, 15),
Tuple.Create(5, 19),
Tuple.Create(5, 20),
Tuple.Create(5, 26),
Tuple.Create(5, 27),
Tuple.Create(5, 31),
Tuple.Create(5, 32),
Tuple.Create(5, 37),
Tuple.Create(5, 38),
Tuple.Create(5, 42),
Tuple.Create(5, 43),
Tuple.Create(5, 50),
Tuple.Create(5, 51),
Tuple.Create(5, 55),
Tuple.Create(5, 56),
Tuple.Create(5, 61),
Tuple.Create(5, 62),
Tuple.Create(5, 66),
Tuple.Create(5, 67),
Tuple.Create(5, 73),
Tuple.Create(5, 74),
Tuple.Create(5, 78),
Tuple.Create(5, 79),
Tuple.Create(5, 84),
Tuple.Create(5, 85),
Tuple.Create(5, 89),
Tuple.Create(5, 90),
Tuple.Create(6, 11),
Tuple.Create(6, 13),
Tuple.Create(6, 15),
Tuple.Create(6, 22),
Tuple.Create(6, 25),
Tuple.Create(6, 27),
Tuple.Create(6, 34),
Tuple.Create(6, 36),
Tuple.Create(6, 38),
Tuple.Create(6, 45),
Tuple.Create(6, 49),
Tuple.Create(6, 51),
Tuple.Create(6, 58),
Tuple.Create(6, 60),
Tuple.Create(6, 62),
Tuple.Create(6, 69),
Tuple.Create(6, 72),
Tuple.Create(6, 74),
Tuple.Create(6, 81),
Tuple.Create(6, 83),
Tuple.Create(6, 85),
Tuple.Create(6, 92),
Tuple.Create(7, 11),
Tuple.Create(7, 12),
Tuple.Create(7, 14),
Tuple.Create(7, 22),
Tuple.Create(7, 24),
Tuple.Create(7, 26),
Tuple.Create(7, 34),
Tuple.Create(7, 35),
Tuple.Create(7, 37),
Tuple.Create(7, 45),
Tuple.Create(7, 48),
Tuple.Create(7, 50),
Tuple.Create(7, 58),
Tuple.Create(7, 59),
Tuple.Create(7, 61),
Tuple.Create(7, 69),
Tuple.Create(7, 71),
Tuple.Create(7, 73),
Tuple.Create(7, 81),
Tuple.Create(7, 82),
Tuple.Create(7, 84),
Tuple.Create(7, 92),
Tuple.Create(8, 11),
Tuple.Create(8, 13),
Tuple.Create(8, 16),
Tuple.Create(8, 22),
Tuple.Create(8, 25),
Tuple.Create(8, 28),
Tuple.Create(8, 34),
Tuple.Create(8, 36),
Tuple.Create(8, 39),
Tuple.Create(8, 45),
Tuple.Create(8, 49),
Tuple.Create(8, 52),
Tuple.Create(8, 58),
Tuple.Create(8, 60),
Tuple.Create(8, 63),
Tuple.Create(8, 69),
Tuple.Create(8, 72),
Tuple.Create(8, 75),
Tuple.Create(8, 81),
Tuple.Create(8, 83),
Tuple.Create(8, 86),
Tuple.Create(8, 92),
Tuple.Create(9, 11),
Tuple.Create(9, 12),
Tuple.Create(9, 16),
Tuple.Create(9, 22),
Tuple.Create(9, 24),
Tuple.Create(9, 28),
Tuple.Create(9, 34),
Tuple.Create(9, 35),
Tuple.Create(9, 39),
Tuple.Create(9, 45),
Tuple.Create(9, 48),
Tuple.Create(9, 52),
Tuple.Create(9, 58),
Tuple.Create(9, 59),
Tuple.Create(9, 63),
Tuple.Create(9, 69),
Tuple.Create(9, 71),
Tuple.Create(9, 75),
Tuple.Create(9, 81),
Tuple.Create(9, 82),
Tuple.Create(9, 86),
Tuple.Create(9, 92),
Tuple.Create(10, 11),
Tuple.Create(10, 14),
Tuple.Create(10, 15),
Tuple.Create(10, 22),
Tuple.Create(10, 26),
Tuple.Create(10, 27),
Tuple.Create(10, 34),
Tuple.Create(10, 37),
Tuple.Create(10, 38),
Tuple.Create(10, 45),
Tuple.Create(10, 50),
Tuple.Create(10, 51),
Tuple.Create(10, 58),
Tuple.Create(10, 61),
Tuple.Create(10, 62),
Tuple.Create(10, 69),
Tuple.Create(10, 73),
Tuple.Create(10, 74),
Tuple.Create(10, 81),
Tuple.Create(10, 84),
Tuple.Create(10, 85),
Tuple.Create(10, 92),
Tuple.Create(11, 17),
Tuple.Create(11, 18),
Tuple.Create(11, 19),
Tuple.Create(11, 20),
Tuple.Create(11, 21),
Tuple.Create(11, 29),
Tuple.Create(11, 30),
Tuple.Create(11, 31),
Tuple.Create(11, 32),
Tuple.Create(11, 33),
Tuple.Create(11, 40),
Tuple.Create(11, 41),
Tuple.Create(11, 42),
Tuple.Create(11, 43),
Tuple.Create(11, 44),
Tuple.Create(11, 53),
Tuple.Create(11, 54),
Tuple.Create(11, 55),
Tuple.Create(11, 56),
Tuple.Create(11, 57),
Tuple.Create(11, 64),
Tuple.Create(11, 65),
Tuple.Create(11, 66),
Tuple.Create(11, 67),
Tuple.Create(11, 68),
Tuple.Create(11, 76),
Tuple.Create(11, 77),
Tuple.Create(11, 78),
Tuple.Create(11, 79),
Tuple.Create(11, 80),
Tuple.Create(11, 87),
Tuple.Create(11, 88),
Tuple.Create(11, 89),
Tuple.Create(11, 90),
Tuple.Create(11, 91),
Tuple.Create(12, 23),
Tuple.Create(12, 25),
Tuple.Create(12, 27),
Tuple.Create(12, 30),
Tuple.Create(12, 32),
Tuple.Create(12, 46),
Tuple.Create(12, 49),
Tuple.Create(12, 51),
Tuple.Create(12, 54),
Tuple.Create(12, 56),
Tuple.Create(12, 70),
Tuple.Create(12, 72),
Tuple.Create(12, 74),
Tuple.Create(12, 77),
Tuple.Create(12, 79),
Tuple.Create(12, 93),
Tuple.Create(13, 23),
Tuple.Create(13, 24),
Tuple.Create(13, 26),
Tuple.Create(13, 29),
Tuple.Create(13, 31),
Tuple.Create(13, 46),
Tuple.Create(13, 48),
Tuple.Create(13, 50),
Tuple.Create(13, 53),
Tuple.Create(13, 55),
Tuple.Create(13, 70),
Tuple.Create(13, 71),
Tuple.Create(13, 73),
Tuple.Create(13, 76),
Tuple.Create(13, 78),
Tuple.Create(13, 93),
Tuple.Create(14, 23),
Tuple.Create(14, 25),
Tuple.Create(14, 28),
Tuple.Create(14, 30),
Tuple.Create(14, 33),
Tuple.Create(14, 46),
Tuple.Create(14, 49),
Tuple.Create(14, 52),
Tuple.Create(14, 54),
Tuple.Create(14, 57),
Tuple.Create(14, 70),
Tuple.Create(14, 72),
Tuple.Create(14, 75),
Tuple.Create(14, 77),
Tuple.Create(14, 80),
Tuple.Create(14, 93),
Tuple.Create(15, 23),
Tuple.Create(15, 24),
Tuple.Create(15, 28),
Tuple.Create(15, 29),
Tuple.Create(15, 33),
Tuple.Create(15, 46),
Tuple.Create(15, 48),
Tuple.Create(15, 52),
Tuple.Create(15, 53),
Tuple.Create(15, 57),
Tuple.Create(15, 70),
Tuple.Create(15, 71),
Tuple.Create(15, 75),
Tuple.Create(15, 76),
Tuple.Create(15, 80),
Tuple.Create(15, 93),
Tuple.Create(16, 23),
Tuple.Create(16, 26),
Tuple.Create(16, 27),
Tuple.Create(16, 31),
Tuple.Create(16, 32),
Tuple.Create(16, 46),
Tuple.Create(16, 50),
Tuple.Create(16, 51),
Tuple.Create(16, 55),
Tuple.Create(16, 56),
Tuple.Create(16, 70),
Tuple.Create(16, 73),
Tuple.Create(16, 74),
Tuple.Create(16, 78),
Tuple.Create(16, 79),
Tuple.Create(16, 93),
Tuple.Create(17, 23),
Tuple.Create(17, 25),
Tuple.Create(17, 27),
Tuple.Create(17, 34),
Tuple.Create(17, 46),
Tuple.Create(17, 49),
Tuple.Create(17, 51),
Tuple.Create(17, 58),
Tuple.Create(17, 70),
Tuple.Create(17, 72),
Tuple.Create(17, 74),
Tuple.Create(17, 81),
Tuple.Create(17, 93),
Tuple.Create(18, 23),
Tuple.Create(18, 24),
Tuple.Create(18, 26),
Tuple.Create(18, 34),
Tuple.Create(18, 46),
Tuple.Create(18, 48),
Tuple.Create(18, 50),
Tuple.Create(18, 58),
Tuple.Create(18, 70),
Tuple.Create(18, 71),
Tuple.Create(18, 73),
Tuple.Create(18, 81),
Tuple.Create(18, 93),
Tuple.Create(19, 23),
Tuple.Create(19, 25),
Tuple.Create(19, 28),
Tuple.Create(19, 34),
Tuple.Create(19, 46),
Tuple.Create(19, 49),
Tuple.Create(19, 52),
Tuple.Create(19, 58),
Tuple.Create(19, 70),
Tuple.Create(19, 72),
Tuple.Create(19, 75),
Tuple.Create(19, 81),
Tuple.Create(19, 93),
Tuple.Create(20, 23),
Tuple.Create(20, 24),
Tuple.Create(20, 28),
Tuple.Create(20, 34),
Tuple.Create(20, 46),
Tuple.Create(20, 48),
Tuple.Create(20, 52),
Tuple.Create(20, 58),
Tuple.Create(20, 70),
Tuple.Create(20, 71),
Tuple.Create(20, 75),
Tuple.Create(20, 81),
Tuple.Create(20, 93),
Tuple.Create(21, 23),
Tuple.Create(21, 26),
Tuple.Create(21, 27),
Tuple.Create(21, 34),
Tuple.Create(21, 46),
Tuple.Create(21, 50),
Tuple.Create(21, 51),
Tuple.Create(21, 58),
Tuple.Create(21, 70),
Tuple.Create(21, 73),
Tuple.Create(21, 74),
Tuple.Create(21, 81),
Tuple.Create(21, 93),
Tuple.Create(22, 23),
Tuple.Create(22, 29),
Tuple.Create(22, 30),
Tuple.Create(22, 31),
Tuple.Create(22, 32),
Tuple.Create(22, 33),
Tuple.Create(22, 46),
Tuple.Create(22, 53),
Tuple.Create(22, 54),
Tuple.Create(22, 55),
Tuple.Create(22, 56),
Tuple.Create(22, 57),
Tuple.Create(22, 70),
Tuple.Create(22, 76),
Tuple.Create(22, 77),
Tuple.Create(22, 78),
Tuple.Create(22, 79),
Tuple.Create(22, 80),
Tuple.Create(22, 93),
Tuple.Create(23, 35),
Tuple.Create(23, 36),
Tuple.Create(23, 37),
Tuple.Create(23, 38),
Tuple.Create(23, 39),
Tuple.Create(23, 40),
Tuple.Create(23, 41),
Tuple.Create(23, 42),
Tuple.Create(23, 43),
Tuple.Create(23, 44),
Tuple.Create(23, 45),
Tuple.Create(23, 59),
Tuple.Create(23, 60),
Tuple.Create(23, 61),
Tuple.Create(23, 62),
Tuple.Create(23, 63),
Tuple.Create(23, 64),
Tuple.Create(23, 65),
Tuple.Create(23, 66),
Tuple.Create(23, 67),
Tuple.Create(23, 68),
Tuple.Create(23, 69),
Tuple.Create(23, 82),
Tuple.Create(23, 83),
Tuple.Create(23, 84),
Tuple.Create(23, 85),
Tuple.Create(23, 86),
Tuple.Create(23, 87),
Tuple.Create(23, 88),
Tuple.Create(23, 89),
Tuple.Create(23, 90),
Tuple.Create(23, 91),
Tuple.Create(23, 92),
Tuple.Create(24, 47),
Tuple.Create(24, 49),
Tuple.Create(24, 51),
Tuple.Create(24, 54),
Tuple.Create(24, 56),
Tuple.Create(24, 60),
Tuple.Create(24, 62),
Tuple.Create(24, 65),
Tuple.Create(24, 67),
Tuple.Create(24, 94),
Tuple.Create(25, 47),
Tuple.Create(25, 48),
Tuple.Create(25, 50),
Tuple.Create(25, 53),
Tuple.Create(25, 55),
Tuple.Create(25, 59),
Tuple.Create(25, 61),
Tuple.Create(25, 64),
Tuple.Create(25, 66),
Tuple.Create(25, 94),
Tuple.Create(26, 47),
Tuple.Create(26, 49),
Tuple.Create(26, 52),
Tuple.Create(26, 54),
Tuple.Create(26, 57),
Tuple.Create(26, 60),
Tuple.Create(26, 63),
Tuple.Create(26, 65),
Tuple.Create(26, 68),
Tuple.Create(26, 94),
Tuple.Create(27, 47),
Tuple.Create(27, 48),
Tuple.Create(27, 52),
Tuple.Create(27, 53),
Tuple.Create(27, 57),
Tuple.Create(27, 59),
Tuple.Create(27, 63),
Tuple.Create(27, 64),
Tuple.Create(27, 68),
Tuple.Create(27, 94),
Tuple.Create(28, 47),
Tuple.Create(28, 50),
Tuple.Create(28, 51),
Tuple.Create(28, 55),
Tuple.Create(28, 56),
Tuple.Create(28, 61),
Tuple.Create(28, 62),
Tuple.Create(28, 66),
Tuple.Create(28, 67),
Tuple.Create(28, 94),
Tuple.Create(29, 47),
Tuple.Create(29, 49),
Tuple.Create(29, 51),
Tuple.Create(29, 58),
Tuple.Create(29, 60),
Tuple.Create(29, 62),
Tuple.Create(29, 69),
Tuple.Create(29, 94),
Tuple.Create(30, 47),
Tuple.Create(30, 48),
Tuple.Create(30, 50),
Tuple.Create(30, 58),
Tuple.Create(30, 59),
Tuple.Create(30, 61),
Tuple.Create(30, 69),
Tuple.Create(30, 94),
Tuple.Create(31, 47),
Tuple.Create(31, 49),
Tuple.Create(31, 52),
Tuple.Create(31, 58),
Tuple.Create(31, 60),
Tuple.Create(31, 63),
Tuple.Create(31, 69),
Tuple.Create(31, 94),
Tuple.Create(32, 47),
Tuple.Create(32, 48),
Tuple.Create(32, 52),
Tuple.Create(32, 58),
Tuple.Create(32, 59),
Tuple.Create(32, 63),
Tuple.Create(32, 69),
Tuple.Create(32, 94),
Tuple.Create(33, 47),
Tuple.Create(33, 50),
Tuple.Create(33, 51),
Tuple.Create(33, 58),
Tuple.Create(33, 61),
Tuple.Create(33, 62),
Tuple.Create(33, 69),
Tuple.Create(33, 94),
Tuple.Create(34, 47),
Tuple.Create(34, 53),
Tuple.Create(34, 54),
Tuple.Create(34, 55),
Tuple.Create(34, 56),
Tuple.Create(34, 57),
Tuple.Create(34, 64),
Tuple.Create(34, 65),
Tuple.Create(34, 66),
Tuple.Create(34, 67),
Tuple.Create(34, 68),
Tuple.Create(34, 94),
Tuple.Create(35, 47),
Tuple.Create(35, 49),
Tuple.Create(35, 51),
Tuple.Create(35, 54),
Tuple.Create(35, 56),
Tuple.Create(35, 70),
Tuple.Create(35, 94),
Tuple.Create(36, 47),
Tuple.Create(36, 48),
Tuple.Create(36, 50),
Tuple.Create(36, 53),
Tuple.Create(36, 55),
Tuple.Create(36, 70),
Tuple.Create(36, 94),
Tuple.Create(37, 47),
Tuple.Create(37, 49),
Tuple.Create(37, 52),
Tuple.Create(37, 54),
Tuple.Create(37, 57),
Tuple.Create(37, 70),
Tuple.Create(37, 94),
Tuple.Create(38, 47),
Tuple.Create(38, 48),
Tuple.Create(38, 52),
Tuple.Create(38, 53),
Tuple.Create(38, 57),
Tuple.Create(38, 70),
Tuple.Create(38, 94),
Tuple.Create(39, 47),
Tuple.Create(39, 50),
Tuple.Create(39, 51),
Tuple.Create(39, 55),
Tuple.Create(39, 56),
Tuple.Create(39, 70),
Tuple.Create(39, 94),
Tuple.Create(40, 47),
Tuple.Create(40, 49),
Tuple.Create(40, 51),
Tuple.Create(40, 58),
Tuple.Create(40, 70),
Tuple.Create(40, 94),
Tuple.Create(41, 47),
Tuple.Create(41, 48),
Tuple.Create(41, 50),
Tuple.Create(41, 58),
Tuple.Create(41, 70),
Tuple.Create(41, 94),
Tuple.Create(42, 47),
Tuple.Create(42, 49),
Tuple.Create(42, 52),
Tuple.Create(42, 58),
Tuple.Create(42, 70),
Tuple.Create(42, 94),
Tuple.Create(43, 47),
Tuple.Create(43, 48),
Tuple.Create(43, 52),
Tuple.Create(43, 58),
Tuple.Create(43, 70),
Tuple.Create(43, 94),
Tuple.Create(44, 47),
Tuple.Create(44, 50),
Tuple.Create(44, 51),
Tuple.Create(44, 58),
Tuple.Create(44, 70),
Tuple.Create(44, 94),
Tuple.Create(45, 47),
Tuple.Create(45, 53),
Tuple.Create(45, 54),
Tuple.Create(45, 55),
Tuple.Create(45, 56),
Tuple.Create(45, 57),
Tuple.Create(45, 70),
Tuple.Create(45, 94),
Tuple.Create(46, 47),
Tuple.Create(46, 59),
Tuple.Create(46, 60),
Tuple.Create(46, 61),
Tuple.Create(46, 62),
Tuple.Create(46, 63),
Tuple.Create(46, 64),
Tuple.Create(46, 65),
Tuple.Create(46, 66),
Tuple.Create(46, 67),
Tuple.Create(46, 68),
Tuple.Create(46, 69),
Tuple.Create(46, 94),
Tuple.Create(47, 71),
Tuple.Create(47, 72),
Tuple.Create(47, 73),
Tuple.Create(47, 74),
Tuple.Create(47, 75),
Tuple.Create(47, 76),
Tuple.Create(47, 77),
Tuple.Create(47, 78),
Tuple.Create(47, 79),
Tuple.Create(47, 80),
Tuple.Create(47, 81),
Tuple.Create(47, 82),
Tuple.Create(47, 83),
Tuple.Create(47, 84),
Tuple.Create(47, 85),
Tuple.Create(47, 86),
Tuple.Create(47, 87),
Tuple.Create(47, 88),
Tuple.Create(47, 89),
Tuple.Create(47, 90),
Tuple.Create(47, 91),
Tuple.Create(47, 92),
Tuple.Create(47, 93),
Tuple.Create(48, 95),
Tuple.Create(49, 95),
Tuple.Create(50, 95),
Tuple.Create(51, 95),
Tuple.Create(52, 95),
Tuple.Create(53, 95),
Tuple.Create(54, 95),
Tuple.Create(55, 95),
Tuple.Create(56, 95),
Tuple.Create(57, 95),
Tuple.Create(58, 95),
Tuple.Create(59, 95),
Tuple.Create(60, 95),
Tuple.Create(61, 95),
Tuple.Create(62, 95),
Tuple.Create(63, 95),
Tuple.Create(64, 95),
Tuple.Create(65, 95),
Tuple.Create(66, 95),
Tuple.Create(67, 95),
Tuple.Create(68, 95),
Tuple.Create(69, 95),
Tuple.Create(70, 95),
Tuple.Create(71, 95),
Tuple.Create(72, 95),
Tuple.Create(73, 95),
Tuple.Create(74, 95),
Tuple.Create(75, 95),
Tuple.Create(76, 95),
Tuple.Create(77, 95),
Tuple.Create(78, 95),
Tuple.Create(79, 95),
Tuple.Create(80, 95),
Tuple.Create(81, 95),
Tuple.Create(82, 95),
Tuple.Create(83, 95),
Tuple.Create(84, 95),
Tuple.Create(85, 95),
Tuple.Create(86, 95),
Tuple.Create(87, 95),
Tuple.Create(88, 95),
Tuple.Create(89, 95),
Tuple.Create(90, 95),
Tuple.Create(91, 95),
Tuple.Create(92, 95),
Tuple.Create(93, 95),
Tuple.Create(94, 95)
    };
    #endregion
  }
}
