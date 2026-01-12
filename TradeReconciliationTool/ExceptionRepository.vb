Imports System.Data
Imports System.Data.SQLite

Public Class ExceptionRepository

    Public Function GetExceptions(statusFilter As String, search As String) As DataTable
        Dim dt As New DataTable()

        Using conn = Db.GetConnection()
            conn.Open()

            Using cmd = conn.CreateCommand()
                Dim sql As String =
"SELECT e.Id,
        e.ImportBatchId,
        e.TradeId,
        e.RuleCode,
        e.FieldName,
        e.Message,
        e.Status,
        e.CreatedAt,
        e.ResolvedAt
 FROM Exceptions e
 WHERE 1=1 "

                If Not String.IsNullOrWhiteSpace(statusFilter) AndAlso statusFilter <> "ALL" Then
                    ' Compare statuses case-insensitively to avoid mismatches like "Open" vs "OPEN"
                    sql &= " AND UPPER(IFNULL(e.Status,'')) = @status "
                    cmd.Parameters.AddWithValue("@status", statusFilter.ToUpperInvariant())
                End If

                If Not String.IsNullOrWhiteSpace(search) Then
                    ' search TradeId or Symbol via join to Trades (optional but useful)
                    sql &= " AND (e.TradeId LIKE @q OR EXISTS (
                                SELECT 1 FROM Trades t
                                WHERE t.ImportBatchId = e.ImportBatchId
                                  AND IFNULL(t.TradeId,'') = IFNULL(e.TradeId,'')
                                  AND IFNULL(t.Symbol,'') LIKE @q
                             )) "
                    cmd.Parameters.AddWithValue("@q", "%" & search.Trim() & "%")
                End If

                sql &= " ORDER BY e.Status ASC, e.CreatedAt DESC;"

                cmd.CommandText = sql

                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Public Sub ResolveException(exceptionId As Long, actor As String, notes As String)
        Dim nowIso As String = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")

        Using conn = Db.GetConnection()
            conn.Open()
            Using tx = conn.BeginTransaction()

                ' Update exception status
                Using cmd = conn.CreateCommand()
                    cmd.CommandText =
"UPDATE Exceptions
 SET Status = 'RESOLVED',
     ResolvedAt = @ra
 WHERE Id = @id AND Status <> 'RESOLVED';"
                    cmd.Parameters.AddWithValue("@ra", nowIso)
                    cmd.Parameters.AddWithValue("@id", exceptionId)
                    Dim rows = cmd.ExecuteNonQuery()
                    If rows = 0 Then
                        Throw New Exception("Exception already resolved or not found.")
                    End If
                End Using

                ' Insert audit record
                Using cmd = conn.CreateCommand()
                    cmd.CommandText =
"INSERT INTO Audit (ExceptionId, Action, Actor, Notes, CreatedAt)
 VALUES (@eid, 'RESOLVED', @actor, @notes, @ca);"
                    cmd.Parameters.AddWithValue("@eid", exceptionId)
                    cmd.Parameters.AddWithValue("@actor", If(String.IsNullOrWhiteSpace(actor), Environment.UserName, actor.Trim()))
                    cmd.Parameters.AddWithValue("@notes", If(String.IsNullOrWhiteSpace(notes), DBNull.Value, notes.Trim()))
                    cmd.Parameters.AddWithValue("@ca", nowIso)
                    cmd.ExecuteNonQuery()
                End Using

                tx.Commit()
            End Using
        End Using
    End Sub

    Public Function GetAuditForException(exceptionId As Long) As DataTable
        Dim dt As New DataTable()

        Using conn = Db.GetConnection()
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText =
"SELECT Id, Action, Actor, Notes, CreatedAt
 FROM Audit
 WHERE ExceptionId = @eid
 ORDER BY CreatedAt DESC;"
                cmd.Parameters.AddWithValue("@eid", exceptionId)

                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Public Function GetDistinctStatuses() As DataTable
        Dim dt As New DataTable()
        Using conn = Db.GetConnection()
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "SELECT Status, COUNT(*) AS Cnt FROM Exceptions GROUP BY Status;"
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

End Class

