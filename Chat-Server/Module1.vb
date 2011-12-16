Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System
Module Module1
    Dim ip As IPAddress = IPAddress.Parse("127.0.0.1")
    Dim port As Integer = 27590
    Dim server As TcpListener = Nothing
    Dim bytes(1024) As Byte
    Dim nachricht As String = Nothing
    Dim Adressen(40) As String
    Dim Nicks(40) As String
    Dim AdressenZaehler As Integer = 0
    Dim RohAdressen() As String
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
			'Sequenz um Ip-Adressen zu speichern
            Dim ep As Net.IPEndPoint = CType(client.Client.RemoteEndPoint, Net.IPEndPoint)
            Dim ad As Integer = 0
            RohAdressen = Split(ep.ToString, ":", 2)
            Adressen(AdressenZaehler) = RohAdressen(0)
            AdressenZaehler = AdressenZaehler + 1
			'Sequenz ende
            Nicks(AdressenZaehler) = ""
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Client {0} verbunden." & vbCrLf, client.Client.RemoteEndPoint)
            While client.Connected
                Try
                    Dim stream As NetworkStream = client.GetStream()
                    Dim i As Int32
                    i = stream.Read(bytes, 0, bytes.Length)
                    nachricht = System.Text.Encoding.ASCII.GetString(bytes, 0, i)
                    Console.ForegroundColor = ConsoleColor.Cyan
                    If Nicks(AdressenZaehler) = "" Then
                        Console.WriteLine("{0} um " & TimeOfDay & " Uhr", client.Client.RemoteEndPoint)
                    Else
                        Console.WriteLine("{0} um " & TimeOfDay & " Uhr", Nicks(AdressenZaehler))
                    End If
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine(nachricht)
                    If nachricht.Substring(0, 1) = "/" Then
                        If nachricht.Substring(1, 4) = "help" Then
                            Dim functions(1024) As Byte
                            functions = System.Text.Encoding.ASCII.GetBytes("/whoami - Your IP")
                            client.GetStream.Write(functions, 0, functions.Length)
                            functions = System.Text.Encoding.ASCII.GetBytes("/gettime - Get date and time")
                            client.GetStream.Write(functions, 0, functions.Length)
                        ElseIf nachricht.Substring(1, 4) = "file" Then
                            Dim functions(1024) As Byte
                            functions = System.Text.Encoding.ASCII.GetBytes("Spaeter Funktion zur Datenuebertragung")
                            client.GetStream.Write(functions, 0, functions.Length)
                        ElseIf nachricht.Substring(1, 6) = "whoami" Then
                            Dim functions(1024) As Byte
                            functions = System.Text.Encoding.ASCII.GetBytes("whoami - You are " + Adressen(AdressenZaehler - 1))
                            client.GetStream.Write(functions, 0, functions.Length)
                        ElseIf nachricht.Substring(1, 7) = "gettime" Then
                            Dim functions(1024) As Byte
                            functions = System.Text.Encoding.ASCII.GetBytes("gettime - Today it's the " + Date.Now)
                            client.GetStream.Write(functions, 0, functions.Length)
                        ElseIf nachricht.Substring(1, 7) = "setnick" Then
                            Nicks(AdressenZaehler) = nachricht.Substring(9, nachricht.Length - 9)
                            Dim functions(1024) As Byte
                            functions = System.Text.Encoding.ASCII.GetBytes("setnick - Your nickname is now " + nachricht.Substring(9, nachricht.Length - 9))
                            client.GetStream.Write(functions, 0, functions.Length)
                        Else
                            client.GetStream.Write(bytes, 0, i)
                        End If
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