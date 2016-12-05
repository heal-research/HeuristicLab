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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("Basic Quadratic Assignment Problem (QAP)", "The Quadratic Assignment Problem (QAP) can be described as the problem of assigning N facilities to N fixed locations such that there is exactly one facility in each location and that the sum of the distances multiplied by the connection strength between the facilities becomes minimal.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 141)]
  [StorableClass]
  public sealed class QAPBasicProblem : SingleObjectiveBasicProblem<PermutationEncoding>,
    IProblemInstanceConsumer<QAPData>,
    IProblemInstanceConsumer<TSPData> {

    [Storable]
    private IValueParameter<DoubleMatrix> weightsParameter;
    public DoubleMatrix Weights {
      get { return weightsParameter.Value; }
      set { weightsParameter.Value = value; }
    }

    [Storable]
    private IValueParameter<DoubleMatrix> distancesParameter;
    public DoubleMatrix Distances {
      get { return distancesParameter.Value; }
      set { distancesParameter.Value = value; }
    }


    [StorableConstructor]
    private QAPBasicProblem(bool deserializing) : base(deserializing) { }
    private QAPBasicProblem(QAPBasicProblem original, Cloner cloner)
      : base(original, cloner) {
      weightsParameter = cloner.Clone(original.weightsParameter);
      distancesParameter = cloner.Clone(original.distancesParameter);
    }
    public QAPBasicProblem() {
      Parameters.Add(weightsParameter = new ValueParameter<DoubleMatrix>("Weights", "The weights matrix.", new DoubleMatrix(5, 5)));
      Parameters.Add(distancesParameter = new ValueParameter<DoubleMatrix>("Distances", "The distances matrix.", new DoubleMatrix(5, 5)));

      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPBasicProblem(this, cloner);
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    public override bool Maximization {
      get { return false; }
    }

    public override double Evaluate(Individual individual, IRandom random) {
      return QAPEvaluator.Apply(individual.Permutation(), Weights, Distances);
    }

    public void Load(QAPData data) {
      var weights = new DoubleMatrix(data.Weights);
      var distances = new DoubleMatrix(data.Distances);
      Name = data.Name;
      Description = data.Description;
      Load(weights, distances);
      if (data.BestKnownQuality.HasValue) BestKnownQuality = data.BestKnownQuality.Value;
      EvaluateAndLoadAssignment(data.BestKnownAssignment);
      OnReset();
    }

    public void Load(TSPData data) {
      if (data.Dimension > 1000)
        throw new System.IO.InvalidDataException("Instances with more than 1000 customers are not supported by the QAP.");
      var weights = new DoubleMatrix(data.Dimension, data.Dimension);
      for (int i = 0; i < data.Dimension; i++)
        weights[i, (i + 1) % data.Dimension] = 1;
      var distances = new DoubleMatrix(data.GetDistanceMatrix());
      Name = data.Name;
      Description = data.Description;
      Load(weights, distances);
      if (data.BestKnownQuality.HasValue) BestKnownQuality = data.BestKnownQuality.Value;
      EvaluateAndLoadAssignment(data.BestKnownTour);
      OnReset();
    }

    public void Load(DoubleMatrix weights, DoubleMatrix distances) {
      if (weights == null || weights.Rows == 0)
        throw new System.IO.InvalidDataException("The given instance does not contain weights!");
      if (weights.Rows != weights.Columns)
        throw new System.IO.InvalidDataException("The weights matrix is not a square matrix!");
      if (distances == null || distances.Rows == 0)
        throw new System.IO.InvalidDataException("The given instance does not contain distances!");
      if (distances.Rows != distances.Columns)
        throw new System.IO.InvalidDataException("The distances matrix is not a square matrix!");
      if (weights.Rows != distances.Columns)
        throw new System.IO.InvalidDataException("The weights matrix and the distance matrix are not of equal size!");

      Weights = weights;
      Distances = distances;
      Encoding.Length = Weights.Rows;

      BestKnownQuality = double.NaN;
    }

    public void EvaluateAndLoadAssignment(int[] assignment) {
      if (assignment == null || assignment.Length == 0) return;
      var vector = new Permutation(PermutationTypes.Absolute, assignment);
      var result = QAPEvaluator.Apply(vector, Weights, Distances);
      BestKnownQuality = result;
    }
  }
}
