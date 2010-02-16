#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Permutation;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Routing.TSP {
  [Item("TSP", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable("Problems")]
  [EmptyStorableClass]
  public sealed class TSP : Problem {
    private ValueParameter<DoubleMatrixData> CoordinatesParameter {
      get { return (ValueParameter<DoubleMatrixData>)Parameters["Coordinates"]; }
    }
    private OperatorParameter SolutionGeneratorParameter {
      get { return (OperatorParameter)Parameters["SolutionGenerator"]; }
    }
    private OperatorParameter EvaluatorParameter {
      get { return (OperatorParameter)Parameters["Evaluator"]; }
    }

    public DoubleMatrixData Coordinates {
      get { return CoordinatesParameter.Value; }
      set { CoordinatesParameter.Value = value; }
    }
    public IOperator SolutionGenerator {
      get { return SolutionGeneratorParameter.Value; }
      set { SolutionGeneratorParameter.Value = value; }
    }
    public IOperator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }

    public TSP()
      : base() {
      Parameters.Add(new ValueParameter<BoolData>("Maximization", "Set to false as the TSP is a minimization problem.", new BoolData(false)));
      Parameters.Add(new ValueParameter<DoubleMatrixData>("Coordinates", "The x- and y-Coordinates of the cities.", new DoubleMatrixData(0, 0)));
      Parameters.Add(new ValueParameter<DoubleData>("BestKnownQuality", "The quality of the best known solution of this TSP instance."));
      Parameters.Add(new OperatorParameter("SolutionGenerator", "The operator which should be used to generate new solutions."));
      Parameters.Add(new OperatorParameter("Evaluator", "The operator which should be used to evaluate solutions."));
    }

    public void ImportFromTSPLIB(string filename) {
      TSPLIBParser parser = new TSPLIBParser(filename);
      parser.Parse();
      Coordinates = new DoubleMatrixData(parser.Vertices);
      int cities = Coordinates.Rows;
      RandomPermutationCreator creator = new RandomPermutationCreator();
      creator.LengthParameter.Value = new IntData(cities);
      SolutionGenerator = creator;
      TSPRoundedEuclideanPathEvaluator evaluator = new TSPRoundedEuclideanPathEvaluator();
      Evaluator = evaluator;
    }
  }
}
