/*
 * SVM.NET Library
 * Copyright (C) 2008 Matthew Johnson
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Adaptions to work with direct C# translation of the java libSVM source code (version 3.1.2) by Gabriel Kronberger
 * 
 */



namespace LibSVM {
  /// <summary>
  /// Deals with the scaling of Problems so they have uniform ranges across all dimensions in order to
  /// result in better SVM performance.
  /// </summary>
  public static class Scaling {
    /// <summary>
    /// Scales a problem using the provided range.  This will not affect the parameter.
    /// </summary>
    /// <param name="prob">The problem to scale</param>
    /// <param name="range">The Range transform to use in scaling</param>
    /// <returns>The Scaled problem</returns>
    public static svm_problem Scale(this RangeTransform range, svm_problem prob) {
      svm_problem scaledProblem = new svm_problem() { l = prob.l, y = new double[prob.l], x = new svm_node[prob.l][] };
      for (int i = 0; i < scaledProblem.l; i++) {
        scaledProblem.x[i] = new svm_node[prob.x[i].Length];
        for (int j = 0; j < scaledProblem.x[i].Length; j++)
          scaledProblem.x[i][j] = new svm_node() { index = prob.x[i][j].index, value = range.Transform(prob.x[i][j].value, prob.x[i][j].index) };
        scaledProblem.y[i] = prob.y[i];
      }
      return scaledProblem;
    }
  }
}
