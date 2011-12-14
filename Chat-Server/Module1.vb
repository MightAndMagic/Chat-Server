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
            'assynchron()

            Dim client As TcpClient = server.AcceptTcpClient
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Client {0} verbunden." & vbCrLf, client.Client.RemoteEndPoint)
            While client.Connected
                Try
                    Dim stream As NetworkStream = client.GetStream()
                    Dim i As Int32
                    i = stream.Read(bytes, 0, bytes.Length)
                    nachricht = System.Text.Encoding.ASCII.GetString(bytes, 0, i)
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("{0} um " & TimeOfDay & " Uhr", client.Client.RemoteEndPoint)
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine(nachricht)
                    If nachricht = "/help" Then
                        Dim functions(1024) As Byte
                        functions = System.Text.Encoding.ASCII.GetBytes("Hier erscheinen die spaeteren Befehle")
                        client.GetStream.Write(functions, 0, functions.Length)
                    ElseIf nachricht = "/file" Then
                        Dim functions(1024) As Byte
                        functions = System.Text.Encoding.ASCII.GetBytes("Spaeter Funktion zur Datenuebertragung")
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
    Sub assynchron()
	'ein sinnloser Kommentar
        Dim ips As Integer = 0
        Dim f As Integer
        Dim g As Integer
        Dim h As Integer
        Dim j As Integer
        For f = 0 To 255
            For g = 0 To 255
                For h = 0 To 255
                    For j = 0 To 255
                        Try
                            Dim thisip As IPAddress = IPAddress.Parse(f + "." + g + "." + h + "." + j)
                            Dim thisserver = New TcpListener(thisip, port)
                            thisserver.Start()
                            Dim thisclient As TcpClient = thisserver.AcceptTcpClient()
                            Adressen(ips) = thisip
                            ips = ips + 1
                        Catch e As Exception
                        End Try

                    Next
                Next
            Next
        Next
    End Sub
End Module