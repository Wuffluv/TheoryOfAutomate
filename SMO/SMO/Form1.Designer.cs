namespace SMO
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.listBoxProcessed = new System.Windows.Forms.ListBox();
            this.listBoxDiscarded = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelProcessed = new System.Windows.Forms.Label();
            this.labelDiscarded = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(466, 618);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(220, 62);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBoxProcessed
            // 
            this.listBoxProcessed.FormattingEnabled = true;
            this.listBoxProcessed.ItemHeight = 16;
            this.listBoxProcessed.Location = new System.Drawing.Point(1082, 36);
            this.listBoxProcessed.Name = "listBoxProcessed";
            this.listBoxProcessed.Size = new System.Drawing.Size(212, 276);
            this.listBoxProcessed.TabIndex = 1;
            // 
            // listBoxDiscarded
            // 
            this.listBoxDiscarded.FormattingEnabled = true;
            this.listBoxDiscarded.ItemHeight = 16;
            this.listBoxDiscarded.Location = new System.Drawing.Point(1082, 352);
            this.listBoxDiscarded.Name = "listBoxDiscarded";
            this.listBoxDiscarded.Size = new System.Drawing.Size(212, 260);
            this.listBoxDiscarded.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1061, 600);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // labelProcessed
            // 
            this.labelProcessed.AutoSize = true;
            this.labelProcessed.Location = new System.Drawing.Point(1079, 12);
            this.labelProcessed.Name = "labelProcessed";
            this.labelProcessed.Size = new System.Drawing.Size(44, 16);
            this.labelProcessed.TabIndex = 4;
            this.labelProcessed.Text = "label1";
            // 
            // labelDiscarded
            // 
            this.labelDiscarded.AutoSize = true;
            this.labelDiscarded.Location = new System.Drawing.Point(1079, 333);
            this.labelDiscarded.Name = "labelDiscarded";
            this.labelDiscarded.Size = new System.Drawing.Size(44, 16);
            this.labelDiscarded.TabIndex = 5;
            this.labelDiscarded.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1306, 753);
            this.Controls.Add(this.labelDiscarded);
            this.Controls.Add(this.labelProcessed);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBoxDiscarded);
            this.Controls.Add(this.listBoxProcessed);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBoxProcessed;
        private System.Windows.Forms.ListBox listBoxDiscarded;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelProcessed;
        private System.Windows.Forms.Label labelDiscarded;
        private System.Windows.Forms.Timer timer1;
    }
}

