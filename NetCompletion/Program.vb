Imports System.Reflection
Imports System.Composition.Hosting
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Completion
Imports Microsoft.CodeAnalysis.Host.Mef

Module Program
    Private Enum Languages
        None
        VisualBasic
        CSharp
    End Enum

    Sub Main(Args As String())
        Try

            'Command Line Arguments
            Dim Language As Languages = Languages.None
            Dim Offset As Integer = 0
            Dim Text As String = String.Empty
            Dim InText As Boolean = True
            For Each Arg In Args

                'Language
                If Arg.ToLower.StartsWith("-l:vb") Then
                    Language = Languages.VisualBasic
                    Debug.WriteLine("Language: Visual Basic")
                ElseIf Arg.ToLower.StartsWith("-l:cs") Then
                    Language = Languages.CSharp
                    Debug.WriteLine("Language: CSharp")
                End If

                'Caret Position Offset
                If Arg.ToLower.StartsWith("-offset:") Then
                    Offset = CInt(Arg.Substring(8))
                    Debug.WriteLine($"Offset: {Offset:#,##0}")
                End If

                'Code Text
                If InText Then
                    InText = False
                    Text = Arg
                    Debug.WriteLine($"Text:{Environment.NewLine}{Text}")
                End If
                If Arg.ToLower.StartsWith("-text") Then
                    InText = True
                End If

            Next

            Text =
"Imports System

Module Program
    Sub Main(args As String())
        Console.
    End Sub
End Module"

            Offset = Text.IndexOf(".")

            'Create Default Host Languges
            Dim Assemblies As New List(Of Assembly) From {
                Assembly.Load("Microsoft.CodeAnalysis"),
                Assembly.Load("Microsoft.CodeAnalysis.Features")
            }

            'Add Specific Languages
            Select Case Language
                Case Languages.VisualBasic
                    Assemblies.Add(Assembly.Load("Microsoft.CodeAnalysis.VisualBasic"))
                    Assemblies.Add(Assembly.Load("Microsoft.CodeAnalysis.VisualBasic.Features"))
                Case Languages.CSharp
                    Assemblies.Add(Assembly.Load("Microsoft.CodeAnalysis.CSharp"))
                    Assemblies.Add(Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features"))
            End Select

            'Configure MefHost
            Dim Types = MefHostServices.DefaultAssemblies.Concat(Assemblies).Distinct().SelectMany(Function(x) x.GetTypes()).ToArray()
            Dim Container = New ContainerConfiguration().WithParts(Types).CreateContainer()
            Dim Host = MefHostServices.Create(Container)

            'Create Workspace
            Dim Workspace = New AdhocWorkspace(Host)
            Dim Project = Workspace.AddProject("TestProject", LanguageNames.VisualBasic)
            Dim Document = Project.AddDocument("TestDocument.vb", Text)

            'Get Code Completion
            Dim Service = CompletionService.GetService(Document)
            If Service IsNot Nothing Then
                'For Offset = 0 To Text.Length
                'Debug.WriteLine("")
                Debug.WriteLine($"Offset: {Offset}")
                    Dim List = Service.GetCompletionsAsync(Document, Offset).Result
                    If List IsNot Nothing Then
                        'Debug.WriteLine($"Total: {List.Items.Count}")

                        Dim Items As New List(Of String)

                        For Each Item In List.Items
                            Console.WriteLine(Item.DisplayText)
                            'Debug.WriteLine(Item.DisplayText)
                            Items.Add(Item.DisplayText)
                        Next

                        Debug.WriteLine(String.Join(",", Items))

                    End If
                'Next
            End If

        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try
    End Sub
End Module
