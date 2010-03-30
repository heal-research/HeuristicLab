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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;

namespace HeuristicLab.Problems.ArtificialAnt {
  [View("AntTrail View")]
  [Content(typeof(AntTrail), true)]
  public sealed partial class AntTrailView : ItemView {
    private const int N_FOOD_ITEMS = 89;
    private const int WORLD_WIDTH = 32;
    private const int WORLD_HEIGHT = 32;
    private const int FOOD = 1;
    private const int EMPTY = 0;
    private int[] SANTA_FE_TRAIL = new int[] {
      0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 
      0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 
      0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 
      0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
    };

    private const int MAX_TIME_STEPS = 600;

    private int[] trail;
    private int currentLocation;
    private int currentDirection;
    Stack<SymbolicExpressionTreeNode> nodeStack;

    public new AntTrail Content {
      get { return (AntTrail)base.Content; }
      set { base.Content = value; }
    }

    public AntTrailView() {
      InitializeComponent();
      ResetTrailInterpreter();
    }

    public AntTrailView(AntTrail content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.SymbolicExpressionTreeChanged -= new EventHandler(Content_SymbolicExpressionTreeChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.SymbolicExpressionTreeChanged += new EventHandler(Content_SymbolicExpressionTreeChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        pictureBox.Image = null;
        pictureBox.Enabled = false;
      } else {
        pictureBox.Enabled = true;
        GenerateImage();
      }
    }

    private void ResetTrailInterpreter() {
      nodeStack = new Stack<SymbolicExpressionTreeNode>();
      currentLocation = 0;
      currentDirection = 0;
      trail = new int[WORLD_HEIGHT * WORLD_WIDTH];
      Array.Copy(SANTA_FE_TRAIL, trail, SANTA_FE_TRAIL.Length);
    }


    private void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          ResetTrailInterpreter();
          SymbolicExpressionTree expression = Content.SymbolicExpressionTree;

          Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
          using (Graphics graphics = Graphics.FromImage(bitmap)) {
            int cellHeight = pictureBox.Height / WORLD_HEIGHT;
            int cellWidth = pictureBox.Width / WORLD_WIDTH;
            // draw world
            for (int i = 0; i < WORLD_HEIGHT; i++) {
              graphics.DrawLine(Pens.Black, 0, i * cellHeight, pictureBox.Width, i * cellHeight);
            }
            for (int j = 0; j < WORLD_WIDTH; j++) {
              graphics.DrawLine(Pens.Black, j * cellWidth, 0, j * cellWidth, pictureBox.Height);
            }
            for (int i = 0; i < WORLD_HEIGHT; i++) {
              for (int j = 0; j < WORLD_WIDTH; j++) {
                if (trail[i * WORLD_HEIGHT + j] == FOOD) graphics.FillEllipse(Brushes.LightBlue, j * cellWidth, i * cellHeight, cellWidth, cellHeight);
              }
            }
            // step ant and draw trail
            nodeStack.Clear();
            int step = 0;
            while (step < MAX_TIME_STEPS) {
              // expression evaluated completly => start at root again
              if (nodeStack.Count == 0)
                nodeStack.Push(expression.Root.SubTrees[0]);

              var currentNode = nodeStack.Pop();
              if (currentNode.Symbol is Left) {
                currentDirection = (currentDirection + 3) % 4;
                step++;
              } else if (currentNode.Symbol is Right) {
                currentDirection = (currentDirection + 1) % 4;
                step++;
              } else if (currentNode.Symbol is Move) {
                currentLocation = NextField();
                trail[currentLocation] = EMPTY;
                float currentCellX = currentLocation % WORLD_WIDTH;
                float currentCellY = currentLocation / WORLD_HEIGHT;
                graphics.FillRectangle(Brushes.Brown,
                  currentCellX * cellWidth + cellWidth * 0.25f, currentCellY * cellHeight + cellHeight * 0.25f,
                  cellWidth * 0.5f, cellHeight * 0.5f);
                step++;
              } else if (currentNode.Symbol is IfFoodAhead) {
                if (trail[NextField()] == FOOD) {
                  nodeStack.Push(currentNode.SubTrees[0]);
                } else {
                  nodeStack.Push(currentNode.SubTrees[1]);
                }
              } else if (currentNode.Symbol is Prog2) {
                nodeStack.Push(currentNode.SubTrees[1]);
                nodeStack.Push(currentNode.SubTrees[0]);
              } else if (currentNode.Symbol is Prog3) {
                nodeStack.Push(currentNode.SubTrees[2]);
                nodeStack.Push(currentNode.SubTrees[1]);
                nodeStack.Push(currentNode.SubTrees[0]);
              } else {
                throw new InvalidOperationException(currentNode.Symbol.ToString());
              }

              
            }
          }
          pictureBox.Image = bitmap;
        }
      }
    }

    private int NextField() {
      int currentLocationX = currentLocation % WORLD_WIDTH;
      int currentLocationY = currentLocation / WORLD_HEIGHT;

      switch (currentDirection) {
        case 0:
          currentLocationX = (currentLocationX + 1) % WORLD_WIDTH; // EAST
          break;
        case 1:
          currentLocationY = (currentLocationY + 1) % WORLD_HEIGHT; // SOUTH
          break;
        case 2:
          currentLocationX = (currentLocationX + WORLD_WIDTH - 1) % WORLD_WIDTH; // WEST
          break;
        case 3:
          currentLocationY = (currentLocationY + WORLD_HEIGHT - 1) % WORLD_HEIGHT; // NORTH
          break;
        default:
          throw new InvalidOperationException();
      }
      return currentLocationY * WORLD_WIDTH + currentLocationX;
    }

    void Content_SymbolicExpressionTreeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_SymbolicExpressionTreeChanged), sender, e);
      else
        GenerateImage();
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
  }
}
