Imports System.Data.SQLite
Imports System.Globalization
Imports System.IO

Public Class TradeImportService

    Public Function ImportTrades(csvPath As String) As String
        Dim batchId As String = Guid.NewGuid().ToString("N")
        Dim nowIso As String = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")

        Dim lines = File.ReadAllLines(csvPath)
        If lines.Length <= 1 Then
            Throw New Exception("CSV has no data rows.")
        End If

        ' Assumes first line is header
        Dim totalRows As Integer = 0

        Using conn = Db.GetConnection()
            conn.Open()

            Using tx = conn.BeginTransaction()

                For i As Integer = 1 To lines.Length - 1
                    Dim raw = lines(i).Trim()
                    If raw = "" Then Continue For

                    totalRows += 1

                    Dim cols = raw.Split(","c)
                    ' Expecting: TradeId,Symbol,Quantity,Price,TradeDate
                    Dim tradeId As String = SafeGet(cols, 0)
                    Dim symbol As String = SafeGet(cols, 1)
                    Dim qtyStr As String = SafeGet(cols, 2)
                    Dim priceStr As String = SafeGet(cols, 3)
                    Dim dateStr As String = SafeGet(cols, 4)

                    Dim qty? As Integer = TryParseInt(qtyStr)
                    Dim price? As Decimal = TryParseDec(priceStr)
                    Dim tradeDate? As DateTime = TryParseDate(dateStr)

                    InsertTrade(conn, batchId, tradeId, symbol, qty, price, dateStr, raw, nowIso)

                    ' Validation -> Exceptions
                    ValidateAndInsertExceptions(conn, batchId, tradeId, symbol, qty, price, tradeDate, nowIso)
                Next

                ' Duplicate TradeId rule (per batch)
                InsertDuplicateTradeIdExceptions(conn, batchId, nowIso)

                ' Import log
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "INSERT INTO ImportLog (ImportBatchId, FileName, TotalRows, ImportedAt) VALUES (@b,@f,@t,@at)"
                    cmd.Parameters.AddWithValue("@b", batchId)
                    cmd.Parameters.AddWithValue("@f", Path.GetFileName(csvPath))
                    cmd.Parameters.AddWithValue("@t", totalRows)
                    cmd.Parameters.AddWithValue("@at", nowIso)
                    cmd.ExecuteNonQuery()
                End Using

                tx.Commit()
            End Using
        End Using

        Return batchId
    End Function

    Private Sub InsertTrade(conn As SQLiteConnection, batchId As String, tradeId As String, symbol As String,
                            qty As Integer?, price As Decimal?, tradeDateRaw As String, rawLine As String, nowIso As String)

        Using cmd = conn.CreateCommand()
            cmd.CommandText =
