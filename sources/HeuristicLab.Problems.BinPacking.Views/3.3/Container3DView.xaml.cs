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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking.Views {
  public partial class Container3DView : UserControl {
    private static readonly Color[] colors = new[] {
      Color.FromRgb(0x40, 0x6A, 0xB7),
      Color.FromRgb(0xB1, 0x6D, 0x01),
      Color.FromRgb(0x4E, 0x8A, 0x06),
      Color.FromRgb(0x75, 0x50, 0x7B),
      Color.FromRgb(0x72, 0x9F, 0xCF),
      Color.FromRgb(0xA4, 0x00, 0x00),
      Color.FromRgb(0xAD, 0x7F, 0xA8),
      Color.FromRgb(0x29, 0x50, 0xCF),
      Color.FromRgb(0x90, 0xB0, 0x60),
      Color.FromRgb(0xF5, 0x89, 0x30),
      Color.FromRgb(0x55, 0x57, 0x53),
      Color.FromRgb(0xEF, 0x59, 0x59),
      Color.FromRgb(0xED, 0xD4, 0x30),
      Color.FromRgb(0x63, 0xC2, 0x16),
    };
    
    private static readonly Color hiddenColor = Color.FromArgb(0x1A, 0xAA, 0xAA, 0xAA);
    private static readonly Color containerColor = Color.FromArgb(0x7F, 0xAA, 0xAA, 0xAA);

    private Point startPos;
    private bool mouseDown = false;
    private bool ctrlDown = false;
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

    private Dictionary<int, DiffuseMaterial> materials;

    public Container3DView() {
      InitializeComponent();
      camMain.Position = new Point3D(0.5, 3, 3); // for design time we use a different camera position 
      materials = new Dictionary<int, DiffuseMaterial>();
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

      var modelGroup = (Model3DGroup)MyModel.Content;
      var hiddenMaterial = new DiffuseMaterial(new SolidColorBrush(hiddenColor));

      foreach (var item in packing.Items) {
        var position = packing.Positions[item.Key];

        var w = position.Rotated ? item.Value.Depth : item.Value.Width;
        var h = item.Value.Height;
        var d = position.Rotated ? item.Value.Width : item.Value.Depth;

        var model = new GeometryModel3D { Geometry = new MeshGeometry3D() };
        DiffuseMaterial material;
        if (selectedItemKey >= 0 && selectedItemKey != item.Key)
          material = hiddenMaterial;
        else {
          if (!materials.TryGetValue(item.Value.Material, out material)) {
            var color = colors[(item.Value.Material - 1) % colors.Length];
            material = new DiffuseMaterial { Brush = new SolidColorBrush(color) };
            materials[item.Value.Material] = material;
          }
        }
        model.Material = material;
        modelGroup.Children.Add(model);

        AddSolidCube((MeshGeometry3D)model.Geometry, position.X, position.Y, position.Z, w, h, d);
      }

      var container = packing.BinShape;
      var containerModel = new GeometryModel3D(new MeshGeometry3D(), new DiffuseMaterial(new SolidColorBrush(containerColor)));
      modelGroup.Children.Add(containerModel);
      AddWireframeCube((MeshGeometry3D)containerModel.Geometry, container.Origin.X - .5, container.Origin.Y - .5, container.Origin.Z - .5, container.Width + 1, container.Height + 1, container.Depth + 1);

      var ratio = Math.Max(container.Width, Math.Max(container.Height, container.Depth));
      scale.ScaleX = 1.0 / ratio;
      scale.ScaleY = 1.0 / ratio;
      scale.ScaleZ = 1.0 / ratio;

      scale.CenterX = .5;
      scale.CenterY = .5;
      scale.CenterZ = 0;
    }


    private void Clear() {
      ((Model3DGroup)MyModel.Content).Children.Clear();
      materials.Clear();
      
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

    private void Container3DView_OnMouseEnter(object sender, MouseEventArgs e) {
      Focus(); // for mouse wheel events
    }

    private void Container3DView_OnKeyDown(object sender, KeyEventArgs e) {
      ctrlDown = e.Key.HasFlag(Key.LeftCtrl) || e.Key.HasFlag(Key.RightCtrl);
    }


    #region helper for cubes
    /// <summary>
    /// Creates a solid cube by adding the respective points and triangles.
    /// </summary>
    /// <param name="mesh">The mesh to which points and triangles are added.</param>
    /// <param name="x">The leftmost point</param>
    /// <param name="y">The frontmost point</param>
    /// <param name="z">The lowest point</param>
    /// <param name="width">The extension to the right</param>
    /// <param name="height">The extension to the back</param>
    /// <param name="depth">The extension to the top</param>
    private void AddSolidCube(MeshGeometry3D mesh, int x, int y, int z, int width, int height, int depth) {
      // ground
      mesh.Positions.Add(new Point3D(x, y, z));
      mesh.Positions.Add(new Point3D(x + width, y, z));
      mesh.Positions.Add(new Point3D(x + width, y + height, z));
      mesh.Positions.Add(new Point3D(x, y + height, z));
      // top
      mesh.Positions.Add(new Point3D(x, y, z + depth));
      mesh.Positions.Add(new Point3D(x + width, y, z + depth));
      mesh.Positions.Add(new Point3D(x + width, y + height, z + depth));
      mesh.Positions.Add(new Point3D(x, y + height, z + depth));

      // front
      AddPlane(mesh, 0, 1, 5, 4);
      // right side
      AddPlane(mesh, 1, 2, 6, 5);
      // back
      AddPlane(mesh, 3, 7, 6, 2);
      // left side
      AddPlane(mesh, 0, 4, 7, 3);
      // top
      AddPlane(mesh, 4, 5, 6, 7);
      // bottom
      AddPlane(mesh, 0, 3, 2, 1);
    }

    /// <summary>
    /// Creates a wireframe cube by adding the respective points and triangles.
    /// </summary>
    /// <param name="mesh">The mesh to which points and triangles are added.</param>
    /// <param name="x">The leftmost point</param>
    /// <param name="y">The frontmost point</param>
    /// <param name="z">The lowest point</param>
    /// <param name="width">The extension to the right</param>
    /// <param name="height">The extension to the back</param>
    /// <param name="depth">The extension to the top</param>
    /// <param name="thickness">The thickness of the frame</param>
    private void AddWireframeCube(MeshGeometry3D mesh, double x, double y, double z, double width, double height, double depth, double thickness = double.NaN) {
      // default thickness of the wireframe is 5% of smallest dimension
      if (double.IsNaN(thickness))
        thickness = Math.Min(width, Math.Min(height, depth)) * 0.05;

      // The cube contains of 8 corner, each corner has 4 points:
      // 1. The corner point
      // 2. A point on the edge to the right of the corner
      // 3. A point on the edge atop or below the corner
      // 4. A point on the edge to the left of the corner

      // Point 0, Front Left Bottom
      mesh.Positions.Add(new Point3D(x, y, z));
      mesh.Positions.Add(new Point3D(x + thickness, y, z));
      mesh.Positions.Add(new Point3D(x, y, z + thickness));
      mesh.Positions.Add(new Point3D(x, y + thickness, z));

      // Point 1, Front Right Bottom
      mesh.Positions.Add(new Point3D(x + width, y, z));
      mesh.Positions.Add(new Point3D(x + width, y + thickness, z));
      mesh.Positions.Add(new Point3D(x + width, y, z + thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y, z));

      // Point 2, Back Right Bottom
      mesh.Positions.Add(new Point3D(x + width, y + height, z));
      mesh.Positions.Add(new Point3D(x + width - thickness, y + height, z));
      mesh.Positions.Add(new Point3D(x + width, y + height, z + thickness));
      mesh.Positions.Add(new Point3D(x + width, y + height - thickness, z));

      // Point 3, Back Left Bottom
      mesh.Positions.Add(new Point3D(x, y + height, z));
      mesh.Positions.Add(new Point3D(x, y + height - thickness, z));
      mesh.Positions.Add(new Point3D(x, y + height, z + thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + height, z));

      // Point 4, Front Left Top
      mesh.Positions.Add(new Point3D(x, y, z + depth));
      mesh.Positions.Add(new Point3D(x + thickness, y, z + depth));
      mesh.Positions.Add(new Point3D(x, y, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x, y + thickness, z + depth));

      // Point 5, Front Right Top
      mesh.Positions.Add(new Point3D(x + width, y, z + depth));
      mesh.Positions.Add(new Point3D(x + width, y + thickness, z + depth));
      mesh.Positions.Add(new Point3D(x + width, y, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y, z + depth));

      // Point 6, Back Right Top
      mesh.Positions.Add(new Point3D(x + width, y + height, z + depth));
      mesh.Positions.Add(new Point3D(x + width - thickness, y + height, z + depth));
      mesh.Positions.Add(new Point3D(x + width, y + height, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + width, y + height - thickness, z + depth));

      // Point 7, Back Left Top
      mesh.Positions.Add(new Point3D(x, y + height, z + depth));
      mesh.Positions.Add(new Point3D(x, y + height - thickness, z + depth));
      mesh.Positions.Add(new Point3D(x, y + height, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + height, z + depth));

      AddPlane(mesh, 0, 4, 6, 2);
      AddPlane(mesh, 0, 3, 5, 4);

      AddPlane(mesh, 4, 8, 10, 6);
      AddPlane(mesh, 4, 7, 9, 8);

      AddPlane(mesh, 8, 12, 14, 10);
      AddPlane(mesh, 8, 11, 13, 12);

      AddPlane(mesh, 0, 2, 14, 12);
      AddPlane(mesh, 0, 12, 15, 1);

      AddPlane(mesh, 0, 1, 17, 16);
      AddPlane(mesh, 0, 16, 19, 3);

      AddPlane(mesh, 4, 20, 23, 7);
      AddPlane(mesh, 4, 5, 21, 20);

      AddPlane(mesh, 8, 24, 27, 11);
      AddPlane(mesh, 8, 9, 25, 24);

      AddPlane(mesh, 12, 28, 31, 15);
      AddPlane(mesh, 12, 13, 29, 28);

      AddPlane(mesh, 16, 18, 22, 20);
      AddPlane(mesh, 16, 20, 21, 19);

      AddPlane(mesh, 20, 22, 26, 24);
      AddPlane(mesh, 20, 24, 25, 23);

      AddPlane(mesh, 24, 28, 29, 27);
      AddPlane(mesh, 24, 26, 30, 28);

      AddPlane(mesh, 28, 30, 18, 16);
      AddPlane(mesh, 28, 16, 17, 31);
    }

    /// <summary>
    /// Adds a plane by two triangles. The indices of the points have to be given
    /// in counter-clockwise sequence.
    /// </summary>
    /// <param name="mesh">The mesh to add the triangles to</param>
    /// <param name="a">The index of the first point</param>
    /// <param name="b">The index of the second point</param>
    /// <param name="c">The index of the third point</param>
    /// <param name="d">The index of the fourth point</param>
    private void AddPlane(MeshGeometry3D mesh, int a, int b, int c, int d) {
      // two triangles form a plane
      mesh.TriangleIndices.Add(a);
      mesh.TriangleIndices.Add(b);
      mesh.TriangleIndices.Add(d);
      mesh.TriangleIndices.Add(c);
      mesh.TriangleIndices.Add(d);
      mesh.TriangleIndices.Add(b);
    }
    #endregion

  }
}
