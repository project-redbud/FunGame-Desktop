namespace Milimoe.FunGame.Desktop.UI
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            ExitButton = new Library.Component.ExitButton(components);
            MinButton = new Library.Component.MinButton(components);
            Username = new Label();
            Password = new Label();
            UsernameText = new TextBox();
            PasswordText = new TextBox();
            RegButton = new Button();
            GoToLogin = new Button();
            ForgetPassword = new Button();
            FastLogin = new Button();
            TransparentRect = new Library.Component.TransparentRect();
            TransparentRect.SuspendLayout();
            SuspendLayout();
            // 
            // Title
            // 
            Title.Font = new Font("LanaPixel", 26.25F, FontStyle.Bold, GraphicsUnit.Point);
            Title.Location = new Point(7, 6);
            Title.Size = new Size(387, 47);
            Title.TabIndex = 8;
            Title.Text = "Welcome to FunGame!";
            Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ExitButton
            // 
            ExitButton.Anchor = AnchorStyles.None;
            ExitButton.BackColor = Color.White;
            ExitButton.BackgroundImage = (Image)resources.GetObject("ExitButton.BackgroundImage");
            ExitButton.FlatAppearance.BorderColor = Color.White;
            ExitButton.FlatAppearance.BorderSize = 0;
            ExitButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 128);
            ExitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 192, 192);
            ExitButton.FlatStyle = FlatStyle.Flat;
            ExitButton.Font = new Font("LanaPixel", 36F, FontStyle.Bold, GraphicsUnit.Point);
            ExitButton.ForeColor = Color.Red;
            ExitButton.Location = new Point(451, 4);
            ExitButton.Name = "ExitButton";
            ExitButton.RelativeForm = this;
            ExitButton.Size = new Size(47, 47);
            ExitButton.TabIndex = 7;
            ExitButton.TextAlign = ContentAlignment.TopLeft;
            ExitButton.UseVisualStyleBackColor = false;
            // 
            // MinButton
            // 
            MinButton.Anchor = AnchorStyles.None;
            MinButton.BackColor = Color.White;
            MinButton.BackgroundImage = (Image)resources.GetObject("MinButton.BackgroundImage");
            MinButton.FlatAppearance.BorderColor = Color.White;
            MinButton.FlatAppearance.BorderSize = 0;
            MinButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            MinButton.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            MinButton.FlatStyle = FlatStyle.Flat;
            MinButton.Font = new Font("LanaPixel", 36F, FontStyle.Bold, GraphicsUnit.Point);
            MinButton.ForeColor = Color.Black;
            MinButton.Location = new Point(398, 4);
            MinButton.Name = "MinButton";
            MinButton.RelativeForm = this;
            MinButton.Size = new Size(47, 47);
            MinButton.TabIndex = 6;
            MinButton.TextAlign = ContentAlignment.TopLeft;
            MinButton.UseVisualStyleBackColor = false;
            // 
            // Username
            // 
            Username.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            Username.Location = new Point(56, 111);
            Username.Name = "Username";
            Username.Size = new Size(75, 33);
            Username.TabIndex = 9;
            Username.Text = "账号";
            Username.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Password
            // 
            Password.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            Password.Location = new Point(56, 144);
            Password.Name = "Password";
            Password.Size = new Size(75, 33);
            Password.TabIndex = 10;
            Password.Text = "密码";
            Password.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UsernameText
            // 
            UsernameText.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            UsernameText.Location = new Point(143, 114);
            UsernameText.Name = "UsernameText";
            UsernameText.Size = new Size(216, 29);
            UsernameText.TabIndex = 0;
            // 
            // PasswordText
            // 
            PasswordText.Font = new Font("LanaPixel", 15F, FontStyle.Regular, GraphicsUnit.Point);
            PasswordText.Location = new Point(143, 148);
            PasswordText.Name = "PasswordText";
            PasswordText.PasswordChar = '*';
            PasswordText.Size = new Size(216, 29);
            PasswordText.TabIndex = 1;
            // 
            // RegButton
            // 
            RegButton.Font = new Font("LanaPixel", 12F, FontStyle.Regular, GraphicsUnit.Point);
            RegButton.Location = new Point(365, 113);
            RegButton.Name = "RegButton";
            RegButton.Size = new Size(81, 33);
            RegButton.TabIndex = 4;
            RegButton.Text = "立即注册";
            RegButton.UseVisualStyleBackColor = true;
            RegButton.Click += RegButton_Click;
            // 
            // GoToLogin
            // 
            GoToLogin.Font = new Font("LanaPixel", 18F, FontStyle.Regular, GraphicsUnit.Point);
            GoToLogin.Location = new Point(277, 216);
            GoToLogin.Name = "GoToLogin";
            GoToLogin.Size = new Size(128, 42);
            GoToLogin.TabIndex = 2;
            GoToLogin.Text = "账号登录";
            GoToLogin.UseVisualStyleBackColor = true;
            GoToLogin.Click += GoToLogin_Click;
            // 
            // ForgetPassword
            // 
            ForgetPassword.Font = new Font("LanaPixel", 12F, FontStyle.Regular, GraphicsUnit.Point);
            ForgetPassword.Location = new Point(365, 147);
            ForgetPassword.Name = "ForgetPassword";
            ForgetPassword.Size = new Size(81, 32);
            ForgetPassword.TabIndex = 5;
            ForgetPassword.Text = "忘记密码";
            ForgetPassword.UseVisualStyleBackColor = true;
            ForgetPassword.Click += ForgetPassword_Click;
            // 
            // FastLogin
            // 
            FastLogin.Font = new Font("LanaPixel", 18F, FontStyle.Regular, GraphicsUnit.Point);
            FastLogin.Location = new Point(114, 216);
            FastLogin.Name = "FastLogin";
            FastLogin.Size = new Size(130, 42);
            FastLogin.TabIndex = 3;
            FastLogin.Text = "快捷登录";
            FastLogin.UseVisualStyleBackColor = true;
            FastLogin.Click += FastLogin_Click;
            // 
            // TransparentRect
            // 
            TransparentRect.BackColor = Color.WhiteSmoke;
            TransparentRect.BorderColor = Color.WhiteSmoke;
            TransparentRect.Controls.Add(Title);
            TransparentRect.Controls.Add(MinButton);
            TransparentRect.Controls.Add(ExitButton);
            TransparentRect.Controls.Add(FastLogin);
            TransparentRect.Controls.Add(UsernameText);
            TransparentRect.Controls.Add(ForgetPassword);
            TransparentRect.Controls.Add(Username);
            TransparentRect.Controls.Add(GoToLogin);
            TransparentRect.Controls.Add(Password);
            TransparentRect.Controls.Add(RegButton);
            TransparentRect.Controls.Add(PasswordText);
            TransparentRect.Location = new Point(0, 0);
            TransparentRect.Name = "TransparentRect";
            TransparentRect.Opacity = 125;
            TransparentRect.Radius = 20;
            TransparentRect.ShapeBorderStyle = Library.Component.TransparentRect.ShapeBorderStyles.ShapeBSNone;
            TransparentRect.Size = new Size(503, 289);
            TransparentRect.TabIndex = 11;
            TransparentRect.TabStop = false;
            TransparentRect.Controls.SetChildIndex(PasswordText, 0);
            TransparentRect.Controls.SetChildIndex(RegButton, 0);
            TransparentRect.Controls.SetChildIndex(Password, 0);
            TransparentRect.Controls.SetChildIndex(GoToLogin, 0);
            TransparentRect.Controls.SetChildIndex(Username, 0);
            TransparentRect.Controls.SetChildIndex(ForgetPassword, 0);
            TransparentRect.Controls.SetChildIndex(UsernameText, 0);
            TransparentRect.Controls.SetChildIndex(FastLogin, 0);
            TransparentRect.Controls.SetChildIndex(ExitButton, 0);
            TransparentRect.Controls.SetChildIndex(MinButton, 0);
            TransparentRect.Controls.SetChildIndex(Title, 0);
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(503, 289);
            Controls.Add(TransparentRect);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Login";
            Opacity = 0.9D;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            TransparentRect.ResumeLayout(false);
            TransparentRect.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Library.Component.ExitButton ExitButton;
        private Library.Component.MinButton MinButton;
        private Label Username;
        private Label Password;
        private TextBox UsernameText;
        private TextBox PasswordText;
        private Button RegButton;
        private Button GoToLogin;
        private Button ForgetPassword;
        private Button FastLogin;
        private Library.Component.TransparentRect TransparentRect;
    }
}