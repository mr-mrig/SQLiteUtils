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
        private bool _isDisposed = false;
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
            TableWrappers = new List<DatabaseObjectWrapper>();

            _tempFileWriters = new Dictionary<string, StreamWriter>();
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

        ~BulkInsertScriptDbWriter()
        {
            if (!_isDisposed)
                Close();
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
                    throw new Exception($"{GetType().Name} - {tempFileName} - Error while closing the transaction: {exc.Message}");
                }
            }
            dest.WriteLine(";");
            dest.Close();
        }


        /// <summary>
        /// Writes as many script files as needed (the bulk inserts are split into different files to avoid high memory requirements when executing them)
        /// </summary>
        /// <param name="processTitle">Title that describes the ongoing process</param>
        /// <param name="scriptGenerator">Function which writes the script file</param>
        /// <param name="rowsPerScriptFile">Number of rows per script file. If not specified then the GymAppSQLiteConfig.UsersPerScriptFile is used.</param>
        /// <param name="rowNum">Number of rows to be inserted</param>
        public void ProcessTransaction(string processTitle, Func<long, long> scriptGenerator, long rowNum, uint rowsPerScriptFile = 0)
        {
            uint currentNewRows = 0;
            long rowCounter = 0;

            uint fileRows = rowsPerScriptFile == 0 ? GymAppSQLiteConfig.UsersPerScriptFile : rowsPerScriptFile;

            // Number of files to be generated
            ushort totalParts = (ushort)Math.Ceiling((float)rowNum / fileRows);

            // Split files so they don't exceed the maximum number of rows per file
            for (ushort iPart = 0; iPart < totalParts; iPart++)
            {
                // Compute number of rows wrt the number of files
                currentNewRows = (uint)(iPart == totalParts - 1 ? rowNum - (iPart * fileRows) : fileRows);

                SqlScriptFilename = GetScriptFileFullpath(processTitle, (ushort)(iPart + 1), totalParts);

                // Write
                rowCounter += scriptGenerator(currentNewRows);
            }

            // Write statistics
            WriteStatFile(GetScriptStatFileFullpath(), rowCounter);
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

                if(SqlConnection != null && SqlConnection?.State != System.Data.ConnectionState.Closed)
                    SqlConnection?.Close();

                _isDisposed = true;
            }
            catch (Exception exc)
            {
                throw new Exception($"{GetType().Name} - Error while closing the writer: {exc.Message}");
            }
        }


        public void Dispose()
        {
            if(!_isDisposed)
                Close(); 
        }
        #endregion


        #region Public Methods

        public string GetInsertStatement(DatabaseObjectWrapper table)
        {
            //return $@";{Environment.NewLine} INSERT INTO {table.TableName} ({string.Join(", ", table.Entry.Select(x => x.Name))}) VALUES";
            return $@";{Environment.NewLine} REPLACE INTO {table.TableName} ({string.Join(", ", table.Entry.Select(x => x.Name))}) VALUES";
        }


        public long GetStatsTargetRows()
        {
            string filepath = GetScriptStatFileFullpath();
            StreamReader reader = null;
            long ret = 0;

            try
            {
                using (reader = new StreamReader(File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                {
                    if (!reader.EndOfStream)
                        ret = long.Parse(reader.ReadLine());
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                reader?.Close();
            }
            return ret;
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Write the parameters specified on the stat file.
        /// The file stores all the rows inserted, hence keep tracks of the previous ones unless deleted or flagged as "overwrite"
        /// </summary>
        /// <param name="filepath">The stat file path</param>
        /// <param name="rowNum">The number of rows just processed</param>
        /// <param name="overwrite">Overwrite the file (IE reset) or keep updating the statistics</param>
        private void WriteStatFile(string filepath, long rowNum, bool overwrite = false)
        {

            using (FileStream fs = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    long rowNumPrev = 0;

                    // Get the number of rows of the previous process, if not overwrite
                    if (!overwrite)
                    {
                        byte[] inBuffer = new byte[fs.Length];

                        if (inBuffer.Length > 0)
                        {
                            fs.Read(inBuffer, 0, inBuffer.Length);
                            rowNumPrev = long.Parse(string.Join("", (inBuffer.Select(x => (char)x))));
                        }
                    }

                    // Update the total rows
                    rowNum += rowNumPrev;

                    // Write the file
                    byte[] outBuffer = rowNum.ToString().ToCharArray().Select(x => (byte)x).ToArray();
                    fs.Position = 0;
                    fs.Write(outBuffer, 0, outBuffer.Length);

                    fs.Close();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }

        }

        /// <summary>
        /// Build the path of the file storing the stats of the current process (one stat file for all the partial scripts of the process).
        /// </summary>
        /// <returns>The file fullpath</returns>
        private string GetScriptStatFileFullpath()
        {
            return Regex.Replace(GymAppSQLiteConfig.SqlScriptStatFilePath, "##suffix##", GymAppSQLiteConfig.SqlScriptStatSuffix) + ".txt";
        }


        /// <summary>
        /// Build the path of the script split in many parts in the format: path\name-suffix-_-partNumber-_of_-totalPartsNumber-.sql
        /// Example: C:\myfolder\myscript_mysuffix_01_of_03.sql
        /// </summary>
        /// <param name="filenameSuffix">Suffix to be appended to the filename</param>
        /// <param name="partNumber">Current file part</param>
        /// <param name="totalPartsNumber">total file parts</param>
        /// <returns>The script fullpath</returns>
        public string GetScriptFileFullpath(string filenameSuffix, ushort partNumber, ushort totalPartsNumber)
        {
            string timestamp = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D3}",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond);

            return Regex.Replace(Regex.Replace(Regex.Replace(
                GymAppSQLiteConfig.SqlScriptFilePath, "##suffix##", filenameSuffix)
                , @"##part##", $"_{partNumber.ToString("d2")}_of_{totalPartsNumber.ToString("d2")}")
                , "##ts##", timestamp);
        }


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
                if(fs.Length >= 9)
                {
                    // Read the last chars of the file
                    fs.Seek(-9, SeekOrigin.End);
                    fs.Read(buffer, 0, buffer.Length);

                    // Reloop
                    fs.Position = 0;

                    // No rows has been inserted if the file ends with "VALUES"
                    return (new string(buffer.Select(x => (char)x).ToArray<char>())) != " VALUES";
                }
                return false;
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }
        #endregion
    }
}