"INSERT INTO Trades (ImportBatchId, TradeId, Symbol, Quantity, Price, TradeDate, RawLine, CreatedAt)
 VALUES (@b,@tid,@sym,@q,@p,@dt,@raw,@ca)"
            cmd.Parameters.AddWithValue("@b", batchId)
            cmd.Parameters.AddWithValue("@tid", If(tradeId, DBNull.Value))
            cmd.Parameters.AddWithValue("@sym", If(symbol, DBNull.Value))
            cmd.Parameters.AddWithValue("@q", If(qty.HasValue, CType(qty.Value, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@p", If(price.HasValue, CType(price.Value, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@dt", If(tradeDateRaw, DBNull.Value))
            cmd.Parameters.AddWithValue("@raw", rawLine)
            cmd.Parameters.AddWithValue("@ca", nowIso)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub ValidateAndInsertExceptions(conn As SQLiteConnection, batchId As String, tradeId As String, symbol As String,
                                          qty As Integer?, price As Decimal?, tradeDate As DateTime?, nowIso As String)

        ' Required fields
        If String.IsNullOrWhiteSpace(tradeId) Then
            InsertException(conn, batchId, tradeId, "MISSING_REQUIRED", "TradeId", "TradeId is required.", nowIso)
        End If
        If String.IsNullOrWhiteSpace(symbol) Then
            InsertException(conn, batchId, tradeId, "MISSING_REQUIRED", "Symbol", "Symbol is required.", nowIso)
        End If
        If Not qty.HasValue Then
            InsertException(conn, batchId, tradeId, "MISSING_REQUIRED", "Quantity", "Quantity is required.", nowIso)
        End If
        If Not price.HasValue Then
            InsertException(conn, batchId, tradeId, "MISSING_REQUIRED", "Price", "Price is required.", nowIso)
        End If
        If Not tradeDate.HasValue Then
            InsertException(conn, batchId, tradeId, "MISSING_REQUIRED", "TradeDate", "TradeDate is required.", nowIso)
        End If

        ' Numeric rules
        If qty.HasValue AndAlso qty.Value <= 0 Then
            InsertException(conn, batchId, tradeId, "INVALID_QTY", "Quantity", "Quantity must be > 0.", nowIso)
        End If
        If price.HasValue AndAlso price.Value <= 0D Then
            InsertException(conn, batchId, tradeId, "INVALID_PRICE", "Price", "Price must be > 0.", nowIso)
        End If

        ' Date rule
        If tradeDate.HasValue AndAlso tradeDate.Value.Date > DateTime.UtcNow.Date Then
            InsertException(conn, batchId, tradeId, "FUTURE_DATE", "TradeDate", "TradeDate cannot be in the future.", nowIso)
        End If
    End Sub

    Private Sub InsertDuplicateTradeIdExceptions(conn As SQLiteConnection, batchId As String, nowIso As String)
        Using cmd = conn.CreateCommand()
            cmd.CommandText =
"SELECT TradeId, COUNT(*) AS Cnt
 FROM Trades
 WHERE ImportBatchId = @b AND TradeId IS NOT NULL AND TRIM(TradeId) <> ''
 GROUP BY TradeId
 HAVING COUNT(*) > 1"
            cmd.Parameters.AddWithValue("@b", batchId)

            Using rdr = cmd.ExecuteReader()
                While rdr.Read()
                    Dim tradeId As String = rdr.GetString(0)
                    InsertException(conn, batchId, tradeId, "DUPLICATE_TRADEID", "TradeId", "Duplicate TradeId found in batch.", nowIso)
                End While
            End Using
        End Using
    End Sub

    Private Sub InsertException(conn As SQLiteConnection, batchId As String, tradeId As String,
                               ruleCode As String, fieldName As String, message As String, nowIso As String)
        Using cmd = conn.CreateCommand()
            cmd.CommandText =
"INSERT INTO Exceptions (ImportBatchId, TradeId, RuleCode, FieldName, Message, Status, CreatedAt)
 VALUES (@b,@tid,@r,@f,@m,'OPEN',@ca)"
            cmd.Parameters.AddWithValue("@b", batchId)
            cmd.Parameters.AddWithValue("@tid", If(String.IsNullOrWhiteSpace(tradeId), DBNull.Value, tradeId))
            cmd.Parameters.AddWithValue("@r", ruleCode)
            cmd.Parameters.AddWithValue("@f", If(fieldName, DBNull.Value))
            cmd.Parameters.AddWithValue("@m", message)
            cmd.Parameters.AddWithValue("@ca", nowIso)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function SafeGet(cols As String(), idx As Integer) As String
        If cols Is Nothing OrElse idx < 0 OrElse idx >= cols.Length Then Return Nothing
        Return cols(idx).Trim()
    End Function

    Private Function TryParseInt(s As String) As Integer?
        Dim v As Integer
        If Integer.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, v) Then Return v
        Return Nothing
    End Function

    Private Function TryParseDec(s As String) As Decimal?
        Dim v As Decimal
        If Decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, v) Then Return v
        Return Nothing
    End Function

    Private Function TryParseDate(s As String) As DateTime?
        Dim v As DateTime
        If DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, v) Then
            Return v.Date
        End If
        Return Nothing
    End Function

End Class

