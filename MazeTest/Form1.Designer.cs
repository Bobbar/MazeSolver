namespace MazeTest
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SolveButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LoadMazeButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.GenMazeButton = new System.Windows.Forms.Button();
            this.GenColumnsRowsTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SolveButton
            // 
            this.SolveButton.Enabled = false;
            this.SolveButton.Location = new System.Drawing.Point(27, 70);
            this.SolveButton.Name = "SolveButton";
            this.SolveButton.Size = new System.Drawing.Size(75, 36);
            this.SolveButton.TabIndex = 0;
            this.SolveButton.Text = "Solve";
            this.SolveButton.UseVisualStyleBackColor = true;
            this.SolveButton.Click += new System.EventHandler(this.SolveButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(151, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(897, 836);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // LoadMazeButton
            // 
            this.LoadMazeButton.Location = new System.Drawing.Point(27, 28);
            this.LoadMazeButton.Name = "LoadMazeButton";
            this.LoadMazeButton.Size = new System.Drawing.Size(75, 23);
            this.LoadMazeButton.TabIndex = 3;
            this.LoadMazeButton.Text = "Load Maze";
            this.LoadMazeButton.UseVisualStyleBackColor = true;
            this.LoadMazeButton.Click += new System.EventHandler(this.LoadMazeButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 40);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GenMazeButton
            // 
            this.GenMazeButton.Location = new System.Drawing.Point(32, 225);
            this.GenMazeButton.Name = "GenMazeButton";
            this.GenMazeButton.Size = new System.Drawing.Size(60, 40);
            this.GenMazeButton.TabIndex = 5;
            this.GenMazeButton.Text = "Gen";
            this.GenMazeButton.UseVisualStyleBackColor = true;
            this.GenMazeButton.Click += new System.EventHandler(this.GenMazeButton_Click);
            // 
            // GenColumnsRowsTextBox
            // 
            this.GenColumnsRowsTextBox.Location = new System.Drawing.Point(12, 292);
            this.GenColumnsRowsTextBox.Name = "GenColumnsRowsTextBox";
            this.GenColumnsRowsTextBox.Size = new System.Drawing.Size(106, 23);
            this.GenColumnsRowsTextBox.TabIndex = 6;
            this.GenColumnsRowsTextBox.Text = "200, 200";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 271);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Columns, Rows:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 860);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GenColumnsRowsTextBox);
            this.Controls.Add(this.GenMazeButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LoadMazeButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.SolveButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button SolveButton;
        private PictureBox pictureBox1;
        private Button LoadMazeButton;
        private Button button1;
        private Button GenMazeButton;
        private TextBox GenColumnsRowsTextBox;
        private Label label1;
    }
}