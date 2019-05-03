namespace database
{
    partial class DbInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CommandInput = new System.Windows.Forms.TextBox();
            this.Execute = new System.Windows.Forms.Button();
            this.CommandOutput = new System.Windows.Forms.RichTextBox();
            this.Save = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.dbInfo = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // CommandInput
            // 
            this.CommandInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommandInput.Location = new System.Drawing.Point(12, 12);
            this.CommandInput.Name = "CommandInput";
            this.CommandInput.Size = new System.Drawing.Size(435, 23);
            this.CommandInput.TabIndex = 0;
            // 
            // Execute
            // 
            this.Execute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Execute.Location = new System.Drawing.Point(453, 12);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(142, 23);
            this.Execute.TabIndex = 1;
            this.Execute.Text = "Execute";
            this.Execute.UseVisualStyleBackColor = true;
            this.Execute.Click += new System.EventHandler(this.Execute_Click);
            // 
            // CommandOutput
            // 
            this.CommandOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommandOutput.Location = new System.Drawing.Point(12, 41);
            this.CommandOutput.Name = "CommandOutput";
            this.CommandOutput.Size = new System.Drawing.Size(435, 340);
            this.CommandOutput.TabIndex = 2;
            this.CommandOutput.Text = "";
            this.CommandOutput.WordWrap = false;
            this.CommandOutput.TextChanged += new System.EventHandler(this.CommandOutput_TextChanged);
            // 
            // Save
            // 
            this.Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Save.Location = new System.Drawing.Point(453, 358);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(142, 23);
            this.Save.TabIndex = 3;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Visible = false;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Clear
            // 
            this.Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Clear.Location = new System.Drawing.Point(453, 329);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(142, 23);
            this.Clear.TabIndex = 4;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(100, 96);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // dbInfo
            // 
            this.dbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbInfo.Location = new System.Drawing.Point(453, 42);
            this.dbInfo.Name = "dbInfo";
            this.dbInfo.ReadOnly = true;
            this.dbInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dbInfo.Size = new System.Drawing.Size(142, 281);
            this.dbInfo.TabIndex = 5;
            this.dbInfo.Text = "";
            this.dbInfo.TextChanged += new System.EventHandler(this.dbInfo_TextChanged);
            // 
            // DbInterface
            // 
            this.AcceptButton = this.Execute;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 393);
            this.Controls.Add(this.dbInfo);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.CommandOutput);
            this.Controls.Add(this.Execute);
            this.Controls.Add(this.CommandInput);
            this.Name = "DbInterface";
            this.Text = "DataBase Interface";
            this.Load += new System.EventHandler(this.DbInterface_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CommandInput;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.RichTextBox CommandOutput;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox dbInfo;
    }
}