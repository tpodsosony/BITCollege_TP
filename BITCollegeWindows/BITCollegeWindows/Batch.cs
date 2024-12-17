using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Data.Entity;
using BITCollege_TP.Data;
using BITCollege_TP;
using BITCollege_TP.Controllers;
using Utility;

namespace BITCollegeWindows
{
    /// <summary>
    /// Batch:  This class provides functionality that will validate
    /// and process incoming xml files.
    /// </summary>
    public class Batch
    {
        //Instance variables
        private String inputFileName;
        private String logFileName;
        private String logData;

        private XDocument xDocument;
        private DateTime curDate = DateTime.Today;
        BITCollege_TPContext db = new BITCollege_TPContext();

        /// <summary>
        /// Processes errors and adds data to logData
        /// </summary>
        /// <param name="beforeQuery"></param>
        /// <param name="afterQuery"></param>
        /// <param name="message"></param>
        private void ProcessErrors(IEnumerable<XElement> beforeQuery, IEnumerable<XElement> afterQuery, String message)
        {
            IEnumerable<XElement> failed = beforeQuery.Except(afterQuery);
            foreach(XElement error in failed) 
            {
                logData += $"\n\rFile: {inputFileName}";
                logData += $"\n\rProgram: {error.Element("program")}";
                logData += $"\n\rStudent Number: {error.Element("student_no")}";
                logData += $"\n\rCourse Number: {error.Element("course_no")}";
                logData += $"\n\rRegistration Number: {error.Element("registration_no")}";
                logData += $"\n\rType: {error.Element("type")}";
                logData += $"\n\rGrade: {error.Element("grade")}";
                logData += $"\n\rNotes: {error.Element("notes")}";
                logData += $"\n\rNodes: {error.Nodes().Count()}";
                logData += $"\n\rMessage: {message}";
            }
        }

        /// <summary>
        /// Process the header of an xml files
        /// Requires the header to meet the standard of 3 values, having an associated programAcronym in the DB, and being of the current date
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ProcessHeader()
        {
            xDocument = XDocument.Load(inputFileName);
            XElement root = xDocument.Element("student_update");

            IEnumerable<XAttribute> attributeList = root.Attributes();
            int numberOfAttributes = attributeList.Count();

            String xmlDate = root.Attribute("date").Value;
            Console.WriteLine(xmlDate);
            String actualDate = curDate.ToString("yyyy-MM-dd");

            //Checks number of attributes
            if (numberOfAttributes == 3) 
            {
                //Compares the xml date to current day date
                if(!xmlDate.Equals(actualDate))
                {
                    throw new Exception("The date is not set to the current day");
                }

                String programAcronym = root.Attribute("program").Value;
                AcademicProgram matchingProgram = db.AcademicPrograms.FirstOrDefault(x => x.ProgramAcronym == programAcronym);

                //Confirms that a matching program exists
                if(matchingProgram == null)
                {
                    throw new Exception("The program acronym does not match with any in the database");
                }

                IEnumerable<XElement> transactions = xDocument.Descendants("transaction");
                IEnumerable<XElement> studentNum = transactions.Elements("student_no");
                int student_noCount = 0;
                //Adding up student numbers in xml file
                foreach(XElement num in studentNum)
                {
                    student_noCount += Int32.Parse(num.Value);
                }

                //confirmes total of student numbers is equal to checksum value
                if (student_noCount != Int32.Parse(root.Attribute("checksum").Value))
                {
                    throw new Exception("The check sum value is incorrect.");
                }


            }
            else
            {
                throw new Exception("The number of attributes is incorrect");
            }
        }

