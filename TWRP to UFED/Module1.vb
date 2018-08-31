Module Module1
    Dim ext4_partitions As New List(Of String)
    Dim emmc_partitions As New List(Of String)
    Sub Main(args() As String)
        If args.Length = 0 Then
            Console.WriteLine("Specificare una cartella contenente il backup di TWRP.")
            Console.WriteLine("Premere un tasto per uscire.")
            Console.ReadKey()
            Exit Sub
        End If
        Dim path_backup As String = args(0).Replace("""", "")
        'Dim path_temp As String = path_backup & "\Temp\"
        Dim path_md5 As String = path_backup & "\MD5\"
        Dim path_output As String = path_backup & "\UFED\"
        Dim paths_array() As String = IO.Directory.GetFiles(path_backup)
        Dim filename_list As New List(Of String)
        Dim filename As String
        Dim partition As String
        IO.Directory.CreateDirectory(path_md5)
        ' IO.Directory.CreateDirectory(path_temp)
        IO.Directory.CreateDirectory(path_output)
        For Each file_path In paths_array
            filename = file_path.Split("\").Last()
            filename_list.Add(filename)
            If filename.Contains("md5") Or filename.Contains("info") Or filename.Contains("log") Then
                IO.File.Move(path_backup & "\" & filename, path_md5 & filename)
                Console.WriteLine("Spostato {0} in {1}", filename, path_md5)
            ElseIf filename.Contains("ext4") Or filename.Contains("f2fs") Then
                partition = filename.Split(".").First
                If Not ext4_partitions.Contains(partition) Then
                    ext4_partitions.Add(partition)
                    Console.WriteLine("Aggiunta partizione {0} alla lista.", partition)
                End If
            ElseIf filename.Contains("emmc") Then
                partition = Text.RegularExpressions.Regex.Replace(filename, "([a-z_]+)[0-9]?[.a-z0-9]+", "$1")
                If Not emmc_partitions.Contains(partition) Then
                    emmc_partitions.Add(partition)
                    Console.WriteLine("Aggiunta partizione {0} alla lista.", partition)
                End If
            End If
        Next

        For Each part In ext4_partitions
            Dim output_file As String = Chr(34) & path_output & part & ".tar.gz" & Chr(34)
            Dim merge_arguments As String = "/c copy /B " & Chr(34) & path_backup & "\" & part & ".*" & Chr(34) & " " & output_file
            Process.Start("cmd", merge_arguments).WaitForExit()
            Console.WriteLine("Unito {0} in {1}", part, path_output)
        Next
        For Each part In emmc_partitions
            Dim command As String = "/c copy /B " & Chr(34) & path_backup & "\" & part & ".*" & Chr(34) & " " & Chr(34) & path_output & part & ".img" & Chr(34)
            Process.Start("cmd", command).WaitForExit()
            Console.WriteLine("Unito {0} in {1}", part, path_output)
        Next
        Console.ReadKey()
    End Sub

End Module
