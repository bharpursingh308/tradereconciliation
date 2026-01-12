<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Import = New Button()
        lblResult = New Label()
        SuspendLayout()
        ' 
        ' Import
        ' 
        Import.Location = New Point(431, 164)
        Import.Name = "Import"
        Import.Size = New Size(94, 29)
        Import.TabIndex = 0
        Import.Text = "Import"
        Import.UseVisualStyleBackColor = True
        ' 
        ' lblResult
        ' 
        lblResult.AutoSize = True
        lblResult.Location = New Point(316, 168)
        lblResult.Name = "lblResult"
        lblResult.RightToLeft = RightToLeft.Yes
        lblResult.Size = New Size(49, 20)
        lblResult.TabIndex = 1
        lblResult.Text = "Result"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(lblResult)
        Controls.Add(Import)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Import As Button
    Friend WithEvents lblResult As Label

End Class