        /// <summary>
        /// Checks for errors inside the xml file
        /// </summary>
        private void ProcessDetails()
        {
            //Gets all transactions
            xDocument = XDocument.Load(inputFileName);
            IEnumerable<XElement> totalTransactions =
            xDocument.Descendants().Where(x => x.Name == "transaction");

            //Confirms transactions have seven child nodes
            IEnumerable<XElement> sevenChild =
            totalTransactions.Where(x => x.Elements().Nodes().Count() == 7);
            ProcessErrors(totalTransactions, sevenChild, "The transaction did not have 7 child nodes.");

            //Confirms transactiosn match with root program
            IEnumerable<XElement> programs =
            sevenChild.Where(x => x.Element("program").Value == xDocument.Root.Attribute("program").Value);
            ProcessErrors(sevenChild, programs, "The program did not match the one in the root element.");

            //Makes sure type is a numeric value
            IEnumerable<XElement> types =
            programs.Where(x => Numeric.IsNumeric(x.Element("type").Value, System.Globalization.NumberStyles.Number));
            ProcessErrors(programs, types, "The type value was not numeric");

            //Confirms grade value is numeric
            IEnumerable<XElement> grades =
            types.Where(x => Numeric.IsNumeric(x.Element("grade").Value, System.Globalization.NumberStyles.Number)
            || x.Element("grade").Value.Equals("*"));
            ProcessErrors(types, grades, "The grade value was not numeric or a *");

            //Confirms type is either 1 or 2
            IEnumerable<XElement> typeNum =
            grades.Where(x => Int32.Parse(x.Element("type").Value) == 1 || Int32.Parse(x.Element("type").Value) == 2);
            ProcessErrors(grades, typeNum, "Type is not equal to one or two.");

            //Confirms gradeType is correct and within range of 0 to 100
            IEnumerable<XElement> gradeTypes = 
            typeNum.Where(x => (Int32.Parse(x.Element("type").Value) == 1 && x.Element("grade").Value.Equals("*"))
            || (Int32.Parse(x.Element("type").Value) == 2 && Double.Parse(x.Element("grade").Value) >= 0
            && Double.Parse(x.Element("grade").Value) <= 100));
            ProcessErrors(typeNum, gradeTypes, "The grade associated with the type is incorrect.");

            //Confirms student number exists in database
            IEnumerable<long> studentNum = db.Students.Select(x => x.StudentNumber).ToList();
            IEnumerable<XElement> studentNumExists =
            gradeTypes.Where(x => studentNum.Contains(long.Parse(x.Element("student_no").Value)));
            ProcessErrors(gradeTypes, studentNumExists, "The student number does not exist in the database");

            //Confirms course number exists in database
            IEnumerable<string> courseNum = db.Courses.Select(x => x.CourseNumber).ToList();
            IEnumerable<XElement> courseNumVerification =
            studentNumExists.Where(x => (x.Element("type").Value.Equals("1") && courseNum.Contains(x.Element("course_no").Value))
            || (x.Element("type").Value.Equals("1") && (x.Element("course_no").Value.Equals("*"))));
            ProcessErrors(studentNumExists, courseNumVerification, "The course number does not exist in the database");

            //Confirms registration number exists in database
            IEnumerable<long> registrationNum = db.Registrations.Select(x => x.RegistrationNumber).ToList();
            IEnumerable<XElement> regVerification =
            courseNumVerification.Where(x => (x.Element("type").Value.Equals("2") && registrationNum.Contains(long.Parse(x.Element("registration_no").Value)))
            || (x.Element("type").Value.Equals("1") && x.Element("registration_no").Value.Equals("*")));
            ProcessErrors(courseNumVerification, regVerification, "The registration number does not exist");

            ProcessTransactions(regVerification);

        }

        /// <summary>
        /// Process transactions and appends to logData
        /// </summary>
        /// <param name="transactionRecords"></param>
        private void ProcessTransactions(IEnumerable<XElement> transactionRecords)
        {
            foreach(XElement transactionRecord in transactionRecords) 
            {
                int studentNum = Int32.Parse(transactionRecord.Element("student_no").Value);
                String courseNum = transactionRecord.Element("course_no").Value;

                int studentID = db.Students.Where(x => x.StudentNumber == studentNum)
                                           .Select(x => x.StudentId)
                                           .FirstOrDefault();

                int courseID = db.Courses.Where(x => x.CourseNumber == courseNum)
                                           .Select(x => x.CourseId)
                                           .FirstOrDefault();

                String notes = transactionRecord.Element("notes").Value;
                String type = transactionRecord.Element("type").Value;
                BITCollegeService registration = new BITCollegeService();

                if (type.Equals("1"))
                {
                    int errorCode = registration.RegisterCourse(studentID, courseID, notes);
                    if (errorCode == 0) 
                    {
                        logData += $"\n\rStudent: {studentID} has successfully registered for course: {courseID} ";
                    }
                    else
                    {
                        logData += $"\n\rREGISTRATION ERROR: {BusinessRules.RegisterError(errorCode)}"; 
                    }
                }
                else if(type.Equals("2")) 
                {
                    double grade = double.Parse(transactionRecord.Element("grade").Value)/100;
                    long registrationNum = long.Parse(transactionRecord.Element("registration_no").Value);

                    int registrationID = db.Registrations.Where(x => x.RegistrationNumber == registrationNum)
                                           .Select(x => x.CourseId)
                                           .FirstOrDefault();

                    try
                    {
                        double? GPA = registration.UpdateGrade(grade, registrationID, notes);
                        if (GPA.HasValue)
                        {
                            logData += $"\r\nA grade of {grade * 100} has been successfully applied to registration : {registrationNum}";
                        }
                        else
                        {
                            logData += $"\r\nGrade update failed for registraion : {registrationID}";
                        }
                    }
                    catch (Exception ex) 
                    {
                        logData += $"\r\nGrade update failed for registration : {registrationNum}";
                    }
                }
            }
        }

        /// <summary>
        /// Writes a new file based on logData and using the value of logFileName as its name
        /// </summary>
        /// <returns></returns>
        public String WriteLogData()
        {
            StreamWriter streamWriter = new StreamWriter(logFileName);

            streamWriter.Write(logData);
            streamWriter.Close();
            String fileData = logData;
            logData = string.Empty;
            logFileName = string.Empty;
            return fileData;
        }

        /// <summary>
        /// Confirms that a file exists based on current date and programAcronym
        /// </summary>
        /// <param name="programAcronym"></param>
        public void ProcessTransmission(String programAcronym)
        {
            //Creating file name for day
            String year = curDate.ToString("yyyy");
            String day = curDate.DayOfYear.ToString("000");
            inputFileName = $"{year}-{day}-{programAcronym}.xml";

            logFileName = $"LOG {year}-{day}-{programAcronym}.txt";

            if (File.Exists(inputFileName))
            {
                try
                {
                    ProcessHeader();
                    ProcessDetails();
                    //begin processing file
                }
                catch (Exception e)
                {
                    logData += $"\n\r{e.Message}" ;
                }

            }
            else
            {
                logData += $"File {inputFileName} does not exist.";
            }
        }
    }
}
