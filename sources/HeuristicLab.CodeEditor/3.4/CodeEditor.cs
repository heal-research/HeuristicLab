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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Documents;
using HeuristicLab.Common;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using Forms = System.Windows.Forms;
using Input = System.Windows.Input;
using Media = System.Windows.Media;

namespace HeuristicLab.CodeEditor {
  public partial class CodeEditor : Forms.UserControl {
    private static readonly Media.Color WarningColor = Media.Colors.Blue;
    private static readonly Media.Color ErrorColor = Media.Colors.Red;
    private static readonly Media.Color ReadOnlyColor = Media.Colors.Moccasin;

    private const string DefaultDocumentFileName = "Document";
    private const string DefaultTextEditorSyntaxHighlighting = "C#";
    private const string DefaultTextEditorFontFamily = "Consolas";
    private const double DefaultTextEditorFontSize = 13.0;
    private const bool DefaultTextEditorShowLineNumbers = true;
    private const bool DefaultTextEditorShowSpaces = true;
    private const bool DefaultTextEditorShowTabs = true;
    private const bool DefaultTextEditorConvertTabsToSpaces = true;
    private const bool DefaultTextEditorHighlightCurrentLine = true;
    private const int DefaultTextEditorIndentationSize = 2;

    private AssemblyLoader assemblyLoader;
    private ILanguageFeatures languageFeatures;
    private TextMarkerService textMarkerService;

    #region Properties
    internal TextEditor TextEditor { get { return avalonEditWrapper.TextEditor; } }
    internal Input.RoutedCommand CompletionCommand;
    internal CompletionWindow CompletionWindow;
    internal OverloadInsightWindow OverloadInsightWindow;

    private TextDocument Doc { get { return TextEditor.Document; } }

    private ITextMarker prefixMarker;
    private string prefix = string.Empty;
    public string Prefix {
      get { return prefix; }
      set {
        if (value == null) value = string.Empty;
        if (prefix == value) return;
        if (prefixMarker != null) textMarkerService.Remove(prefixMarker);
        Doc.Remove(0, prefix.Length);
        prefix = value;
        if (value.Length > 0) {
          Doc.Insert(0, prefix);
          prefixMarker = textMarkerService.Create(0, prefix.Length);
          prefixMarker.BackgroundColor = ReadOnlyColor;
        }
      }
    }

    private ITextMarker suffixMarker;
    private string suffix = string.Empty;
    public string Suffix {
      get { return suffix; }
      set {
        if (value == null) value = string.Empty;
        if (suffix == value) return;
        if (suffixMarker != null) textMarkerService.Remove(suffixMarker);
        Doc.Remove(Doc.TextLength - suffix.Length, suffix.Length);
        suffix = value;
        if (value.Length > 0) {
          int offset = Doc.TextLength;
          Doc.Insert(offset, suffix);
          suffixMarker = textMarkerService.Create(offset, suffix.Length);
          suffixMarker.BackgroundColor = ReadOnlyColor;
        }
      }
    }

    public string UserCode {
      get { return Doc.GetText(prefix.Length, Doc.TextLength - suffix.Length - prefix.Length); }
      set {
        if (Doc.Text == value) return;
        Doc.Replace(prefix.Length, Doc.TextLength - suffix.Length - prefix.Length, value);
      }
    }

    #region TextEditor
    [DefaultValue(DefaultTextEditorSyntaxHighlighting)]
    public string TextEditorSyntaxHighlighting {
      get { return TextEditor.SyntaxHighlighting.Name; }
      set {
        TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(value);
        ApplyLanguageFeatures();
      }
    }

    [DefaultValue(DefaultTextEditorShowLineNumbers)]
    public bool TextEditorShowLineNumbers {
      get { return TextEditor.ShowLineNumbers; }
      set { TextEditor.ShowLineNumbers = value; }
    }

    [DefaultValue(DefaultTextEditorShowSpaces)]
    public bool TextEditorShowSpaces {
      get { return TextEditor.Options.ShowSpaces; }
      set { TextEditor.Options.ShowSpaces = value; }
    }

    [DefaultValue(DefaultTextEditorShowTabs)]
    public bool TextEditorShowTabs {
      get { return TextEditor.Options.ShowTabs; }
      set { TextEditor.Options.ShowTabs = value; }
    }

    [DefaultValue(DefaultTextEditorConvertTabsToSpaces)]
    public bool TextEditorConvertTabsToSpaces {
      get { return TextEditor.Options.ConvertTabsToSpaces; }
      set { TextEditor.Options.ConvertTabsToSpaces = value; }
    }

    [DefaultValue(DefaultTextEditorHighlightCurrentLine)]
    public bool TextEditorHighlightCurrentLine {
      get { return TextEditor.Options.HighlightCurrentLine; }
      set { TextEditor.Options.HighlightCurrentLine = value; }
    }

