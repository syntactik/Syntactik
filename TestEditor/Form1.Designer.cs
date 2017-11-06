namespace TestEditor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sourceTextBox = new System.Windows.Forms.RichTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.domTextBox = new System.Windows.Forms.TextBox();
            this.errorsTextBox = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lineLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.columnLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.modeStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.pairStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jsonStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sourceTextBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(837, 534);
            this.splitContainer1.SplitterDistance = 406;
            this.splitContainer1.TabIndex = 0;
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.AcceptsTab = true;
            this.sourceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sourceTextBox.Location = new System.Drawing.Point(0, 0);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.Size = new System.Drawing.Size(406, 534);
            this.sourceTextBox.TabIndex = 0;
            this.sourceTextBox.Text = "";
            this.sourceTextBox.WordWrap = false;
            this.sourceTextBox.SelectionChanged += new System.EventHandler(this.sourceTextBox_SelectionChanged);
            this.sourceTextBox.TextChanged += new System.EventHandler(this.SourceTextBox_TextChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.domTextBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.errorsTextBox);
            this.splitContainer2.Size = new System.Drawing.Size(427, 534);
            this.splitContainer2.SplitterDistance = 384;
            this.splitContainer2.TabIndex = 0;
            // 
            // domTextBox
            // 
            this.domTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.domTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.domTextBox.Location = new System.Drawing.Point(0, 0);
            this.domTextBox.Multiline = true;
            this.domTextBox.Name = "domTextBox";
            this.domTextBox.ReadOnly = true;
            this.domTextBox.Size = new System.Drawing.Size(427, 384);
            this.domTextBox.TabIndex = 1;
            this.domTextBox.WordWrap = false;
            // 
            // errorsTextBox
            // 
            this.errorsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorsTextBox.Location = new System.Drawing.Point(0, 0);
            this.errorsTextBox.Multiline = true;
            this.errorsTextBox.Name = "errorsTextBox";
            this.errorsTextBox.Size = new System.Drawing.Size(427, 146);
            this.errorsTextBox.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lineLabel,
            this.toolStripStatusLabel2,
            this.columnLabel,
            this.modeStripDropDownButton});
            this.statusStrip1.Location = new System.Drawing.Point(0, 534);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(837, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel1.Text = "line: ";
            // 
            // lineLabel
            // 
            this.lineLabel.AutoSize = false;
            this.lineLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(50, 17);
            this.lineLabel.Text = "1";
            this.lineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(51, 17);
            this.toolStripStatusLabel2.Text = "column:";
            // 
            // columnLabel
            // 
            this.columnLabel.AutoSize = false;
            this.columnLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.columnLabel.Name = "columnLabel";
            this.columnLabel.Size = new System.Drawing.Size(50, 17);
            this.columnLabel.Text = "1";
            // 
            // modeStripDropDownButton
            // 
            this.modeStripDropDownButton.AutoSize = false;
            this.modeStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.modeStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pairStripMenuItem,
            this.xmlStripMenuItem,
            this.jsonStripMenuItem});
            this.modeStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("modeStripDropDownButton.Image")));
            this.modeStripDropDownButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.modeStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.modeStripDropDownButton.Name = "modeStripDropDownButton";
            this.modeStripDropDownButton.Size = new System.Drawing.Size(70, 20);
            this.modeStripDropDownButton.Text = "PAIR";
            this.modeStripDropDownButton.ToolTipText = " ";
            this.modeStripDropDownButton.TextChanged += new System.EventHandler(this.SourceTextBox_TextChanged);
            // 
            // pairStripMenuItem
            // 
            this.pairStripMenuItem.Name = "pairStripMenuItem";
            this.pairStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pairStripMenuItem.Text = "PAIR";
            this.pairStripMenuItem.Click += new System.EventHandler(this.pairStripMenuItem_Click);
            // 
            // xmlStripMenuItem
            // 
            this.xmlStripMenuItem.Name = "xmlStripMenuItem";
            this.xmlStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.xmlStripMenuItem.Text = "XML";
            this.xmlStripMenuItem.Click += new System.EventHandler(this.xmlStripMenuItem_Click);
            // 
            // jsonStripMenuItem
            // 
            this.jsonStripMenuItem.Name = "jsonStripMenuItem";
            this.jsonStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.jsonStripMenuItem.Text = "JSON";
            this.jsonStripMenuItem.Click += new System.EventHandler(this.jsonStripMenuItem_Click);
            // 
            // jsonToolStripMenuItem
            // 
            this.jsonToolStripMenuItem.Name = "jsonToolStripMenuItem";
            this.jsonToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.jsonToolStripMenuItem.Text = "Json";
            this.jsonToolStripMenuItem.CheckedChanged += new System.EventHandler(this.jsonToolStripMenuItem_CheckedChanged);
            // 
            // xmlToolStripMenuItem
            // 
            this.xmlToolStripMenuItem.Name = "xmlToolStripMenuItem";
            this.xmlToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.xmlToolStripMenuItem.Text = "Xml";
            // 
            // pairToolStripMenuItem
            // 
            this.pairToolStripMenuItem.Name = "pairToolStripMenuItem";
            this.pairToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.pairToolStripMenuItem.Text = "Pair";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(837, 556);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test of Syntactik parser.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox domTextBox;
        private System.Windows.Forms.TextBox errorsTextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lineLabel;
        private System.Windows.Forms.ToolStripStatusLabel columnLabel;
        private System.Windows.Forms.RichTextBox sourceTextBox;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem jsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pairToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripDropDownButton modeStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem pairStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xmlStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jsonStripMenuItem;
    }
}

