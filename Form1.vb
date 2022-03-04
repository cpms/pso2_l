Imports Newtonsoft.Json

Public Class Form1
    Private Sub RefreshInfo()
        If Class1.ReadOneString("pso2", "token", Class1.ConfigFilePath) = "" Then
            TextBox1.Text = "尚未获取token，请先进行登录操作"
        Else
            TextBox1.Text = "-reboot -sgtoken " & Class1.ReadOneString("pso2", "token", Class1.ConfigFilePath) & "@" & Class1.ReadOneString("pso2", "userId", Class1.ConfigFilePath) & " -optimize"
            TextBox2.Text = Class1.ReadOneString("pso2", "time", Class1.ConfigFilePath)
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form2.ShowDialog()
        RefreshInfo()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshInfo()
        If Class1.ReadOneString("pso2", "gameexe", Class1.ConfigFilePath) <> "" Then TextBox3.Text = Class1.ReadOneString("pso2", "gameexe", Class1.ConfigFilePath)
        If Class1.ReadOneString("pso2", "cloudgameexe", Class1.ConfigFilePath) <> "" Then TextBox4.Text = Class1.ReadOneString("pso2", "cloudgameexe", Class1.ConfigFilePath)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Dir(Application.StartupPath & "\pso2launcher.exe") <> "" Then
            Process.Start(Application.StartupPath & "\pso2launcher.exe")
        Else
            MessageBox.Show("请将程序放在游戏目录下", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Dir(Environment.GetFolderPath(Environment.SpecialFolder.Personal) & "\SEGA\PHANTASYSTARONLINE2\_version.ver") = "" Then
            MessageBox.Show("_version.ver文件不存在，无需处理", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            My.Computer.FileSystem.CopyFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) & "\SEGA\PHANTASYSTARONLINE2\_version.ver", Environment.GetFolderPath(Environment.SpecialFolder.Personal) & "\SEGA\PHANTASYSTARONLINE2\version.ver", True)
            My.Computer.FileSystem.DeleteFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) & "\SEGA\PHANTASYSTARONLINE2\_version.ver")
            MessageBox.Show("已成功去除官方登陆器文件校验", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Dir(Application.StartupPath & "\" & TextBox3.Text) <> "" Then
            If Class1.ReadOneString("pso2", "token", Class1.ConfigFilePath) <> "" Then
                Process.Start(Application.StartupPath & "\" & TextBox3.Text, TextBox1.Text)
            Else
                If MessageBox.Show("尚未获取token，可以启动游戏，但是无法自动登录，是否继续", "提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = 1 Then Process.Start(Application.StartupPath & "\" & TextBox3.Text, "-reboot -optimize")
            End If
        Else
            MessageBox.Show("请将程序放在游戏目录下", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Class1.ReadOneString("pso2", "token", Class1.ConfigFilePath) <> "" Then
            If Dir(Application.StartupPath & "\" & TextBox4.Text) <> "" Then
                Process.Start(Application.StartupPath & "\" & TextBox4.Text, "-userID " & Class1.ReadOneString("pso2", "userId", Class1.ConfigFilePath) & " -token " & Class1.ReadOneString("pso2", "token", Class1.ConfigFilePath))
            Else
                MessageBox.Show("请将程序放在游戏目录下", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show("尚未获取token，启动Cloud版游戏必须先进行登录操作", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub TextBox3_Leave(sender As Object, e As EventArgs) Handles TextBox3.Leave
        Class1.WriteOneString("pso2", "gameexe", TextBox3.Text, Class1.ConfigFilePath)
    End Sub
    Private Sub TextBox4_Leave(sender As Object, e As EventArgs) Handles TextBox3.Leave
        Class1.WriteOneString("pso2", "cloudgameexe", TextBox4.Text, Class1.ConfigFilePath)
    End Sub
End Class
