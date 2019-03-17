using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.Initializator
{
    static class DietDayTypeStatic
    {


        public static readonly List<List<DatabaseColumnWrapper>> ReservedEntries = new List<List<DatabaseColumnWrapper>>()
        {
            new List<DatabaseColumnWrapper>()
            {
                new DatabaseColumnWrapper()
                {
                    Name = "Name",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "On",
                },

                new DatabaseColumnWrapper()
                {
                    Name = "Description",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Training day",
                },
            },

            new List<DatabaseColumnWrapper>()
            {
                new DatabaseColumnWrapper()
                {
                    Name = "Name",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Off",
                },

                new DatabaseColumnWrapper()
                {
                    Name = "Description",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Rest day",
                },
            },

            new List<DatabaseColumnWrapper>()
            {
                new DatabaseColumnWrapper()
                {
                    Name = "Name",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Refeed",
                },

                new DatabaseColumnWrapper()
                {
                    Name = "Description",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Caloric refeed day",
                },
            },

            new List<DatabaseColumnWrapper>()
            {
                new DatabaseColumnWrapper()
                {
                    Name = "Name",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Fasting",
                },

                new DatabaseColumnWrapper()
                {
                    Name = "Description",
                    Affinity = System.Data.SQLite.TypeAffinity.Text,
                    Value = "Fasting or very low calories day",
                },
            },

        };



        #region Properties
        public static int GetMaxId()
        {
            return ReservedEntries.Count;
        }
        #endregion

    }
}
