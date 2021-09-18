using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

//Önce Nuget Package Manager'dan CodeDom'u indir

namespace CodeDomStudy
{
    public static class MyCodeGenerator
    {
        public static void Generate()
        {
            CodeCompileUnit codeUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(typeof(Program).Namespace);
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeUnit.Namespaces.Add(codeNamespace);

            CodeTypeDeclaration codeClass = new CodeTypeDeclaration("GeneratedClass");
            codeNamespace.Types.Add(codeClass);

            ////writing main method
            //CodeMethodInvokeExpression mainExpression = new CodeMethodInvokeExpression(
            //    new CodeTypeReferenceExpression("Console"), "WriteLine", new CodePrimitiveExpression("Selamın Aleyküm"));
            //CodeEntryPointMethod mainMethod = new CodeEntryPointMethod();
            //mainMethod.Statements.Add(mainExpression);
            //codeClass.Members.Add(mainMethod);

            //writing method
            CodeMemberMethod method2 = new CodeMemberMethod();
            method2.Name = "SayHello";
            method2.Attributes = MemberAttributes.Public | MemberAttributes.Final; //final yazmazsak virtual metod oluyor
            CodeMethodInvokeExpression method2Expression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("Console"), "WriteLine",
                new CodePrimitiveExpression("Hello babacan"));
            method2.Statements.Add(method2Expression);
            codeClass.Members.Add(method2);

            //writing another method
            CodeMemberMethod method3 = new CodeMemberMethod();
            method3.Name = "Add";
            method3.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;

            method3.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(int[]), CodeTypeReferenceOptions.GenericTypeParameter), "numbers"));
            method3.Parameters[0].CustomAttributes
                .Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));

            method3.Statements.Add(
                new CodeVariableDeclarationStatement(typeof(int), "sum", new CodePrimitiveExpression(0)));

            var forLoop = new CodeIterationStatement();

            //forLoop.InitStatement = new CodeSnippetStatement("int i=0"); //code snippet ile yazı olarak kodu da yazabiliriz ama bana çok mantıklı gelmedi
            forLoop.InitStatement =
                new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));

            forLoop.TestExpression =
                new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
                    CodeBinaryOperatorType.LessThan,
                    new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("numbers"), "Length"));

            forLoop.IncrementStatement = new CodeAssignStatement(
                new CodeVariableReferenceExpression("i"),
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("i"),
                    CodeBinaryOperatorType.Add,
                    new CodePrimitiveExpression(1)));

            //forLoop.IncrementStatement = new CodeSnippetStatement("i++"); //code snippet kullanarak böyle de yapabilirdik

            forLoop.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("sum"),
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("sum"),
                    CodeBinaryOperatorType.Add,
                    new CodeVariableReferenceExpression("i"))));

            method3.Statements.Add(forLoop);
            method3.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("sum")));
            method3.ReturnType = new CodeTypeReference(typeof(int));
            codeClass.Members.Add(method3);

            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\"; //bu da desktop'un path'ini alır aklımızda bulunsun

            CodeMemberMethod method4 = new CodeMemberMethod();
            method4.Name = "Add2";
            method4.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
            method4.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(int[])), "numbers"));

            method4.Parameters[0].CustomAttributes
                .Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));

            method4.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)), "sum",
                new CodePrimitiveExpression(0)));

            var forLoopMethod = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Array"), "ForEach",
                new CodeArgumentReferenceExpression("numbers"), new CodeSnippetExpression("number=>sum+=number"));

            method4.Statements.Add(forLoopMethod);
            method4.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("sum")));
            method4.ReturnType = new CodeTypeReference(typeof(int));
            codeClass.Members.Add(method4);

            string projectPath =
                Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));

            string fileToGeneratePath = $@"{projectPath}\GeneratedClass.cs";

            CodeGeneratorOptions codeOptions = new CodeGeneratorOptions();
            codeOptions.IndentString = "\t";
            codeOptions.BlankLinesBetweenMembers = true;

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            if (File.Exists(fileToGeneratePath))
            {
                File.Delete(fileToGeneratePath);
            }

            using (StreamWriter streamWriter = new StreamWriter(fileToGeneratePath, false))
            {
                codeProvider.GenerateCodeFromCompileUnit(codeUnit, streamWriter, codeOptions);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }
    }
}