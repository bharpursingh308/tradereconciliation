Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Db.EnsureCreated()
    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles Import.Click

        Try
            Using dlg As New OpenFileDialog()
                dlg.Filter = "CSV files (*.csv)|*.csv"
                dlg.Title = "Select trades CSV"
                If dlg.ShowDialog() <> DialogResult.OK Then Return

                Dim svc As New TradeImportService()
                Dim batchId = svc.ImportTrades(dlg.FileName)

                Dim c = GetCountsForBatch(batchId)
                MessageBox.Show($"Batch={batchId}{Environment.NewLine}Trades={c.Item1}{Environment.NewLine}Exceptions={c.Item2}{Environment.NewLine}DB={Db.DbPath()}")

                lblResult.Text = $"Imported. BatchId: {batchId}"
                MessageBox.Show(
                    "Import complete." & Environment.NewLine &
                    "BatchId: " & batchId & Environment.NewLine &
                    "DB File: " & Db.DbPath(),
                    "Done"
                    )

                Dim f As New ExceptionsForm(batchId)
                f.Show()

            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "Import failed")
        End Try

    End Sub

    Private Function GetCountsForBatch(batchId As String) As Tuple(Of Integer, Integer)
        Using conn = Db.GetConnection()
            conn.Open()

            Dim trades As Integer
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT COUNT(*) FROM Trades WHERE ImportBatchId = @b"
                cmd.Parameters.AddWithValue("@b", batchId)
                trades = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            Dim exs As Integer
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT COUNT(*) FROM Exceptions WHERE ImportBatchId = @b"
                cmd.Parameters.AddWithValue("@b", batchId)
                exs = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            Return Tuple.Create(trades, exs)
        End Using
    End Function

End Class