    [DefaultValue(DefaultTextEditorIndentationSize)]
    public int TextEditorIndentationSize {
      get { return TextEditor.Options.IndentationSize; }
      set { TextEditor.Options.IndentationSize = value; }
    }
    #endregion

    public bool ReadOnly {
      get { return TextEditor.IsReadOnly; }
      set { TextEditor.IsReadOnly = value; }
    }
    #endregion

    public CodeEditor() {
      InitializeComponent();
      InitializeTextEditor();
    }

    private void InitializeTextEditor() {
      #region AssemblyLoader
      assemblyLoader = new AssemblyLoader();
      assemblyLoader.AssembliesLoading += (sender, args) => OnAssembliesLoading(args.Value);
      assemblyLoader.InternalAssembliesLoaded += (sender, args) => OnInternalAssembliesLoaded(args.Value);
      assemblyLoader.AssembliesLoaded += (sender, args) => OnAssembliesLoaded(args.Value);
      assemblyLoader.AssembliesUnloading += (sender, args) => OnAssembliesUnloading(args.Value);
      assemblyLoader.InternalAssembliesUnloaded += (sender, args) => OnInternalAssembliesUnloaded(args.Value);
      assemblyLoader.AssembliesUnloaded += (sender, args) => OnAssembliesUnloaded(args.Value);
      #endregion

      #region TextMarkerService
      textMarkerService = new TextMarkerService(TextEditor.Document);
      TextEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
      TextEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
      TextEditor.TextArea.TextView.Services.AddService(typeof(ITextMarkerService), textMarkerService);

      #endregion

      #region ReadOnlySectionProvider
      TextEditor.TextArea.ReadOnlySectionProvider = new MethodDefinitionReadOnlySectionProvider(this);
      #endregion

      #region SearchPanel
      SearchPanel.Install(TextEditor);
      #endregion

      #region CompletionCommand
      CompletionCommand = new Input.RoutedCommand();
      CompletionCommand.InputGestures.Add(new Input.KeyGesture(Input.Key.Space, Input.ModifierKeys.Control));
      #endregion

      #region MoveLinesUpCommand
      var moveLinesUpCommand = new Input.RoutedCommand();
      moveLinesUpCommand.InputGestures.Add(new Input.KeyGesture(Input.Key.Up, Input.ModifierKeys.Alt));
      var moveLinesUpCommandBinding = new Input.CommandBinding(moveLinesUpCommand, (sender, args) => ExecuteMoveLinesCommand(MovementDirection.Up));
      TextEditor.CommandBindings.Add(moveLinesUpCommandBinding);
      #endregion

      #region MoveLinesDownCommand
      var moveLinesDownCommand = new Input.RoutedCommand();
      moveLinesDownCommand.InputGestures.Add(new Input.KeyGesture(Input.Key.Down, Input.ModifierKeys.Alt));
      var moveLinesDownCommandBinding = new Input.CommandBinding(moveLinesDownCommand, (sender, args) => ExecuteMoveLinesCommand(MovementDirection.Down));
      TextEditor.CommandBindings.Add(moveLinesDownCommandBinding);
      #endregion

      #region GoToLineCommand
      var goToLineCommand = new Input.RoutedCommand();
      goToLineCommand.InputGestures.Add(new Input.KeyGesture(Input.Key.G, Input.ModifierKeys.Control));
      var goToLineCommandBinding = new Input.CommandBinding(goToLineCommand, (sender, args) => ExecuteGoToLineCommand());
      TextEditor.CommandBindings.Add(goToLineCommandBinding);
      #endregion

      TextEditorSyntaxHighlighting = DefaultTextEditorSyntaxHighlighting;
      TextEditorShowLineNumbers = DefaultTextEditorShowLineNumbers;
      TextEditorShowSpaces = DefaultTextEditorShowSpaces;
      TextEditorShowTabs = DefaultTextEditorShowTabs;
      TextEditorConvertTabsToSpaces = DefaultTextEditorConvertTabsToSpaces;
      TextEditorHighlightCurrentLine = DefaultTextEditorHighlightCurrentLine;
      TextEditorIndentationSize = DefaultTextEditorIndentationSize;

      Doc.FileName = DefaultDocumentFileName;

      TextEditor.FontFamily = new Media.FontFamily(DefaultTextEditorFontFamily);
      TextEditor.FontSize = DefaultTextEditorFontSize;
      TextEditor.Options.EnableVirtualSpace = true;
      TextEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(TextEditor.Options);

      TextEditor.TextChanged += (sender, args) => {
        textMarkerService.RemoveAll(x => x != prefixMarker && x != suffixMarker);
        OnTextEditorTextChanged();
      };
    }

