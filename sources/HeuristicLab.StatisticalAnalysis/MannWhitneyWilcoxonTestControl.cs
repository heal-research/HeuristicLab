#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.StatisticalAnalysis {
  public partial class MannWhitneyWilcoxonTestControl : EditorBase {
    public MannWhitneyWilcoxonTestControl()
      : base() {
      InitializeComponent();
      alphaTextBox.Text = ((double)0.05).ToString();
    }

    private void ArrayToString<T>(T[] array, TextBox p) {
      StringBuilder s = new StringBuilder();
      foreach (T element in array)
        s.Append(element + "; ");
      s.Remove(s.Length - 2, 2);
      p.Text = s.ToString();
    }

    private double[] ConvertStringToArray(TextBox p) {
      string t = p.Text;
      string[] s = t.Split(new char[] { ';' });
      double[] tmp = new double[s.Length];
      try {
        for (int i = 0; i < s.Length; i++) {
          tmp[i] = double.Parse(s[i]);
        }
      }
      catch (FormatException) {
        return null;
      }
      return tmp;
    }

    private void testApproximateButton_Click(object sender, EventArgs e) {
      double[] p1 = ConvertStringToArray(p1TextBox);
      double[] p2 = ConvertStringToArray(p2TextBox);
      if (p1.Length < 10 || p2.Length < 10) {
        MessageBox.Show("Sample size is too small to approximate, provide at least 10 samples in each population.");
        return;
      }
      double alpha = Double.Parse(alphaTextBox.Text);
      double pVal = MannWhitneyWilcoxonTest.TwoTailedTest(p1, p2);
      if (pVal <= alpha) {
        resultLabel.Text = "The hypothesis H0 can be rejected at " + alpha.ToString() + ", the p-Value is " + pVal.ToString() + "\nThe samples are likely not to stem from the same distribution.";
      } else {
        resultLabel.Text = "The hypothesis H0 cannot be rejected at " + alpha.ToString();
      }
    }

    private void testExactButton_Click(object sender, EventArgs e) {
      double[] p1 = ConvertStringToArray(p1TextBox);
      double[] p2 = ConvertStringToArray(p2TextBox);
      if (p1.Length > 20 || p2.Length > 20) {
        MessageBox.Show("Sample size is too large for exact calculation, do not use more than 20 samples in each population.");
        return;
      }
      double alpha = Double.Parse(alphaTextBox.Text);
      if (MannWhitneyWilcoxonTest.TwoTailedTest(p1, p2, alpha)) {
        resultLabel.Text = "The hypothesis H0 can be rejected at " + alpha.ToString() + "\nThe samples are likely not to stem from the same distribution.";
      } else {
        resultLabel.Text = "The hypothesis H0 cannot be rejected at " + alpha.ToString();
      }
    }
  }
}
