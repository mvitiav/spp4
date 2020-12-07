using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class TestGen : ItestGen
    {
        private static string GetNameSpace(SyntaxNode node)
        {
            string ClssNameSpace = "";

            while (!(node.Parent is CompilationUnitSyntax))
            {
                if (node.Parent is ClassDeclarationSyntax)
                {
                    ClssNameSpace = ClssNameSpace.Insert(0, '.' + (node.Parent as ClassDeclarationSyntax).Identifier.Text);
                }
                if (node.Parent is NamespaceDeclarationSyntax)
                {
                    ClssNameSpace = ClssNameSpace.Insert(0, '.' + ((node.Parent as NamespaceDeclarationSyntax).Name as IdentifierNameSyntax).Identifier.Text);
                }
                node = node.Parent;
            }
            return ClssNameSpace.Remove(0, 1);
        }

        private static FieldDeclarationSyntax GenerateField(SyntaxKind kind, string IdentifierName, string IdentifierType)
        {
            return SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(IdentifierType))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(IdentifierName)))))
                        .WithModifiers(
                            SyntaxFactory.TokenList(
                                SyntaxFactory.Token(kind)));
        }
        private MemberDeclarationSyntax[] generateSetup(ClassDeclarationSyntax cds, IEnumerable<MemberDeclarationSyntax> methods) {
            List<MemberDeclarationSyntax> setupMethods = new List<MemberDeclarationSyntax>();

            var SetUp = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "Setup")
                       .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                           .AddAttributeLists(SyntaxFactory.SingletonList<AttributeListSyntax>(
                               SyntaxFactory.AttributeList(
                                   SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                       SyntaxFactory.Attribute(
                                           SyntaxFactory.IdentifierName("SetUp"))))).ToArray()).WithBody(SyntaxFactory.Block());

            if (!cds.Modifiers.Any(SyntaxKind.StaticKeyword)) {
                setupMethods.Add(GenerateField(SyntaxKind.PrivateKeyword, "_"+cds.Identifier.Text+"Instance",cds.Identifier.Text));
            }

            List<ConstructorDeclarationSyntax> constructors = new List<ConstructorDeclarationSyntax>();
            foreach (MemberDeclarationSyntax member in cds.Members.Where(cstr => (cstr is ConstructorDeclarationSyntax)))
            {
                constructors.Add(member as ConstructorDeclarationSyntax);
            }

            string constString = "_" + cds.Identifier.Text + "Instance = new " + cds.Identifier.Text + "(";
            if (constructors.Count > 0)
            {
                ConstructorDeclarationSyntax constructor = constructors.OrderBy(x => x.ParameterList.Parameters.Count).First();
                ParameterSyntax[] parametrs = constructor.ParameterList.Parameters.ToArray();
                foreach (ParameterSyntax param in parametrs)
                {
                    if (param.Type.ToString()[0] == 'I')
                    {
                        setupMethods.Add(
                        GenerateField(
                            SyntaxKind.PrivateKeyword,
                            "_" + param.Identifier.Text + "Dependency",
                            "Mock<" + param.Type.ToString() + ">"));
                        //SetUp = SetUp.AddBodyStatements(GenerateMockClass(param.Type.ToString(), "_" + param.Identifier.Text + "Dependency"));
                        
                        SetUp = SetUp.AddBodyStatements(SyntaxFactory.ParseStatement(string.Format("{0} = new Mock<{1}>();", param.Type.ToString(), param.Identifier.Text)));
                        constString += "_" + param.Identifier.Text + "Dependency.Object, ";
                    }
                    else
                    {
                        //SetUp = SetUp.AddBodyStatements(GenerateVar(param.Type.ToString(), param.Identifier.Text));
                        SetUp = SetUp.AddBodyStatements(SyntaxFactory.ParseStatement(param.Type.ToString() + ' '+ param.Identifier.ToString() + " = default;"));
                        constString += param.Identifier.Text + ", ";
                    }
                }
                constString = constString.Remove(constString.Length - 2, 1);
            }
            constString += ");";
            SetUp = SetUp.AddBodyStatements(SyntaxFactory.ParseStatement(constString));
            setupMethods.Add(SetUp);

            return setupMethods.ToArray();
        }
        
            private MethodDeclarationSyntax[] generateMethodTests(ClassDeclarationSyntax cds,IEnumerable<MemberDeclarationSyntax> methods) {
            List<MethodDeclarationSyntax> testMethods = new List<MethodDeclarationSyntax>();

            foreach (MethodDeclarationSyntax mds in methods)
            {
                var tempMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), mds.Identifier + "Test")
                       .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                           .AddAttributeLists(SyntaxFactory.SingletonList<AttributeListSyntax>(
                               SyntaxFactory.AttributeList(
                                   SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                       SyntaxFactory.Attribute(
                                           SyntaxFactory.IdentifierName("Test"))))).ToArray()).WithBody(SyntaxFactory.Block());
                string argz = "";

                foreach (var arg in mds.ParameterList.Parameters) {

                    if ((arg.Type.ToString().EndsWith ("ble")) ||(arg.Type.ToString()[0] == 'I'))
                    {
                        argz+="_"+ arg.Identifier.Text + "Dependency.Object, ";
                    }
                    else {
                        tempMethod = tempMethod.AddBodyStatements(SyntaxFactory.ParseStatement(arg.Type.ToString()+' '+arg.Identifier.ToString()+" = default;"));
                        argz += arg.Identifier.ToString() + ", ";
                    }
                }
                if (argz.Length > 0)
                {
                    argz = argz.Substring(0,argz.Length-2);
                }

                string tmpMthString="";
                
                    if (mds.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                    tmpMthString += mds.Identifier.ToString();
                    }
                    else
                    {
                    tmpMthString += "_" + mds.Identifier.Text + "Instance";
                    }
                tmpMthString += "." + mds.Identifier.Text + "(" + argz + ");";
                    tempMethod = tempMethod.AddBodyStatements(SyntaxFactory.ParseStatement(tmpMthString));

                if (mds.ReturnType.ToString() != "void")
                {
                    tmpMthString.Insert(0, mds.ReturnType.ToString() + " actual = ");
                    //tempMethod = tempMethod.AddBodyStatements(GenerateVar(mds.ReturnType.ToString(), "expected")).
                    tempMethod = tempMethod.AddBodyStatements(SyntaxFactory.ParseStatement(mds.ReturnType.ToString() + " expected = default;"))
                    .AddBodyStatements(SyntaxFactory.ParseStatement("Assert.That(actual, Is.EqualTo(expected));"));
                }
              
                tempMethod =tempMethod.AddBodyStatements(SyntaxFactory.ParseStatement("Assert.Fail(\"autogenerated\");"));
                testMethods.Add(tempMethod);
            }


            return testMethods.ToArray();
        }

        private List<TestInfo> analyzeNode(SyntaxNode node, CompilationUnitSyntax root) {
            List<TestInfo> tests = new List<TestInfo>();
            //var classes = node.ChildNodes().Where(cn => cn is ClassDeclarationSyntax);
            var innerNodes = node.ChildNodes().Where(cn => cn is ClassDeclarationSyntax || cn is NamespaceDeclarationSyntax);
            var classes= innerNodes.Where(cn => cn is ClassDeclarationSyntax);
            foreach (ClassDeclarationSyntax cds in classes) 
            {
                Console.WriteLine(cds.Identifier);
                if (!(cds.Modifiers.Any(SyntaxKind.AbstractKeyword)))
                {
                    var pubMethods = cds.Members.Where(a => a is MethodDeclarationSyntax)
                        .Where(b => b.Modifiers.Where(c => c.Kind() == SyntaxKind.PublicKeyword).Any());
                    if (pubMethods.Count() > 0) {

                        var syntaxFactory = SyntaxFactory.CompilationUnit();
                        syntaxFactory = syntaxFactory.AddUsings(root.Usings.ToArray())
                        .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("NUnit.Framework")))
                        .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Moq")))
                        .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(GetNameSpace(cds))));
                        var namespaceName = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(GetNameSpace(cds) + ".Test"));


                        ClassDeclarationSyntax TestClass = SyntaxFactory.ClassDeclaration(cds.Identifier.Text + "Tests")
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))

                        .AddAttributeLists(SyntaxFactory.SingletonList<AttributeListSyntax>(
                                SyntaxFactory.AttributeList(
                                    SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                        SyntaxFactory.Attribute(
                                            SyntaxFactory.IdentifierName("TestFixture"))))).ToArray())
                        .AddMembers(generateSetup(cds, pubMethods))
                        .AddMembers(generateMethodTests(cds, pubMethods));

                        namespaceName = namespaceName.AddMembers(TestClass);
                        syntaxFactory = syntaxFactory.AddMembers(namespaceName);

                        tests.Add(new TestInfo(GetNameSpace(cds)+'.'+cds.Identifier.ToString(), syntaxFactory.NormalizeWhitespace().ToFullString()));
                    }
                    
                }

            }
          
            return tests;
        }
        public TestInfo[] generate(string source)
        {
            List<TestInfo> tests = new List<TestInfo>();
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            var root = tree.GetCompilationUnitRoot();

            var namespaces = root.DescendantNodes().Where(namespaze => namespaze is NamespaceDeclarationSyntax);                      

            foreach (NamespaceDeclarationSyntax ns in namespaces) {
  
                tests.AddRange(analyzeNode(ns,root));
            }


            return tests.ToArray();
        }
    }
}
