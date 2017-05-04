using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;


namespace PersistenceCodeFix {
  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PersistenceCodeFixProvider)), Shared]
  public class PersistenceCodeFixProvider : CodeFixProvider {
    private const string title = "Convert to Persistence-4.0";

    public sealed override ImmutableArray<string> FixableDiagnosticIds {
      get { return ImmutableArray.Create(MissingStorableTypeAnalyzer.DiagnosticId); }
    }

    public sealed override FixAllProvider GetFixAllProvider() {
      // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
      return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      var diagnostic = context.Diagnostics.First();
      var diagnosticSpan = diagnostic.Location.SourceSpan;

      // Find the type declaration identified by the diagnostic.
      var typeDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

      // Register a code action that will invoke the fix.
      context.RegisterCodeFix(
        CodeAction.Create(
          title: title,
          createChangedSolution: c => AddStorableTypeAttribute(context.Document, typeDeclaration, c),
          equivalenceKey: title),
        diagnostic);
    }


    private async Task<Solution> AddStorableTypeAttribute(Document document, BaseTypeDeclarationSyntax typeDecl, CancellationToken cancellationToken) {
      var newGuid = Guid.NewGuid();
      var attributeLists = typeDecl.AttributeLists.ToDictionary(l => l, l => (AttributeListSyntax)null);
      bool renamed = false;
      AttributeListSyntax newAttrList = null;
      SyntaxList<AttributeListSyntax> oldAttrList;

      foreach (var kvp in attributeLists.ToArray()) {
        var newList = SyntaxFactory.AttributeList();
        foreach (var attr in kvp.Key.Attributes) {
          if (attr.Name.WithoutTrivia().ToString() == "StorableClass") {
            // rename
            var guidArgument = SyntaxFactory.AttributeArgument(
                SyntaxFactory.LiteralExpression(
                  SyntaxKind.StringLiteralExpression,
                  SyntaxFactory.Literal(newGuid.ToString())));

            // collect existing arguments (if any) and append the GUID argument
            var argumentList = new List<AttributeArgumentSyntax>();
            if (attr.ArgumentList != null) argumentList.AddRange(attr.ArgumentList.Arguments);
            argumentList.Add(guidArgument);

            newList = newList.WithAttributes(
              newList.Attributes.Add(
                attr.WithName(SyntaxFactory.IdentifierName("StorableType"))
                .WithLeadingTrivia(attr.GetLeadingTrivia())
                .WithTrailingTrivia(attr.GetTrailingTrivia())
                .WithArgumentList(SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(argumentList)))));

            renamed = true;
          } else {
            newList = newList.WithAttributes(newList.Attributes.Add(attr));
          }
        }

        attributeLists[kvp.Key] = newList
          .WithLeadingTrivia(kvp.Key.GetLeadingTrivia())
          .WithTrailingTrivia(kvp.Key.GetTrailingTrivia());
      }

      if (!renamed) {
        oldAttrList = typeDecl.AttributeLists;


        newAttrList = SyntaxFactory.AttributeList(
          SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
            SyntaxFactory.Attribute(
                SyntaxFactory.IdentifierName("StorableType"))
              .WithArgumentList(
                SyntaxFactory.AttributeArgumentList(
                  SyntaxFactory.SingletonSeparatedList<AttributeArgumentSyntax>(
                    SyntaxFactory.AttributeArgument(
                        SyntaxFactory.LiteralExpression(
                          SyntaxKind.StringLiteralExpression,
                          SyntaxFactory.Literal(newGuid.ToString())))
                      )))));

      } else {
        oldAttrList = SyntaxFactory.List(attributeLists.Values);
      }

      BaseTypeDeclarationSyntax newTypeDecl;
      var classDecl = typeDecl as ClassDeclarationSyntax;
      var structDecl = typeDecl as StructDeclarationSyntax;
      var interfaceDecl = typeDecl as InterfaceDeclarationSyntax;
      var enumDecl = typeDecl as EnumDeclarationSyntax;
      if (classDecl != null) {
        if (newAttrList != null) newTypeDecl = classDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList.Add(newAttrList));
        else newTypeDecl = classDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList);
      } else if (structDecl != null) {
        if (newAttrList != null) newTypeDecl = structDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList.Add(newAttrList));
        else newTypeDecl = structDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList);
      } else if (interfaceDecl != null) {
        if (newAttrList != null) newTypeDecl = interfaceDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList.Add(newAttrList));
        else newTypeDecl = interfaceDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList);
      } else if (enumDecl != null) {
        if (newAttrList != null) newTypeDecl = enumDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList.Add(newAttrList));
        else newTypeDecl = enumDecl.WithoutLeadingTrivia().WithoutTrailingTrivia().WithAttributeLists(oldAttrList);
      } else throw new NotSupportedException();


      var root = await document.GetSyntaxRootAsync(cancellationToken);

      var oldUsings = (root as CompilationUnitSyntax).Usings;


      var newRoot = root
        .ReplaceNode(typeDecl, newTypeDecl
          .WithAdditionalAnnotations(Formatter.Annotation)
          .WithLeadingTrivia(typeDecl.GetLeadingTrivia())
          .WithTrailingTrivia(typeDecl.GetTrailingTrivia())
        ) as CompilationUnitSyntax;


      // add using HeuristicLab.Persistence if something was changed or added and the document does already contain the using directive 
      if ((newAttrList != null || renamed) &&
        oldUsings.All(sn => sn.Name.WithoutTrivia().ToString() != "HeuristicLab.Persistence")) {
        var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("HeuristicLab.Persistence"));
        newRoot = newRoot.WithUsings(oldUsings.Add(newUsing));
      }

      return document
        .WithSyntaxRoot(newRoot)
        .Project.Solution;
    }
  }
}