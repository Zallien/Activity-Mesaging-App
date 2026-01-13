Imports System.Net.Sockets
Imports System.IO
Imports System.Threading.Tasks

Public Class Form1

    Private clientSocket As TcpClient
    Private clientStream As NetworkStream
    Private reader As StreamReader
    Private writer As StreamWriter
    Private isConnected As Boolean = False

    Private Const ROLE As String = "Client"

    '==================== FORM LOAD ====================
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Text = "Socket Client"
    End Sub

    '==================== CONNECT ====================
    Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConnect.Click
        If Not isConnected Then
            Try
                clientSocket = New TcpClient()
                clientSocket.Connect(txtIP.Text, Integer.Parse(txtPort.Text))

                clientStream = clientSocket.GetStream()
                reader = New StreamReader(clientStream)
                writer = New StreamWriter(clientStream)
                writer.AutoFlush = True

                isConnected = True
                txtStatus.AppendText("Connected to server." & Environment.NewLine)

                ' Start listening to server messages
                Task.Factory.StartNew(AddressOf ListenForMessages)

            Catch ex As Exception
                txtStatus.AppendText("Connection error: " & ex.Message & Environment.NewLine)
            End Try
        End If
    End Sub

    '==================== RECEIVE MESSAGES ====================
    Private Sub ListenForMessages()
        Try
            While isConnected
                Dim msg As String = reader.ReadLine()
                If msg Is Nothing Then Exit While

                Invoke(Sub()
                           txtReceived.AppendText(msg & Environment.NewLine)
                       End Sub)
            End While
        Catch
            Invoke(Sub()
                       txtStatus.AppendText("Disconnected from server." & Environment.NewLine)
                   End Sub)
        End Try

        isConnected = False
    End Sub

    '==================== SEND MESSAGE ====================
    Private Sub btnSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
        If isConnected AndAlso txtMessage.Text.Trim() <> "" Then
            Try
                Dim message As String = ROLE & " (" & txtName.Text & "): " & txtMessage.Text
                writer.WriteLine(message)

                txtReceived.AppendText("Me: " & txtMessage.Text & Environment.NewLine)
                txtMessage.Clear()

            Catch ex As Exception
                txtStatus.AppendText("Send error: " & ex.Message & Environment.NewLine)
            End Try
        End If
    End Sub

    '==================== DISCONNECT ====================
    Private Sub btnDisconnect_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDisconnect.Click
        If isConnected Then
            Try
                isConnected = False

                If writer IsNot Nothing Then writer.Close()
                If reader IsNot Nothing Then reader.Close()
                If clientStream IsNot Nothing Then clientStream.Close()
                If clientSocket IsNot Nothing Then clientSocket.Close()

                txtStatus.AppendText("Disconnected from server." & Environment.NewLine)

            Catch ex As Exception
                txtStatus.AppendText("Disconnect error: " & ex.Message & Environment.NewLine)
            End Try
        End If
    End Sub

    '==================== CLEANUP ====================
    Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
        Try
            isConnected = False

            If writer IsNot Nothing Then writer.Close()
            If reader IsNot Nothing Then reader.Close()
            If clientStream IsNot Nothing Then clientStream.Close()
            If clientSocket IsNot Nothing Then clientSocket.Close()
        Catch
        End Try

        MyBase.OnFormClosing(e)
    End Sub

End Class
