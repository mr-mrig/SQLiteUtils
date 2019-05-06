﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class CommentWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "Comment";
        #endregion


        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        private int _postIdMin;
        private int _postIdMax;
        #endregion


        #region Properties
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the FitnessDayEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public CommentWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, true)
        {
            _userIdMin = 1;
            _userIdMax = DatabaseUtility.GetTableMaxId(connection, "User", true);

            _postIdMin = 1;
            _postIdMax = DatabaseUtility.GetTableMaxId(connection, "Post", true);
        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {

                    case "Id":

                        col.Value = parentId;
                        break;

                    case "Body":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(50, 500));
                        break;

                    case "CreatedOn":

                        col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        break;


                    case "LastUpdate":

                        col.Value = null;
                        break;

                    case "UserId":

                        col.Value = RandomFieldGenerator.RandomInt(_userIdMin, _userIdMax + 1);
                        break;

                    case "PostId":

                        col.Value = RandomFieldGenerator.RandomInt(_postIdMin, _postIdMax + 1);
                        break;


                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
                        break;
                }
            }

            // Create new ID
            try
            {
                checked
                {
                    MaxId = parentId > 0 ? parentId : MaxId + 1;
                };
            }
            catch (OverflowException)
            {
                return null;
            }

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }

        #endregion
    }
}
