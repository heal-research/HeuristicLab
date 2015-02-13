#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Analysis.Statistics {
  public class PairwiseTest {
    public static double TTest(double[] group1, double[] group2) {
      double left, right, both;
      alglib.unequalvariancettest(group1, group1.Length, group2, group2.Length, out both, out left, out right);
      return both;
    }

    public static double MannWhitneyUTest(double[] group1, double[] group2) {
      double mwuBothtails, mwuLefttail, mwuRighttail;
      alglib.mannwhitneyutest(group1, group1.Length, group2, group2.Length, out mwuBothtails, out mwuLefttail, out mwuRighttail);
      return mwuBothtails;
    }
  }
}
