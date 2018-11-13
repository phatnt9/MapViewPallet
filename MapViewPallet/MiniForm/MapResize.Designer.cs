namespace MapViewPallet.MiniForm
{
    partial class MapResize
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
            this.tb_Width = new System.Windows.Forms.TextBox();
            this.btn_Close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Apply = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Height = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tb_Width
            // 
            this.tb_Width.Location = new System.Drawing.Point(49, 12);
            this.tb_Width.Name = "tb_Width";
            this.tb_Width.Size = new System.Drawing.Size(129, 20);
            this.tb_Width.TabIndex = 0;
            this.tb_Width.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_Width_KeyPress);
            // 
            // btn_Close
            // 
            this.btn_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Close.Location = new System.Drawing.Point(96, 82);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(82, 28);
            this.btn_Close.TabIndex = 1;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width";
            // 
            // btn_Apply
            // 
            this.btn_Apply.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Apply.Location = new System.Drawing.Point(8, 82);
            this.btn_Apply.Name = "btn_Apply";
            this.btn_Apply.Size = new System.Drawing.Size(82, 28);
            this.btn_Apply.TabIndex = 1;
            this.btn_Apply.Text = "Apply";
            this.btn_Apply.UseVisualStyleBackColor = true;
            this.btn_Apply.Click += new System.EventHandler(this.btn_Apply_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Height";
            // 
            // tb_Height
            // 
            this.tb_Height.Location = new System.Drawing.Point(49, 38);
            this.tb_Height.Name = "tb_Height";
            this.tb_Height.Size = new System.Drawing.Size(129, 20);
            this.tb_Height.TabIndex = 0;
            this.tb_Height.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_Height_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 119);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Apply);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.tb_Height);
            this.Controls.Add(this.tb_Width);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Resize";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_Width;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Apply;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Height;
    }
}