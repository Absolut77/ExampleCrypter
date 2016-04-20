Imports System.Text
Imports System.CodeDom
Imports System.IO


Public Class Codedom

    Public Shared Function compile_Stub(ByVal input As String, ByVal output As String, ByVal resources As String, ByVal showError As Boolean, Optional ByVal icon_Path As String = Nothing) As Boolean

        Dim provider_Args As New Dictionary(Of String, String)()
        provider_Args.Add("CompilerVersion", "v2.0")

        Dim provider As New Microsoft.VisualBasic.VBCodeProvider(provider_Args)
        Dim c_Param As New Compiler.CompilerParameters
        Dim c_Args As String = " /target:winexe /platform:x86 /optimize "

        If Not icon_Path = Nothing Then
            c_Args = c_Args & " /win32icon:" & icon_Path
        Else
            c_Args = c_Args & " /win32icon:" & generateicon()
        End If

        c_Param.GenerateExecutable = True
        c_Param.OutputAssembly = output
        c_Param.EmbeddedResources.Add(resources)
        c_Param.CompilerOptions = c_Args
        c_Param.IncludeDebugInformation = False
        c_Param.ReferencedAssemblies.AddRange({"System.Dll", "System.Windows.Forms.Dll"})

        Dim c_Result As Compiler.CompilerResults = provider.CompileAssemblyFromSource(c_Param, input)
        If c_Result.Errors.Count = 0 Then
            Return True
        Else
            If showError Then
                For Each _Error As Compiler.CompilerError In c_Result.Errors
                    MessageBox.Show(_Error.ToString)
                Next
                Return False
            End If
            Return False
        End If

    End Function

    Public Shared Function compile_Core(ByVal input As String, ByVal output As String, ByVal resources As String, ByVal showError As Boolean) As Boolean

        Dim provider_Args As New Dictionary(Of String, String)()
        provider_Args.Add("CompilerVersion", "v2.0")

        Dim provider As New Microsoft.VisualBasic.VBCodeProvider(provider_Args)
        Dim c_Param As New Compiler.CompilerParameters
        Dim c_Args As String = " /target:library /platform:x86 /optimize /define:_MYTYPE=\""""Empty\"""""

        c_Param.GenerateExecutable = True
        c_Param.OutputAssembly = output
        c_Param.CompilerOptions = c_Args
        c_Param.IncludeDebugInformation = False
        c_Param.EmbeddedResources.Add(resources)
        c_Param.ReferencedAssemblies.AddRange({"System.Dll", "System.Windows.Forms.Dll"})

        Dim comp_Result As Compiler.CompilerResults = provider.CompileAssemblyFromSource(c_Param, input)
        If comp_Result.Errors.Count = 0 Then
            Return True
        Else
            If showError Then
                For Each _Error As Compiler.CompilerError In comp_Result.Errors
                    MessageBox.Show(_Error.ToString)
                Next
                Return False
            End If
            Return False
        End If
    End Function

    Public Shared Function generateicon() As String
        Dim width As Integer = 50
        Dim height As Integer = 50
        Dim bmp As New Bitmap(width, height)
        Dim rand As Random = New Random()
        For y As Integer = 0 To height - 1
            For x As Integer = 0 To width - 1
                Dim a As Integer = rand.Next(256)
                Dim r As Integer = rand.Next(256)
                Dim g As Integer = rand.Next(256)
                Dim b As Integer = rand.Next(256)
                bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b))
            Next
        Next
        Dim HIcon As IntPtr = bmp.GetHicon()
        Dim newIcon As Icon = Icon.FromHandle(HIcon)

        Dim finalpath As String = Path.GetTempPath() + "randomicon.ico"
        If IO.File.Exists(finalpath) Then
            IO.File.Delete(finalpath)
        End If
        Dim oFileStream As FileStream = New IO.FileStream(finalpath, FileMode.CreateNew)
        newIcon.Save(oFileStream)
        oFileStream.Close()
        Return finalpath
    End Function

End Class
