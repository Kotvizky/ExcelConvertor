namespace ExcelReader
{
    partial class FormDialogTmp
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bDrop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(25, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(318, 20);
            this.textBox1.TabIndex = 0;
            // 
            // bDrop
            // 
            this.bDrop.Location = new System.Drawing.Point(72, 76);
            this.bDrop.Name = "bDrop";
            this.bDrop.Size = new System.Drawing.Size(212, 24);
            this.bDrop.TabIndex = 1;
            this.bDrop.Text = "Drop";
            this.bDrop.UseVisualStyleBackColor = true;
            this.bDrop.Click += new System.EventHandler(this.bDrop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Temporary table has been created";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // FormDialogTmp
            // 
            this.AcceptButton = this.bDrop;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 112);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bDrop);
            this.Controls.Add(this.textBox1);
            this.Name = "FormDialogTmp";
            this.Text = "FormDialogTmp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bDrop;
        private System.Windows.Forms.Label label1;
    }
}