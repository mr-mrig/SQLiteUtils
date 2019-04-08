using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SQLiteUtils.Model;



namespace SQLiteUtils.Util
{



    class BulkInsertScriptDbWriter : IDbWriter
    {



        #region Consts
        #endregion


        #region Private Fields
        private Dictionary<string, StreamWriter> _tempFileWriters;
        #endregion

        #region Properties

        public bool CleanTempFiles { get; set; } = true;
        public string WorkingDir { get; set; } = string.Empty;
        public string DbPath { get; set; } = string.Empty;
        public SQLiteConnection SqlConnection { get; set; } = null;
        public List<DatabaseObjectWrapper> TableWrappers { get; set; }
        public string SqlScriptFilename { get; set; } = "BulkInsertScript.sql";
        #endregion


        #region Ctors

        public BulkInsertScriptDbWriter(string workingDir, string dbPath)
        {
            WorkingDir = workingDir;
            DbPath = dbPath;

            _tempFileWriters = new Dictionary<string, StreamWriter>();

            if (SqlConnection == null || SqlConnection?.State == System.Data.ConnectionState.Closed)
                SqlConnection = DatabaseUtility.NewFastestSQLConnection(DbPath);

            TableWrappers = new List<DatabaseObjectWrapper>();

            if (SqlConnection == null)
                throw new SQLiteException("Db not found");
        }


        public BulkInsertScriptDbWriter(string workingDir, string dbPath, string sqlScriptFilename)
            : this(workingDir, dbPath)
        {
            SqlScriptFilename = sqlScriptFilename;
        }


        public BulkInsertScriptDbWriter(string workingDir, string dbPath, List<DatabaseObjectWrapper> tableWrappers) 
            : this(workingDir, dbPath)
        {
            TableWrappers = tableWrappers;
        }


        public BulkInsertScriptDbWriter()
        {
            _tempFileWriters = new Dictionary<string, StreamWriter>();
        }
        #endregion




        #region IDbWriter Implementation

        public void Open()
        {
            // Open SQL connection
            try
            {
                SqlConnection = DatabaseUtility.OpenFastestSQLConnection(SqlConnection, DbPath);
            }
            catch (Exception exc)
            {
                throw new Exception($"{GetType().Name} - Error while opening the SQL connection: {exc.Message}");
            }
        }

        public void StartTransaction()
        {

            // Open and init temp file streams.
            try
            {
                int fileCounter = 0;
                _tempFileWriters = new Dictionary<string, StreamWriter>();

                foreach (DatabaseObjectWrapper table in TableWrappers)
                {
                    // Open the stream
                    _tempFileWriters.Add(table.TableName, new StreamWriter(
                        File.Open(GetTableTempPath(WorkingDir, table.TableName, fileCounter++), FileMode.Create, FileAccess.ReadWrite)));
                    // Write Insert Into header statement
                    _tempFileWriters[table.TableName].WriteLine(GetInsertStatement(table));
                }
            }
            catch(Exception exc)
            {
                throw new Exception($"{GetType().Name} - Error while opening the transaction: {exc.Message}");
            }
        }


        public void Append()
        {
            throw new NotImplementedException();
        }


        public void EndTransaction()
        {
            StreamWriter dest = new StreamWriter(File.Open(Path.Combine(WorkingDir, SqlScriptFilename), FileMode.Create, FileAccess.Write));

            // First close the stream writers
            _tempFileWriters.Values.ToList().ForEach(x => x.Close());

            // Append the temporary files into the destination file before deleting them
            foreach (string tempFileName in Directory.GetFiles(WorkingDir).Where(x => IsTempFile(x)))
            {
                try
                {
                    FileStream fs = File.Open(tempFileName, FileMode.Open, FileAccess.ReadWrite);

                    // Check if any rows has been inserted, otherwise the Insert statement must not be written (in order to avoid Sql errors)
                    if (AnyRowInserted(fs))
                    {
                        // Remove the exceeding comma from the file before appending it
                        fs.SetLength(fs.Length - 2);

                        fs.CopyTo(dest.BaseStream);
                    }
                    fs.Flush();
                    fs.Close();

                    if (CleanTempFiles)
                        File.Delete(tempFileName);
                }
                catch (Exception exc)
                {
                    dest.Close();
                    throw new Exception($"{GetType().Name} - Error while closing the transaction: {exc.Message}");
                }
            }
            dest.WriteLine(";");
            dest.Close();
        }


        public void Write(DatabaseObjectWrapper entry)
        {
            try
            {
                _tempFileWriters[entry.TableName].Write($@" ( {string.Join(", ", entry.ToSqlString())} ), ");
            }
            catch (Exception exc)
            {
                throw new Exception($"{GetType().Name} - Error while writing the script file {exc.Message}");
            }
        }


        public void Close()
        {
            try
            {
                _tempFileWriters.Where(x => x.Value != null).Select(x => x.Value).ToList().ForEach(x => x?.Close());
            }
            catch (Exception exc)
            {
                throw new Exception($"{GetType().Name} - Error while closing the transaction: {exc.Message}");
            }
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Public Methods

        public string GetInsertStatement(DatabaseObjectWrapper table)
        {
            return $@";{Environment.NewLine} INSERT INTO {table.TableName} ({string.Join(", ", table.Entry.Select(x => x.Name))}) VALUES";
        }
        #endregion


        #region Private Methods

        private string GetTableTempPath(string folderPath, string tableName, int fileCounter)
        {
            return Path.Combine(folderPath, "temp_" + (fileCounter).ToString("d3") + "_" + tableName + ".txt");
        }

        /// <summary>
        /// Checks if the specified file is a temporary one.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Is / Isn't a temporary file</returns>
        private bool IsTempFile(string fileName)
        {
            return Regex.IsMatch(fileName, "temp_[0-9]{3}_[a-zA-Z0-9.]+.txt");
        }


        /// <summary>
        /// Checks whether any row has been inserted in the table stored int the selected file.
        /// </summary>
        /// <param name="fs">The temp file storing the bulk inserts</param>
        /// <returns>Rows inserted / not inserted</returns>
        private bool AnyRowInserted(FileStream fs)
        {
            byte[] buffer = new byte[7];

            try
            {
                // Read the last chars of the file
                fs.Seek(-9, SeekOrigin.End);
                fs.Read(buffer, 0, buffer.Length);

                // Reloop
                fs.Position = 0;

                // No rows has been inserted if the file ends with "VALUES"
                return (new string(buffer.Select(x => (char)x).ToArray<char>())) != " VALUES";
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion
    }
}
