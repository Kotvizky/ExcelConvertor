namespace ExcelReader
{
    partial class FormFunc
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tInPar = new System.Windows.Forms.TextBox();
            this.tOutPar = new System.Windows.Forms.TextBox();
            this.tFunc = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tStrTabFields = new System.Windows.Forms.TextBox();
            this.ttabFields = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tStrInPar = new System.Windows.Forms.TextBox();
            this.tStrOutPar = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(57, 344);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(241, 344);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 28);
            this.button2.TabIndex = 6;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tInPar
            // 
            this.tInPar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tInPar.BackColor = System.Drawing.SystemColors.Info;
            this.tInPar.Location = new System.Drawing.Point(13, 19);
            this.tInPar.Name = "tInPar";
            this.tInPar.Size = new System.Drawing.Size(393, 20);
            this.tInPar.TabIndex = 8;
            // 
            // tOutPar
            // 
            this.tOutPar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tOutPar.BackColor = System.Drawing.SystemColors.Info;
            this.tOutPar.Location = new System.Drawing.Point(11, 20);
            this.tOutPar.Name = "tOutPar";
            this.tOutPar.Size = new System.Drawing.Size(391, 20);
            this.tOutPar.TabIndex = 9;
            // 
            // tFunc
            // 
            this.tFunc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tFunc.Location = new System.Drawing.Point(19, 20);
            this.tFunc.Name = "tFunc";
            this.tFunc.ReadOnly = true;
            this.tFunc.Size = new System.Drawing.Size(100, 22);
            this.tFunc.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tStrTabFields);
            this.groupBox1.Controls.Add(this.ttabFields);
            this.groupBox1.Location = new System.Drawing.Point(12, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(415, 71);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "tabFields";
            // 
            // tStrTabFields
            // 
            this.tStrTabFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tStrTabFields.Location = new System.Drawing.Point(11, 45);
            this.tStrTabFields.Name = "tStrTabFields";
            this.tStrTabFields.Size = new System.Drawing.Size(393, 20);
            this.tStrTabFields.TabIndex = 14;
            // 
            // ttabFields
            // 
            this.ttabFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ttabFields.BackColor = System.Drawing.SystemColors.Info;
            this.ttabFields.Location = new System.Drawing.Point(11, 19);
            this.ttabFields.Name = "ttabFields";
            this.ttabFields.Size = new System.Drawing.Size(393, 20);
            this.ttabFields.TabIndex = 13;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tStrInPar);
            this.groupBox2.Controls.Add(this.tInPar);
            this.groupBox2.Location = new System.Drawing.Point(10, 129);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(415, 84);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "InPar";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tStrOutPar);
            this.groupBox3.Controls.Add(this.tOutPar);
            this.groupBox3.Location = new System.Drawing.Point(12, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(415, 97);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "OutPar";
            // 
            // tStrInPar
            // 
            this.tStrInPar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tStrInPar.Location = new System.Drawing.Point(13, 45);
            this.tStrInPar.Name = "tStrInPar";
            this.tStrInPar.Size = new System.Drawing.Size(393, 20);
            this.tStrInPar.TabIndex = 15;
            // 
            // tStrOutPar
            // 
            this.tStrOutPar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tStrOutPar.Location = new System.Drawing.Point(11, 55);
            this.tStrOutPar.Name = "tStrOutPar";
            this.tStrOutPar.Size = new System.Drawing.Size(391, 20);
            this.tStrOutPar.TabIndex = 10;
            // 
            // FormFunc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 384);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tFunc);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "FormFunc";
            this.Text = "FormFunc";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tInPar;
        private System.Windows.Forms.TextBox tOutPar;
        private System.Windows.Forms.TextBox tFunc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tStrTabFields;
        private System.Windows.Forms.TextBox ttabFields;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tStrInPar;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tStrOutPar;
    }
}