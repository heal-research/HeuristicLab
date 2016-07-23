/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 * along with HeuristicLab. If not, see<http://www.gnu.org/licenses/> .
 */

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking.Views {
  public partial class Container3DView : UserControl {
    private Point startPos;
    private bool mouseDown = false;
    private double startAngleX;
    private double startAngleY;
    private int selectedItemKey;

    private BinPacking<BinPacking3D.PackingPosition, PackingShape, PackingItem> packing;
    public BinPacking<BinPacking3D.PackingPosition, PackingShape, PackingItem> Packing {
      get { return packing; }
      set {
        if (packing != value) {
          this.packing = value;
          ClearSelection(); // also updates visualization
        }
      }
    }

    public Container3DView() {
      InitializeComponent();
      camMain.Position = new Point3D(0.5, 3, 3); // for design time we use a different camera position 
      Clear();
    }


    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
      base.OnRenderSizeChanged(sizeInfo);
      var s = Math.Min(sizeInfo.NewSize.Height, sizeInfo.NewSize.Width);
      var mySize = new Size(s, s);
      viewport3D1.RenderSize = mySize;
    }

    public void SelectItem(int itemKey) {
      // selection of an item should make all other items semi-transparent
      selectedItemKey = itemKey;
      UpdateVisualization();
    }
    public void ClearSelection() {
      // remove all transparency
      selectedItemKey = -1;
      UpdateVisualization();
    }

    private void UpdateVisualization() {
      Clear();
      if (packing == null) return; // nothing to display

      // draw all items
      // order by Z position to reduce artifacts (because of transparent objects) 
      // TODO: improve code to reduce artifacts 
      //  - from triangle definitions and lighting
      //  - from rotation and Z-ordering

      foreach (var item in packing.Items.OrderBy(i => packing.Positions[i.Key].Z)) {
        var position = packing.Positions[item.Key];

        var w = position.Rotated ? item.Value.Depth : item.Value.Width;
        var h = item.Value.Height;
        var d = position.Rotated ? item.Value.Width : item.Value.Depth;

        // ignore the item.Material

        // if nothing is selected then draw all cubes opaque 
        // otherwise draw only the selected cube opaque and all others transparent
        if (selectedItemKey < 0 || selectedItemKey == item.Key) {
          AddCube(meshMain, position.X, position.Y, position.Z, w, h, d);
        } else {
          AddCube(meshTransparent, position.X, position.Y, position.Z, w, h, d, addInsideTriangles: true);
        }
      }

      var container = packing.BinShape;
      // draw a transparent container
      AddCube(meshTransparent, container.Origin.X, container.Origin.Y, container.Origin.Z, container.Width, container.Height, container.Depth, addInsideTriangles: true);

      // TODO: support cuboids with different side lengths
      // apply scaling so that the container fits into the unit cube (necessary for the transformations)
      scale.ScaleX = 1.0 / (container.Width);
      scale.ScaleY = 1.0 / (container.Height);
      scale.ScaleZ = 1.0 / (container.Depth);
    }


    private void Clear() {
      meshInsides.Positions.Clear();
      meshInsides.TriangleIndices.Clear();

      meshMain.Positions.Clear();
      meshMain.TriangleIndices.Clear();

      meshTransparent.Positions.Clear();
      meshTransparent.TriangleIndices.Clear();

      mouseDown = false;
      startAngleX = 0;
      startAngleY = 0;
    }

    private void Container3DView_MouseMove(object sender, MouseEventArgs e) {
      if (!mouseDown) return;
      var pos = e.GetPosition((IInputElement)this);
      rotateX.Angle = startAngleX + (pos.X - startPos.X) / 4;
      rotateY.Angle = startAngleY + (pos.Y - startPos.Y) / 4;
    }

    private void Container3DView_MouseDown(object sender, MouseButtonEventArgs e) {
      startAngleX = rotateX.Angle;
      startAngleY = rotateY.Angle;
      this.startPos = e.GetPosition((IInputElement)this);
      this.mouseDown = true;
    }

    private void Container3DView_MouseUp(object sender, MouseButtonEventArgs e) {
      mouseDown = false;
    }

    private void Container3DView_OnMouseWheel(object sender, MouseWheelEventArgs e) {
      if (e.Delta > 0) {
        scaleZoom.ScaleX *= 1.1;
        scaleZoom.ScaleY *= 1.1;
        scaleZoom.ScaleZ *= 1.1;
      } else if (e.Delta < 0) {
        scaleZoom.ScaleX /= 1.1;
        scaleZoom.ScaleY /= 1.1;
        scaleZoom.ScaleZ /= 1.1;
      }
    }


    #region helper for cubes

    private void AddCube(MeshGeometry3D mesh, int x, int y, int z, int width, int height, int depth, bool addInsideTriangles = false) {
      AddOutsideTriangles(mesh, AddPoints(mesh, x, y, z, width, height, depth));
      if (addInsideTriangles) AddInsideTriangles(meshInsides, AddPoints(meshInsides, x, y, z, width, height, depth));
    }

    private void AddOutsideTriangles(MeshGeometry3D mesh, int[] pointIdx) {
      // point indices counter-clockwise
      // back side
      mesh.TriangleIndices.Add(pointIdx[0]);
      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[1]);

      mesh.TriangleIndices.Add(pointIdx[0]);
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[2]);

      // bottom side
      mesh.TriangleIndices.Add(pointIdx[5]);
      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[0]);

      mesh.TriangleIndices.Add(pointIdx[5]);
      mesh.TriangleIndices.Add(pointIdx[0]);
      mesh.TriangleIndices.Add(pointIdx[1]);

      // right side
      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[5]);

      mesh.TriangleIndices.Add(pointIdx[1]);
      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[5]);

      // left side
      mesh.TriangleIndices.Add(pointIdx[7]);
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[4]);

      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[0]);

      // top side
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[7]);
      mesh.TriangleIndices.Add(pointIdx[6]);

      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[2]);

      // front side
      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[7]);
      mesh.TriangleIndices.Add(pointIdx[4]);

      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[5]);
    }

    private void AddInsideTriangles(MeshGeometry3D mesh, int[] pointIdx) {
      // for each cube we also draw the triangles facing inside because they are visible when a cube is transparent
      // point indices clockwise

      // back side
      mesh.TriangleIndices.Add(pointIdx[1]);
      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[0]);

      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[0]);

      // bottom side
      mesh.TriangleIndices.Add(pointIdx[0]);
      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[5]);

      mesh.TriangleIndices.Add(pointIdx[1]);
      mesh.TriangleIndices.Add(pointIdx[0]);
      mesh.TriangleIndices.Add(pointIdx[5]);

      // right side
      mesh.TriangleIndices.Add(pointIdx[5]);
      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[2]);

      mesh.TriangleIndices.Add(pointIdx[5]);
      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[1]);

      // left side
      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[7]);

      mesh.TriangleIndices.Add(pointIdx[0]);
      mesh.TriangleIndices.Add(pointIdx[3]);
      mesh.TriangleIndices.Add(pointIdx[4]);

      // top side
      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[7]);
      mesh.TriangleIndices.Add(pointIdx[3]);

      mesh.TriangleIndices.Add(pointIdx[2]);
      mesh.TriangleIndices.Add(pointIdx[6]);
      mesh.TriangleIndices.Add(pointIdx[3]);

      // front side
      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[7]);
      mesh.TriangleIndices.Add(pointIdx[6]);

      mesh.TriangleIndices.Add(pointIdx[5]);
      mesh.TriangleIndices.Add(pointIdx[4]);
      mesh.TriangleIndices.Add(pointIdx[6]);
    }

    private int[] AddPoints(MeshGeometry3D mesh, int x, int y, int z, int w, int h, int d) {
      // ground
      mesh.Positions.Add(new Point3D(x, y, z));
      mesh.Positions.Add(new Point3D(x + w, y, z));
      mesh.Positions.Add(new Point3D(x + w, y + h, z));
      mesh.Positions.Add(new Point3D(x, y + h, z));
      // top
      mesh.Positions.Add(new Point3D(x, y, z + d));
      mesh.Positions.Add(new Point3D(x + w, y, z + d));
      mesh.Positions.Add(new Point3D(x + w, y + h, z + d));
      mesh.Positions.Add(new Point3D(x, y + h, z + d));

      return Enumerable.Range(mesh.Positions.Count - 8, 8).ToArray();
    }
    #endregion

    private void Container3DView_OnMouseEnter(object sender, MouseEventArgs e) {
      Focus(); // for mouse wheel events
    }
  }
}
