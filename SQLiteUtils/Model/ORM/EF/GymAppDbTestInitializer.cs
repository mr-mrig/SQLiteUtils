using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SQLiteUtils.Model.ORM.EF
{


    public class GymAppDbTestInitializer : CreateDatabaseIfNotExists<GymAppDbTestContext>
    {



        protected override void Seed(GymAppDbTestContext context)
        {
            base.Seed(context);

            // Initialization data
        }



    }
}
