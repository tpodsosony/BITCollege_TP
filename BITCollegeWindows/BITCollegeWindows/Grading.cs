using BITCollege_TP.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class Grading : Form
    {
        BITCollege_TPContext db = new BITCollege_TPContext();

        ///given:  student and registration data will passed throughout 
        ///application. This object will be used to store the current
        ///student and selected registration
        ConstructorData constructorData;


        /// <summary>
        /// given:  This constructor will be used when called from the
        /// Student form.  This constructor will receive 
        /// specific information about the student and registration
        /// further code required:  
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public Grading(ConstructorData constructor)
        {
            InitializeComponent();
            constructorData = constructor;

            //Initial form setup
            studentNumberMaskedLabel.Text = constructorData.Student.StudentNumber.ToString();
            fullNameLabel1.Text = constructorData.Student.FullName;
            descriptionLabel1.Text = constructorData.Student.AcademicProgram.Description;

            courseNumberMaskedLabel.Text = constructorData.Registration.Course.CourseNumber;
            titleLabel1.Text = constructorData.Registration.Course.Title;
            courseTypeLabel1.Text = constructorData.Registration.Course.CourseType;

            //Makes sure grade follows proper format
            if (constructor.Registration.Grade != null)
            {
                //gradeTextBox.Text = $"{constructorData.Registration.Grade:P2}";
            }

            
        }

        /// <summary>
        /// given: This code will navigate back to the Student form with
        /// the specific student and registration data that launched
        /// this form.
        /// </summary>
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //return to student with the data selected for this form
            StudentData student = new StudentData(constructorData);
            student.MdiParent = this.MdiParent;
            student.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Always open in this form in the top right corner of the frame.
        /// further code required:
        /// </summary>
        private void Grading_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            courseNumberMaskedLabel.Mask = Utility.BusinessRules.CourseFormat(constructorData.Registration.Course.CourseNumber);

            if(!constructorData.Registration.Grade.HasValue)
            {
                gradeTextBox.Enabled = true;
                lnkUpdate.Enabled = true;
                lblExisting.Visible = false;
            }
            else
            {
                gradeTextBox.Enabled = false;
                lnkUpdate.Enabled = false;
                lblExisting.Visible = true;
            }
        }

        /// <summary>
        /// Handles the logic for updating a student grade
        /// </summary>
        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Makes sure the string is available to convert to a double
            String notFormatted = Utility.Numeric.ClearFormatting(gradeTextBox.Text, "%");

            //Updates students grade
            if(Utility.Numeric.IsNumeric(notFormatted, System.Globalization.NumberStyles.Any))
            {
                double dividedGrade = Convert.ToDouble(gradeTextBox.Text) / 100;
                if(dividedGrade >= 0 && dividedGrade <= 1)
                {
                    BITCollegeService temp = new BITCollegeService();
                    double? newGPA = temp.UpdateGrade(dividedGrade *100, constructorData.Registration.RegistrationId, "");
                }
                else
                {
                    MessageBox.Show("Invalid range. Grade must be entered as a decimal value between 0 and 1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                gradeTextBox.Enabled = false;
            }
            else
            {
                MessageBox.Show("Invalid grade. Please enter a valid numeric value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
