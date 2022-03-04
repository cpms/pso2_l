Imports System.Net
Imports System.IO
Imports System.Text
Public Class Class1
    Public Shared ConfigFilePath As String = Application.StartupPath & "\appconfig.ini"
    Public Structure Pso2UserInfo
        Dim id As String '用户名
        Dim password As String '密码
    End Structure
    Public Structure Pso2OtpInfo
        Dim userId As String
        Dim token As String
        Dim otp As String 'otp密码
    End Structure
    Public Function GetErrInfo(ErrCode As Integer) As String
        If ErrCode = 5001 Then
            Return "密码错误"
        ElseIf ErrCode = 5002 Then
            Return "用户名错误"
        ElseIf ErrCode = 1001 Then
            Return "参数错误"
        End If
        Return "未知错误"
    End Function
    Public Function PostRequest(xmlRequest As String, postUrl As String) As String '参数1为post的json，参数2为url
        'Dim xml As String = xmlRequest
        Dim encoding As Encoding = Encoding.GetEncoding("utf-8")
        Dim request As HttpWebRequest = WebRequest.Create(postUrl)
        request.Method = "POST"
        request.ContentType = "application/json; charset=utf-8"
        request.UserAgent = "PSO2 Launcher"
        request.Headers.Add("Cache-Control", "no-cache")
        request.Headers.Add("Pragma", "no-cache")
        request.Timeout = 6000
        Dim postdata() As Byte = encoding.GetBytes(xmlRequest)
        request.ContentLength = postdata.Length
        Dim requesstream As Stream = request.GetRequestStream()
        requesstream.Write(postdata, 0, postdata.Length)
        requesstream.Close()
        Dim response As WebResponse = request.GetResponse()
        Dim responsestream As StreamReader = New StreamReader(response.GetResponseStream())
        Dim html As String = responsestream.ReadToEnd()
        requesstream.Close()
        response.Close()
        Return html
    End Function
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Long
    Private Declare Function GetWindowsDirectory Lib "kernel32" Alias "GetWindowsDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Long) As Long
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lplFilename As String) As Long
    Public Shared Function WriteOneString(section As String, key As String, value As String, filename As String) As Long
        Dim X As Boolean
        Dim Nbuff As New String(CType(" ", Char), 1000)
        Nbuff = value + Chr(0)
        X = WritePrivateProfileString(section, key, Nbuff, filename)
        WriteOneString = X
    End Function

    Public Shared Function ReadOneString(section As String, key As String, Filename As String) As String
        Dim X As Long, i As Integer
        Dim Buffer As New String(CType(" ", Char), 1000)
        X = GetPrivateProfileString(section, key, "", Buffer, 1000, Filename)
        i = InStr(Buffer, Chr(0))
        ReadOneString = Trim(Left(Buffer, i - 1))
    End Function
End Class
