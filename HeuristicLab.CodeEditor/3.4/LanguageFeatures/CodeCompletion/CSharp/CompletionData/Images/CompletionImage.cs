﻿#region License Information
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.AvalonEdit.CodeCompletion {
  /// <summary>
  /// Provides icons for code-completion.
  /// </summary>
  public class CompletionImage {
    /*
    #region Non-Entity Images
    static readonly BitmapImage namespaceImage = LoadBitmap("NameSpace");

    /// <summary>
    /// Gets the image for namespaces.
    /// </summary>
    public static ImageSource NamespaceImage {
      get { return namespaceImage; }
    }

    static BitmapImage LoadBitmap(string name) {
      BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/HeuristicLab.CodeEditor-3.4;component/LanguageFeatures/CodeCompletion/CSharp/CompletionData/Images/" + name + ".png"));
      image.Freeze();
      return image;
    }
    #endregion

    #region Entity Images
    static readonly CompletionImage imageClass = new CompletionImage("Class", false);
    static readonly CompletionImage imageStruct = new CompletionImage("Struct", false);
    static readonly CompletionImage imageInterface = new CompletionImage("Interface", false);
    static readonly CompletionImage imageDelegate = new CompletionImage("Delegate", false);
    static readonly CompletionImage imageEnum = new CompletionImage("Enum", false);
    static readonly CompletionImage imageStaticClass = new CompletionImage("StaticClass", false);

    /// <summary>Gets the image used for non-static classes.</summary>
    public static CompletionImage Class { get { return imageClass; } }

    /// <summary>Gets the image used for structs.</summary>
    public static CompletionImage Struct { get { return imageStruct; } }

    /// <summary>Gets the image used for interfaces.</summary>
    public static CompletionImage Interface { get { return imageInterface; } }

    /// <summary>Gets the image used for delegates.</summary>
    public static CompletionImage Delegate { get { return imageDelegate; } }

    /// <summary>Gets the image used for enums.</summary>
    public static CompletionImage Enum { get { return imageEnum; } }

    /// <summary>Gets the image used for modules/static classes.</summary>
    public static CompletionImage StaticClass { get { return imageStaticClass; } }

    static readonly CompletionImage imageField = new CompletionImage("Field", true);
    static readonly CompletionImage imageFieldReadOnly = new CompletionImage("FieldReadOnly", true);
    static readonly CompletionImage imageLiteral = new CompletionImage("Literal", false);
    static readonly CompletionImage imageEnumValue = new CompletionImage("EnumValue", false);
    static readonly CompletionImage imageTemplate = new CompletionImage("Template", false);

    /// <summary>Gets the image used for non-static classes.</summary>
    public static CompletionImage Field { get { return imageField; } }

    /// <summary>Gets the image used for structs.</summary>
    public static CompletionImage ReadOnlyField { get { return imageFieldReadOnly; } }

    /// <summary>Gets the image used for constants.</summary>
    public static CompletionImage Literal { get { return imageLiteral; } }

    /// <summary>Gets the image used for enum values.</summary>
    public static CompletionImage EnumValue { get { return imageEnumValue; } }

    /// <summary>Gets the image used for templates.</summary>
    public static CompletionImage Template { get { return imageTemplate; } }

    static readonly CompletionImage imageMethod = new CompletionImage("Method", true);
    static readonly CompletionImage imageConstructor = new CompletionImage("Constructor", true);
    static readonly CompletionImage imageVirtualMethod = new CompletionImage("VirtualMethod", true);
    static readonly CompletionImage imageOperator = new CompletionImage("Operator", false);
    static readonly CompletionImage imageExtensionMethod = new CompletionImage("ExtensionMethod", true);
    static readonly CompletionImage imagePInvokeMethod = new CompletionImage("PInvokeMethod", true);
    static readonly CompletionImage imageProperty = new CompletionImage("Property", true);
    static readonly CompletionImage imageIndexer = new CompletionImage("Indexer", true);
    static readonly CompletionImage imageEvent = new CompletionImage("Event", true);

    /// <summary>Gets the image used for methods.</summary>
    public static CompletionImage Method { get { return imageMethod; } }

    /// <summary>Gets the image used for constructos.</summary>
    public static CompletionImage Constructor { get { return imageConstructor; } }

    /// <summary>Gets the image used for virtual methods.</summary>
    public static CompletionImage VirtualMethod { get { return imageVirtualMethod; } }

    /// <summary>Gets the image used for operators.</summary>
    public static CompletionImage Operator { get { return imageOperator; } }

    /// <summary>Gets the image used for extension methods.</summary>
    public static CompletionImage ExtensionMethod { get { return imageExtensionMethod; } }

    /// <summary>Gets the image used for P/Invoke methods.</summary>
    public static CompletionImage PInvokeMethod { get { return imagePInvokeMethod; } }

    /// <summary>Gets the image used for properties.</summary>
    public static CompletionImage Property { get { return imageProperty; } }

    /// <summary>Gets the image used for indexers.</summary>
    public static CompletionImage Indexer { get { return imageIndexer; } }

    /// <summary>Gets the image used for events.</summary>
    public static CompletionImage Event { get { return imageEvent; } }

    /// <summary>
    /// Gets the CompletionImage instance for the specified entity.
    /// Returns null when no image is available for the entity type.
    /// </summary>
    public static CompletionImage GetCompletionImage(IEntity entity) {
      if (entity == null)
        throw new ArgumentNullException("entity");
      switch (entity.SymbolKind) {
        case SymbolKind.TypeDefinition:
          return GetCompletionImageForType(((ITypeDefinition)entity).Kind, entity.IsStatic);
        case SymbolKind.Field:
          IField field = (IField)entity;
          if (field.IsConst) {
            if (field.DeclaringTypeDefinition != null && field.DeclaringTypeDefinition.Kind == TypeKind.Enum)
              return imageEnumValue;
            else
              return imageLiteral;
          }
          return field.IsReadOnly ? imageFieldReadOnly : imageField;
        case SymbolKind.Method:
          IMethod method = (IMethod)entity;
          if (method.IsExtensionMethod)
            return imageExtensionMethod;
          else
            return method.IsOverridable ? imageVirtualMethod : imageMethod;
        case SymbolKind.Property:
          return imageProperty;
        case SymbolKind.Indexer:
          return imageIndexer;
        case SymbolKind.Event:
          return imageEvent;
        case SymbolKind.Operator:
        case SymbolKind.Destructor:
          return imageOperator;
        case SymbolKind.Constructor:
          return imageConstructor;
        default:
          return null;
      }
    }

    /// <summary>
    /// Gets the CompletionImage instance for the specified entity.
    /// Returns null when no image is available for the entity type.
    /// </summary>
    public static CompletionImage GetCompletionImage(IUnresolvedEntity entity) {
      if (entity == null)
        throw new ArgumentNullException("entity");
      switch (entity.SymbolKind) {
        case SymbolKind.TypeDefinition:
          return GetCompletionImageForType(((IUnresolvedTypeDefinition)entity).Kind, entity.IsStatic);
        case SymbolKind.Field:
          IUnresolvedField field = (IUnresolvedField)entity;
          if (field.IsConst) {
            if (field.DeclaringTypeDefinition != null && field.DeclaringTypeDefinition.Kind == TypeKind.Enum)
              return imageEnumValue;
            else
              return imageLiteral;
          }
          return field.IsReadOnly ? imageFieldReadOnly : imageField;
        case SymbolKind.Method:
          IUnresolvedMethod method = (IUnresolvedMethod)entity;
          return method.IsOverridable ? imageVirtualMethod : imageMethod;
        case SymbolKind.Property:
          return imageProperty;
        case SymbolKind.Indexer:
          return imageIndexer;
        case SymbolKind.Event:
          return imageEvent;
        case SymbolKind.Operator:
        case SymbolKind.Destructor:
          return imageOperator;
        case SymbolKind.Constructor:
          return imageConstructor;
        default:
          return null;
      }
    }

    static CompletionImage GetCompletionImageForType(TypeKind typeKind, bool isStatic) {
      switch (typeKind) {
        case TypeKind.Interface:
          return imageInterface;
        case TypeKind.Struct:
        case TypeKind.Void:
          return imageStruct;
        case TypeKind.Delegate:
          return imageDelegate;
        case TypeKind.Enum:
          return imageEnum;
        case TypeKind.Class:
          return isStatic ? imageStaticClass : imageClass;
        case TypeKind.Module:
          return imageStaticClass;
        default:
          return null;
      }
    }

    /// <summary>
    /// Gets the image for the specified entity.
    /// Returns null when no image is available for the entity type.
    /// </summary>
    public static ImageSource GetImage(IEntity entity) {
      CompletionImage image = GetCompletionImage(entity);
      if (image != null)
        return image.GetImage(entity.Accessibility, entity.IsStatic);
      else
        return null;
    }

    /// <summary>
    /// Gets the image for the specified entity.
    /// Returns null when no image is available for the entity type.
    /// </summary>
    public static ImageSource GetImage(IUnresolvedEntity entity) {
      CompletionImage image = GetCompletionImage(entity);
      if (image != null)
        return image.GetImage(entity.Accessibility, entity.IsStatic);
      else
        return null;
    }
    #endregion

    #region Overlays
    static readonly BitmapImage overlayStatic = LoadBitmap("OverlayStatic");

    /// <summary>
    /// Gets the overlay image for the static modifier.
    /// </summary>
    public ImageSource StaticOverlay { get { return overlayStatic; } }

    const int AccessibilityOverlaysLength = 5;

    static readonly BitmapImage[] accessibilityOverlays = new BitmapImage[AccessibilityOverlaysLength] {
      null,
      LoadBitmap("OverlayPrivate"),
      LoadBitmap("OverlayProtected"),
      LoadBitmap("OverlayInternal"),
      LoadBitmap("OverlayProtectedInternal")
    };

    /// <summary>
    /// Gets an overlay image for the specified accessibility.
    /// Returns null if no overlay exists (for example, public members don't use overlays).
    /// </summary>
    public static ImageSource GetAccessibilityOverlay(NRefactory.TypeSystem.Accessibility accessibility) {
      return accessibilityOverlays[GetAccessibilityOverlayIndex(accessibility)];
    }

    static int GetAccessibilityOverlayIndex(NRefactory.TypeSystem.Accessibility accessibility) {
      switch (accessibility) {
        case NRefactory.TypeSystem.Accessibility.Private:
          return 1;
        case NRefactory.TypeSystem.Accessibility.Protected:
          return 2;
        case NRefactory.TypeSystem.Accessibility.Internal:
          return 3;
        case NRefactory.TypeSystem.Accessibility.ProtectedOrInternal:
        case NRefactory.TypeSystem.Accessibility.ProtectedAndInternal:
          return 4;
        default:
          return 0;
      }
    }
    #endregion

    #region Instance Members (add overlay to entity image)
    readonly string imageName;
    readonly bool showStaticOverlay;

    private CompletionImage(string imageName, bool showStaticOverlay) {
      this.imageName = imageName;
      this.showStaticOverlay = showStaticOverlay;
    }

    ImageSource[] images = new ImageSource[2 * AccessibilityOverlaysLength];
    // 0..N-1  = base image + accessibility overlay
    // N..2N-1 = base image + static overlay + accessibility overlay

    /// <summary>
    /// Gets the image without any overlays.
    /// </summary>
    public ImageSource BaseImage {
      get {
        ImageSource image = images[0];
        if (image == null) {
          image = LoadBitmap(imageName);
          Thread.MemoryBarrier();
          images[0] = image;
        }
        return image;
      }
    }

    /// <summary>
    /// Gets this image combined with the specified accessibility overlay.
    /// </summary>
    public ImageSource GetImage(NRefactory.TypeSystem.Accessibility accessibility, bool isStatic = false) {
      int accessibilityIndex = GetAccessibilityOverlayIndex(accessibility);
      int index;
      if (isStatic && showStaticOverlay)
        index = accessibilityOverlays.Length + accessibilityIndex;
      else
        index = accessibilityIndex;

      if (index == 0)
        return this.BaseImage;

      ImageSource image = images[index];
      if (image == null) {
        DrawingGroup g = new DrawingGroup();
        Rect iconRect = new Rect(0, 0, 16, 16);
        g.Children.Add(new ImageDrawing(this.BaseImage, iconRect));

        if (accessibilityOverlays[accessibilityIndex] != null)
          g.Children.Add(new ImageDrawing(accessibilityOverlays[accessibilityIndex], iconRect));

        image = new DrawingImage(g);
        image.Freeze();
        Thread.MemoryBarrier();
        images[index] = image;
      }
      return image;
    }

    /// <inheritdoc/>
    public override string ToString() {
      return "[CompletionImage " + imageName + "]";
    }
    #endregion
    */
  }
}
