#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GeSymbolicRegressionSampleTest {
    #region artificial ant
    private const string GeArtificialAntSampleFileName = "GE_ArtificialAnt";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGeArtificialAntSampleTest() {
      var geaa = CreateGeArtificialAntSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, GeArtificialAntSampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(geaa, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGeArtificalAntSampleTest() {
      var ga = CreateGeArtificialAntSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
    }

    public OffspringSelectionGeneticAlgorithm CreateGeArtificialAntSample() {
      OffspringSelectionGeneticAlgorithm ga = new OffspringSelectionGeneticAlgorithm();

      #region Problem Configuration
      var problem = new HeuristicLab.Problems.GrammaticalEvolution.GEArtificialAntProblem();
      #endregion
      #region Algorithm Configuration
      ga.Name = "Grammatical Evolution - Artificial Ant (SantaFe)";
      ga.Description = "Grammatical evolution algorithm for solving a artificial ant problem";
      ga.Problem = problem;
      SamplesUtils.ConfigureOsGeneticAlgorithmParameters<GenderSpecificSelector, Encodings.IntegerVectorEncoding.SinglePointCrossover, Encodings.IntegerVectorEncoding.UniformOnePositionManipulator>(
        ga, 200, 1, 50, 0.05, 200);
      #endregion

      return ga;
    }
    #endregion

    #region symbolic regression
    private const string GeSymbolicRegressionSampleFileName = "GE_SymbReg";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGeSymbolicRegressionSampleTest() {
      var geSymbReg = CreateGeSymbolicRegressionSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, GeSymbolicRegressionSampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(geSymbReg, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGeSymbolicRegressionSampleTest() {
      var ga = CreateGeSymbolicRegressionSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
    }

    public OffspringSelectionGeneticAlgorithm CreateGeSymbolicRegressionSample() {
      var ga = new OffspringSelectionGeneticAlgorithm();

      #region Problem Configuration
      var problem = new HeuristicLab.Problems.GrammaticalEvolution.GESymbolicRegressionSingleObjectiveProblem();

      #endregion
      #region Algorithm Configuration
      ga.Name = "Grammatical Evolution - Symbolic Regression (Poly-10)";
      ga.Description = "Grammatical evolution algorithm for solving a symbolic regression problem problem";
      ga.Problem = problem;
      problem.Load(new PolyTen().GenerateRegressionData());

      // must occur after loading problem data because the grammar creates symbols for random constants once the data is loaded
      var consts = problem.SymbolicExpressionTreeGrammar.AllowedSymbols.OfType<Constant>().ToList();
      foreach (var c in consts) {
        problem.SymbolicExpressionTreeGrammar.RemoveSymbol(c);
      }

      SamplesUtils.ConfigureOsGeneticAlgorithmParameters<GenderSpecificSelector, Encodings.IntegerVectorEncoding.SinglePointCrossover, Encodings.IntegerVectorEncoding.UniformOnePositionManipulator>(
        ga, 1000, 1, 50, 0.05, 200);
      #endregion

      return ga;
    }
    #endregion
  }
}
