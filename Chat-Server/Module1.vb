Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System
Module Module1
    Dim ip As IPAddress = IPAddress.Parse("127.0.0.1")
    Dim port As Integer = 27590
    Dim server As TcpListener = Nothing
    Dim nachricht As String = Nothing
    Dim ipAdressen(40) As IPAddress
    Dim Adressen(40) As IPEndPoint
    Dim Nicks(40) As String
    Dim clients(40) As TcpClient
    Dim clientID As Integer = 0
    Dim RohAdressen() As String
    Sub Main()
        Console.Title = "OpenSchoolChat - Server"
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("OpenSchoolChat - Server | Alpha, Build 4")
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
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Listening for connections...")
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Socket exception: {0}", e)
        End Try
    End Sub
    Sub listen()
        clients(clientID) = server.AcceptTcpClient()
        Dim localID As Integer = clientID
        'Sequenz um Ip-Adressen zu speichern
        Dim EndPoint As Net.IPEndPoint = CType(clients(localID).Client.RemoteEndPoint, Net.IPEndPoint)
        Adressen(localID) = EndPoint
        RohAdressen = Split(EndPoint.ToString, ":", 2)
        ipAdressen(localID) = IPAddress.Parse(RohAdressen(0))
        Nicks(localID) = ""
        clientID = clientID + 1
        'Sequenz ende
        Dim listenThread As New Threading.Thread(AddressOf listen)
        listenThread.Start()
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Client {0} verbunden.", clients(localID).Client.RemoteEndPoint)
        Dim bytes(1024) As Byte
        While clients(localID).Connected
            Try
                Dim stream As NetworkStream = clients(localID).GetStream()
                Dim i As Int32
                i = stream.Read(bytes, 0, bytes.Length)
                nachricht = Encoding.Default.GetString(bytes, 0, i)
                Console.ForegroundColor = ConsoleColor.Cyan
                If Nicks(clientID) = "" Then
                    Console.WriteLine("{0} um " & TimeOfDay & " Uhr", clients(localID).Client.RemoteEndPoint)
                Else
                    Console.WriteLine("{0} um " & TimeOfDay & " Uhr", Nicks(clientID))
                End If
                Console.ForegroundColor = ConsoleColor.White
                Console.WriteLine(nachricht)
                If nachricht.Substring(0, 1) = "/" Then
                    Dim functions(1024) As Byte
                    If getFunction(nachricht) = "help" Then
                        functions = Encoding.Default.GetBytes("/whoami - Your IP" + vbCrLf + "/gettime - Get date and time" + vbCrLf + "/setnick - Set your nickname")
                        stream.Write(functions, 0, functions.Length)
                    ElseIf getFunction(nachricht) = "file" Then
                        functions = Encoding.Default.GetBytes("Spaeter Funktion zur Datenuebertragung")
                        stream.Write(functions, 0, functions.Length)
                    ElseIf getFunction(nachricht) = "whoami" Then
                        functions = Encoding.Default.GetBytes("whoami - You are " + clients(localID).Client.RemoteEndPoint.ToString)
                        stream.Write(functions, 0, functions.Length)
                    ElseIf getFunction(nachricht) = "gettime" Then
                        functions = Encoding.Default.GetBytes("gettime - Today it's the " + Date.Now)
                        stream.Write(functions, 0, functions.Length)
                    ElseIf getFunction(nachricht) = "setnick" Then
                        Try
                            Nicks(localID) = nachricht.Substring(9, nachricht.Length - 9)
                            If Nicks(localID) <> "" Then
                                functions = Encoding.Default.GetBytes("setnick - Your nickname is now " + nachricht.Substring(9, nachricht.Length - 9))
                            Else
                                functions = Encoding.Default.GetBytes("Please enter a nickname after /setnick!")
                            End If
                        Catch ex As Exception
                            functions = Encoding.Default.GetBytes("Please enter a nickname after /setnick!")
                        End Try
                        stream.Write(functions, 0, functions.Length)
                    Else
                        functions = Encoding.Default.GetBytes("This is not a valid command. See /help for further information.")
                        stream.Write(functions, 0, functions.Length)
                    End If
                End If
                send(bytes, i, localID)
            Catch e As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("{0} closed the connection.", clients(localID).Client.RemoteEndPoint)
            End Try
        End While
    End Sub
    Function getFunction(ByVal nachricht) As String
        Try
            If nachricht.Substring(1, 4) = "help" Then
                Return "help"
            ElseIf nachricht.Substring(1, 4) = "file" Then
                Return "file"
            ElseIf nachricht.Substring(1, 6) = "whoami" Then
                Return "whoami"
            ElseIf nachricht.Substring(1, 7) = "gettime" Then
                Return "gettime"
            ElseIf nachricht.Substring(1, 7) = "setnick" Then
                Return "setnick"
            Else
                Return "noFunction"
            End If
        Catch ex As Exception
            Return "noFunction"
        End Try
        Return "noFunction"
    End Function
    Sub send(ByVal bytes, ByVal bytesLength, ByVal client)
        For zähler As Integer = 0 To clientID - 1
            Dim stream As NetworkStream = clients(zähler).GetStream()
            stream.Write(bytes, 0, bytesLength)
        Next
    End Sub
End Module