    #region Assembly Management
    public void AddAssembly(Assembly a) {
      assemblyLoader.AddAssembly(a);
    }

    public void AddAssemblies(IEnumerable<Assembly> assemblies) {
      assemblyLoader.AddAssemblies(assemblies);
    }

    public async Task AddAssembliesAsync(IEnumerable<Assembly> assemblies) {
      await assemblyLoader.AddAssembliesAsync(assemblies);
    }

    public void RemoveAssembly(Assembly a) {
      assemblyLoader.RemoveAssembly(a);
    }
    #endregion

    public void ScrollToPosition(int line, int column) {
      var segment = GetSegmentAtLocation(line, column);
      TextEditor.CaretOffset = segment.Offset + segment.Length;
      TextEditor.ScrollToLine(line);
    }

    public void ScrollAfterPrefix() {
      var location = Doc.GetLocation(prefix.Length);
      ScrollToPosition(location.Line, location.Column);
    }

    private enum MovementDirection { Up, Down }
    private void ExecuteMoveLinesCommand(MovementDirection movementDirection) {
      var textArea = TextEditor.TextArea;
      var selection = textArea.Selection;
      var caret = textArea.Caret;

      var selectionStartPosition = selection.StartPosition;
      var selectionEndPosition = selection.EndPosition;
      var caretPosition = caret.Position;
      int caretOffset = caret.Offset;

      bool advancedPositionCalcualtion = selection is RectangleSelection || !selection.IsEmpty;

      int selectionStartOffset, selectionEndOffset;
      if (advancedPositionCalcualtion) {
        if (selectionStartPosition.CompareTo(selectionEndPosition) > 0) {
          var temp = selectionStartPosition;
          selectionStartPosition = selectionEndPosition;
          selectionEndPosition = temp;
        }
        selectionStartOffset = Doc.GetOffset(selectionStartPosition.Location);
        selectionEndOffset = Doc.GetOffset(selectionEndPosition.Location);
      } else {
        selectionStartOffset = selectionEndOffset = TextEditor.SelectionStart;
      }

      int selectionLength = selection.Length;

      var startLine = Doc.GetLineByOffset(selectionStartOffset);
      var endLine = Doc.GetLineByOffset(selectionEndOffset);

      if (selection.IsMultiline && selectionEndOffset == endLine.Offset) {
        if (movementDirection == MovementDirection.Down && endLine.TotalLength == 0) return;
        endLine = endLine.PreviousLine;
      }

      if (movementDirection == MovementDirection.Up && startLine.LineNumber == 1) return;
      if (movementDirection == MovementDirection.Down && endLine.LineNumber == Doc.LineCount) return;

      int startOffset = startLine.Offset;
      string primaryText = Doc.GetText(startOffset, endLine.EndOffset - startOffset);
      string primaryDelimiter = Doc.GetText(endLine.EndOffset, endLine.DelimiterLength);

      var secondaryLine = movementDirection == MovementDirection.Up ? startLine.PreviousLine : endLine.NextLine;
      string secondaryText = Doc.GetText(secondaryLine.Offset, secondaryLine.Length);
      string secondaryDelimiter = Doc.GetText(secondaryLine.EndOffset, secondaryLine.DelimiterLength);

      if (string.IsNullOrEmpty(primaryText + primaryDelimiter) || string.IsNullOrEmpty(secondaryText + secondaryDelimiter)) return;

      if (movementDirection == MovementDirection.Up) {
        string replacementText = primaryText + secondaryDelimiter + secondaryText + primaryDelimiter;
        Doc.Replace(secondaryLine.Offset, replacementText.Length, replacementText);
        int correctionLength = secondaryText.Length + secondaryDelimiter.Length;
        selectionStartOffset -= correctionLength;
        caretOffset -= correctionLength;
      } else {
        string replacementText = secondaryText + primaryDelimiter + primaryText + secondaryDelimiter;
        Doc.Replace(startLine.Offset, replacementText.Length, replacementText);
        int correctionLength = secondaryText.Length + primaryDelimiter.Length;
        selectionStartOffset += correctionLength;
        caretOffset += correctionLength;
      }

      if (advancedPositionCalcualtion) {
        var newSelectionStartLocation = Doc.GetLocation(selectionStartOffset);
        int selectionLineOffset = newSelectionStartLocation.Line - Math.Min(selectionStartPosition.Line, selectionEndPosition.Line);
        selectionStartPosition.Line += selectionLineOffset;
        selectionEndPosition.Line += selectionLineOffset;
        if (selectionEndPosition.Line > Doc.LineCount) {
          var newLocation = Doc.GetLocation(Doc.TextLength);
          selectionEndPosition.Line = newLocation.Line;
          selectionEndPosition.Column = newLocation.Column;
          selectionEndPosition.VisualColumn = newLocation.Column - 1;
          selectionEndPosition.IsAtEndOfLine = selectionStartPosition.IsAtEndOfLine; // actual value does not matter; needed for comparison
        }

        if (selectionStartPosition == selectionEndPosition)
          textArea.ClearSelection();
        else {
          if (selection is RectangleSelection)
            textArea.Selection = new RectangleSelection(textArea, selectionStartPosition, selectionEndPosition);
          else
            textArea.Selection = new SimpleSelection(textArea, selectionStartPosition, selectionEndPosition);
        }
      } else {
        TextEditor.SelectionStart = selectionStartOffset;
        TextEditor.SelectionLength = selectionLength;
      }

      var newCaretLocation = Doc.GetLocation(Math.Min(caretOffset, Doc.TextLength));
      var newCaretPosition = new TextViewPosition(newCaretLocation);
      if (caretPosition.VisualColumn > caretPosition.Column) {
        newCaretPosition.VisualColumn = caretPosition.VisualColumn;
      }
      caret.Position = newCaretPosition;
    }

