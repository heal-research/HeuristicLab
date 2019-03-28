#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public class SymbolicSolutionExcelExporter : IDataAnalysisSolutionExporter {
    protected const string TRAININGSTART = "TrainingStart";
    protected const string TRAININGEND = "TrainingEnd";
    protected const string TESTSTART = "TestStart";
    protected const string TESTEND = "TestEnd";


    public string FileTypeFilter {
      get { return "Excel 2007 file (*.xlsx)|*.xlsx"; }
    }
    public bool Supports(IDataAnalysisSolution solution) {
      return solution is ISymbolicDataAnalysisSolution &&
        solution is IRegressionSolution;
    }

    public virtual void Export(IDataAnalysisSolution solution, string fileName) {
      var symbSolution = solution as ISymbolicDataAnalysisSolution;
      if (symbSolution == null) throw new NotSupportedException("This solution cannot be exported to Excel");
      var formatter = new SymbolicDataAnalysisExpressionExcelFormatter();
      var formula = formatter.Format(symbSolution.Model.SymbolicExpressionTree, solution.ProblemData.Dataset);
      ExportChart(fileName, symbSolution, formula);
    }

    private void ExportChart(string fileName, ISymbolicDataAnalysisSolution solution, string formula) {
      FileInfo newFile = new FileInfo(fileName);
      if (newFile.Exists) {
        newFile.Delete();
        newFile = new FileInfo(fileName);
      }
      var formulaParts = formula.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

      using (ExcelPackage package = new ExcelPackage(newFile)) {
        ExcelWorksheet modelWorksheet = package.Workbook.Worksheets.Add("Model");
        FormatModelSheet(modelWorksheet, solution, formulaParts);

        ExcelWorksheet datasetWorksheet = package.Workbook.Worksheets.Add("Dataset");
        WriteDatasetToExcel(datasetWorksheet, solution.ProblemData);

        ExcelWorksheet inputsWorksheet = package.Workbook.Worksheets.Add("Inputs");
        WriteInputSheet(inputsWorksheet, datasetWorksheet, formulaParts.Skip(2), solution.ProblemData.Dataset);

        if (solution is IRegressionSolution) {
          ExcelWorksheet estimatedWorksheet = package.Workbook.Worksheets.Add("Estimated Values");
          WriteEstimatedWorksheet(estimatedWorksheet, datasetWorksheet, formulaParts, solution as IRegressionSolution);

          ExcelWorksheet chartsWorksheet = package.Workbook.Worksheets.Add("Charts");
          AddCharts(chartsWorksheet);
        }
        package.Workbook.Properties.Title = "Excel Export";
        package.Workbook.Properties.Author = "HEAL";
        package.Workbook.Properties.Comments = "Excel export of a symbolic data analysis solution from HeuristicLab";

        package.Save();
      }
    }

    private void FormatModelSheet(ExcelWorksheet modelWorksheet, ISymbolicDataAnalysisSolution solution, IEnumerable<string> formulaParts) {
      int row = 1;
      modelWorksheet.Cells[row, 1].Value = "Model";
      modelWorksheet.Cells[row, 2].Value = solution.Name;

      foreach (var part in formulaParts) {
        modelWorksheet.Cells[row, 4].Value = part;
        row++;
      }

      row = 2;
      modelWorksheet.Cells[row, 1].Value = "Model Depth";
      modelWorksheet.Cells[row, 2].Value = solution.Model.SymbolicExpressionTree.Depth;
      row++;

      modelWorksheet.Cells[row, 1].Value = "Model Length";
      modelWorksheet.Cells[row, 2].Value = solution.Model.SymbolicExpressionTree.Length;
      row += 2;

      modelWorksheet.Cells[row, 1].Value = "Estimation Limits Lower";
      modelWorksheet.Cells[row, 2].Value = Math.Max(solution.Model.LowerEstimationLimit, -9.99999999999999E+307); // minimal value supported by excel
      modelWorksheet.Names.Add("EstimationLimitLower", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Estimation Limits Upper";
      modelWorksheet.Cells[row, 2].Value = Math.Min(solution.Model.UpperEstimationLimit, 9.99999999999999E+307);  // maximal value supported by excel
      modelWorksheet.Names.Add("EstimationLimitUpper", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row += 2;

      modelWorksheet.Cells[row, 1].Value = "Trainings Partition Start";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TrainingPartition.Start;
      modelWorksheet.Names.Add(TRAININGSTART, modelWorksheet.Cells[row, 2]);
      row++;

      modelWorksheet.Cells[row, 1].Value = "Trainings Partition End";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TrainingPartition.End;
      modelWorksheet.Names.Add(TRAININGEND, modelWorksheet.Cells[row, 2]);
      row++;

      modelWorksheet.Cells[row, 1].Value = "Test Partition Start";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TestPartition.Start;
      modelWorksheet.Names.Add(TESTSTART, modelWorksheet.Cells[row, 2]);
      row++;

      modelWorksheet.Cells[row, 1].Value = "Test Partition End";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TestPartition.End;
      modelWorksheet.Names.Add(TESTEND, modelWorksheet.Cells[row, 2]);
      row += 2;

      string excelTrainingTarget = Indirect("B", true);
      string excelTrainingEstimated = Indirect("C", true);
      string excelTrainingAbsoluteError = Indirect("D", true);
      string excelTrainingRelativeError = Indirect("E", true);
      string excelTrainingMeanError = Indirect("F", true);
      string excelTrainingMSE = Indirect("G", true);

      string excelTestTarget = Indirect("B", false);
      string excelTestEstimated = Indirect("C", false);
      string excelTestAbsoluteError = Indirect("D", false);
      string excelTestRelativeError = Indirect("E", false);
      string excelTestMeanError = Indirect("F", false);
      string excelTestMSE = Indirect("G", false);

      modelWorksheet.Cells[row, 1].Value = "Pearson's R² (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("POWER(PEARSON({0},{1}),2)", excelTrainingTarget, excelTrainingEstimated);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Pearson's R² (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("POWER(PEARSON({0},{1}),2)", excelTestTarget, excelTestEstimated);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean Squared Error (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTrainingMSE);
      modelWorksheet.Names.Add("TrainingMSE", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean Squared Error (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTestMSE);
      modelWorksheet.Names.Add("TestMSE", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean absolute error (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTrainingAbsoluteError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean absolute error (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTestAbsoluteError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean error (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTrainingMeanError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean error (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTestMeanError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Average relative error (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTrainingRelativeError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.00%";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Average relative error (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTestRelativeError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.00%";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Normalized Mean Squared error (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("TrainingMSE / VAR({0})", excelTrainingTarget);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Normalized Mean Squared error  (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("TestMSE / VAR({0})", excelTestTarget);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";

      modelWorksheet.Cells["A1:B" + row].AutoFitColumns();

      AddModelTreePicture(modelWorksheet, solution.Model);
    }

    protected string Indirect(string column, bool training) {
      if (training) {
        return string.Format("INDIRECT(\"'Estimated Values'!{0}\"&{1}+2&\":{0}\"&{2}+1)", column, TRAININGSTART, TRAININGEND);
      } else {
        return string.Format("INDIRECT(\"'Estimated Values'!{0}\"&{1}+2&\":{0}\"&{2}+1)", column, TESTSTART, TESTEND);
      }
    }

    private void AddCharts(ExcelWorksheet chartsWorksheet) {
      chartsWorksheet.Names.AddFormula("AllId", "OFFSET('Estimated Values'!$A$1,1,0, COUNTA('Estimated Values'!$A:$A)-1)");
      chartsWorksheet.Names.AddFormula("AllTarget", "OFFSET('Estimated Values'!$B$1,1,0, COUNTA('Estimated Values'!$B:$B)-1)");
      chartsWorksheet.Names.AddFormula("AllEstimated", "OFFSET('Estimated Values'!$C$1,1,0, COUNTA('Estimated Values'!$C:$C)-1)");
      chartsWorksheet.Names.AddFormula("TrainingId", "OFFSET('Estimated Values'!$A$1,Model!TrainingStart + 1,0, Model!TrainingEnd - Model!TrainingStart)");
      chartsWorksheet.Names.AddFormula("TrainingTarget", "OFFSET('Estimated Values'!$B$1,Model!TrainingStart + 1,0, Model!TrainingEnd - Model!TrainingStart)");
      chartsWorksheet.Names.AddFormula("TrainingEstimated", "OFFSET('Estimated Values'!$C$1,Model!TrainingStart + 1,0, Model!TrainingEnd - Model!TrainingStart)");
      chartsWorksheet.Names.AddFormula("TestId", "OFFSET('Estimated Values'!$A$1,Model!TestStart + 1,0, Model!TestEnd - Model!TestStart)");
      chartsWorksheet.Names.AddFormula("TestTarget", "OFFSET('Estimated Values'!$B$1,Model!TestStart + 1,0, Model!TestEnd - Model!TestStart)");
      chartsWorksheet.Names.AddFormula("TestEstimated", "OFFSET('Estimated Values'!$C$1,Model!TestStart + 1,0, Model!TestEnd - Model!TestStart)");

      var scatterPlot = chartsWorksheet.Drawings.AddChart("scatterPlot", eChartType.XYScatter);
      scatterPlot.SetSize(800, 400);
      scatterPlot.SetPosition(0, 0);
      scatterPlot.Title.Text = "Scatter Plot";
      var seriesAll = scatterPlot.Series.Add("AllTarget", "AllEstimated");
      seriesAll.Header = "All";
      var seriesTraining = scatterPlot.Series.Add("TrainingTarget", "TrainingEstimated");
      seriesTraining.Header = "Training";
      var seriesTest = scatterPlot.Series.Add("TestTarget", "TestEstimated");
      seriesTest.Header = "Test";

      var lineChart = chartsWorksheet.Drawings.AddChart("lineChart", eChartType.XYScatterLinesNoMarkers);
      lineChart.SetSize(800, 400);
      lineChart.SetPosition(400, 0);
      lineChart.Title.Text = "LineChart";
      var lineTarget = lineChart.Series.Add("AllTarget", "AllId");
      lineTarget.Header = "Target";
      var lineAll = lineChart.Series.Add("AllEstimated", "AllId");
      lineAll.Header = "All";
      var lineTraining = lineChart.Series.Add("TrainingEstimated", "TrainingId");
      lineTraining.Header = "Training";
      var lineTest = lineChart.Series.Add("TestEstimated", "TestId");
      lineTest.Header = "Test";
    }

    protected void AddModelTreePicture(ExcelWorksheet modelWorksheet, ISymbolicDataAnalysisModel model) {
      SymbolicExpressionTreeChart modelTreePicture = new SymbolicExpressionTreeChart();
      modelTreePicture.Tree = model.SymbolicExpressionTree;
      string tmpFilename = Path.GetTempFileName();
      modelTreePicture.Width = 1000;
      modelTreePicture.Height = 500;
      modelTreePicture.SaveImageAsEmf(tmpFilename);

      FileInfo fi = new FileInfo(tmpFilename);
      var excelModelTreePic = modelWorksheet.Drawings.AddPicture("ModelTree", fi);
      excelModelTreePic.SetSize(50);
      excelModelTreePic.SetPosition(2, 0, 6, 0);
    }

    private void WriteEstimatedWorksheet(ExcelWorksheet estimatedWorksheet, ExcelWorksheet datasetWorksheet, string[] formulaParts, IRegressionSolution solution) {
      string preparedFormula = PrepareFormula(formulaParts);
      int rows = solution.ProblemData.Dataset.Rows;
      estimatedWorksheet.Cells[1, 1].Value = "Id";
      estimatedWorksheet.Cells[1, 2].Value = "Target Variable";
      estimatedWorksheet.Cells[1, 3].Value = "Estimated Values";
      estimatedWorksheet.Cells[1, 4].Value = "Absolute Error";
      estimatedWorksheet.Cells[1, 5].Value = "Relative Error";
      estimatedWorksheet.Cells[1, 6].Value = "Error";
      estimatedWorksheet.Cells[1, 7].Value = "Squared Error";
      estimatedWorksheet.Cells[1, 9].Value = "Unbounded Estimated Values";
      estimatedWorksheet.Cells[1, 10].Value = "Bounded Estimated Values";

      estimatedWorksheet.Cells[1, 1, 1, 10].AutoFitColumns();

      // fill in id, target variable and unbounded estimated values
      int targetIndex = solution.ProblemData.Dataset.VariableNames.ToList().FindIndex(x => x.Equals(solution.ProblemData.TargetVariable)) + 1;
      for (int i = 0; i < rows; i++) {
        estimatedWorksheet.Cells[i + 2, 1].Value = i; // id
        estimatedWorksheet.Cells[i + 2, 2].Formula = datasetWorksheet.Cells[i + 2, targetIndex].FullAddress; // target variable
        estimatedWorksheet.Cells[i + 2, 9].Formula = string.Format(preparedFormula, i + 2); // unbounded estimated values
      }
      estimatedWorksheet.Cells["B2:B" + (rows + 1)].Style.Numberformat.Format = "0.000";

      estimatedWorksheet.Cells["C2:C" + (rows + 1)].Formula = "J2";
      estimatedWorksheet.Cells["C2:C" + (rows + 1)].Style.Numberformat.Format = "0.000";
      estimatedWorksheet.Cells["D2:D" + (rows + 1)].Formula = "ABS(B2 - C2)";
      estimatedWorksheet.Cells["D2:D" + (rows + 1)].Style.Numberformat.Format = "0.000";
      estimatedWorksheet.Cells["E2:E" + (rows + 1)].Formula = "ABS(D2 / B2)";
      estimatedWorksheet.Cells["E2:E" + (rows + 1)].Style.Numberformat.Format = "0.000";
      estimatedWorksheet.Cells["F2:F" + (rows + 1)].Formula = "C2 - B2";
      estimatedWorksheet.Cells["F2:F" + (rows + 1)].Style.Numberformat.Format = "0.000";
      estimatedWorksheet.Cells["G2:G" + (rows + 1)].Formula = "POWER(F2, 2)";
      estimatedWorksheet.Cells["G2:G" + (rows + 1)].Style.Numberformat.Format = "0.000";

      estimatedWorksheet.Cells["I2:I" + (rows + 1)].Style.Numberformat.Format = "0.000";
      estimatedWorksheet.Cells["J2:J" + (rows + 1)].Formula = "IFERROR(IF(I2 > Model!EstimationLimitUpper, Model!EstimationLimitUpper, IF(I2 < Model!EstimationLimitLower, Model!EstimationLimitLower, I2)), AVERAGE(Model!EstimationLimitLower, Model!EstimationLimitUpper))";
      estimatedWorksheet.Cells["J2:J" + (rows + 1)].Style.Numberformat.Format = "0.000";
    }

    protected string PrepareFormula(string[] formulaParts) {
      string preparedFormula = formulaParts[0];
      foreach (var part in formulaParts.Skip(2)) {
        var varMap = part.Split(new string[] { " = " }, StringSplitOptions.None);
        var columnName = "$" + varMap[1] + "1";
        preparedFormula = preparedFormula.Replace(columnName, "Inputs!$" + varMap[1] + "{0}");   //{0} will be replaced later with the row number 
      }
      return preparedFormula;
    }

    protected void WriteInputSheet(ExcelWorksheet inputsWorksheet, ExcelWorksheet datasetWorksheet, IEnumerable<string> list, IDataset dataset) {
      //remark the performance of EPPlus drops dramatically 
      //if the data is not written row wise (from left to right) due the internal indices used.
      var variableNames = dataset.VariableNames.Select((v, i) => new { variable = v, index = i + 1 }).ToDictionary(v => v.variable, v => v.index);
      var nameMapping = list.Select(x => x.Split('=')[0].Trim()).ToArray();

      for (int row = 1; row <= dataset.Rows + 1; row++) {
        for (int column = 1; column < nameMapping.Length + 1; column++) {
          int variableIndex = variableNames[nameMapping[column - 1]];
          inputsWorksheet.Cells[row, column].Formula = datasetWorksheet.Cells[row, variableIndex].FullAddress;
        }
      }
    }

    protected void WriteDatasetToExcel(ExcelWorksheet datasetWorksheet, IDataAnalysisProblemData problemData) {
      //remark the performance of EPPlus drops dramatically 
      //if the data is not written row wise (from left to right) due the internal indices used.
      IDataset dataset = problemData.Dataset;
      var variableNames = dataset.VariableNames.ToList();
      var doubleVariables = new HashSet<string>(dataset.DoubleVariables);

      for (int col = 1; col <= variableNames.Count; col++)
        datasetWorksheet.Cells[1, col].Value = variableNames[col - 1];

      for (int row = 0; row < dataset.Rows; row++) {
        for (int col = 0; col < variableNames.Count; col++) {
          if (doubleVariables.Contains(variableNames[col]))
            datasetWorksheet.Cells[row + 2, col + 1].Value = dataset.GetDoubleValue(variableNames[col], row);
          else
            datasetWorksheet.Cells[row + 2, col + 1].Value = dataset.GetValue(row, col);
        }
      }
    }
  }
}
