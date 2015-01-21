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
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICompletionData = ICSharpCode.NRefactory.Completion.ICompletionData;

namespace HeuristicLab.CodeEditor {
  internal class CSharpCodeCompletionDataFactory : ICompletionDataFactory, IParameterCompletionDataFactory {
    private readonly CSharpCodeCompletionContext context;

    public CSharpCodeCompletionDataFactory(CSharpCodeCompletionContext context) {
      this.context = context;
    }

    #region ICompletionDataFactory Members
    public ICompletionData CreateEntityCompletionData(IEntity entity) {
      return new EntityCompletionData(entity);
    }

    public ICompletionData CreateEntityCompletionData(IEntity entity, string text) {
      return new EntityCompletionData(entity) { CompletionText = text, DisplayText = text };
    }

    public ICompletionData CreateTypeCompletionData(IType type, bool fullName, bool isInAttributeContext, bool addForTypeCreation) {
      var typeDef = type.GetDefinition();
      if (typeDef != null) { return new EntityCompletionData(typeDef); }

      string name = fullName ? type.FullName : type.Name;
      if (isInAttributeContext && name.EndsWith("Attribute", StringComparison.Ordinal) && name.Length > "Attribute".Length) {
        name = name.Substring(0, name.Length - "Attribute".Length);
      }
      return new CompletionData(name);
    }

    public ICompletionData CreateMemberCompletionData(IType type, IEntity member) {
      return new EntityCompletionData(member);
    }

    public ICompletionData CreateLiteralCompletionData(string title, string description = null, string insertText = null) {
      return new CompletionData(title) {
        Description = description,
        CompletionText = insertText ?? title,
        Image = CompletionImage.Literal.BaseImage,
        Priority = 2
      };
    }

    public ICompletionData CreateXmlDocCompletionData(string title, string description = null, string insertText = null) {
      return new CompletionData(title) {
        Description = description,
        CompletionText = insertText ?? title,
        Image = CompletionImage.Template.BaseImage,
      };
    }

    public ICompletionData CreateNamespaceCompletionData(INamespace ns) {
      return new CompletionData(ns.Name) { Image = CompletionImage.NamespaceImage };
    }

    public ICompletionData CreateVariableCompletionData(IVariable variable) {
      return new VariableCompletionData(variable);
    }

    public ICompletionData CreateVariableCompletionData(ITypeParameter parameter) {
      return new CompletionData(parameter.Name);
    }

    public ICompletionData CreateEventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterDefinition, IUnresolvedMember currentMember, IUnresolvedTypeDefinition currentType) {
      return new CompletionData(varName);
    }

    public ICompletionData CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m) {
      return new CompletionData(m.Name);
    }

    public ICompletionData CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m) {
      return new CompletionData(m.Name);
    }

    public ICompletionData CreateImportCompletionData(IType type, bool useFullName, bool addForTypeCreation) {
      var typeDef = type.GetDefinition();
      return new ImportCompletionData(typeDef, context.TypeResolveContextAtCaret, useFullName);
    }

    public IEnumerable<ICompletionData> CreateCodeTemplateCompletionData() {
      yield break;
    }

    public ICompletionData CreateFormatItemCompletionData(string format, string description, object example) {
      return null;
    }

    public IEnumerable<ICompletionData> CreatePreProcessorDefinesCompletionData() {
      yield break;
    }
    #endregion

    #region IParameterCompletionDataFactory Members
    public IParameterDataProvider CreateConstructorProvider(int startOffset, IType type, ICSharpCode.NRefactory.CSharp.AstNode thisInitializer) {
      return CreateMethodDataProvider(startOffset, type.GetConstructors());
    }

    public IParameterDataProvider CreateConstructorProvider(int startOffset, IType type) {
      return CreateMethodDataProvider(startOffset, type.GetConstructors());
    }

    public IParameterDataProvider CreateDelegateDataProvider(int startOffset, IType type) {
      return CreateMethodDataProvider(startOffset, new[] { type.GetDelegateInvokeMethod() });
    }

    public IParameterDataProvider CreateIndexerParameterDataProvider(int startOffset, IType type, IEnumerable<IProperty> accessibleIndexers, ICSharpCode.NRefactory.CSharp.AstNode resolvedNode) {
      return null;
    }

    public IParameterDataProvider CreateMethodDataProvider(int startOffset, IEnumerable<IMethod> methods) {
      return new CSharpOverloadProvider(context, startOffset, from m in methods where m != null select new CSharpInsightItem(m));
    }

    public IParameterDataProvider CreateTypeParameterDataProvider(int startOffset, IEnumerable<IMethod> methods) {
      return CreateMethodDataProvider(startOffset, methods);
    }

    public IParameterDataProvider CreateTypeParameterDataProvider(int startOffset, IEnumerable<IType> types) {
      return null;
    }
    #endregion
  }
}