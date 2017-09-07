Imports System.Reflection
Imports System.Composition.Hosting
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Completion
Imports Microsoft.CodeAnalysis.Host.Mef

Module Program
    Sub Main(args As String())

        Dim Code As String =
"Imports System

Module Program
    Sub Main(args As String())
        Console.
    End Sub
End Module"

        Console.WriteLine(Code)

        Dim CaretPosition = Code.IndexOf(".")
        Console.WriteLine(Code.Substring(CaretPosition - 9, 10))

        'Dim assemblies = {Assembly.Load("Microsoft.CodeAnalysis"), Assembly.Load("Microsoft.CodeAnalysis.CSharp"), Assembly.Load("Microsoft.CodeAnalysis.Features"), Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features")}
        'Dim partTypes = MefHostServices.DefaultAssemblies.Concat(assemblies).Distinct().SelectMany(Function(x) x.GetTypes()).ToArray()
        'Dim compositionContext = New ContainerConfiguration().WithParts(partTypes).CreateContainer()
        'Dim host = MefHostServices.Create(compositionContext)
        'Dim workspace = New AdhocWorkspace(host)
        'Dim document = workspace.AddProject("TestProject", LanguageNames.CSharp).AddDocument("TestDocument.cs", "console.")
        'Dim service = CompletionService.GetService(document)

        Dim assemblies = {Assembly.Load("Microsoft.CodeAnalysis"), Assembly.Load("Microsoft.CodeAnalysis.VisualBasic"), Assembly.Load("Microsoft.CodeAnalysis.Features"), Assembly.Load("Microsoft.CodeAnalysis.VisualBasic.Features")}
        Dim partTypes = MefHostServices.DefaultAssemblies.Concat(assemblies).Distinct().SelectMany(Function(x) x.GetTypes()).ToArray()
        Dim compositionContext = New ContainerConfiguration().WithParts(partTypes).CreateContainer()
        Dim host = MefHostServices.Create(compositionContext)
        Dim workspace = New AdhocWorkspace(host)
        Dim document = workspace.AddProject("TestProject", LanguageNames.VisualBasic).AddDocument("TestDocument.vb", Code)
        Dim service = CompletionService.GetService(document)

        Console.WriteLine("Starting...")

        If service IsNot Nothing Then
            Dim List = service.GetCompletionsAsync(document, 0).Result
            If List IsNot Nothing Then
                For Each Item In List.Items
                    Console.WriteLine(Item.DisplayText)
                Next
            End If
        End If

        Console.WriteLine("Finished!")

    End Sub
End Module
