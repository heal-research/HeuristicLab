#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  public partial class DataAnalysisImportTypeDialog : Form {

    public static readonly List<KeyValuePair<DateTimeFormatInfo, string>> dateTimeFormats = new List<KeyValuePair<DateTimeFormatInfo, string>>{
      new KeyValuePair<DateTimeFormatInfo, string>(DateTimeFormatInfo.GetInstance(new CultureInfo("de-DE")), "dd/mm/yyyy hh:MM:ss" ),
      new KeyValuePair<DateTimeFormatInfo, string>(DateTimeFormatInfo.InvariantInfo, "mm/dd/yyyy hh:MM:ss" ),
      new KeyValuePair<DateTimeFormatInfo, string>(DateTimeFormatInfo.InvariantInfo, "yyyy/mm/dd hh:MM:ss" ),
      new KeyValuePair<DateTimeFormatInfo, string>(DateTimeFormatInfo.InvariantInfo, "mm/yyyy/dd hh:MM:ss" )
    };

    public static readonly List<KeyValuePair<char, string>> POSSIBLE_SEPARATORS = new List<KeyValuePair<char, string>>{
      new KeyValuePair<char, string>(',', ", (Comma)" ),
      new KeyValuePair<char, string>(';', "; (Semicolon)" ),
      new KeyValuePair<char, string>('\t', "\\t (Tab)")
    };

    public static readonly List<KeyValuePair<NumberFormatInfo, string>> POSSIBLE_DECIMAL_SEPARATORS = new List<KeyValuePair<NumberFormatInfo, string>>{
      new KeyValuePair<NumberFormatInfo, string>(NumberFormatInfo.InvariantInfo, ". (Period)" ),
      new KeyValuePair<NumberFormatInfo, string>(NumberFormatInfo.GetInstance(new CultureInfo("de-DE")), ", (Comma)" )      
    };

    public string Path {
      get { return ProblemTextBox.Text; }
    }

    public DataAnalysisImportType ImportType {
      get {
        return new DataAnalysisImportType() {
          Shuffle = ShuffleDataCheckbox.Checked,
          Training = TrainingTestTrackBar.Value
        };
      }
    }

    public DataAnalysisCSVFormat CSVFormat {
      get {
        return new DataAnalysisCSVFormat() {
          Separator = (char)SeparatorComboBox.SelectedValue,
          NumberFormatInfo = (NumberFormatInfo)DecimalSeparatorComboBox.SelectedValue,
          DateTimeFormatInfo = (DateTimeFormatInfo)DateTimeFormatComboBox.SelectedValue
        };
      }
    }

    public DataAnalysisImportTypeDialog() {
      InitializeComponent();

      SeparatorComboBox.DataSource = POSSIBLE_SEPARATORS;
      SeparatorComboBox.ValueMember = "Key";
      SeparatorComboBox.DisplayMember = "Value";
      DecimalSeparatorComboBox.DataSource = POSSIBLE_DECIMAL_SEPARATORS;
      DecimalSeparatorComboBox.ValueMember = "Key";
      DecimalSeparatorComboBox.DisplayMember = "Value";
      DateTimeFormatComboBox.DataSource = dateTimeFormats;
      DateTimeFormatComboBox.ValueMember = "Key";
      DateTimeFormatComboBox.DisplayMember = "Value";
    }

    private void TrainingTestTrackBar_ValueChanged(object sender, System.EventArgs e) {
      TrainingLabel.Text = "Training: " + TrainingTestTrackBar.Value + " %";
      TestLabel.Text = "Test: " + (TrainingTestTrackBar.Maximum - TrainingTestTrackBar.Value) + " %";
    }

    protected virtual void OpenFileButtonClick(object sender, System.EventArgs e) {
      if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;

      ProblemTextBox.Text = openFileDialog.FileName;
      ParseCSVFile();
    }

    protected virtual void CSVFormatComboBoxSelectionChangeCommitted(object sender, EventArgs e) {
      if (string.IsNullOrEmpty(ProblemTextBox.Text)) return;

      ParseCSVFile();
    }

    protected void ParseCSVFile() {
      PreviewDatasetMatrix.Content = null;
      try {
        TableFileParser csvParser = new TableFileParser();
        csvParser.Parse(ProblemTextBox.Text,
                        (NumberFormatInfo)DecimalSeparatorComboBox.SelectedValue,
                        (DateTimeFormatInfo)DateTimeFormatComboBox.SelectedValue,
                        (char)SeparatorComboBox.SelectedValue);
        IEnumerable<string> variableNamesWithType = GetVariableNamesWithType(csvParser);
        PreviewDatasetMatrix.Content = new Dataset(variableNamesWithType, csvParser.Values);

        CheckAdditionalConstraints(csvParser);

        ErrorTextBox.Text = String.Empty;
        ErrorTextBox.Visible = false;
        OkButton.Enabled = true;
      }
      catch (Exception ex) {
        OkButton.Enabled = false;
        ErrorTextBox.Text = ex.Message;
        ErrorTextBox.Visible = true;
      }
    }

    protected virtual void CheckAdditionalConstraints(TableFileParser csvParser) {
      if (!csvParser.Values.Any(x => x is List<double>)) {
        throw new ArgumentException("No double column could be found!");
      }
    }

    private IEnumerable<string> GetVariableNamesWithType(TableFileParser csvParser) {
      IList<string> variableNamesWithType = csvParser.VariableNames.ToList();
      for (int i = 0; i < csvParser.Values.Count; i++) {
        if (csvParser.Values[i] is List<double>) {
          variableNamesWithType[i] += " (Double)";
        } else if (csvParser.Values[i] is List<string>) {
          variableNamesWithType[i] += " (String)";
        } else if (csvParser.Values[i] is List<DateTime>) {
          variableNamesWithType[i] += " (DateTime)";
        } else {
          throw new ArgumentException("The variable values must be of type List<double>, List<string> or List<DateTime>");
        }
      }
      return variableNamesWithType;
    }

    protected void ControlToolTip_DoubleClick(object sender, EventArgs e) {
      Control control = sender as Control;
      if (control != null) {
        using (TextDialog dialog = new TextDialog(control.Name, (string)control.Tag, true)) {
          dialog.ShowDialog(this);
        }
      }
    }
  }
}
