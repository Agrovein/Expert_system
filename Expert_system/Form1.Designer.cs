
namespace Expert_system
{
    partial class startForm
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
            this.UserBTN = new System.Windows.Forms.Button();
            this.AdminBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UserBTN
            // 
            this.UserBTN.Location = new System.Drawing.Point(236, 155);
            this.UserBTN.Name = "UserBTN";
            this.UserBTN.Size = new System.Drawing.Size(151, 71);
            this.UserBTN.TabIndex = 1;
            this.UserBTN.Text = "Користувач";
            this.UserBTN.UseVisualStyleBackColor = true;
            this.UserBTN.Click += new System.EventHandler(this.button1_Click);
            // 
            // AdminBTN
            // 
            this.AdminBTN.Location = new System.Drawing.Point(442, 155);
            this.AdminBTN.Name = "AdminBTN";
            this.AdminBTN.Size = new System.Drawing.Size(151, 71);
            this.AdminBTN.TabIndex = 2;
            this.AdminBTN.Text = "Експерт";
            this.AdminBTN.UseVisualStyleBackColor = true;
            this.AdminBTN.Click += new System.EventHandler(this.button2_Click);
            // 
            // startForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 450);
            this.Controls.Add(this.AdminBTN);
            this.Controls.Add(this.UserBTN);
            this.Name = "startForm";
            this.Text = "Початкова форма";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button UserBTN;
        private System.Windows.Forms.Button AdminBTN;
    }
}

