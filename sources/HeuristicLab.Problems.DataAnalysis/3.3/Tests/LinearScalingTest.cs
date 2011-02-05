#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class LinearScalingTest {
    [TestMethod]
    public void CalculateScalingParametersTest() {
      var testData = new double[,] {
     {5,1,1,1,2,1,3,1,1,2},
     {5,4,4,5,7,10,3,2,1,2},
     {3,1,1,1,2,2,3,1,1,2},
     {6,8,8,1,3,4,3,7,1,2},
     {4,1,1,3,2,1,3,1,1,2},
     {8,10,10,8,7,10,9,7,1,4},            
     {1,1,1,1,2,10,3,1,1,2},              
     {2,1,2,1,2,1,3,1,1,2},                 
     {2,1,1,1,2,1,1,1,5,2},                 
     {4,2,1,1,2,1,2,1,1,2},                   
     {1,1,1,1,1,1,3,1,1,2},    
     {2,1,1,1,2,1,2,1,1,2},                   
     {5,3,3,3,2,3,4,4,1,4},                          
     {8,7,5,10,7,9,5,5,4,4},          
     {7,4,6,4,6,1,4,3,1,4},                          
     {4,1,1,1,2,1,2,1,1,2},     
     {4,1,1,1,2,1,3,1,1,2},      
     {10,7,7,6,4,10,4,1,2,4},  
     {6,1,1,1,2,1,3,1,1,2},     
     {7,3,2,10,5,10,5,4,4,4},   
     {10,5,5,3,6,7,7,10,1,4} 
      };

      double alpha, beta;
      int n = testData.GetLength(0);
      {
        IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                select testData[rows, 0];
        IEnumerable<double> y = from rows in Enumerable.Range(0, n)
                                select testData[rows, 1];
        SymbolicRegressionScaledMeanSquaredErrorEvaluator.CalculateScalingParameters(x, y, out beta, out alpha);

        Assert.AreEqual(alpha, 2.757281, 1.0E-6);
        Assert.AreEqual(beta, 0.720267, 1.0E-6);

        IEnumerable<double> scaledY = from value in y select value * beta + alpha;
        Assert.AreEqual(x.Average(), scaledY.Average(), 1.0E-6);
      }
      {
        IEnumerable<double> x = from rows in Enumerable.Range(0, n)
                                select testData[rows, 2] * 1.0E3;
        IEnumerable<double> y = from rows in Enumerable.Range(0, n)
                                select testData[rows, 8] * 1.0E-3;
        SymbolicRegressionScaledMeanSquaredErrorEvaluator.CalculateScalingParameters(x, y, out beta, out alpha);

        IEnumerable<double> scaledY = from value in y select value * beta + alpha;
        Assert.AreEqual(x.Average(), scaledY.Average(), 1.0E-6);
      }
    }
  }
}
