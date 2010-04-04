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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using HeuristicLab.Data;
using System.Text;

namespace HeuristicLab.Problems.DataAnalysis.Regression {
  public class CsvFileParser {
    private const string VARIABLENAMES = "VARIABLENAMES";
    private Tokenizer tokenizer;
    private List<string> variableNames;
    private List<List<double>> rowValues;

    private int rows;
    public int Rows {
      get { return rows; }
      set { rows = value; }
    }

    private int columns;
    public int Columns {
      get { return columns; }
      set { columns = value; }
    }

    private double[,] values;
    public double[,] Values {
      get {
        return values;
      }
    }

    public IEnumerable<string> VariableNames {
      get {
        if (variableNames.Count > 0) return variableNames;
        else {
          string[] names = new string[columns];
          for (int i = 0; i < names.Length; i++) {
            names[i] = "X" + i.ToString("000");
          }
          return names;
        }
      }
    }

    public CsvFileParser() {
      rowValues = new List<List<double>>();
      variableNames = new List<string>();
    }

    private void Reset() {
      variableNames.Clear();
      rowValues.Clear();
    }

    public void Parse(string fileName) {
      TryParse(fileName);
      // translate the list of samples into a DoubleMatrixData item
      rows = rowValues.Count;
      columns = rowValues[0].Count;
      values = new double[rows, columns];

      int rowIndex = 0;
      int columnIndex = 0;
      foreach (List<double> row in rowValues) {
        columnIndex = 0;
        foreach (double element in row) {
          values[rowIndex, columnIndex++] = element;
        }
        rowIndex++;
      }
    }

    private void TryParse(string fileName) {
      Exception lastEx = null;
      NumberFormatInfo[] possibleFormats = new NumberFormatInfo[] { NumberFormatInfo.InvariantInfo };
      foreach (NumberFormatInfo numberFormat in possibleFormats) {
        using (StreamReader reader = new StreamReader(fileName)) {
          tokenizer = new Tokenizer(reader, numberFormat);
          try {
            // parse the file
            Parse();
            return; // parsed without errors -> return;
          }
          catch (DataFormatException ex) {
            lastEx = ex;
          }
        }
      }
      // all number formats threw an exception -> rethrow the last exception
      throw lastEx;
    }

    #region tokenizer
    internal enum TokenTypeEnum {
      NewLine, Separator, String, Double
    }

    internal class Token {
      public TokenTypeEnum type;
      public string stringValue;
      public double doubleValue;

      public Token(TokenTypeEnum type, string value) {
        this.type = type;
        stringValue = value;
        doubleValue = 0.0;
      }

      public override string ToString() {
        return stringValue;
      }
    }


    internal class Tokenizer {
      private StreamReader reader;
      private List<Token> tokens;
      private NumberFormatInfo numberFormatInfo;

      private int currentLineNumber = 0;
      public int CurrentLineNumber {
        get { return currentLineNumber; }
        private set { currentLineNumber = value; }
      }
      private string currentLine;
      public string CurrentLine {
        get { return currentLine; }
        private set { currentLine = value; }
      }

      private Token newlineToken;
      public Token NewlineToken {
        get { return newlineToken; }
        private set { newlineToken = value; }
      }
      private Token separatorToken;
      public Token SeparatorToken {
        get { return separatorToken; }
        private set { separatorToken = value; }
      }

      public Tokenizer(StreamReader reader, NumberFormatInfo numberFormatInfo, char separator) {
        this.reader = reader;
        this.numberFormatInfo = numberFormatInfo;
        separatorToken = new Token(TokenTypeEnum.Separator, separator.ToString());
        newlineToken = new Token(TokenTypeEnum.NewLine, Environment.NewLine);
        tokens = new List<Token>();
        ReadNextTokens();
      }
      public Tokenizer(StreamReader reader, NumberFormatInfo numberFormatInfo)
        : this(reader, numberFormatInfo, ';') {
      }

      private void ReadNextTokens() {
        if (!reader.EndOfStream) {
          CurrentLine = reader.ReadLine();
          var newTokens = from str in Split(CurrentLine)
                          let trimmedStr = str.Trim()
                          where !string.IsNullOrEmpty(trimmedStr)
                          select MakeToken(trimmedStr.Trim());

          tokens.AddRange(newTokens);
          tokens.Add(NewlineToken);
          CurrentLineNumber++;
        }
      }

