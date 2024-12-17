﻿using BITCollege_TP;
using BITCollege_TP.Controllers;
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
    public partial class History : Form
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
        public History(ConstructorData constructorData)
        {
            InitializeComponent();
            this.constructorData = constructorData;
            studentNumberMaskedTextBox.Text = constructorData.Student.StudentNumber.ToString();
            fullNameLabel1.Text = constructorData.Student.FullName;
            descriptionLabel1.Text = constructorData.Student.AcademicProgram.Description;
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
        /// given:  Open this form in top right corner of the frame.
        /// further code required:
        /// </summary>
        private void History_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            try
            {
                var query = (from Registrations in db.Registrations
                             join Courses in db.Courses on Registrations.CourseId equals Courses.CourseId
                             where Registrations.StudentId == constructorData.Student.StudentId
                             select new
                             {
                                 Registrations.RegistrationNumber,
                                 Registrations.RegistrationDate,
                                 Course = Courses.Title,
                                 Registrations.Grade,
                                 Registrations.Notes,
                                 Registrations.StudentId

                             }).ToList();
                registrationBindingSource.DataSource = query;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
