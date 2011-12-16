Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Module Module1
    Dim ip As IPAddress = IPAddress.Parse("127.0.0.1")
    Dim port As Integer = 27590
    Dim server As TcpListener = Nothing
    Dim bytes(1024) As Byte
    Dim nachricht As String = Nothing
    Dim Adressen(40) As IPAddress
    Sub Main()
        Console.Title = "OpenSchoolChat - Server"
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("OpenSchoolChat - Server | Alpha, Build 3")
        initialisieren(ip, port)
        listen()
        Console.ReadLine()
    End Sub
    Sub initialisieren(ByVal ip, ByVal port)
        Try
            server = New TcpListener(ip, port)
            server.Start()
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Server created on port {0}.", port)
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Socket exception: {0}", e)
        End Try
    End Sub
    Sub listen()
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("Listening for connections...")
        While True
            Dim client As TcpClient = server.AcceptTcpClient
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Client {0} verbunden." & vbCrLf, client.Client.RemoteEndPoint)
            While client.Connected
                Try
                    Dim stream As NetworkStream = client.GetStream()
                    Dim i As Int32
                    i = stream.Read(bytes, 0, bytes.Length)
                    nachricht = System.Text.Encoding.Default.GetString(bytes, 0, i)
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("{0} um " & TimeOfDay & " Uhr", client.Client.RemoteEndPoint)
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine(nachricht)
                    If nachricht = "/help" Then
                        Dim functions(1024) As Byte
                        functions = System.Text.Encoding.Default.GetBytes("Hier erscheinen die späteren Befehle.")
                        client.GetStream.Write(functions, 0, functions.Length)
                    ElseIf nachricht = "/file" Then
                        Dim functions(1024) As Byte
                        functions = System.Text.Encoding.Default.GetBytes("Später Funktion zur Datenübertragung.")
                        client.GetStream.Write(functions, 0, functions.Length)
                    Else
                        client.GetStream.Write(bytes, 0, i)
                    End If
                Catch e As Exception
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine(vbCrLf & "Connection closed!")
                    listen()
                End Try
            End While
        End While
    End Sub
End Module