      private IEnumerable<string> Split(string line) {
        StringBuilder subStr = new StringBuilder();
        foreach (char c in line) {
          if (c == ';') {
            yield return subStr.ToString();
            subStr = new StringBuilder();
            yield return c.ToString();
          } else {
            subStr.Append(c);
          }
        }
        yield return subStr.ToString();
      }

      private Token MakeToken(string strToken) {
        Token token = new Token(TokenTypeEnum.String, strToken);
        if (strToken.Equals(SeparatorToken.stringValue)) {
          return SeparatorToken;
        } else if (double.TryParse(strToken, NumberStyles.Float, numberFormatInfo, out token.doubleValue)) {
          token.type = TokenTypeEnum.Double;
          return token;
        }

        // couldn't parse the token as an int or float number so return a string token
        return token;
      }

      public Token Peek() {
        return tokens[0];
      }

      public Token Next() {
        Token next = tokens[0];
        tokens.RemoveAt(0);
        if (tokens.Count == 0) {
          ReadNextTokens();
        }
        return next;
      }

      public bool HasNext() {
        return tokens.Count > 0 || !reader.EndOfStream;
      }
    }
    #endregion

    #region parsing
    private void Parse() {
      ParseVariableNames();
      if (!tokenizer.HasNext()) Error("Couldn't parse data values. Probably because of incorrect number format (the parser expects english number format with a '.' as decimal separator).", "", tokenizer.CurrentLineNumber);
      ParseValues();
      if (rowValues.Count == 0) Error("Couldn't parse data values. Probably because of incorrect number format (the parser expects english number format with a '.' as decimal separator).", "", tokenizer.CurrentLineNumber);
    }

    private void ParseValues() {
      while (tokenizer.HasNext()) {
        List<double> row = new List<double>();
        row.Add(NextValue(tokenizer));
        while (tokenizer.HasNext() && tokenizer.Peek() == tokenizer.SeparatorToken) {
          Expect(tokenizer.SeparatorToken);
          row.Add(NextValue(tokenizer));
        }
        Expect(tokenizer.NewlineToken);
        // all rows have to have the same number of values            
        // the first row defines how many samples are needed
        if (rowValues.Count > 0 && rowValues[0].Count != row.Count) {
          Error("The first row of the dataset has " + rowValues[0].Count + " columns." +
            "\nLine " + tokenizer.CurrentLineNumber + " has " + row.Count + " columns.", "", tokenizer.CurrentLineNumber);
        }
        // add the current row to the collection of rows and start a new row
        rowValues.Add(row);
        row = new List<double>();
      }
    }

    private double NextValue(Tokenizer tokenizer) {
      if (tokenizer.Peek() == tokenizer.SeparatorToken || tokenizer.Peek() == tokenizer.NewlineToken) return double.NaN;
      Token current = tokenizer.Next();
      if (current.type == TokenTypeEnum.Separator || current.type == TokenTypeEnum.String) {
        return double.NaN;
      } else if (current.type == TokenTypeEnum.Double) {
        // just take the value
        return current.doubleValue;
      }
      // found an unexpected token => throw error 
      Error("Unexpected token.", current.stringValue, tokenizer.CurrentLineNumber);
      // this line is never executed because Error() throws an exception
      throw new InvalidOperationException();
    }

    private void ParseVariableNames() {
      // if the first line doesn't start with a double value then we assume that the
      // first line contains variable names
      if (tokenizer.HasNext() && tokenizer.Peek().type != TokenTypeEnum.Double) {

        List<Token> tokens = new List<Token>();
        Token valueToken;
        valueToken = tokenizer.Next();
        tokens.Add(valueToken);
        while (tokenizer.HasNext() && tokenizer.Peek() == tokenizer.SeparatorToken) {
          Expect(tokenizer.SeparatorToken);
          valueToken = tokenizer.Next();
          if (valueToken != tokenizer.NewlineToken) {
            tokens.Add(valueToken);
          }
        }
        if (valueToken != tokenizer.NewlineToken) {
          Expect(tokenizer.NewlineToken);
        }
        variableNames = tokens.Select(x => x.stringValue.Trim()).ToList();
      }
    }

    private void Expect(Token expectedToken) {
      Token actualToken = tokenizer.Next();
      if (actualToken != expectedToken) {
        Error("Expected: " + expectedToken, actualToken.stringValue, tokenizer.CurrentLineNumber);
      }
    }

    private void Error(string message, string token, int lineNumber) {
      throw new DataFormatException("Error while parsing.\n" + message, token, lineNumber);
    }
    #endregion
  }
}
