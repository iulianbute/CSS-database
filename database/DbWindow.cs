using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace database
{
    [ExcludeFromCodeCoverage]
    public partial class DbInterface : Form
    {
        public DbInterface(string[] init)
        {
            InitializeComponent();
            foreach(string comm in init)
            {
                CommandInput.Text = comm;
                Execute_Click(this, null);
                CommandInput.Clear();
            }
        }

        private void UpdateDbInfo()
        {
            dbInfo.Clear();
            dbInfo.Text += "Databases:\n" + String.Join("\n", Database.GetKnownDBs()) + "\n\n";
            dbInfo.Text += "Current DB:\n" + DbInterpretter.currentDb.name + "\n\n";
            dbInfo.Text += "Tables:\n" + String.Join("\n", DbInterpretter.currentDb.Keys);
        }

        private void Execute_Click(object sender, EventArgs e)
        {
            CommandOutput.AppendText("\n>>>" + CommandInput.Text + "\n");
            CommandOutput.AppendText(DbInterpretter.Execute(CommandInput.Text) + '\n');
            CommandInput.Clear();
            if (!DbInterpretter.isRunning) Close();
            UpdateDbInfo();
        }

        private void DbInterface_Load(object sender, EventArgs e)
        {
            UpdateDbInfo();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            CommandOutput.Clear();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveLogFileDialog = new SaveFileDialog()
            {
                FileName = "dbLog.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveLogFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveLogFileDialog.FileName))
                    sw.WriteLine(CommandOutput.Text);
            }
        }

        private void dbInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void CommandOutput_TextChanged(object sender, EventArgs e)
        {
            CommandOutput.ScrollToCaret();
        }
    }
}
