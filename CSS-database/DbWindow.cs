using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace database
{
    public partial class DbInterface : Form
    {
        public DbInterface()
        {
            InitializeComponent();
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
            if (!DbInterpretter.running) Close();
            UpdateDbInfo();
        }

        private void DbInterface_Load(object sender, EventArgs e)
        {
            DbInterpretter.running = true;
            Console.Out.WriteLine("Known databases:\n" + String.Join("\n", Database.GetKnownDBs()));
            if (Debug.on)
                try
                {
                    DbInterpretter.Execute("conn testDB");
                    DbInterpretter.Execute("restore");
                }
                catch { Debug.Write("Failed debug setup"); }
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
    }
}
