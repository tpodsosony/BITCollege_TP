using BITCollege_TP.Data;
using BITCollege_TP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BITCollege_TP.Controllers;
using Utility;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CollegeRegistration" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CollegeRegistration.svc or CollegeRegistration.svc.cs at the Solution Explorer and start debugging.
    public class CollegeRegistration : ICollegeRegistration
    {
        BITCollege_TPContext db = new BITCollege_TPContext();
        public void DoWork()
        {
        }

        /// <summary>
        /// Drops the registratration in the parameters from the registration table 
        /// </summary>
        public Boolean DropCourse(int registrationId)
        {
            Boolean dropped = false;

            BITCollege_TP.Registration record = (
                    from Registrations 
                    in db.Registrations
                    where Registrations.RegistrationId == registrationId
                    select Registrations).Single();
            try
            {
                db.Registrations.Remove(record);
                db.SaveChanges();
                dropped = true;
            }
            catch(Exception)
            {
                
            }

            return dropped;
        }

        /// <summary>
        /// Sets CourseNumber to next available course number
        /// </summary>
        public int RegisterCourse(int studentId, int courseId, String notes)
        {
            //Setting return value
            int returnCode = 0;

            ///Query for later use
            IQueryable<Registration> registrations = from Registration
                                                     in db.Registrations
                                                     where Registration.StudentId == studentId && Registration.CourseId == courseId
                                                     select Registration;

            IEnumerable<Course> courses = from Course 
                          in db.Courses 
                          where Course.CourseId == courseId 
                          select Course;

            Course courseCheck = courses.FirstOrDefault();

            IEnumerable<Student> students = from Student in db.Students where Student.StudentId == studentId select Student;

            Student studentCheck = students.FirstOrDefault();

            //Incomplete Registration Check
            IEnumerable<Registration> nullCourses = registrations.Where(x => x.Grade == null);
            if (nullCourses.Count() > 0)
            {
                returnCode = -100;
            }

            //Mastery Attempt Check
            if(BusinessRules.CourseTypeLookup(courseCheck.CourseType).Equals(CourseType.MASTERY))
            {
                MasteryCourse masteryCourse = (MasteryCourse)courseCheck;
                int maximumAttempts = masteryCourse.MaximumAttempts;

                IEnumerable<Registration> completedCourses = registrations.Where(x=>x.Grade != null);
                if(completedCourses.Count() >= maximumAttempts)
                {
                    returnCode = -200;
                }
            }

            if(returnCode == 0)
            {
                Registration newRegistration = new Registration();
                newRegistration.CourseId = courseId;
                newRegistration.StudentId = studentId;
                newRegistration.Notes = notes;
                newRegistration.RegistrationDate = DateTime.Now;

                try
                {
                    db.Registrations.Add(newRegistration);
                    db.SaveChanges();
                }
                catch(Exception) { }


                double tuitionAmount = courseCheck.TuitionAmount;

                try
                {
                    studentCheck.OutstandingFees += tuitionAmount;
                    db.SaveChanges();
                }
                catch (Exception) 
                {
                    returnCode = -300;
                }
                

            }
            return returnCode;
        }

        public double? UpdateGrade(double grade, int registrationId, String notes)
        {
            Registration registrations = (from Registration
                                   in db.Registrations
                                   where Registration.Grade == grade && Registration.RegistrationId== registrationId
                                    select Registration).Single();

            registrations.Notes = notes;
            registrations.Grade = grade;

            try
            {
                db.SaveChanges();
            }
            catch { }
            

            
            double GPA = (double)CalculateGradePointAverage(registrations.StudentId);

            return GPA;
        }

        private double? CalculateGradePointAverage(int studentId)
        {
            double grade;
            CourseType courseType;
            double gradePoint;
            double gradePointValue;
            double totalCreditHours = 0;
            double totalGradePointValue = 0;
            double? calculatedGradePointAverage;

            IQueryable<Registration> allRegistrations = db.Registrations.Where(x=>x.Grade!=null && x.StudentId == studentId);

            foreach(Registration record in allRegistrations.ToList()) 
            {
                grade = (double)record.Grade;
                courseType = BusinessRules.CourseTypeLookup(record.Course.CourseType);
                if(!courseType.Equals("Audit"))
                {
                    gradePoint = BusinessRules.GradeLookup(grade, courseType);
                    gradePointValue = gradePoint * record.Course.CreditHours;
                    totalCreditHours+= record.Course.CreditHours;
                    totalGradePointValue += gradePointValue;
                }
                
            }
            if(totalCreditHours == 0)
            {
                calculatedGradePointAverage = null;
            }
            else
            {
                calculatedGradePointAverage = totalGradePointValue / totalCreditHours;
            }
            Student studentGPA = (from Student
                                               in db.Students
                                          where Student.StudentId == studentId
                                          select Student).Single();
            studentGPA.GradePointAverage = calculatedGradePointAverage;

            try
            {
                db.SaveChanges();
            }
            catch (Exception) { }
           

            return 0;
        }
    }
}
