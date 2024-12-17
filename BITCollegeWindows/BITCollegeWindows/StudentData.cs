using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BITCollege_TP.Data;
using BITCollege_TP;

namespace BITCollegeWindows
{
    public partial class StudentData : Form
    {

        BITCollege_TPContext db = new BITCollege_TPContext();


        ///Given: Student and Registration data will be retrieved
        ///in this form and passed throughout application
        ///These variables will be used to store the current
        ///Student and selected Registration
        ConstructorData constructorData = new ConstructorData();

        /// <summary>
        /// This constructor will be used when this form is opened from
        /// the MDI Frame.
        /// </summary>
        public StudentData()
        {
            InitializeComponent();
            studentNumberMaskedTextBox.Leave += StudentNumberMaskedTextBox_Leave;
            //lnkUpdateGrade.LinkClicked += lnkUpdateGrade_LinkClicked;
            //lnkViewDetails.LinkClicked += lnkViewDetails_LinkClicked;
        }

        private void StudentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            long studentNum = 0;
            long.TryParse(studentNumberMaskedTextBox.Text, out studentNum);
            Student temp = db.Students.Where(x => x.StudentNumber == studentNum).SingleOrDefault();
            if (temp !=null)
            {
                studentBindingSource.DataSource = temp;
                IQueryable<Registration> registration = db.Registrations.Where(x => x.StudentId == temp.StudentId);
                if(registration.Any())
                {
                    registrationBindingSource.DataSource = registration.ToList();
                    lnkUpdateGrade.Enabled = true;
                    lnkViewDetails.Enabled = true;
                }
                else
                {
                    registrationBindingSource.DataSource = typeof(Registration);
                    lnkUpdateGrade.Enabled = false;
                    lnkViewDetails.Enabled = false;

                }

            }
            else {
                lnkUpdateGrade.Enabled = false;
                lnkViewDetails.Enabled = false;
                studentBindingSource.DataSource = typeof(Student);
                registrationBindingSource.DataSource = typeof(Registration);

                MessageBox.Show($"Student {studentNum} does not exist.", "Invalid Student Number", MessageBoxButtons.OK);

            }
            if(constructorData.Registration!=null)
            {
                registrationNumberComboBox.Text = constructorData.Registration.RegistrationNumber.ToString(); ;
            }


        }

        /// <summary>
        /// given:  This constructor will be used when returning to StudentData
        /// from another form.  This constructor will pass back
        /// specific information about the student and registration
        /// based on activites taking place in another form.
        /// </summary>
        /// <param name="constructorData">constructorData object containing
        /// specific student and registration data.</param>
        public StudentData (ConstructorData constructor)
        {
            InitializeComponent();
            constructorData = constructor;
            studentNumberMaskedTextBox.Text = constructor.Student.StudentNumber.ToString();
            StudentNumberMaskedTextBox_Leave(null, null);
        }

        

        /// <summary>
        /// given: Open grading form passing constructor data.
        /// </summary>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            populateConstructorData();
            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }


        /// <summary>
        /// given: Open history form passing constructor data.
        /// </summary>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            populateConstructorData();
            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Opens the form in top right corner of the frame.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            //keeps location of form static when opened and closed
            this.Location = new Point(0, 0);
        }

        private void populateConstructorData()
        {
            constructorData.Student = (Student)studentBindingSource.Current;
            constructorData.Registration = (Registration)registrationBindingSource.Current;
        }
    }
}
