Imports System.Data.SQLite
Imports System.IO

Public Module Db
    Private ReadOnly DbFile As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bankops.db")

    Public Function GetConnection() As SQLiteConnection
        Dim cs As String = $"Data Source={DbFile};Version=3;"
        Return New SQLiteConnection(cs)
    End Function

    Public Sub EnsureCreated()
        If Not File.Exists(DbFile) Then
            SQLiteConnection.CreateFile(DbFile)
        End If

        Using conn = GetConnection()
            conn.Open()
            Using cmd = conn.CreateCommand()
                cmd.CommandText =
"CREATE TABLE IF NOT EXISTS Trades (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  ImportBatchId TEXT NOT NULL,
  TradeId TEXT,
  Symbol TEXT,
  Quantity INTEGER,
  Price REAL,
  TradeDate TEXT,
  RawLine TEXT,
  CreatedAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Exceptions (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  ImportBatchId TEXT NOT NULL,
  TradeId TEXT,
  RuleCode TEXT NOT NULL,
  FieldName TEXT,
  Message TEXT NOT NULL,
  Status TEXT NOT NULL,
  CreatedAt TEXT NOT NULL,
  ResolvedAt TEXT
);
CREATE TABLE IF NOT EXISTS Audit (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  ExceptionId INTEGER NOT NULL,
  Action TEXT NOT NULL,
  Actor TEXT NOT NULL,
  Notes TEXT,
  CreatedAt TEXT NOT NULL,
  FOREIGN KEY(ExceptionId) REFERENCES Exceptions(Id)
);
CREATE TABLE IF NOT EXISTS ImportLog (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  ImportBatchId TEXT NOT NULL,
  FileName TEXT NOT NULL,
  TotalRows INTEGER NOT NULL,
  ImportedAt TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS IX_Exceptions_Status ON Exceptions(Status);
CREATE INDEX IF NOT EXISTS IX_Trades_Batch ON Trades(ImportBatchId);"
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Function DbPath() As String
        Return DbFile
    End Function

End Module