    private void ExecuteGoToLineCommand() {
      using (var dlg = new GoToLineDialog(TextEditor)) {
        var result = dlg.ShowDialog(this);
        if (result == Forms.DialogResult.OK) {
          int lineNumber = dlg.Line;
          var line = Doc.GetLineByNumber(lineNumber);
          int offset = line.Offset;
          if (TextUtilities.GetLeadingWhitespace(Doc, line).Length > 0)
            offset = TextUtilities.GetNextCaretPosition(Doc, offset, LogicalDirection.Forward, CaretPositioningMode.WordBorder);
          TextEditor.CaretOffset = offset;
        }
      }
    }

    #region Compiler Errors
    public void ShowCompileErrors(CompilerErrorCollection compilerErrors) {
      if (compilerErrors == null) return;

      textMarkerService.RemoveAll(x => x != prefixMarker && x != suffixMarker);

      foreach (CompilerError error in compilerErrors) {
        var startLocation = Doc.GetLocation(prefix.Length);
        if (error.Line == 1) error.Column += startLocation.Column;
        error.Line += startLocation.Line;
        AddErrorMarker(error);
      }
    }

    private void AddErrorMarker(CompilerError error) {
      var segment = GetSegmentAtLocation(error.Line, error.Column);
      var marker = textMarkerService.Create(segment.Offset, segment.Length);
      marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
      marker.MarkerColor = error.IsWarning ? WarningColor : ErrorColor;
    }

    private ISegment GetSegmentAtLocation(int line, int column) {
      line = Math.Max(Doc.GetLocation(prefix.Length).Line, line - 1);
      line = Math.Min(Doc.GetLocation(Doc.TextLength - suffix.Length).Line, line);

      var startOffset = Doc.GetOffset(line, column);
      var lineEndOffset = Doc.GetLineByNumber(line).EndOffset;
      var endOffset = TextUtilities.GetNextCaretPosition(Doc, startOffset, LogicalDirection.Forward, CaretPositioningMode.WordBorder);
      if (endOffset < 0) endOffset = startOffset;
      endOffset = Math.Min(lineEndOffset + 1, endOffset);

      var segment = new TextSegment { StartOffset = startOffset, EndOffset = endOffset };

      return segment;
    }
    #endregion

    private void ApplyLanguageFeatures() {
      switch (TextEditorSyntaxHighlighting) {
        case "XML":
          languageFeatures = new XmlLanguageFeatures(this);
          break;
        default:
          languageFeatures = new CSharpLanguageFeatures(this);
          break;
      }
    }

    #region Events
    public event EventHandler TextEditorTextChanged;
    private void OnTextEditorTextChanged() {
      var handler = TextEditorTextChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesLoading;
    private void OnAssembliesLoading(IEnumerable<Assembly> args) {
      var handler = AssembliesLoading;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesLoaded;
    private void OnAssembliesLoaded(IEnumerable<Assembly> args) {
      var handler = AssembliesLoaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<IUnresolvedAssembly>>> InternalAssembliesLoaded;
    private void OnInternalAssembliesLoaded(IEnumerable<IUnresolvedAssembly> args) {
      var handler = InternalAssembliesLoaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<IUnresolvedAssembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesUnloading;
    private void OnAssembliesUnloading(IEnumerable<Assembly> args) {
      var handler = AssembliesUnloading;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesUnloaded;
    private void OnAssembliesUnloaded(IEnumerable<Assembly> args) {
      var handler = AssembliesUnloaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<IUnresolvedAssembly>>> InternalAssembliesUnloaded;
    private void OnInternalAssembliesUnloaded(IEnumerable<IUnresolvedAssembly> args) {
      var handler = InternalAssembliesUnloaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<IUnresolvedAssembly>>(args));
    }
    #endregion
  }
}
