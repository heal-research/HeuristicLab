using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace PersistenceCodeFix {
  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingStorableTypeFix)), Shared]
  public sealed class MissingStorableTypeFix : CodeFixProvider, IDocumentCodeFixProvider {
    private const string title = "Add StorableType attribute";

    public sealed override ImmutableArray<string> FixableDiagnosticIds {
      get { return ImmutableArray.Create(MissingStorableTypeAnalyzer.DiagnosticId); }
    }

    public sealed override FixAllProvider GetFixAllProvider() {
      return SequentialFixAllProvider.Instance;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      var diagnostic = context.Diagnostics.First();
      var diagnosticSpan = diagnostic.Location.SourceSpan;
      var baseTypeDecl = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

      context.RegisterCodeFix(
        CodeAction.Create(
          title: title,
          createChangedDocument: c => AddStorableTypeAttribute(context.Document, baseTypeDecl, c),
          equivalenceKey: title),
        diagnostic);
    }

    private static async Task<Document> AddStorableTypeAttribute(Document document, BaseTypeDeclarationSyntax baseTypeDecl, CancellationToken cancellationToken) {
      // create new identifier
      var name = SyntaxFactory.IdentifierName("StorableType");

      // create guid arg
      var guid = Guid.NewGuid();
      var guidArg = SyntaxFactory.AttributeArgument(
        SyntaxFactory.LiteralExpression(
          SyntaxKind.StringLiteralExpression,
          SyntaxFactory.Literal(guid.ToString())
        ));

      var attrArgs = SyntaxFactory.AttributeArgumentList();
      attrArgs = attrArgs.AddArguments(guidArg);

      // create new attribute
      var attrDecl = SyntaxFactory.Attribute(name)
        .WithArgumentList(attrArgs)
        .WithAdditionalAnnotations(Formatter.Annotation);

      // create new attribute list
      var attrList = SyntaxFactory.AttributeList();
      attrList = attrList.AddAttributes(attrDecl);

      BaseTypeDeclarationSyntax newBaseTypeDecl;

      // add new attribute list
      switch (baseTypeDecl.Kind()) {
        case SyntaxKind.ClassDeclaration:
          var classDecl = (ClassDeclarationSyntax)baseTypeDecl;
          var classAttrLists = classDecl.AttributeLists;
          classAttrLists = AddToAttributeLists(classAttrLists, attrList);
          newBaseTypeDecl = classDecl.WithAttributeLists(classAttrLists);
          break;
        case SyntaxKind.StructDeclaration:
          var structDecl = (StructDeclarationSyntax)baseTypeDecl;
          var structAttrLists = structDecl.AttributeLists;
          structAttrLists = AddToAttributeLists(structAttrLists, attrList);
          newBaseTypeDecl = structDecl.WithAttributeLists(structAttrLists);
          break;
        case SyntaxKind.EnumDeclaration:
          var enumDecl = (EnumDeclarationSyntax)baseTypeDecl;
          var enumAttrLists = enumDecl.AttributeLists;
          enumAttrLists = AddToAttributeLists(enumAttrLists, attrList);
          newBaseTypeDecl = enumDecl.WithAttributeLists(enumAttrLists);
          break;
        case SyntaxKind.InterfaceDeclaration:
          var interfaceDecl = (InterfaceDeclarationSyntax)baseTypeDecl;
          var interfaceAttrLists = interfaceDecl.AttributeLists;
          interfaceAttrLists = AddToAttributeLists(interfaceAttrLists, attrList);
          newBaseTypeDecl = interfaceDecl.WithAttributeLists(interfaceAttrLists);
          break;
        default: throw new NotSupportedException();
      }

      newBaseTypeDecl = newBaseTypeDecl.WithAdditionalAnnotations(Formatter.Annotation);

      var root = await document.GetSyntaxRootAsync(cancellationToken) as CompilationUnitSyntax;
      var newRoot = root.ReplaceNode(baseTypeDecl, newBaseTypeDecl);

      // add using of HeuristicLab.Persistence if the document does not already have it 
      var oldUsings = root.Usings;
      if (oldUsings.All(x => x.Name.WithoutTrivia().ToString() != "HeuristicLab.Persistence")) {
        var persistenceUsing = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("HeuristicLab.Persistence"));
        newRoot = newRoot.WithUsings(oldUsings.Add(persistenceUsing));
      }

      return document.WithSyntaxRoot(newRoot);
    }

    private static SyntaxList<AttributeListSyntax> AddToAttributeLists(SyntaxList<AttributeListSyntax> attrLists, AttributeListSyntax attrList) {
      return attrLists == null ? SyntaxFactory.List(new[] { attrList }) : attrLists.Add(attrList);
    }

    public Task<Document> FixDocumentAsync(Document document, SyntaxNode node, CancellationToken cancellationToken) {
      return AddStorableTypeAttribute(document, (BaseTypeDeclarationSyntax)node, cancellationToken);
    }
  }
}