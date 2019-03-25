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
        public const string SqlScriptFilename = "BulkInsertScript";
        #endregion


        #region Private Fields
        private Dictionary<string, StreamWriter> _tempFileWriters = new Dictionary<string, StreamWriter>();
        #endregion

        #region Properties

        public bool CleanTempFiles { get; set; } = true;
        public string WorkingDir { get; set; } = string.Empty;
        public string DbPath { get; set; } = string.Empty;
        public SQLiteConnection SqlConnection { get; set; } = null;
        public List<DatabaseObjectWrapper> TableWrappers { get; set; } = new List<DatabaseObjectWrapper>();
        #endregion


        #region Ctors

        public BulkInsertScriptDbWriter(string workingDir, string dbPath)
        {
            WorkingDir = workingDir;
            DbPath = dbPath;

            SqlConnection = DatabaseUtility.NewFastestSQLConnection(DbPath);
        }


        public BulkInsertScriptDbWriter(string workingDir, string dbPath, List<DatabaseObjectWrapper> tableWrappers) : this(workingDir, dbPath)
        {
            TableWrappers = tableWrappers;
        }


        public BulkInsertScriptDbWriter()
        {

        }
        #endregion




        #region IDbWriter Implementation

        public bool Open()
        {
            // Open SQL connection
            try
            {
                SqlConnection = DatabaseUtility.OpenFastestSQLConnection(SqlConnection, DbPath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool StartTransaction()
        {

            // Open and init temp file streams.
            try
            {
                int fileCounter = 0;

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
                return false;
            }

            return true;
        }


        public bool Append()
        {
            throw new NotImplementedException();
        }


        public bool EndTransaction()
        {
            StreamWriter dest = new StreamWriter(File.OpenWrite(SqlScriptFilename));

            // First close the stream writers
            _tempFileWriters.Values.ToList().ForEach(x => x.Close());

            // Append the temporary files into the destination file before deleting them
            foreach (string tempFileName in Directory.GetFiles(WorkingDir).Where(x => IsTempFile(x)))
            {
                try
                {
                    FileStream fs = File.Open(tempFileName, FileMode.Open, FileAccess.ReadWrite);

                    // Remove the exceeding comma from the file before appending it
                    fs.SetLength(fs.Length - 2);

                    fs.CopyTo(dest.BaseStream);
                    dest.WriteLine(";");
                    dest.WriteLine("");

                    fs.Flush();
                    fs.Close();

                    if (CleanTempFiles)
                        File.Delete(tempFileName);
                }
                catch(Exception exc)
                {
                    dest.Close();
                    return false;
                }
            }
            dest.Close();

            return true;
        }


        public bool Write()
        {
            throw new NotImplementedException();
        }


        public bool Close()
        {
            try
            {
                _tempFileWriters.Where(x => x.Value != null).Select(x => x.Value).ToList().ForEach(x => x?.Close());
            }
            catch
            {
                return false;
            }

            return true;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Public Methods

        private string GetInsertStatement(DatabaseObjectWrapper table)
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

        #endregion
    }
}
