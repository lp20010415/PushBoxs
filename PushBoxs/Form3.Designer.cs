
namespace PushBoxs
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.error4 = new System.Windows.Forms.ErrorProvider(this.components);
            this.error1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.error2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.error3 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.error4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.error1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.error2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.error3)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(130, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(185, 25);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(130, 51);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(185, 25);
            this.textBox2.TabIndex = 4;
            this.textBox2.WordWrap = false;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.textBox2.MouseEnter += new System.EventHandler(this.textBox2_MouseEnter);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(130, 82);
            this.textBox3.Name = "textBox3";
            this.textBox3.PasswordChar = '*';
            this.textBox3.Size = new System.Drawing.Size(185, 25);
            this.textBox3.TabIndex = 5;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(130, 113);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(185, 25);
            this.textBox4.TabIndex = 6;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(5, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "密       码:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.MouseEnter += new System.EventHandler(this.label3_MouseEnter);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(14, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "校  验  码:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.MouseEnter += new System.EventHandler(this.label5_MouseEnter);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(5, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "账       号:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(5, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "再次输入密码:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(130, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(185, 34);
            this.button1.TabIndex = 11;
            this.button1.Text = "注册";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tip
            // 
            this.tip.Active = false;
            this.tip.AutomaticDelay = 0;
            this.tip.IsBalloon = true;
            // 
            // error4
            // 
            this.error4.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.error4.ContainerControl = this;
            this.error4.Icon = ((System.Drawing.Icon)(resources.GetObject("error4.Icon")));
            // 
            // error1
            // 
            this.error1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.error1.ContainerControl = this;
            // 
            // error2
            // 
            this.error2.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.error2.ContainerControl = this;
            // 
            // error3
            // 
            this.error3.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.error3.ContainerControl = this;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(353, 187);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Name = "Form3";
            this.Text = "注册账号";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form3_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.error4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.error1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.error2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.error3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.ErrorProvider error4;
        private System.Windows.Forms.ErrorProvider error1;
        private System.Windows.Forms.ErrorProvider error2;
        private System.Windows.Forms.ErrorProvider error3;
    }
}