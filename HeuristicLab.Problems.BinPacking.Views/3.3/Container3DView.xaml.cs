#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HeuristicLab.Problems.BinPacking3D;
using HeuristicLab.Collections;

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
    private static readonly Color residualSpaceColor = Color.FromArgb(0x7F, 0xAA, 0xAA, 0x00);

    private Point startPos;
    private bool mouseDown = false;
    private double startAngleX;
    private double startAngleY;
    private int selectedItemKey;
    private int selectedExtremePointIndex;
    private bool showExtremePoints;
    private bool showResidualSpaces;

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

    public ObservableDictionary<BinPacking3D.PackingPosition, IList<ResidualSpace>> ResidualSpaces { get; set; }
    public ObservableCollection<BinPacking3D.PackingPosition> ExtremePoints { get; set; }

    public Container3DView() {
      InitializeComponent();
      camMain.Position = new Point3D(0.5, 3, 3); // for design time we use a different camera position 
      materials = new Dictionary<int, DiffuseMaterial>();
      ResidualSpaces = new ObservableDictionary<BinPacking3D.PackingPosition, IList<ResidualSpace>>();
      ExtremePoints = new ObservableCollection<BinPacking3D.PackingPosition>();
      selectedExtremePointIndex = -1;
      Clear();
    }


    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
      base.OnRenderSizeChanged(sizeInfo);
      var s = Math.Min(sizeInfo.NewSize.Height, sizeInfo.NewSize.Width);
      var mySize = new Size(s, s);
      viewport3D1.RenderSize = mySize;
    }

    /// <summary>
    /// Selects another extreme point
    /// </summary>
    /// <param name="index"></param>
    public void SelectExtremePoint(int index) {
      selectedExtremePointIndex = index;
      UpdateVisualization();
    }

    public void SelectItem(int itemKey) {
      // selection of an item should make all other items semi-transparent
      selectedItemKey = itemKey;
      UpdateVisualization();
    }

    public void ClearSelection() {
      // remove all transparency
      selectedItemKey = -1;
      selectedExtremePointIndex = -1;
      UpdateVisualization();
    }

    private void UpdateVisualization() {
      Clear();
      if (packing == null)
        return; // nothing to display

      var modelGroup = (Model3DGroup)MyModel.Content;
      var hiddenMaterial = new DiffuseMaterial(new SolidColorBrush(hiddenColor));

      if (selectedItemKey >= 0) {
        var selectedItem = packing.Items.Single(x => selectedItemKey == x.Key);
        var selectedPos = packing.Positions[selectedItem.Key];

        var colorIdx = selectedItem.Value.Layer;
        while (colorIdx < 0)
          colorIdx += colors.Length;
        colorIdx = colorIdx % colors.Length;
        var color = colors[colorIdx];
        var material = new DiffuseMaterial { Brush = new SolidColorBrush(color) };
        materials[selectedItem.Value.Layer] = material;

        var selectedModel = new GeometryModel3D { Geometry = new MeshGeometry3D(), Material = material };
        AddSolidCube((MeshGeometry3D)selectedModel.Geometry, selectedPos.X, selectedPos.Y, selectedPos.Z,
          selectedItem.Value.WidthInView,
          selectedItem.Value.HeightInView,
          selectedItem.Value.DepthInView);
        modelGroup.Children.Add(selectedModel);

        foreach (var item in packing.Items.Where(x => selectedItemKey != x.Key)) {
          var position = packing.Positions[item.Key];

          var w = item.Value.WidthInView;
          var h = item.Value.HeightInView;
          var d = item.Value.DepthInView;

          var model = new GeometryModel3D { Geometry = new MeshGeometry3D(), Material = hiddenMaterial };
          AddWireframeCube((MeshGeometry3D)model.Geometry, position.X, position.Y, position.Z, w, h, d, 1);
          modelGroup.Children.Add(model);
        }


      } else {
        foreach (var item in packing.Items) {
          var position = packing.Positions[item.Key];

          var w = item.Value.WidthInView;
          var h = item.Value.HeightInView;
          var d = item.Value.DepthInView;

          var model = new GeometryModel3D { Geometry = new MeshGeometry3D() };
          DiffuseMaterial material;
          if (!materials.TryGetValue(item.Value.Layer, out material)) {
            var colorIdx = item.Value.Layer;
            while (colorIdx < 0)
              colorIdx += colors.Length;
            colorIdx = colorIdx % colors.Length;
            var color = colors[colorIdx];
            material = new DiffuseMaterial { Brush = new SolidColorBrush(color) };
            materials[item.Value.Layer] = material;
          }
          var selectedModel = new GeometryModel3D { Geometry = new MeshGeometry3D(), Material = material };
          AddSolidCube((MeshGeometry3D)selectedModel.Geometry, position.X, position.Y, position.Z, w, h, d);
          modelGroup.Children.Add(selectedModel);
        }
      }


      AddExtremePoints(modelGroup);

      var container = packing.BinShape;
      var containerModel = new GeometryModel3D(new MeshGeometry3D(), new DiffuseMaterial(new SolidColorBrush(containerColor)));
      modelGroup.Children.Add(containerModel);
      AddWireframeCube((MeshGeometry3D)containerModel.Geometry, container.Origin.X - .5, container.Origin.Y - .5, container.Origin.Z - .5, container.Width + 1, container.Height + 1, container.Depth + 1);

      var ratio = Math.Max(container.Width, Math.Max(container.Height, container.Depth));
      scale.ScaleX = 1.0 / ratio;
      scale.ScaleY = 1.0 / ratio;
      scale.ScaleZ = 1.0 / ratio;

      scaleZoom.CenterX = rotateX.CenterX = rotateY.CenterX = container.Width / (2.0 * ratio);
      scaleZoom.CenterY = rotateX.CenterY = rotateY.CenterY = container.Height / (2.0 * ratio);
      scaleZoom.CenterZ = rotateX.CenterZ = rotateY.CenterZ = container.Depth / (2.0 * ratio);

      camMain.Position = new Point3D(
        scaleZoom.CenterX,
        3,
        3);
      camMain.LookDirection = new Vector3D(
        0,
        scaleZoom.CenterY - camMain.Position.Y,
        scaleZoom.CenterZ - camMain.Position.Z);
    }

    private void AddExtremePoints(Model3DGroup modelGroup) {
      if (showExtremePoints) {
        // draw extreme-points
        var maxMag = (int)Math.Log10(Math.Max(packing.BinShape.Depth, Math.Max(packing.BinShape.Height, packing.BinShape.Width)));
        var cubeSize = (int)Math.Max(Math.Pow(10, maxMag - 2), 1);
        BinPacking3D.PackingPosition selectedExtremePoint = null;
        if (selectedExtremePointIndex < 0) {
          foreach (var ep in ExtremePoints) {
            AddResidualSpacesForExtremePoint(modelGroup, ep);
          }
        } else {
          selectedExtremePoint = ExtremePoints.ToList()[selectedExtremePointIndex];
          AddResidualSpacesForExtremePoint(modelGroup, selectedExtremePoint);
        }

        foreach (var ep in ExtremePoints) {
          Color color;
          if (ep.Equals(selectedExtremePoint)) {
            color = Colors.Yellow;
          } else {
            color = Colors.Red;
          }
          var epModel = new GeometryModel3D { Geometry = new MeshGeometry3D(), Material = new DiffuseMaterial() { Brush = new SolidColorBrush(color) } };
          AddSolidCube((MeshGeometry3D)epModel.Geometry, ep.X, ep.Y, ep.Z, cubeSize, cubeSize, cubeSize);
          modelGroup.Children.Add(epModel);
        }
      }
    }

    private void AddResidualSpacesForExtremePoint(Model3DGroup modelGroup, BinPacking3D.PackingPosition extremePoint) {
      if (showResidualSpaces) {
        foreach (var rs in ResidualSpaces[extremePoint]) {
          var containerModel1 = new GeometryModel3D(new MeshGeometry3D(), new DiffuseMaterial(new SolidColorBrush(residualSpaceColor)));

          modelGroup.Children.Add(containerModel1);

          AddWireframeCube((MeshGeometry3D)containerModel1.Geometry,
            extremePoint.X,
            extremePoint.Y,
            extremePoint.Z,
            rs.Width,
            rs.Height,
            rs.Depth);
        }
      }
       
    }


    private void Clear() {
      ((Model3DGroup)MyModel.Content).Children.Clear();
      materials.Clear();

      mouseDown = false;
      startAngleX = 0;
      startAngleY = 0;
    }

    private void Container3DView_MouseMove(object sender, MouseEventArgs e) {
      if (!mouseDown)
        return;
      var pos = e.GetPosition((IInputElement)this);

      ((AxisAngleRotation3D)rotateX.Rotation).Angle = startAngleX + (pos.X - startPos.X) / 4;
      ((AxisAngleRotation3D)rotateY.Rotation).Angle = startAngleY + (pos.Y - startPos.Y) / 4;
    }

    private void Container3DView_MouseDown(object sender, MouseButtonEventArgs e) {
      startAngleX = ((AxisAngleRotation3D)rotateX.Rotation).Angle;
      startAngleY = ((AxisAngleRotation3D)rotateY.Rotation).Angle;
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
    private void ShowExtremePointsCheckBoxOnChecked(object sender, RoutedEventArgs e) {
      showExtremePoints = true;
      UpdateVisualization();
    }
    private void ShowExtremePointsCheckBoxOnUnchecked(object sender, RoutedEventArgs e) {
      showExtremePoints = false;
      UpdateVisualization();
    }

    private void ShowResidualSpacesCheckBoxOnChecked(object sender, RoutedEventArgs e) {
      showResidualSpaces = true;
      UpdateVisualization();
    }
    private void ShowResidualSpacesCheckBoxOnUnchecked(object sender, RoutedEventArgs e) {
      showResidualSpaces = false;
      UpdateVisualization();
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

      // Point 0, non-edge
      mesh.Positions.Add(new Point3D(x + thickness, y, z + thickness));
      mesh.Positions.Add(new Point3D(x, y + thickness, z + thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + thickness, z));

      // Point 1, non-edge
      mesh.Positions.Add(new Point3D(x + width, y + thickness, z + thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y, z + thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y + thickness, z));

      // Point 2, non-edge
      mesh.Positions.Add(new Point3D(x + width - thickness, y + height, z + thickness));
      mesh.Positions.Add(new Point3D(x + width, y + height - thickness, z + thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y + height - thickness, z));

      // Point 3, non-edge
      mesh.Positions.Add(new Point3D(x, y + height - thickness, z + thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + height, z + thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + height - thickness, z));

      // Point 4, non-edge
      mesh.Positions.Add(new Point3D(x + thickness, y, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x, y + thickness, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + thickness, z + depth));

      // Point 5, non-edge
      mesh.Positions.Add(new Point3D(x + width, y + thickness, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y + thickness, z + depth));

      // Point 6, non-edge
      mesh.Positions.Add(new Point3D(x + width - thickness, y + height, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + width, y + height - thickness, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + width - thickness, y + height - thickness, z + depth));

      // Point 7, non-edge
      mesh.Positions.Add(new Point3D(x, y + height - thickness, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + height, z + depth - thickness));
      mesh.Positions.Add(new Point3D(x + thickness, y + height - thickness, z + depth));

      // Draw the 24 corner plates
      AddPlane(mesh, 0, 1, 32, 2);
      AddPlane(mesh, 0, 2, 33, 3);
      AddPlane(mesh, 0, 3, 34, 1);

      AddPlane(mesh, 4, 6, 36, 7);
      AddPlane(mesh, 4, 5, 35, 6);
      AddPlane(mesh, 4, 7, 37, 5);

      AddPlane(mesh, 8, 10, 39, 11);
      AddPlane(mesh, 8, 9, 38, 10);
      AddPlane(mesh, 8, 11, 40, 9);

      AddPlane(mesh, 12, 13, 41, 14);
      AddPlane(mesh, 12, 14, 42, 15);
      AddPlane(mesh, 12, 15, 43, 13);

      AddPlane(mesh, 16, 18, 44, 17);
      AddPlane(mesh, 16, 19, 45, 18);
      AddPlane(mesh, 16, 17, 46, 19);

      AddPlane(mesh, 20, 23, 48, 22);
      AddPlane(mesh, 20, 22, 47, 21);
      AddPlane(mesh, 20, 21, 49, 23);

      AddPlane(mesh, 24, 27, 51, 26);
      AddPlane(mesh, 24, 26, 50, 25);
      AddPlane(mesh, 24, 25, 52, 27);

      AddPlane(mesh, 28, 31, 54, 30);
      AddPlane(mesh, 28, 30, 53, 29);
      AddPlane(mesh, 28, 29, 55, 31);

      // Draw the connecting plates
      // on the bottom
      AddPlane(mesh, 1, 7, 36, 32);
      AddPlane(mesh, 1, 34, 37, 7);

      AddPlane(mesh, 5, 11, 39, 35);
      AddPlane(mesh, 5, 37, 40, 11);

      AddPlane(mesh, 9, 15, 42, 38);
      AddPlane(mesh, 9, 40, 43, 15);

      AddPlane(mesh, 13, 3, 33, 41);
      AddPlane(mesh, 13, 43, 34, 3);

      // between bottom and top
      AddPlane(mesh, 2, 32, 44, 18);
      AddPlane(mesh, 2, 18, 45, 33);

      AddPlane(mesh, 6, 22, 48, 36);
      AddPlane(mesh, 6, 35, 47, 22);

      AddPlane(mesh, 10, 26, 51, 39);
      AddPlane(mesh, 10, 38, 50, 26);

      AddPlane(mesh, 14, 30, 54, 42);
      AddPlane(mesh, 14, 41, 53, 30);

      // on the top
      AddPlane(mesh, 17, 44, 48, 23);
      AddPlane(mesh, 17, 23, 49, 46);

      AddPlane(mesh, 21, 47, 51, 27);
      AddPlane(mesh, 21, 27, 52, 49);

      AddPlane(mesh, 25, 50, 54, 31);
      AddPlane(mesh, 25, 31, 55, 52);

      AddPlane(mesh, 29, 19, 46, 55);
      AddPlane(mesh, 29, 53, 45, 19);
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
