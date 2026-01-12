Imports System.Data.SQLite

Public Class ExceptionsForm

    Private ReadOnly _repo As New ExceptionRepository()
    Private _currentBatchId As String = Nothing 'optional filter later
    Private _isInitializing As Boolean = True
    Public Sub New(Optional importBatchId As String = Nothing)
        InitializeComponent()
        _currentBatchId = importBatchId
    End Sub

    Private Sub ExceptionsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _isInitializing = True
        ' Status dropdown
        'gridExceptions.AutoGenerateColumns = True
        'gridExceptions.ReadOnly = True
        'gridExceptions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        'gridExceptions.MultiSelect = False

        cmbStatus.Items.Clear()
        cmbStatus.Items.Add("OPEN")
        cmbStatus.Items.Add("RESOLVED")
        cmbStatus.Items.Add("ALL")

        cmbStatus.SelectedItem = "ALL"

        txtActor.Text = Environment.UserName

        _isInitializing = False

        LoadGrid()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadGrid()
    End Sub

    Private Sub cmbStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStatus.SelectedIndexChanged
        If _isInitializing Then Return
        LoadGrid()
    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            LoadGrid()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub LoadGrid()

        Dim dt As New DataTable()
        Using conn = Db.GetConnection()
            conn.Open()

            ' absolute truth: how many exceptions exist in THIS db file?
            Using cmd As SQLiteCommand = conn.CreateCommand()
                cmd.CommandText = "SELECT COUNT(*) FROM Exceptions;"
                Dim total = Convert.ToInt32(cmd.ExecuteScalar())
                MessageBox.Show("DB=" & Db.DbPath() & Environment.NewLine & "Exceptions total=" & total)
            End Using

            ' load all rows (no filters)
            Using cmd As SQLiteCommand = conn.CreateCommand()
                cmd.CommandText = "SELECT Id, ImportBatchId, TradeId, RuleCode, FieldName, Message, Status, CreatedAt, ResolvedAt FROM Exceptions ORDER BY CreatedAt DESC;"
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        MessageBox.Show("DT Rows=" & dt.Rows.Count & " | Grid Rows=" & gridExceptions.Rows.Count & " | Cols=" & gridExceptions.Columns.Count)


        Dim statusFilter = If(cmbStatus.SelectedItem Is Nothing, "ALL", cmbStatus.SelectedItem.ToString())
        Dim q = txtSearch.Text

        'Dim dt = _repo.GetExceptions(statusFilter, q)

        ' Bind the DataTable to the DataGridView
        gridExceptions.SuspendLayout()
        gridExceptions.DataSource = Nothing
        gridExceptions.AutoGenerateColumns = True
        gridExceptions.DataSource = dt
        gridExceptions.Visible = True
        gridExceptions.ResumeLayout()
        gridExceptions.Refresh()

        ' Nice column names - run after binding
        If gridExceptions.Columns.Contains("ImportBatchId") Then gridExceptions.Columns("ImportBatchId").HeaderText = "Batch"
        If gridExceptions.Columns.Contains("TradeId") Then gridExceptions.Columns("TradeId").HeaderText = "Trade ID"
        If gridExceptions.Columns.Contains("RuleCode") Then gridExceptions.Columns("RuleCode").HeaderText = "Rule"
        If gridExceptions.Columns.Contains("FieldName") Then gridExceptions.Columns("FieldName").HeaderText = "Field"
        If gridExceptions.Columns.Contains("CreatedAt") Then gridExceptions.Columns("CreatedAt").HeaderText = "Created"
        If gridExceptions.Columns.Contains("ResolvedAt") Then gridExceptions.Columns("ResolvedAt").HeaderText = "Resolved"

        ' Hide internal columns if you want
        ' gridExceptions.Columns("ImportBatchId").Visible = False
    End Sub

    Private Function GetSelectedExceptionId() As Long?
        If gridExceptions.CurrentRow Is Nothing Then Return Nothing
        Dim val = gridExceptions.CurrentRow.Cells("Id").Value
        If val Is Nothing OrElse IsDBNull(val) Then Return Nothing
        Return Convert.ToInt64(val)
    End Function

    Private Sub btnResolve_Click(sender As Object, e As EventArgs) Handles btnResolve.Click
        Dim idOpt = GetSelectedExceptionId()
        If Not idOpt.HasValue Then
            MessageBox.Show("Select an exception first.")
            Return
        End If

        Try
            _repo.ResolveException(idOpt.Value, txtActor.Text, txtNotes.Text)
            txtNotes.Clear()
            LoadGrid()
            MessageBox.Show("Exception resolved and audit recorded.", "Done")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error")
        End Try
    End Sub

End Class
