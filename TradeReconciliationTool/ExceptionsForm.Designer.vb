<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ExceptionsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        cmbStatus = New ComboBox()
        txtSearch = New TextBox()
        btnRefresh = New Button()
        gridExceptions = New DataGridView()
        txtNotes = New TextBox()
        txtActor = New TextBox()
        btnResolve = New Button()
        CType(gridExceptions, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' cmbStatus
        ' 
        cmbStatus.FormattingEnabled = True
        cmbStatus.Items.AddRange(New Object() {"OPEN", "RESOLVED", "ALL"})
        cmbStatus.Location = New Point(240, 58)
        cmbStatus.Name = "cmbStatus"
        cmbStatus.Size = New Size(151, 28)
        cmbStatus.TabIndex = 0
        cmbStatus.Text = "OPEN"
        ' 
        ' txtSearch
        ' 
        txtSearch.Location = New Point(422, 59)
        txtSearch.Name = "txtSearch"
        txtSearch.Size = New Size(151, 27)
        txtSearch.TabIndex = 1
        txtSearch.Text = "TradeID"
        ' 
        ' btnRefresh
        ' 
        btnRefresh.Location = New Point(615, 59)
        btnRefresh.Name = "btnRefresh"
        btnRefresh.Size = New Size(146, 29)
        btnRefresh.TabIndex = 2
        btnRefresh.Text = "Refresh"
        btnRefresh.UseVisualStyleBackColor = True
        ' 
        ' gridExceptions
        ' 
        gridExceptions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        gridExceptions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        gridExceptions.Location = New Point(12, 108)
        gridExceptions.MultiSelect = False
        gridExceptions.Name = "gridExceptions"
        gridExceptions.ReadOnly = True
        gridExceptions.RowHeadersWidth = 51
        gridExceptions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        gridExceptions.Size = New Size(776, 204)
        gridExceptions.TabIndex = 3
        ' 
        ' txtNotes
        ' 
        txtNotes.Location = New Point(100, 360)
        txtNotes.MinimumSize = New Size(0, 60)
        txtNotes.Multiline = True
        txtNotes.Name = "txtNotes"
        txtNotes.Size = New Size(150, 60)
        txtNotes.TabIndex = 4
        ' 
        ' txtActor
        ' 
        txtActor.Location = New Point(323, 370)
        txtActor.Name = "txtActor"
        txtActor.Size = New Size(135, 27)
        txtActor.TabIndex = 5
        txtActor.Text = "‏Environment.UserName"
        ' 
        ' btnResolve
        ' 
        btnResolve.Font = New Font("Microsoft Sans Serif", 9F)
        btnResolve.Location = New Point(564, 378)
        btnResolve.Name = "btnResolve"
        btnResolve.Size = New Size(150, 29)
        btnResolve.TabIndex = 6
        btnResolve.Text = "Mark Resolve"
        btnResolve.UseVisualStyleBackColor = True
        ' 
        ' ExceptionsForm
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(btnResolve)
        Controls.Add(txtActor)
        Controls.Add(txtNotes)
        Controls.Add(gridExceptions)
        Controls.Add(btnRefresh)
        Controls.Add(txtSearch)
        Controls.Add(cmbStatus)
        Name = "ExceptionsForm"
        Text = "ExceptionsForm"
        CType(gridExceptions, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cmbStatus As ComboBox
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents gridExceptions As DataGridView
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents txtActor As TextBox
    Friend WithEvents btnResolve As Button
End Class
