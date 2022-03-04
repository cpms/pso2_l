Imports Newtonsoft.Json
Public Class Form2
    Dim userId As String = ""
    Dim token As String = ""
    Dim IsUserAuthOk As Boolean
    Dim otpinfo As Class1.Pso2OtpInfo
    Dim login As New Class1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim userinfo As Class1.Pso2UserInfo
        Dim responseStr As String

        If TextBox1.Text = "" Then
            MessageBox.Show("用户名不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        ElseIf TextBox2.Text = "" Then
            MessageBox.Show("密码不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        otpinfo.otp = TextBox3.Text

        If IsUserAuthOk = True Then '账号密码是否已经验证成功
            If otpinfo.otp = "" Then
                MessageBox.Show("此账户绑定了OTP，但是没有输入一次性密码，请返回后重新输入", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            Try
                responseStr = login.PostRequest(JsonConvert.SerializeObject(otpinfo), "https://auth.pso2.jp/auth/v1/otpAuth") '验证一次性密码
            Catch ex As Exception
                If ex.Message = "操作超时" Then MessageBox.Show("无法连接认证服务器，请检查IE代理设置或加速器设置", "连接超时", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try

            Dim OtpAuthResult As Object = JsonConvert.DeserializeObject(responseStr)
            If OtpAuthResult("result") = 5001 Then
                MessageBox.Show("一次性密码验证失败，请重新输入一次性密码" & vbCrLf & "请注意：连续6次OTP验证失败将导致账户封禁", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            ElseIf OtpAuthResult("result") = 0 Then
                Writeini()
                MessageBox.Show("已成功登录SEGA账号，并获取到Token，可以返回主界面继续", "验证成功", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Close()
            End If
        Else
            userinfo.id = TextBox1.Text
            userinfo.password = TextBox2.Text
            Try
                responseStr = login.PostRequest(JsonConvert.SerializeObject(userinfo), "https://auth.pso2.jp/auth/v1/auth") '验证账号密码
            Catch ex As Exception
                If ex.Message = "操作超时" Then MessageBox.Show("无法连接认证服务器，请检查IE代理设置或加速器设置", "连接超时", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try

            Dim UserAuthResult As Object = JsonConvert.DeserializeObject(responseStr)
            If UserAuthResult("result") = 5002 Then
                MessageBox.Show("用户名不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf UserAuthResult("result") = 5001 Then
                MessageBox.Show("密码错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf UserAuthResult("result") = 0 Then '账号密码验证成功
                IsUserAuthOk = True
                userId = UserAuthResult("userId")
                token = UserAuthResult("token")
                otpinfo.userId = userId
                otpinfo.token = token
                If UserAuthResult("otpRequired") = True Then '需要二次验证
                    If otpinfo.otp = "" Then
                        MessageBox.Show("账号密码验证成功" & vbCrLf & "此账户绑定了OTP，但是没有输入一次性密码，请返回后重新输入", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TextBox1.Enabled = False
                        TextBox2.Enabled = False
                        Exit Sub
                    End If
                    Try
                        responseStr = login.PostRequest(JsonConvert.SerializeObject(otpinfo), "https://auth.pso2.jp/auth/v1/otpAuth") '验证一次性密码
                    Catch ex As Exception
                        If ex.Message = "操作超时" Then MessageBox.Show("无法连接认证服务器，请检查IE代理设置或加速器设置", "连接超时", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End Try
                    Dim OtpAuthResult As Object = JsonConvert.DeserializeObject(responseStr)
                    If OtpAuthResult("result") = 5001 Then
                        MessageBox.Show("账号密码验证成功" & vbCrLf & "但一次性密码验证失败，请重新输入一次性密码" & vbCrLf & "请注意：连续6次OTP验证失败将导致账户封禁", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        TextBox1.Enabled = False
                        TextBox2.Enabled = False
                        Exit Sub
                    ElseIf OtpAuthResult("result") = 0 Then
                        Writeini()
                        MessageBox.Show("已成功登录sega账号，并获取到Token，可以返回主界面继续", "验证成功", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Me.Close()
                    End If
                Else
                    Writeini()
                    MessageBox.Show("已成功登录sega账号，并获取到Token，可以返回主界面继续", "验证成功", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Close()
                End If
            End If
        End If

    End Sub
    Private Sub Writeini()
        Class1.WriteOneString("pso2", "userId", userId, Class1.ConfigFilePath)
        Class1.WriteOneString("pso2", "token", token, Class1.ConfigFilePath)
        Class1.WriteOneString("pso2", "time", Now(), Class1.ConfigFilePath)
        If CheckBox1.Checked = True Then Class1.WriteOneString("pso2", "segaid", TextBox1.Text, Class1.ConfigFilePath)
        If CheckBox2.Checked = True Then Class1.WriteOneString("pso2", "segapasswd", TextBox2.Text, Class1.ConfigFilePath)
    End Sub
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim SaveSegaId As Boolean = False
        Dim SaveSegaPasswd As Boolean = False
        If Class1.ReadOneString("pso2", "savesegaid", Class1.ConfigFilePath) <> "" Then savesegaid = Class1.ReadOneString("pso2", "savesegaid", Class1.ConfigFilePath)
        If Class1.ReadOneString("pso2", "savesegapasswd", Class1.ConfigFilePath) <> "" Then savesegapasswd = Class1.ReadOneString("pso2", "savesegapasswd", Class1.ConfigFilePath)
        CheckBox1.Checked = savesegaid
        CheckBox2.Checked = savesegapasswd
        TextBox1.Text = Class1.ReadOneString("pso2", "segaid", Class1.ConfigFilePath)
        TextBox2.Text = Class1.ReadOneString("pso2", "segapasswd", Class1.ConfigFilePath)
        IsUserAuthOk = False
        TextBox1.Enabled = True
        TextBox2.Enabled = True
        TextBox3.Text = ""
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MessageBox.Show("这里将进行SEGA账户认证并获取Token，以实现游戏内免登录" & vbCrLf & "如果账号绑定了OTP，还需要同时输入谷歌验证器的一次性密码" & vbCrLf & "如果账号没有绑定OTP，一次性密码框里输入的内容将会被忽略" & vbCrLf & "本程序默认使用IE代理设置", "说明", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Class1.WriteOneString("pso2", "savesegaid", CheckBox1.Checked, Class1.ConfigFilePath)
        If CheckBox1.Checked = False Then
            Class1.WriteOneString("pso2", "segaid", "", Class1.ConfigFilePath)
            Class1.WriteOneString("pso2", "segapasswd", "", Class1.ConfigFilePath)
            CheckBox2.Checked = False
            Class1.WriteOneString("pso2", "savesegapasswd", CheckBox2.Checked, Class1.ConfigFilePath)
        End If
    End Sub

    Private Sub CheckBox2_Click(sender As Object, e As EventArgs) Handles CheckBox2.Click
        If CheckBox2.Checked = True Then
            If MessageBox.Show("密码将明文保存在配置文件中，可能不安全，是否继续？", "提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = 1 Then
                CheckBox1.Checked = True
                Class1.WriteOneString("pso2", "savesegapasswd", CheckBox1.Checked, Class1.ConfigFilePath)
            Else
                CheckBox2.Checked = False
            End If
        Else
            Class1.WriteOneString("pso2", "segapasswd", "", Class1.ConfigFilePath)
            Class1.WriteOneString("pso2", "savesegapasswd", CheckBox2.Checked, Class1.ConfigFilePath)
        End If
    End Sub

End Class