using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_TP.Data
{
    public class BITCollege_TPContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BITCollege_TPContext() : base("name=BITCollege_TPContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_TP.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.SuspendState> SuspendStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.HonoursState> HonoursStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.AuditCourse> AuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.MasteryCourse> MasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.NextUniqueNumber> NextUniqueNumbers { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.NextGradedCourse> NextGradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.NextMasteryCourse> NextMasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_TP.NextStudent> NextStudents { get; set; }
    }
}
