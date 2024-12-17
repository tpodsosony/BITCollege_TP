﻿using BITCollege_TP.Data;
using BITCollege_TP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BITCollegeWindows
{
    internal class BITCollegeService
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
            catch (Exception)
            {

            }

            return dropped;
        }

        /// <summary>
        /// Registers student for next course based off the parameters
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
            if (BusinessRules.CourseTypeLookup(courseCheck.CourseType).Equals(CourseType.MASTERY))
            {
                MasteryCourse masteryCourse = (MasteryCourse)courseCheck;
                int maximumAttempts = masteryCourse.MaximumAttempts;

                IEnumerable<Registration> completedCourses = registrations.Where(x => x.Grade != null);
                if (completedCourses.Count() >= maximumAttempts)
                {
                    returnCode = -200;
                }
            }

            //Registers student for new course if conditions are met
            if (returnCode == 0)
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
                catch (Exception) { }

                //Adds up students tuition rates
                double tuitionAmount = courseCheck.TuitionAmount;

                try
                {
                    //Change to have a call of the tuitionRateAdjusment method
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

        /// <summary>
        /// Updates a students grade on a registered course
        /// </summary>
        public double? UpdateGrade(double grade, int registrationId, String notes)
        {
            //Creates a registrations query pasted
            Registration registrations = (from Registration
                                          in db.Registrations
                                          where  Registration.RegistrationId == registrationId
                                          select Registration).Single();

            registrations.Notes = notes;
            registrations.Grade = grade;

            //Saves changes
            try
            {
                db.SaveChanges();
            }
            catch { }


            //Calls a private method
            double GPA = (double)CalculateGradePointAverage(registrations.StudentId);

            return GPA;
        }

        /// <summary>
        /// Calculates the parameters 
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        private double? CalculateGradePointAverage(int studentId)
        {
            //Variables to be used later
            double grade;
            CourseType courseType;
            double gradePoint;
            double gradePointValue;
            double totalCreditHours = 0;
            double totalGradePointValue = 0;
            double? calculatedGradePointAverage;

            //Query's registrations based off grade and ID
            IQueryable<Registration> allRegistrations = db.Registrations.Where(x => x.Grade != null && x.StudentId == studentId);

            //Loops through each registration
            foreach (Registration record in allRegistrations.ToList())
            {
                grade = (double)record.Grade;
                courseType = BusinessRules.CourseTypeLookup(record.Course.CourseType);
                //Makes sure the course is not equal to audit type before its values are checked
                if (!courseType.Equals("Audit"))
                {
                    gradePoint = BusinessRules.GradeLookup(grade, courseType);
                    gradePointValue = gradePoint * record.Course.CreditHours;
                    totalCreditHours += record.Course.CreditHours;
                    totalGradePointValue += gradePointValue;
                }

            }

            //If statement to make sure there is no division by 0 
            if (totalCreditHours == 0)
            {
                calculatedGradePointAverage = null;
            }
            else
            {
                //Caluclates the Grade Point Average
                calculatedGradePointAverage = totalGradePointValue / totalCreditHours;
            }

            //Finds the student to apply the GPA to
            Student studentGPA = (from Student
                                  in db.Students
                                  where Student.StudentId == studentId
                                  select Student).Single();
            studentGPA.GradePointAverage = calculatedGradePointAverage;

            //Saves changes to db
            try
            {
                studentGPA.ChangeState();
                db.SaveChanges();
            }
            catch (Exception) { }


            return calculatedGradePointAverage;
        }
    }
}
