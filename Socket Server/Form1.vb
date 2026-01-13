Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Threading.Tasks

Public Class Form1

    Private serverSocket As TcpListener
    Private clientSocket As TcpClient
    Private serverStream As NetworkStream
    Private reader As StreamReader
    Private writer As StreamWriter
    Private isConnected As Boolean = False

    Private ConnectionPanelShow As Boolean = False

    Private Const ROLE As String = "Server"

    '==================== FORM LOAD ====================
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Panel1.Visible = ConnectionPanelShow
        Panel1.Location = New Point(12, 95)
        Me.Text = "Socket Server"
    End Sub

    '==================== ACCEPT CLIENT ====================
    Private Sub AcceptClient()
        Try
            clientSocket = serverSocket.AcceptTcpClient()
            serverStream = clientSocket.GetStream()

            reader = New StreamReader(serverStream)
            writer = New StreamWriter(serverStream)
            writer.AutoFlush = True

            isConnected = True

            Invoke(Sub()
                       txtStatus.AppendText("Client connected." & Environment.NewLine)
                   End Sub)

            ListenForMessages()

        Catch ex As Exception
            Invoke(Sub()
                       txtStatus.AppendText("Accept error: " & ex.Message & Environment.NewLine)
                   End Sub)
        End Try
    End Sub

    '==================== RECEIVE MESSAGES ====================
    Private Sub ListenForMessages()
        Try
            While isConnected
                Dim msg As String = reader.ReadLine()
                If msg Is Nothing Then Exit While

                Invoke(Sub()
                           txtChatHistory.AppendText(msg & Environment.NewLine)
                       End Sub)
            End While
        Catch
            Invoke(Sub()
                       txtStatus.AppendText("Client disconnected." & Environment.NewLine)
                   End Sub)
        End Try

        isConnected = False
    End Sub

    '==================== CLEANUP ====================
    Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
        Try
            isConnected = False

            If writer IsNot Nothing Then writer.Close()
            If reader IsNot Nothing Then reader.Close()
            If serverStream IsNot Nothing Then serverStream.Close()
            If clientSocket IsNot Nothing Then clientSocket.Close()
            If serverSocket IsNot Nothing Then serverSocket.Stop()
        Catch
        End Try

        MyBase.OnFormClosing(e)
    End Sub

    Private Sub btnStartServer_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartServer.Click
        If isConnected = False Then
            Try
                serverSocket = New TcpListener(IPAddress.Parse(txtIP.Text), Integer.Parse(txtPort.Text))
                serverSocket.Start()

                txtStatus.AppendText("Server started. Waiting for client..." & Environment.NewLine)

                Task.Factory.StartNew(AddressOf AcceptClient)

            Catch ex As Exception
                txtStatus.AppendText("Server error: " & ex.Message & Environment.NewLine)
            End Try
        End If
    End Sub

    Private Sub btnStopServer_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStopServer.Click
        Try
            isConnected = False

            If writer IsNot Nothing Then writer.Close()
            If reader IsNot Nothing Then reader.Close()
            If serverStream IsNot Nothing Then serverStream.Close()
            If clientSocket IsNot Nothing Then clientSocket.Close()
            If serverSocket IsNot Nothing Then serverSocket.Stop()

            txtStatus.AppendText("Server stopped." & Environment.NewLine)

        Catch ex As Exception
            txtStatus.AppendText("Stop error: " & ex.Message & Environment.NewLine)
        End Try
    End Sub

    Private Sub btnSend_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        If isConnected Then
            Dim message As String = ROLE & " (" & txtDisplayName.Text & "): " & txtMessage.Text
            writer.WriteLine(message)

            txtChatHistory.AppendText(message & Environment.NewLine)
            txtMessage.Clear()
        End If
    End Sub

    Private Sub Label7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label7.Click

    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        ConnectionPanelShow = Not ConnectionPanelShow
        Panel1.Visible = ConnectionPanelShow
    End Sub
End Class
