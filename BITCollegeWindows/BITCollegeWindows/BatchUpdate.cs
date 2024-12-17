using BITCollege_TP;
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
    public partial class BatchUpdate : Form

    {
        private Batch batch = new Batch();
        private BITCollege_TPContext db = new BITCollege_TPContext();

        public BatchUpdate()
        {
            InitializeComponent();
            descriptionComboBox.Enabled = false;
        }

        /// <summary>
        /// Handles the Batch processing
        /// Further code to be added.
        /// </summary>
        private void lnkProcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(radSelect.Checked)
            {
                batch.ProcessTransmission(descriptionComboBox.SelectedValue.ToString());
                string data = batch.WriteLogData();
                rtxtLog.Text = data;
            }
            else if(radAll.Checked)
            {
                foreach(AcademicProgram academicProgram in descriptionComboBox.Items )
                {
                    batch.ProcessTransmission(academicProgram.ProgramAcronym.ToString());
                    String log =batch.WriteLogData();
                    rtxtLog.Text = log;
                }
            }
        }

        /// <summary>
        /// given:  Always open this form in top right of frame.
        /// Further code to be added.
        /// </summary>
        private void BatchUpdate_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            IQueryable<AcademicProgram> academicPrograms = db.AcademicPrograms;
            List<AcademicProgram> programList = academicPrograms.ToList();

            academicProgramBindingSource.DataSource = programList;
        }

        private void descriptionLabel_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Batch batch = new Batch();
            batch.ProcessTransmission("VT");
            rtxtLog.Text = batch.WriteLogData();
        }

        private void radSelect_CheckedChanged(object sender, EventArgs e)
        {
            if(radSelect.Checked)
            {
                descriptionComboBox.Enabled = true;
            }
            else 
            { 
                descriptionComboBox.Enabled = false; 
            }
        }
    }
}
