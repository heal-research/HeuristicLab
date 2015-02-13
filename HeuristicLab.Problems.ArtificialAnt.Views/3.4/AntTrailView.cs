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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ArtificialAnt.Views {
  [View("AntTrail View")]
  [Content(typeof(AntTrail), true)]
  public sealed partial class AntTrailView : ItemView {
    public new AntTrail Content {
      get { return (AntTrail)base.Content; }
      set { base.Content = value; }
    }

    public AntTrailView() {
      InitializeComponent();
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
      this.playButton.Enabled = Content != null && !Locked;
      GenerateImage();
    }

    protected override void OnLockedChanged() {
      this.playButton.Enabled = Content != null && !Locked;
      base.OnLockedChanged();
    }

    private void GenerateImage() {
      animationTimer.Stop();
      pictureBox.Image = null;
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content != null) {
          var nodeStack = new Stack<SymbolicExpressionTreeNode>();
          int rows = Content.World.Rows;
          int columns = Content.World.Columns;
          SymbolicExpressionTree expression = Content.SymbolicExpressionTree;

          DrawWorld();
          using (Graphics graphics = Graphics.FromImage(pictureBox.Image)) {
            float cellHeight = pictureBox.Height / (float)rows;
            float cellWidth = pictureBox.Width / (float)columns;

            AntInterpreter interpreter = new AntInterpreter();
            interpreter.MaxTimeSteps = Content.MaxTimeSteps.Value;
            interpreter.Expression = Content.SymbolicExpressionTree;
            interpreter.World = Content.World;
            int currentAntLocationColumn;
            int currentAntLocationRow;
            // draw initial ant
            interpreter.AntLocation(out currentAntLocationRow, out currentAntLocationColumn);
            DrawAnt(graphics, currentAntLocationRow, currentAntLocationColumn, interpreter.AntDirection, cellWidth, cellHeight);
            // interpret ant code and draw trail
            while (interpreter.ElapsedTime < interpreter.MaxTimeSteps) {
              interpreter.Step();
              interpreter.AntLocation(out currentAntLocationRow, out currentAntLocationColumn);
              DrawAnt(graphics, currentAntLocationRow, currentAntLocationColumn, interpreter.AntDirection, cellWidth, cellHeight);
            }
          }
          pictureBox.Refresh();
        }
      }
    }

    private void DrawWorld() {
      int rows = Content.World.Rows;
      int columns = Content.World.Columns;
      Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
      using (Graphics graphics = Graphics.FromImage(bitmap)) {
        float cellHeight = pictureBox.Height / (float)rows;
        float cellWidth = pictureBox.Width / (float)columns;
        // draw world
        for (int i = 0; i < rows; i++) {
          graphics.DrawLine(Pens.Black, 0, i * cellHeight, pictureBox.Width, i * cellHeight);
        }
        for (int j = 0; j < columns; j++) {
          graphics.DrawLine(Pens.Black, j * cellWidth, 0, j * cellWidth, pictureBox.Height);
        }
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < columns; j++) {
            if (Content.World[i, j])
              graphics.FillEllipse(Brushes.LightBlue, j * cellWidth, i * cellHeight, cellWidth, cellHeight);
          }
        }
        pictureBox.Image = bitmap;
      }
    }

    private void DrawAnt(Graphics g, int row, int column, int direction, float cellWidth, float cellHeight) {
      //g.FillRectangle(Brushes.White, column * cellWidth, row * cellHeight,
      //      cellWidth, cellHeight);
      // draw ant body
      g.FillRectangle(Brushes.Brown,
            column * cellWidth + cellWidth * 0.25f, row * cellHeight + cellHeight * 0.25f,
            cellWidth * 0.5f, cellHeight * 0.5f);
      // show ant direction
      float centerX = column * cellWidth + cellWidth * 0.5f;
      float centerY = row * cellHeight + cellHeight * 0.5f;
      float directionX = centerX;
      float directionY = centerY;
      switch (direction) {
        case 0: { // EAST
            directionX = centerX + cellWidth * 0.5f;
            break;
          }
        case 1: { // SOUTH
            directionY = directionY + cellHeight * 0.5f;
            break;
          }
        case 2: { // WEST 
            directionX = centerX - cellWidth * 0.5f;
            break;
          }
        case 3: { // NORTH
            directionY = directionY - cellHeight * 0.5f;
            break;
          }
        default: throw new InvalidOperationException();
      }
      g.DrawLine(Pens.Brown, centerX, centerY, directionX, directionY);
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

    #region animation
    private AntInterpreter animationInterpreter;
    private void playButton_Click(object sender, EventArgs e) {
      playButton.Enabled = false;
      int rows = Content.World.Rows;
      int columns = Content.World.Columns;
      SymbolicExpressionTree expression = Content.SymbolicExpressionTree;
      var nodeStack = new Stack<SymbolicExpressionTreeNode>();

      animationInterpreter = new AntInterpreter();
      animationInterpreter.MaxTimeSteps = Content.MaxTimeSteps.Value;
      animationInterpreter.Expression = Content.SymbolicExpressionTree;
      animationInterpreter.World = Content.World;

      DrawWorld();
      using (Graphics graphics = Graphics.FromImage(pictureBox.Image)) {
        float cellHeight = pictureBox.Height / (float)Content.World.Rows;
        float cellWidth = pictureBox.Width / (float)Content.World.Columns;
        // draw initial ant
        int currentAntLocationColumn;
        int currentAntLocationRow;
        animationInterpreter.AntLocation(out currentAntLocationRow, out currentAntLocationColumn);
        DrawAnt(graphics, currentAntLocationRow, currentAntLocationColumn, animationInterpreter.AntDirection, cellWidth, cellHeight);
        pictureBox.Refresh();
      }

      animationTimer.Start();
    }

    private void animationTimer_Tick(object sender, EventArgs e) {
      using (Graphics graphics = Graphics.FromImage(pictureBox.Image)) {
        float cellHeight = pictureBox.Height / (float)Content.World.Rows;
        float cellWidth = pictureBox.Width / (float)Content.World.Columns;
        int currentAntLocationColumn;
        int currentAntLocationRow;
        // interpret ant code and draw trail
        animationInterpreter.Step();
        animationInterpreter.AntLocation(out currentAntLocationRow, out currentAntLocationColumn);
        DrawAnt(graphics, currentAntLocationRow, currentAntLocationColumn, animationInterpreter.AntDirection, cellWidth, cellHeight);
        pictureBox.Refresh();
        if (animationInterpreter.ElapsedTime < animationInterpreter.MaxTimeSteps) {
          animationTimer.Start();
        } else {
          animationTimer.Stop();
          playButton.Enabled = true;
        }
      }
    }
    #endregion
  }
}
