using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.UI
{
    partial class ForgetPassword : GeneralForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForgetPassword));
            ExitButton = new ExitButton(components);
            UsernameText = new TextBox();
            FindPassword = new Button();
            TransparentRect = new TransparentRect();
            Username = new Label();
            Email = new Label();
            EmailText = new TextBox();
            TransparentRect.SuspendLayout();
            SuspendLayout();
            // 
            // Title
            // 
            Title.Font = new Font("LanaPixel", 26.25F, FontStyle.Bold, GraphicsUnit.Point);
            Title.Location = new Point(7, 6);
            Title.Size = new Size(312, 47);
            Title.TabIndex = 8;
            Title.Text = "Forget Password";
            Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ExitButton
            // 
            ExitButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExitButton.BackColor = Color.White;
            ExitButton.BackgroundImage = (Image)resources.GetObject("ExitButton.BackgroundImage");
            ExitButton.FlatAppearance.BorderColor = Color.White;
            ExitButton.FlatAppearance.BorderSize = 0;
            ExitButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 128);
            ExitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 192, 192);
            ExitButton.FlatStyle = FlatStyle.Flat;
            ExitButton.Font = new Font("LanaPixel", 36F, FontStyle.Bold, GraphicsUnit.Point);
            ExitButton.ForeColor = Color.Red;
            ExitButton.Location = new Point(329, 6);
            ExitButton.Name = "ExitButton";
            ExitButton.RelativeForm = this;
            ExitButton.Size = new Size(47, 47);
            ExitButton.TabIndex = 7;
            ExitButton.TextAlign = ContentAlignment.TopLeft;
            ExitButton.UseVisualStyleBackColor = false;
            // 
            // UsernameText
            // 
            UsernameText.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            UsernameText.Location = new Point(125, 109);
            UsernameText.Name = "UsernameText";
            UsernameText.Size = new Size(216, 29);
            UsernameText.TabIndex = 0;
            // 
            // FindPassword
            // 
            FindPassword.Font = new Font("LanaPixel", 18F, FontStyle.Regular, GraphicsUnit.Point);
            FindPassword.Location = new Point(128, 208);
            FindPassword.Name = "FindPassword";
            FindPassword.Size = new Size(130, 42);
            FindPassword.TabIndex = 3;
            FindPassword.Text = "找回密码";
            FindPassword.UseVisualStyleBackColor = true;
            FindPassword.Click += FindPassword_Click;
            // 
            // TransparentRect
            // 
            TransparentRect.BackColor = Color.WhiteSmoke;
            TransparentRect.BorderColor = Color.WhiteSmoke;
            TransparentRect.Controls.Add(Title);
            TransparentRect.Controls.Add(ExitButton);
            TransparentRect.Controls.Add(FindPassword);
            TransparentRect.Controls.Add(UsernameText);
            TransparentRect.Controls.Add(Username);
            TransparentRect.Controls.Add(Email);
            TransparentRect.Controls.Add(EmailText);
            TransparentRect.Location = new Point(0, 0);
            TransparentRect.Name = "TransparentRect";
            TransparentRect.Opacity = 125;
            TransparentRect.Radius = 20;
            TransparentRect.ShapeBorderStyle = TransparentRect.ShapeBorderStyles.ShapeBSNone;
            TransparentRect.Size = new Size(382, 271);
            TransparentRect.TabIndex = 11;
            TransparentRect.TabStop = false;
            TransparentRect.Controls.SetChildIndex(EmailText, 0);
            TransparentRect.Controls.SetChildIndex(Email, 0);
            TransparentRect.Controls.SetChildIndex(Username, 0);
            TransparentRect.Controls.SetChildIndex(UsernameText, 0);
            TransparentRect.Controls.SetChildIndex(FindPassword, 0);
            TransparentRect.Controls.SetChildIndex(ExitButton, 0);
            TransparentRect.Controls.SetChildIndex(Title, 0);
            // 
            // Username
            // 
            Username.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            Username.Location = new Point(38, 106);
            Username.Name = "Username";
            Username.Size = new Size(75, 33);
            Username.TabIndex = 9;
            Username.Text = "账号";
            Username.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Email
            // 
            Email.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            Email.Location = new Point(38, 139);
            Email.Name = "Email";
            Email.Size = new Size(75, 33);
            Email.TabIndex = 10;
            Email.Text = "邮箱";
            Email.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EmailText
            // 
            EmailText.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            EmailText.Location = new Point(125, 143);
            EmailText.Name = "EmailText";
            EmailText.Size = new Size(216, 29);
            EmailText.TabIndex = 1;
            // 
            // ForgetPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(382, 271);
            Controls.Add(TransparentRect);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ForgetPassword";
            Opacity = 0.9D;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            TransparentRect.ResumeLayout(false);
            TransparentRect.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Library.Component.ExitButton ExitButton;
        private TextBox UsernameText;
        private Button FindPassword;
        private Library.Component.TransparentRect TransparentRect;
        private Label Username;
        private Label Email;
        private TextBox EmailText;
    }
}