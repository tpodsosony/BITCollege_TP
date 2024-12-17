/*
 * Name: Tal Podsosony
 * Date: anuary 12, 2024
 * Assignment 1
 * The model component of the MVC pattern
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web.ModelBinding;
using System.Web.Mvc;
using Utility;
using BITCollege_TP.Data;

namespace BITCollege_TP
{
    /// <summary>
    /// Student Model - to represent Student table in database
    /// </summary>
    public class Student
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Student\nNumber")]
        //Double Check this
        public long StudentNumber { get; set; }

        [Required]
        [Display(Name = "First\nName")]
        public String FirstName { get; set; }

        [Required]
        [Display(Name = "Last\nName")]
        public String LastName { get; set; }

        [Required]
        public String Address { get; set; }

        [Required]
        public String City { get; set; }

        //Double check this
        [Required(ErrorMessage = "Must be a valid province")]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)")]

        public String Province { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Grade Point\nAverage")]
        [DisplayFormat(DataFormatString = "{0:F}")]
        [Range(0, 4.50)]
        public double? GradePointAverage { get; set; }

        [Required]
        [Display(Name = "Fees")]
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:c}")]
        public double OutstandingFees { get; set; }

        public String Notes { get; set; }

        [Required]
        [Display(Name = "Name")]
        public String FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public String FullAddress
        {
            get
            {
                return $"{Address} {City} {Province}";
            }

        }

        private BITCollege_TPContext db = new BITCollege_TPContext();

        /// <summary>
        /// Chnages the student GradePointStateId based on their GradePointAverage
        /// </summary>
        public void ChangeState()
        {
            GradePointState before = db.GradePointStates.Find(GradePointStateId);

            int after = 0;
            while (before.GradePointStateId != after)
            {
                before.StateChangeCheck(this);
                after = before.GradePointStateId;
                before = db.GradePointStates.Find(GradePointStateId);
            }

        }
        /// <summary>
        /// Sets StudentNumber to the next available number
        /// </summary>
        public void SetNextSudentNumber()
        {
            StudentNumber = (long)StoredProcedures.NextNumber("NextStudent");
        }

        //navigational properties
        public virtual GradePointState GradePointState { get; set; }
        public virtual AcademicProgram AcademicProgram { get; set; }
        public virtual ICollection<Registration> Registration { get; set; }
    }

    /// <summary>
    /// AcademicProgram Model - to represent AcademicPrograms table in database
    /// </summary>
    public class AcademicProgram
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name = "Program")]
        public String ProgramAcronym { get; set; }
        [Required]
        [Display(Name = "Program\nName")]
        public String Description { get; set; }

        //navigational properties
        public virtual ICollection<Course> Course { get; set; }
        public virtual ICollection<Student> Student { get; set; }
    }

    /// <summary>
    /// GradePointState Model - to represent GradePointStates table in database
    /// </summary>
    public abstract class GradePointState
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GradePointStateId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:F}")]
        [Display(Name = "Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:F}")]
        [Display(Name = "Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:F}")]
        [Display(Name = "Tuition Rate\nFactor")]
        public Double TuitionRateFactor { get; set; }

        [Display(Name = "State")]
        public String Description
        {
            get
            {
                return BusinessRules.ParseString(GetType().Name, "State");
            }
        }

        protected static BITCollege_TPContext db = new BITCollege_TPContext();

        public virtual double TuitionRateAdjustment(Student student)
        {

            return 1.0;
        }

        public virtual void StateChangeCheck(Student student)
        {

        }

        //navigational properties
        public virtual ICollection<Student> Student { get; set; }

    }

    /// <summary>
    /// SuspendState Model - to represent SuspendStates table in database
    /// </summary>
    public class SuspendState : GradePointState
    {
        private static SuspendState suspendState;

        /// <summary>
        /// Constructor implamention Singleton design
        /// </summary>
        private SuspendState()
        {
            UpperLimit = 1.00;
            LowerLimit = 0.00;
            TuitionRateFactor = 1.1;
        }
        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of SuspendedState returned
        /// </summary>
        /// <returns></returns>
        public static SuspendState GetInstance()
        {

            if (suspendState == null)
            {
                suspendState = db.SuspendStates.FirstOrDefault();


                if (suspendState == null)
                {
                    SuspendState temp = new SuspendState();
                    db.SuspendStates.Add(temp);
                    db.SaveChanges();
                }
            }
            return suspendState;
        }

        /// <summary>
        /// Changes the state of a Students GradePointStateId based on the constraints
        /// of the constructor
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            ProbationState temp = ProbationState.GetInstance();
            if (student.GradePointAverage > this.UpperLimit)
                student.GradePointStateId = temp.GradePointStateId;
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the adjusted value of a Students TuitionRate based on certain conditions
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double temp = TuitionRateFactor;
            if (student.GradePointAverage < 0.75 && student.GradePointAverage >= 0.5)
            {
                temp += 0.02;
            }
            else if (student.GradePointAverage < 0.5)
            {
                temp += 0.05;
            }
            return temp;
        }
    }

    /// <summary>
    /// ProbationState Model - to represent ProbationStates table in database
    /// </summary>
    public class ProbationState : GradePointState
    {
        private static ProbationState probationState;

        /// <summary>
        /// Constructor
        /// </summary>
        private ProbationState()
        {
            LowerLimit = 1.00;
            UpperLimit = 2.00;
            TuitionRateFactor = 1.075;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of ProbationState returned
        /// </summary>
        /// <returns></returns>
        public static ProbationState GetInstance()
        {

            if (probationState == null)
            {
                probationState = db.ProbationStates.FirstOrDefault();
                if (probationState == null)
                {
                    ProbationState temp = new ProbationState();
                    db.ProbationStates.Add(temp);
                    db.SaveChanges();
                }
            }

            return probationState;
        }

        /// <summary>
        /// Changes the state of a Students GradePointStateId based on the constraints
        /// of the constructor
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > this.UpperLimit)
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;

            if (student.GradePointStateId < this.LowerLimit)
                student.GradePointStateId = SuspendState.GetInstance().GradePointStateId;
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the adjusted value of a Students TuitionRate based on certain conditions
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double temp = TuitionRateFactor;
            int count = 0;
            foreach (Registration reg in student.Registration)
            {
                if (reg.Grade != null)
                    count++;
            }
            if (count >= 5)
            {
                temp -= 0.04;
            }
            return temp;
        }
    }

    /// <summary>
    /// RegularState Model - to represent RegularStats table in database
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState regularState;

        /// <summary>
        /// Private construcotr for Singleton use
        /// </summary>
        private RegularState()
        {
            LowerLimit = 2.00;
            UpperLimit = 3.70;
            TuitionRateFactor = 1.0;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of RegularState returned
        /// </summary>
        /// <returns></returns>
        public static RegularState GetInstance()
        {

            if (regularState == null)
            {
                regularState = db.RegularStates.FirstOrDefault();

                if (regularState == null)
                {
                    RegularState temp = new RegularState();
                    db.RegularStates.Add(temp);
                    db.SaveChanges();
                }
            }

            return regularState;
        }

        /// <summary>
        /// Changes the state of a Students GradePointStateId based on the constraints
        /// of the constructor
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > this.UpperLimit)
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;

            if (student.GradePointAverage < this.LowerLimit)
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            db.SaveChanges();
        }
    }

    /// <summary>
    /// HonoursState Model - to represent HonoursStates table in database
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState honoursState;

        /// <summary>
        /// Constructor
        /// </summary>
        private HonoursState()
        {
            LowerLimit = 3.70;
            UpperLimit = 4.50;
            TuitionRateFactor = 0.9;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of HonoursState returned
        /// </summary>
        /// <returns></returns>
        public static HonoursState GetInstance()
        {


            if (honoursState == null)
            {
                honoursState = db.HonoursStates.FirstOrDefault();


                if (honoursState == null)
                {
                    HonoursState temp = new HonoursState();
                    db.HonoursStates.Add(temp);
                }
            }
            db.SaveChanges();
            return honoursState;
        }

        /// <summary>
        /// Changes the state of a Students GradePointStateId based on the constraints
        /// of the constructor
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {

            if (student.GradePointAverage < this.LowerLimit)
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the adjusted value of a Students TuitionRate based on certain conditions
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double temp = TuitionRateFactor;
            int count = 0;
            foreach (Registration reg in student.Registration)
            {
                if (reg.Grade != null)
                    count++;
            }
            if (count >= 5)
            {
                temp -= 0.05;
            }
            if (student.GradePointAverage > 4.25)
            {
                temp -= 0.02;
            }
            return temp;
        }

    }

    /// <summary>
    /// Course Model - to represent Courses table in database
    /// </summary>
    public abstract class Course
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Course\nNumber")]
        public String CourseNumber { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:F}")]
        [Display(Name = "Credit\nHours")]
        public double CreditHours { get; set; }

        [Required]
        [Display(Name = "Tuition")]
        [DisplayFormat(DataFormatString = "${0:F}")]
        public double TuitionAmount { get; set; }

        [Display(Name = "Course\nType")]
        public String CourseType
        {
            get
            {
                return BusinessRules.ParseString(GetType().Name, "Course");
            }
        }
        public String Notes { get; set; }

        //navigational properties
        public virtual AcademicProgram AcademicProgram { get; set; }
        public virtual ICollection<Registration> Registration { get; set; }

        /// <summary>
        /// Sets CourseNumber to next available course number
        /// </summary>
        public virtual void SetNextCourseNumber() { }
    }

    /// <summary>
    ///GradedCourse Model - to represent GradedCourses table in database
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "Assignments")]
        public double AssignmentWeight { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "Exams")]
        public double ExamWeight { get; set; }

        /// <summary>
        /// Sets CourseNumber to next available course number
        /// </summary>
        public override void SetNextCourseNumber() 
        {
            CourseNumber = "G-" + StoredProcedures.NextNumber("NextGradedCourse");
        }
    }

    public class AuditCourse : Course 
    {
        /// <summary>
        /// Sets CourseNumber to next available course number
        /// </summary>
        public override void SetNextCourseNumber()
        {
            CourseNumber = "A-" + StoredProcedures.NextNumber("NextAuditCourse");
        }
    }

    /// <summary>
    ///MasteryCourse Model - to represent MasteryCourses table in database
    /// </summary>
    public class MasteryCourse : Course
    {
        [Required]
        [Display(Name = "Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }

        /// <summary>
        /// Sets CourseNumber to next available course number
        /// </summary>
        public override void SetNextCourseNumber()
        {
            CourseNumber = "M-" + StoredProcedures.NextNumber("NextMasteryCourse");
        }
    }

    /// <summary>
    /// Registration Model - to represent Registrations table in database
    /// </summary>
    public class Registration
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Display(Name = "Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Date")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText = "Ungraded")]
        [Range(0, 1)]
        public double? Grade { get; set; }

        public String Notes { get; set; }

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }

        /// <summary>
        /// Sets RegistrationNumber to next available registration number
        /// </summary>
        public void SetNextRegistrationNumber()
        {
            RegistrationNumber = (long)StoredProcedures.NextNumber("NextRegistration");
        }
    }

    /// <summary>
    /// NextUniqueNumber model - to represent NextUniqueNumber table in database
    /// </summary>
    public abstract class NextUniqueNumber
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NextUniqueNumberId { get; set; }

        [Required]
        public long NextAvailableNumber { get; set; }

        protected static BITCollege_TPContext db = new BITCollege_TPContext();
    }

    /// <summary>
    /// 
    /// </summary>
    public class NextStudent : NextUniqueNumber
    {
        private static NextStudent nextStudent;

        private const int NEXT_AVAILABLE_NUM = 20000000;

        /// <summary>
        /// Constructor
        /// </summary>
        private NextStudent()
        {
            NextAvailableNumber = NEXT_AVAILABLE_NUM;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of NextStudent returned
        /// </summary>
        /// <returns></returns>
        public static NextStudent GetInstance()
        {
            if (nextStudent == null)
            {
                nextStudent = db.NextStudents.FirstOrDefault();


                if (nextStudent == null)
                {
                    NextStudent temp = new NextStudent();
                    db.NextStudents.Add(temp);
                }
            }
            db.SaveChanges();
            return nextStudent;
        }

    }

    /// <summary>
    /// NextRegistration model - to represent NextRegistration table in database
    /// </summary>
    public class NextRegistration : NextUniqueNumber
    {
        private static NextRegistration nextRegistration;

        private const int NEXT_AVAILABLE_NUM = 700;

        /// <summary>
        /// Constructor
        /// </summary>
        private NextRegistration()
        {
            NextAvailableNumber = NEXT_AVAILABLE_NUM;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of NextRegistration returned
        /// </summary>
        /// <returns></returns>
        public static NextRegistration GetInstance()
        {
            if (nextRegistration == null)
            {
                nextRegistration = db.NextRegistrations.FirstOrDefault();


                if (nextRegistration == null)
                {
                    NextRegistration temp = new NextRegistration();
                    db.NextRegistrations.Add(temp);
                }
            }
            db.SaveChanges();
            return nextRegistration;
        }

    }

    /// <summary>
    /// NextGradedCourse model - to represent NextGradedCourse table in database
    /// </summary>
    public class NextGradedCourse : NextUniqueNumber
    {
        private static NextGradedCourse nextGradedCourse;

        private const int NEXT_AVAILABLE_NUM = 200000;

        /// <summary>
        /// Constructor
        /// </summary>
        private NextGradedCourse()
        {
            NextAvailableNumber = NEXT_AVAILABLE_NUM;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of NextGradedCourse returned
        /// </summary>
        /// <returns></returns>
        public static NextGradedCourse GetInstance()
        {
            if (nextGradedCourse == null)
            {
                nextGradedCourse = db.NextGradedCourses.FirstOrDefault();


                if (nextGradedCourse == null)
                {
                    NextGradedCourse temp = new NextGradedCourse();
                    db.NextGradedCourses.Add(temp);
                }
            }
            db.SaveChanges();
            return nextGradedCourse;
        }

    }

    /// <summary>
    /// NextAuditCourse model - to represent NextAuditCourse table in database
    /// </summary>
    public class NextAuditCourse : NextUniqueNumber
    {
        private static NextAuditCourse nextAuditCourse;

        private const int NEXT_AVAILABLE_NUM = 2000;
        /// <summary>
        /// Constructor
        /// </summary>
        private NextAuditCourse()
        {
            NextAvailableNumber = NEXT_AVAILABLE_NUM;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of NextAuditCourse returned
        /// </summary>
        /// <returns></returns>
        public static NextAuditCourse GetInstance()
        {
            if (nextAuditCourse == null)
            {
                nextAuditCourse = db.NextAuditCourses.FirstOrDefault();


                if (nextAuditCourse == null)
                {
                    NextAuditCourse temp = new NextAuditCourse();
                    db.NextAuditCourses.Add(temp);
                }
            }
            db.SaveChanges();
            return nextAuditCourse;
        }

    }

    /// <summary>
    /// NextMasteryCourse model - to represent NextMasteryCourse table in database
    /// </summary>
    public class NextMasteryCourse : NextUniqueNumber
    {
        private static NextMasteryCourse nextMasteryCourse;

        private const int NEXT_AVAILABLE_NUM = 20000;


        /// <summary>
        /// Constructor
        /// </summary>
        private NextMasteryCourse()
        {
            NextAvailableNumber = NEXT_AVAILABLE_NUM;
        }

        /// <summary>
        /// Implaments the Singleton design to ensure there is only one instance of NextMasteryCourse returned
        /// </summary>
        /// <returns></returns>
        public static NextMasteryCourse GetInstance()
        {
            if (nextMasteryCourse == null)
            {
                nextMasteryCourse = db.NextMasteryCourses.FirstOrDefault();


                if (nextMasteryCourse == null)
                {
                    NextMasteryCourse temp = new NextMasteryCourse();
                    db.NextMasteryCourses.Add(temp);
                }
            }
            db.SaveChanges();
            return nextMasteryCourse;
        }

    }

  
}