using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.UI
{
    partial class Main
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            Exit = new ExitButton(components);
            MinForm = new MinButton(components);
            Connection = new Label();
            Light = new Label();
            SendTalkText = new Button();
            TalkText = new TextBox();
            StartMatch = new Button();
            RoomSetting = new Button();
            Login = new Button();
            NowAccount = new Label();
            AccountSetting = new Button();
            About = new Button();
            Room = new Label();
            RoomText = new TextBox();
            PresetText = new ComboBox();
            RoomBox = new GroupBox();
            NowRoomID = new TextBox();
            CopyRoomID = new Button();
            RoomList = new ListBox();
            QueryRoom = new Button();
            RefreshRoomList = new Button();
            Notice = new GroupBox();
            NoticeText = new TextArea();
            InfoBox = new GroupBox();
            TransparentRectControl = new TransparentRect();
            GameInfo = new TextArea();
            QuitRoom = new Button();
            CreateRoom = new Button();
            Logout = new Button();
            CheckHasPass = new CheckBox();
            Stock = new Button();
            Store = new Button();
            Copyright = new LinkLabel();
            StopMatch = new Button();
            CheckIsRank = new CheckBox();
            ComboRoomType = new ComboBox();
            ComboGameModule = new ComboBox();
            ComboGameMap = new ComboBox();
            RoomBox.SuspendLayout();
            Notice.SuspendLayout();
            InfoBox.SuspendLayout();
            TransparentRectControl.SuspendLayout();
            SuspendLayout();
            // 
            // Title
            // 
            Title.BackColor = Color.Transparent;
            Title.Font = new Font("LanaPixel", 26.25F, FontStyle.Bold);
            Title.Location = new Point(3, 3);
            Title.Size = new Size(689, 47);
            Title.TabIndex = 96;
            Title.Text = "FunGame";
            Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Exit
            // 
            Exit.Anchor = AnchorStyles.None;
            Exit.BackColor = Color.White;
            Exit.BackgroundImage = Properties.Resources.exit;
            Exit.FlatAppearance.BorderColor = Color.White;
            Exit.FlatAppearance.BorderSize = 0;
            Exit.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 128);
            Exit.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 192, 192);
            Exit.FlatStyle = FlatStyle.Flat;
            Exit.Font = new Font("LanaPixel", 36F, FontStyle.Bold);
            Exit.ForeColor = Color.Red;
            Exit.Location = new Point(750, 3);
            Exit.Name = "Exit";
            Exit.RelativeForm = null;
            Exit.Size = new Size(47, 47);
            Exit.TabIndex = 16;
            Exit.TextAlign = ContentAlignment.TopLeft;
            Exit.UseVisualStyleBackColor = false;
            Exit.Click += Exit_Click;
            // 
            // MinForm
            // 
            MinForm.Anchor = AnchorStyles.None;
            MinForm.BackColor = Color.White;
            MinForm.BackgroundImage = Properties.Resources.min;
            MinForm.BackgroundImageLayout = ImageLayout.Center;
            MinForm.FlatAppearance.BorderColor = Color.LightGray;
            MinForm.FlatAppearance.BorderSize = 0;
            MinForm.FlatAppearance.MouseDownBackColor = Color.Gray;
            MinForm.FlatAppearance.MouseOverBackColor = Color.DarkGray;
            MinForm.FlatStyle = FlatStyle.Flat;
            MinForm.Font = new Font("LanaPixel", 36F, FontStyle.Bold);
            MinForm.ForeColor = Color.Red;
            MinForm.Location = new Point(698, 3);
            MinForm.Name = "MinForm";
            MinForm.RelativeForm = this;
            MinForm.Size = new Size(47, 47);
            MinForm.TabIndex = 15;
            MinForm.TextAlign = ContentAlignment.TopLeft;
            MinForm.UseVisualStyleBackColor = false;
            // 
            // Connection
            // 
            Connection.BackColor = Color.Transparent;
            Connection.Font = new Font("LanaPixel", 12F);
            Connection.Location = new Point(649, 424);
            Connection.Margin = new Padding(3);
            Connection.Name = "Connection";
            Connection.Size = new Size(130, 23);
            Connection.TabIndex = 92;
            Connection.Text = "等待连接服务器";
            Connection.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Light
            // 
            Light.BackColor = Color.Transparent;
            Light.Image = Properties.Resources.yellow;
            Light.Location = new Point(777, 426);
            Light.Name = "Light";
            Light.Size = new Size(18, 18);
            Light.TabIndex = 93;
            // 
            // SendTalkText
            // 
            SendTalkText.Anchor = AnchorStyles.None;
            SendTalkText.BackColor = Color.Transparent;
            SendTalkText.BackgroundImage = Properties.Resources.send;
            SendTalkText.BackgroundImageLayout = ImageLayout.Center;
            SendTalkText.FlatAppearance.BorderSize = 0;
            SendTalkText.FlatAppearance.MouseDownBackColor = Color.Teal;
            SendTalkText.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 192, 192);
            SendTalkText.FlatStyle = FlatStyle.Flat;
            SendTalkText.Font = new Font("LanaPixel", 11.25F);
            SendTalkText.Location = new Point(608, 421);
            SendTalkText.Name = "SendTalkText";
            SendTalkText.Size = new Size(51, 27);
            SendTalkText.TabIndex = 4;
            SendTalkText.TextAlign = ContentAlignment.TopLeft;
            SendTalkText.UseVisualStyleBackColor = false;
            SendTalkText.Click += SendTalkText_Click;
            // 
            // TalkText
            // 
            TalkText.AllowDrop = true;
            TalkText.Font = new Font("LanaPixel", 12.75F);
            TalkText.ForeColor = Color.DarkGray;
            TalkText.Location = new Point(317, 422);
            TalkText.Name = "TalkText";
            TalkText.Size = new Size(289, 26);
            TalkText.TabIndex = 3;
            TalkText.Text = "向消息队列发送消息...";
            TalkText.WordWrap = false;
            TalkText.Click += TalkText_ClickAndFocused;
            TalkText.GotFocus += TalkText_ClickAndFocused;
            TalkText.KeyUp += TalkText_KeyUp;
            TalkText.Leave += TalkText_Leave;
            // 
            // StartMatch
            // 
            StartMatch.Font = new Font("LanaPixel", 12F);
            StartMatch.Location = new Point(665, 214);
            StartMatch.Name = "StartMatch";
            StartMatch.Size = new Size(132, 35);
            StartMatch.TabIndex = 9;
            StartMatch.Text = "开始匹配";
            StartMatch.UseVisualStyleBackColor = true;
            StartMatch.Click += StartMatch_Click;
            // 
            // RoomSetting
            // 
            RoomSetting.Font = new Font("LanaPixel", 12F);
            RoomSetting.Location = new Point(665, 254);
            RoomSetting.Name = "RoomSetting";
            RoomSetting.Size = new Size(132, 35);
            RoomSetting.TabIndex = 11;
            RoomSetting.Text = "房间设置";
            RoomSetting.UseVisualStyleBackColor = true;
            RoomSetting.Visible = false;
            RoomSetting.Click += RoomSetting_Click;
            // 
            // Login
            // 
            Login.Font = new Font("LanaPixel", 15.75F);
            Login.Location = new Point(665, 380);
            Login.Name = "Login";
            Login.Size = new Size(132, 39);
            Login.TabIndex = 13;
            Login.Text = "登录账号";
            Login.UseVisualStyleBackColor = true;
            Login.Click += Login_Click;
            // 
            // NowAccount
            // 
            NowAccount.BackColor = Color.Transparent;
            NowAccount.Font = new Font("LanaPixel", 12F);
            NowAccount.Location = new Point(551, 9);
            NowAccount.Name = "NowAccount";
            NowAccount.Size = new Size(141, 41);
            NowAccount.TabIndex = 91;
            NowAccount.Text = "请登录账号";
            NowAccount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AccountSetting
            // 
            AccountSetting.Font = new Font("LanaPixel", 12F);
            AccountSetting.Location = new Point(665, 342);
            AccountSetting.Name = "AccountSetting";
            AccountSetting.Size = new Size(65, 32);
            AccountSetting.TabIndex = 12;
            AccountSetting.Text = "设置";
            AccountSetting.UseVisualStyleBackColor = true;
            // 
            // About
            // 
            About.Font = new Font("LanaPixel", 12F);
            About.Location = new Point(733, 341);
            About.Name = "About";
            About.Size = new Size(65, 32);
            About.TabIndex = 13;
            About.Text = "关于";
            About.UseVisualStyleBackColor = true;
            // 
            // Room
            // 
            Room.BackColor = Color.Transparent;
            Room.Font = new Font("LanaPixel", 12F);
            Room.Location = new Point(665, 293);
            Room.Name = "Room";
            Room.Size = new Size(132, 45);
            Room.TabIndex = 90;
            Room.Text = "房间号：114514";
            Room.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RoomText
            // 
            RoomText.AllowDrop = true;
            RoomText.Font = new Font("LanaPixel", 12F);
            RoomText.ForeColor = Color.DarkGray;
            RoomText.Location = new Point(6, 226);
            RoomText.Name = "RoomText";
            RoomText.Size = new Size(114, 25);
            RoomText.TabIndex = 1;
            RoomText.Text = "键入房间代号...";
            RoomText.WordWrap = false;
            RoomText.Click += RoomText_ClickAndFocused;
            RoomText.GotFocus += RoomText_ClickAndFocused;
            RoomText.KeyUp += RoomText_KeyUp;
            RoomText.Leave += RoomText_Leave;
            // 
            // PresetText
            // 
            PresetText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PresetText.DropDownStyle = ComboBoxStyle.DropDownList;
            PresetText.Font = new Font("LanaPixel", 11.25F);
            PresetText.FormattingEnabled = true;
            PresetText.Items.AddRange(new object[] { "- 快捷消息 -" });
            PresetText.Location = new Point(195, 422);
            PresetText.Name = "PresetText";
            PresetText.Size = new Size(121, 26);
            PresetText.TabIndex = 2;
            PresetText.SelectedIndexChanged += PresetText_SelectedIndexChanged;
            // 
            // RoomBox
            // 
            RoomBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RoomBox.BackColor = Color.Transparent;
            RoomBox.Controls.Add(NowRoomID);
            RoomBox.Controls.Add(CopyRoomID);
            RoomBox.Controls.Add(RoomList);
            RoomBox.Controls.Add(RoomText);
            RoomBox.Controls.Add(QueryRoom);
            RoomBox.Font = new Font("LanaPixel", 12F);
            RoomBox.Location = new Point(3, 56);
            RoomBox.Name = "RoomBox";
            RoomBox.Size = new Size(186, 258);
            RoomBox.TabIndex = 0;
            RoomBox.TabStop = false;
            RoomBox.Text = "房间列表";
            // 
            // NowRoomID
            // 
            NowRoomID.AllowDrop = true;
            NowRoomID.Font = new Font("LanaPixel", 12F);
            NowRoomID.ForeColor = Color.DarkGray;
            NowRoomID.Location = new Point(6, 226);
            NowRoomID.Name = "NowRoomID";
            NowRoomID.ReadOnly = true;
            NowRoomID.Size = new Size(114, 25);
            NowRoomID.TabIndex = 1;
            NowRoomID.Text = "1919810";
            NowRoomID.Visible = false;
            NowRoomID.WordWrap = false;
            // 
            // CopyRoomID
            // 
            CopyRoomID.Font = new Font("LanaPixel", 12F);
            CopyRoomID.Location = new Point(126, 225);
            CopyRoomID.Name = "CopyRoomID";
            CopyRoomID.Size = new Size(51, 27);
            CopyRoomID.TabIndex = 2;
            CopyRoomID.Text = "复制";
            CopyRoomID.UseVisualStyleBackColor = true;
            CopyRoomID.Visible = false;
            CopyRoomID.Click += CopyRoomID_Click;
            // 
            // RoomList
            // 
            RoomList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RoomList.BorderStyle = BorderStyle.FixedSingle;
            RoomList.FormattingEnabled = true;
            RoomList.ItemHeight = 19;
            RoomList.Location = new Point(0, 26);
            RoomList.Name = "RoomList";
            RoomList.Size = new Size(186, 192);
            RoomList.TabIndex = 0;
            RoomList.MouseDoubleClick += RoomList_MouseDoubleClick;
            // 
            // QueryRoom
            // 
            QueryRoom.Font = new Font("LanaPixel", 12F);
            QueryRoom.Location = new Point(126, 225);
            QueryRoom.Name = "QueryRoom";
            QueryRoom.Size = new Size(51, 27);
            QueryRoom.TabIndex = 2;
            QueryRoom.Text = "加入";
            QueryRoom.UseVisualStyleBackColor = true;
            QueryRoom.Click += QueryRoom_Click;
            // 
            // RefreshRoomList
            // 
            RefreshRoomList.Font = new Font("LanaPixel", 12F);
            RefreshRoomList.Image = Properties.Resources.refresh;
            RefreshRoomList.Location = new Point(162, 248);
            RefreshRoomList.Name = "RefreshRoomList";
            RefreshRoomList.Size = new Size(24, 24);
            RefreshRoomList.TabIndex = 1;
            RefreshRoomList.UseVisualStyleBackColor = true;
            RefreshRoomList.Click += RefreshRoomList_Click;
            // 
            // Notice
            // 
            Notice.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Notice.BackColor = Color.Transparent;
            Notice.Controls.Add(NoticeText);
            Notice.Font = new Font("LanaPixel", 12F);
            Notice.Location = new Point(3, 317);
            Notice.Name = "Notice";
            Notice.Size = new Size(186, 110);
            Notice.TabIndex = 94;
            Notice.TabStop = false;
            Notice.Text = "通知公告";
            // 
            // NoticeText
            // 
            NoticeText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            NoticeText.BorderStyle = BorderStyle.None;
            NoticeText.EmptyTextTip = null;
            NoticeText.Location = new Point(6, 24);
            NoticeText.Name = "NoticeText";
            NoticeText.ReadOnly = true;
            NoticeText.Size = new Size(174, 86);
            NoticeText.TabIndex = 0;
            NoticeText.Text = "";
            // 
            // InfoBox
            // 
            InfoBox.BackColor = Color.Transparent;
            InfoBox.Controls.Add(TransparentRectControl);
            InfoBox.Font = new Font("LanaPixel", 12F);
            InfoBox.Location = new Point(195, 56);
            InfoBox.Name = "InfoBox";
            InfoBox.Size = new Size(464, 363);
            InfoBox.TabIndex = 95;
            InfoBox.TabStop = false;
            InfoBox.Text = "消息队列";
            // 
            // TransparentRectControl
            // 
            TransparentRectControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TransparentRectControl.BackColor = Color.Transparent;
            TransparentRectControl.BorderColor = Color.Transparent;
            TransparentRectControl.Controls.Add(GameInfo);
            TransparentRectControl.Location = new Point(0, 20);
            TransparentRectControl.Name = "TransparentRectControl";
            TransparentRectControl.Opacity = 125;
            TransparentRectControl.Radius = 20;
            TransparentRectControl.ShapeBorderStyle = TransparentRect.ShapeBorderStyles.ShapeBSNone;
            TransparentRectControl.Size = new Size(464, 343);
            TransparentRectControl.TabIndex = 2;
            TransparentRectControl.TabStop = false;
            // 
            // GameInfo
            // 
            GameInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GameInfo.BorderStyle = BorderStyle.None;
            GameInfo.EmptyTextTip = null;
            GameInfo.EmptyTextTipColor = Color.Transparent;
            GameInfo.Location = new Point(6, 6);
            GameInfo.Name = "GameInfo";
            GameInfo.ReadOnly = true;
            GameInfo.ScrollBars = RichTextBoxScrollBars.Vertical;
            GameInfo.Size = new Size(452, 331);
            GameInfo.TabIndex = 1;
            GameInfo.Text = "";
            // 
            // QuitRoom
            // 
            QuitRoom.Font = new Font("LanaPixel", 12F);
            QuitRoom.Location = new Point(665, 212);
            QuitRoom.Name = "QuitRoom";
            QuitRoom.Size = new Size(132, 35);
            QuitRoom.TabIndex = 9;
            QuitRoom.Text = "退出房间";
            QuitRoom.UseVisualStyleBackColor = true;
            QuitRoom.Visible = false;
            QuitRoom.Click += QuitRoom_Click;
            // 
            // CreateRoom
            // 
            CreateRoom.Font = new Font("LanaPixel", 12F);
            CreateRoom.Location = new Point(666, 253);
            CreateRoom.Name = "CreateRoom";
            CreateRoom.Size = new Size(132, 35);
            CreateRoom.TabIndex = 10;
            CreateRoom.Text = "创建房间";
            CreateRoom.UseVisualStyleBackColor = true;
            CreateRoom.Click += CreateRoom_Click;
            // 
            // Logout
            // 
            Logout.Font = new Font("LanaPixel", 15.75F);
            Logout.Location = new Point(665, 380);
            Logout.Name = "Logout";
            Logout.Size = new Size(132, 39);
            Logout.TabIndex = 14;
            Logout.Text = "退出登录";
            Logout.UseVisualStyleBackColor = true;
            Logout.Visible = false;
            Logout.Click += Logout_Click;
            // 
            // CheckHasPass
            // 
            CheckHasPass.BackColor = Color.Transparent;
            CheckHasPass.Font = new Font("LanaPixel", 12F);
            CheckHasPass.Location = new Point(737, 181);
            CheckHasPass.Name = "CheckHasPass";
            CheckHasPass.Size = new Size(60, 24);
            CheckHasPass.TabIndex = 9;
            CheckHasPass.Text = "密码";
            CheckHasPass.TextAlign = ContentAlignment.MiddleCenter;
            CheckHasPass.UseVisualStyleBackColor = false;
            // 
            // Stock
            // 
            Stock.Font = new Font("LanaPixel", 12F);
            Stock.Location = new Point(661, 56);
            Stock.Name = "Stock";
            Stock.Size = new Size(65, 32);
            Stock.TabIndex = 5;
            Stock.Text = "库存";
            Stock.UseVisualStyleBackColor = true;
            // 
            // Store
            // 
            Store.Font = new Font("LanaPixel", 12F);
            Store.Location = new Point(732, 56);
            Store.Name = "Store";
            Store.Size = new Size(65, 32);
            Store.TabIndex = 6;
            Store.Text = "商店";
            Store.UseVisualStyleBackColor = true;
            // 
            // Copyright
            // 
            Copyright.ActiveLinkColor = Color.FromArgb(0, 64, 64);
            Copyright.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Copyright.BackColor = Color.Transparent;
            Copyright.Font = new Font("LanaPixel", 10.5F);
            Copyright.LinkArea = new LinkArea(6, 8);
            Copyright.LinkBehavior = LinkBehavior.AlwaysUnderline;
            Copyright.LinkColor = Color.Teal;
            Copyright.Location = new Point(3, 430);
            Copyright.Name = "Copyright";
            Copyright.Size = new Size(186, 23);
            Copyright.TabIndex = 97;
            Copyright.TabStop = true;
            Copyright.Text = "©2022 Milimoe. 米粒的糖果屋";
            Copyright.TextAlign = ContentAlignment.MiddleLeft;
            Copyright.UseCompatibleTextRendering = true;
            Copyright.LinkClicked += Copyright_LinkClicked;
            // 
            // StopMatch
            // 
            StopMatch.Font = new Font("LanaPixel", 12F);
            StopMatch.Location = new Point(665, 213);
            StopMatch.Name = "StopMatch";
            StopMatch.Size = new Size(132, 35);
            StopMatch.TabIndex = 10;
            StopMatch.Text = "停止匹配";
            StopMatch.UseVisualStyleBackColor = true;
            StopMatch.Visible = false;
            StopMatch.Click += StopMatch_Click;
            StopMatch.MouseLeave += StopMatch_MouseLeave;
            StopMatch.MouseHover += StopMatch_MouseHover;
            // 
            // CheckIsRank
            // 
            CheckIsRank.BackColor = Color.Transparent;
            CheckIsRank.Enabled = false;
            CheckIsRank.Font = new Font("LanaPixel", 12F);
            CheckIsRank.Location = new Point(671, 181);
            CheckIsRank.Name = "CheckIsRank";
            CheckIsRank.Size = new Size(60, 24);
            CheckIsRank.TabIndex = 98;
            CheckIsRank.Text = "排位";
            CheckIsRank.TextAlign = ContentAlignment.MiddleCenter;
            CheckIsRank.UseVisualStyleBackColor = false;
            // 
            // ComboRoomType
            // 
            ComboRoomType.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ComboRoomType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboRoomType.Font = new Font("LanaPixel", 11.25F);
            ComboRoomType.FormattingEnabled = true;
            ComboRoomType.Items.AddRange(new object[] { "- 房间类型 -" });
            ComboRoomType.Location = new Point(665, 94);
            ComboRoomType.Name = "ComboRoomType";
            ComboRoomType.Size = new Size(130, 26);
            ComboRoomType.TabIndex = 99;
            ComboRoomType.SelectionChangeCommitted += ComboRoomType_SelectionChangeCommitted;
            // 
            // ComboGameModule
            // 
            ComboGameModule.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ComboGameModule.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboGameModule.Font = new Font("LanaPixel", 11.25F);
            ComboGameModule.FormattingEnabled = true;
            ComboGameModule.Items.AddRange(new object[] { "- 请选择类型 -" });
            ComboGameModule.Location = new Point(665, 122);
            ComboGameModule.Name = "ComboGameModule";
            ComboGameModule.Size = new Size(130, 26);
            ComboGameModule.TabIndex = 100;
            ComboGameModule.SelectionChangeCommitted += ComboGameModule_SelectionChangeCommitted;
            // 
            // ComboGameMap
            // 
            ComboGameMap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ComboGameMap.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboGameMap.Font = new Font("LanaPixel", 11.25F);
            ComboGameMap.FormattingEnabled = true;
            ComboGameMap.Items.AddRange(new object[] { "- 请选择类型 -" });
            ComboGameMap.Location = new Point(665, 150);
            ComboGameMap.Name = "ComboGameMap";
            ComboGameMap.Size = new Size(130, 26);
            ComboGameMap.TabIndex = 101;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.back;
            ClientSize = new Size(800, 450);
            Controls.Add(ComboGameMap);
            Controls.Add(ComboGameModule);
            Controls.Add(ComboRoomType);
            Controls.Add(CheckIsRank);
            Controls.Add(RefreshRoomList);
            Controls.Add(StopMatch);
            Controls.Add(Copyright);
            Controls.Add(Store);
            Controls.Add(Stock);
            Controls.Add(Logout);
            Controls.Add(RoomSetting);
            Controls.Add(QuitRoom);
            Controls.Add(InfoBox);
            Controls.Add(CreateRoom);
            Controls.Add(Notice);
            Controls.Add(RoomBox);
            Controls.Add(PresetText);
            Controls.Add(SendTalkText);
            Controls.Add(TalkText);
            Controls.Add(Room);
            Controls.Add(About);
            Controls.Add(AccountSetting);
            Controls.Add(NowAccount);
            Controls.Add(Login);
            Controls.Add(CheckHasPass);
            Controls.Add(StartMatch);
            Controls.Add(Light);
            Controls.Add(Connection);
            Controls.Add(MinForm);
            Controls.Add(Exit);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FunGame";
            Controls.SetChildIndex(Exit, 0);
            Controls.SetChildIndex(Title, 0);
            Controls.SetChildIndex(MinForm, 0);
            Controls.SetChildIndex(Connection, 0);
            Controls.SetChildIndex(Light, 0);
            Controls.SetChildIndex(StartMatch, 0);
            Controls.SetChildIndex(CheckHasPass, 0);
            Controls.SetChildIndex(Login, 0);
            Controls.SetChildIndex(NowAccount, 0);
            Controls.SetChildIndex(AccountSetting, 0);
            Controls.SetChildIndex(About, 0);
            Controls.SetChildIndex(Room, 0);
            Controls.SetChildIndex(TalkText, 0);
            Controls.SetChildIndex(SendTalkText, 0);
            Controls.SetChildIndex(PresetText, 0);
            Controls.SetChildIndex(RoomBox, 0);
            Controls.SetChildIndex(Notice, 0);
            Controls.SetChildIndex(CreateRoom, 0);
            Controls.SetChildIndex(InfoBox, 0);
            Controls.SetChildIndex(QuitRoom, 0);
            Controls.SetChildIndex(RoomSetting, 0);
            Controls.SetChildIndex(Logout, 0);
            Controls.SetChildIndex(Stock, 0);
            Controls.SetChildIndex(Store, 0);
            Controls.SetChildIndex(Copyright, 0);
            Controls.SetChildIndex(StopMatch, 0);
            Controls.SetChildIndex(RefreshRoomList, 0);
            Controls.SetChildIndex(CheckIsRank, 0);
            Controls.SetChildIndex(ComboRoomType, 0);
            Controls.SetChildIndex(ComboGameModule, 0);
            Controls.SetChildIndex(ComboGameMap, 0);
            RoomBox.ResumeLayout(false);
            RoomBox.PerformLayout();
            Notice.ResumeLayout(false);
            InfoBox.ResumeLayout(false);
            TransparentRectControl.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ExitButton Exit;
        private MinButton MinForm;
        private Label Connection;
        private Label Light;
        private Button StartMatch;
        private Button RoomSetting;
        private Button Login;
        private Label NowAccount;
        private Button AccountSetting;
        private Button About;
        private Label Room;
        private Button SendTalkText;
        private TextBox TalkText;
        private TextBox RoomText;
        private ComboBox PresetText;
        private GroupBox RoomBox;
        private ListBox RoomList;
        private GroupBox Notice;
        private GroupBox InfoBox;
        private Button QuitRoom;
        private Button CreateRoom;
        private Button QueryRoom;
        private Button Logout;
        private CheckBox CheckHasPass;
        private Button Stock;
        private Button Store;
        private LinkLabel Copyright;
        private Button StopMatch;
        private Library.Component.TextArea GameInfo;
        private Library.Component.TextArea NoticeText;
        private Library.Component.TransparentRect TransparentRectControl;
        private TextBox NowRoomID;
        private Button CopyRoomID;
        private Button RefreshRoomList;
        private CheckBox CheckIsRank;
        private ComboBox ComboGameMap;
        private ComboBox ComboGameModule;
        private ComboBox ComboRoomType;
    }
}