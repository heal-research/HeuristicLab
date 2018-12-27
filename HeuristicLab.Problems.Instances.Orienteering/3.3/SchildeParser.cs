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
using System.Globalization;
using System.IO;
using System.Linq;

namespace HeuristicLab.Problems.Instances.Orienteering {
  public class SchildeParser {
    private static readonly IFormatProvider FormatProvider = new CultureInfo("en-US");

    private int currentPoint;
    private int currentDistance;

    public double[,] Coordinates { get; private set; }
    public double[,] Distances { get; private set; }
    public double[,] Scores { get; private set; }
    public int StartingPoint { get; private set; }
    public int TerminalPoint { get; private set; }
    public double UpperBoundConstraint { get; private set; }
    public double LowerConstraint { get; private set; }

    public void Reset() {
      currentPoint = 0;
      currentDistance = 0;

      Coordinates = null;
      Distances = null;
      Scores = null;
      StartingPoint = 0;
      TerminalPoint = 0;
      UpperBoundConstraint = 0.0;
      LowerConstraint = 0.0;

    }

    public void Parse(string file) {
      using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        Parse(stream);
      }
    }
    public void Parse(Stream stream) {
      int lineNumber = 0;

      using (var reader = new StreamReader(stream)) {
        // Read the lines from the file
        while (!reader.EndOfStream) {
          string lineBuffer = reader.ReadLine();

          // Remove all whitespace
          lineBuffer = new string(lineBuffer.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());

          // If the line contains data, continue
          if (lineBuffer.Length > 1) {
            // If the line contains not only a comment, process it
            int commentPosition = lineBuffer.IndexOf("//");
            if (commentPosition != 0) {
              // Count the lines that include data
              lineNumber++;

              // Remove a comment if there is one
              if (commentPosition > 0)
                lineBuffer = lineBuffer.Remove(commentPosition);

              // Convert the line to lowercase characters
              lineBuffer = lineBuffer.ToLower();

              // Skip version info in file
              if (lineNumber > 1) {
                // Extract the needed information
                ExtractData(lineBuffer);
              }
            }
          }
        }
      }
    }
    private void ExtractData(string inputString) {
      string temp;
      int pointCount;

      switch (inputString[0]) {
        case 'p': // Point description
          if (inputString.Length < 8)
            throw new IOException("Invalid Definition P");

          // Remove the 'p,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Get the position_x
          string positionX = inputString.Substring(0, inputString.IndexOf(','));
          // Remove the position_x
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Get the postion_y
          string positionY = inputString.Substring(0, inputString.IndexOf(','));
          // Remove the position_y to get the scores
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);

          Coordinates[currentPoint, 0] = double.Parse(positionX, FormatProvider);
          Coordinates[currentPoint, 1] = double.Parse(positionY, FormatProvider);

          // Extract all score values for this point
          int scoreIndex = 0;
          while (true) {
            string score;
            if (inputString.IndexOf(',') != -1) {
              score = inputString.Substring(0, inputString.IndexOf(','));
              inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
              Scores[currentPoint, scoreIndex++] = int.Parse(score, FormatProvider);
            } else {
              score = inputString;
              Scores[currentPoint, scoreIndex++] = int.Parse(score, FormatProvider);
              break;
            }
          }
          currentPoint++;
          break;

        case 'd': // Distance information
          if (inputString.Length < 11)
            throw new IOException("Invalid Definition D");

          //var tempDistances = new List<double>();

          // Remove the 'd,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);

          // Extract all distances for this point
          int distanceIndex = 0;
          while (true) {
            string distance;
            if (inputString.IndexOf(',') != -1) {
              distance = inputString.Substring(0, inputString.IndexOf(','));
              inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
              Distances[currentDistance, distanceIndex++] = double.Parse(distance, FormatProvider);
            } else {
              distance = inputString;
              Distances[currentDistance, distanceIndex++] = double.Parse(distance, FormatProvider);
              break;
            }
          }
          break;

        case 'n': // Number of points
          if (inputString.Length < 3)
            throw new IOException("Invalid Definition N");

          // Remove the 'n,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Extract the number of points

          pointCount = int.Parse(inputString, FormatProvider);
          Coordinates = new double[pointCount, 2];
          break;

        case 'm': // 1 if Distance-Matrix is given
          if (inputString.Length < 3)
            throw new IOException("Invalid Definition M");

          // Remove the 'm,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
                    
          pointCount = Coordinates.GetLength(1);
          Distances = new double[pointCount, pointCount];
          break;

        case 'c': // Number of constraints
          if (inputString.Length < 3)
            throw new IOException("Invalid Definition C");

          // Remove the 'c,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          break;

        case 's': // Number of scores per point
          if (inputString.Length < 3)
            throw new IOException("Invalid Definition S");

          // Remove the 's,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          int scoreCount = int.Parse(inputString, FormatProvider);

          pointCount = Coordinates.GetLength(0);
          Scores = new double[pointCount, scoreCount];

          break;

        case 'b': // Index of the starting point
          if (inputString.Length < 3)
            throw new IOException("Invalid Definition B");

          // Remove the 'b,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          StartingPoint = int.Parse(inputString, FormatProvider);
          break;

        case 'e': // Index of the terminus point
          if (inputString.Length < 3)
            throw new IOException("Invalid Definition E");

          // Remove the 'e,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          TerminalPoint = int.Parse(inputString, FormatProvider);
          break;

        case 'u': // Upper bound constraint
          if (inputString.Length < 5)
            throw new IOException("Invalid Definition U");

          // Remove the 'u,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Get the target
          temp = inputString.Substring(0, inputString.IndexOf(','));
          // Remove the target to get the target value
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);

          UpperBoundConstraint = double.Parse(inputString, FormatProvider);
          break;

        case 'l': // Lower bound constraint
          if (inputString.Length < 5)
            throw new IOException("Invalid Definition L");

          // Remove the 'l,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Get the target
          temp = inputString.Substring(0, inputString.IndexOf(','));
          // Remove the target to get the target value
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);

          LowerConstraint = double.Parse(inputString, FormatProvider);
          break;

        case 'x': // Maximization constraint
          if (inputString.Length < 5)
            throw new IOException("Invalid Definition X");

          // Remove the 'x,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Get the target
          temp = inputString.Substring(0, inputString.IndexOf(','));
          // Remove the target to get the target value
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);

          break;

        case 'i': // Minimization constraint
          if (inputString.Length < 5)
            throw new IOException("Invalid Definition I");

          // Remove the 'i,'
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);
          // Get the target
          temp = inputString.Substring(0, inputString.IndexOf(','));
          // Remove the target to get the target value
          inputString = inputString.Remove(0, inputString.IndexOf(',') + 1);

          break;
      }
    }
  }
}