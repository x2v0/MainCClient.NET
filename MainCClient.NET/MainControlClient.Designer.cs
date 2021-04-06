namespace MainCClient.NET
{
   partial class MainControlClient
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
         if (disposing && (components != null)) {
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainControlClient));
         this.label1 = new System.Windows.Forms.Label();
         this.SrvName = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.PortNum = new System.Windows.Forms.NumericUpDown();
         this.ConnectBtn = new System.Windows.Forms.Button();
         this.label3 = new System.Windows.Forms.Label();
         this.MessagesLB = new System.Windows.Forms.ListBox();
         this.QuitBtn = new System.Windows.Forms.Button();
         this.LedImages = new System.Windows.Forms.ImageList(this.components);
         this.LoadPlanBtn = new System.Windows.Forms.Button();
         this.ClearPlanBtn = new System.Windows.Forms.Button();
         this.GetStateBtn = new System.Windows.Forms.Button();
         this.SendDataBtn = new System.Windows.Forms.Button();
         this.StartPlanBtn = new System.Windows.Forms.Button();
         this.PausePlanBtn = new System.Windows.Forms.Button();
         this.StopPlanBtn = new System.Windows.Forms.Button();
         this.TableGrid = new System.Windows.Forms.DataGridView();
         this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Xangle = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Yangle = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Emev = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Intensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.ResultX = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.ResultZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.ResultIntensity = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.ClearLogBtn = new System.Windows.Forms.Button();
         this.LedFinish = new System.Windows.Forms.Label();
         this.LedPause = new System.Windows.Forms.Label();
         this.LedProcess = new System.Windows.Forms.Label();
         this.LedReady = new System.Windows.Forms.Label();
         this.LedNotReady = new System.Windows.Forms.Label();
         this.LedConnect = new System.Windows.Forms.Label();
         this.NumberLbl = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.PortNum)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.TableGrid)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // SrvName
         // 
         resources.ApplyResources(this.SrvName, "SrvName");
         this.SrvName.Name = "SrvName";
         // 
         // label2
         // 
         resources.ApplyResources(this.label2, "label2");
         this.label2.Name = "label2";
         // 
         // PortNum
         // 
         resources.ApplyResources(this.PortNum, "PortNum");
         this.PortNum.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.PortNum.Name = "PortNum";
         this.PortNum.Value = new decimal(new int[] {
            9996,
            0,
            0,
            0});
         // 
         // ConnectBtn
         // 
         resources.ApplyResources(this.ConnectBtn, "ConnectBtn");
         this.ConnectBtn.Name = "ConnectBtn";
         this.ConnectBtn.UseVisualStyleBackColor = true;
         this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // MessagesLB
         // 
         this.MessagesLB.BackColor = System.Drawing.Color.WhiteSmoke;
         resources.ApplyResources(this.MessagesLB, "MessagesLB");
         this.MessagesLB.FormattingEnabled = true;
         this.MessagesLB.Name = "MessagesLB";
         // 
         // QuitBtn
         // 
         resources.ApplyResources(this.QuitBtn, "QuitBtn");
         this.QuitBtn.Name = "QuitBtn";
         this.QuitBtn.UseVisualStyleBackColor = true;
         this.QuitBtn.Click += new System.EventHandler(this.OnQuitClick);
         // 
         // LedImages
         // 
         this.LedImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("LedImages.ImageStream")));
         this.LedImages.TransparentColor = System.Drawing.Color.Transparent;
         this.LedImages.Images.SetKeyName(0, "bullet_red.png");
         this.LedImages.Images.SetKeyName(1, "bullet_green.png");
         this.LedImages.Images.SetKeyName(2, "bullet_blue.png");
         this.LedImages.Images.SetKeyName(3, "bullet_red_bright.png");
         this.LedImages.Images.SetKeyName(4, "bullet_green_bright.png");
         this.LedImages.Images.SetKeyName(5, "bullet_blue_bright.png");
         // 
         // LoadPlanBtn
         // 
         resources.ApplyResources(this.LoadPlanBtn, "LoadPlanBtn");
         this.LoadPlanBtn.Name = "LoadPlanBtn";
         this.LoadPlanBtn.UseVisualStyleBackColor = true;
         this.LoadPlanBtn.Click += new System.EventHandler(this.LoadPlanBtn_Click);
         // 
         // ClearPlanBtn
         // 
         resources.ApplyResources(this.ClearPlanBtn, "ClearPlanBtn");
         this.ClearPlanBtn.Name = "ClearPlanBtn";
         this.ClearPlanBtn.UseVisualStyleBackColor = true;
         this.ClearPlanBtn.Click += new System.EventHandler(this.ClearPlanBtn_Click);
         // 
         // GetStateBtn
         // 
         resources.ApplyResources(this.GetStateBtn, "GetStateBtn");
         this.GetStateBtn.Name = "GetStateBtn";
         this.GetStateBtn.UseVisualStyleBackColor = true;
         this.GetStateBtn.Click += new System.EventHandler(this.GetStateBtn_Click);
         // 
         // SendDataBtn
         // 
         resources.ApplyResources(this.SendDataBtn, "SendDataBtn");
         this.SendDataBtn.Name = "SendDataBtn";
         this.SendDataBtn.UseVisualStyleBackColor = true;
         this.SendDataBtn.Click += new System.EventHandler(this.SendDataBtn_Click);
         // 
         // StartPlanBtn
         // 
         resources.ApplyResources(this.StartPlanBtn, "StartPlanBtn");
         this.StartPlanBtn.Name = "StartPlanBtn";
         this.StartPlanBtn.UseVisualStyleBackColor = true;
         this.StartPlanBtn.Click += new System.EventHandler(this.StartPlanBtn_Click);
         // 
         // PausePlanBtn
         // 
         resources.ApplyResources(this.PausePlanBtn, "PausePlanBtn");
         this.PausePlanBtn.Name = "PausePlanBtn";
         this.PausePlanBtn.UseVisualStyleBackColor = true;
         this.PausePlanBtn.Click += new System.EventHandler(this.PausePlanBtn_Click);
         // 
         // StopPlanBtn
         // 
         resources.ApplyResources(this.StopPlanBtn, "StopPlanBtn");
         this.StopPlanBtn.Name = "StopPlanBtn";
         this.StopPlanBtn.UseVisualStyleBackColor = true;
         this.StopPlanBtn.Click += new System.EventHandler(this.StopPlanBtn_Click);
         // 
         // TableGrid
         // 
         this.TableGrid.BackgroundColor = System.Drawing.Color.White;
         this.TableGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Xangle,
            this.Yangle,
            this.Emev,
            this.Intensity,
            this.Result,
            this.ResultX,
            this.ResultZ,
            this.ResultIntensity});
         this.TableGrid.GridColor = System.Drawing.Color.White;
         resources.ApplyResources(this.TableGrid, "TableGrid");
         this.TableGrid.Name = "TableGrid";
         // 
         // Id
         // 
         this.Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
         this.Id.FillWeight = 20F;
         resources.ApplyResources(this.Id, "Id");
         this.Id.Name = "Id";
         // 
         // Xangle
         // 
         this.Xangle.FillWeight = 70F;
         resources.ApplyResources(this.Xangle, "Xangle");
         this.Xangle.Name = "Xangle";
         // 
         // Yangle
         // 
         this.Yangle.FillWeight = 70F;
         resources.ApplyResources(this.Yangle, "Yangle");
         this.Yangle.Name = "Yangle";
         // 
         // Emev
         // 
         this.Emev.FillWeight = 60F;
         resources.ApplyResources(this.Emev, "Emev");
         this.Emev.Name = "Emev";
         // 
         // Intensity
         // 
         this.Intensity.FillWeight = 90F;
         resources.ApplyResources(this.Intensity, "Intensity");
         this.Intensity.Name = "Intensity";
         // 
         // Result
         // 
         this.Result.FillWeight = 80F;
         resources.ApplyResources(this.Result, "Result");
         this.Result.Name = "Result";
         // 
         // ResultX
         // 
         this.ResultX.FillWeight = 90F;
         resources.ApplyResources(this.ResultX, "ResultX");
         this.ResultX.Name = "ResultX";
         // 
         // ResultZ
         // 
         this.ResultZ.FillWeight = 90F;
         resources.ApplyResources(this.ResultZ, "ResultZ");
         this.ResultZ.Name = "ResultZ";
         // 
         // ResultIntensity
         // 
         this.ResultIntensity.FillWeight = 90F;
         resources.ApplyResources(this.ResultIntensity, "ResultIntensity");
         this.ResultIntensity.Name = "ResultIntensity";
         // 
         // ClearLogBtn
         // 
         resources.ApplyResources(this.ClearLogBtn, "ClearLogBtn");
         this.ClearLogBtn.Name = "ClearLogBtn";
         this.ClearLogBtn.UseVisualStyleBackColor = true;
         this.ClearLogBtn.Click += new System.EventHandler(this.ClearLogBtn_Click);
         // 
         // LedFinish
         // 
         resources.ApplyResources(this.LedFinish, "LedFinish");
         this.LedFinish.ImageList = this.LedImages;
         this.LedFinish.Name = "LedFinish";
         // 
         // LedPause
         // 
         resources.ApplyResources(this.LedPause, "LedPause");
         this.LedPause.ImageList = this.LedImages;
         this.LedPause.Name = "LedPause";
         // 
         // LedProcess
         // 
         resources.ApplyResources(this.LedProcess, "LedProcess");
         this.LedProcess.ImageList = this.LedImages;
         this.LedProcess.Name = "LedProcess";
         // 
         // LedReady
         // 
         resources.ApplyResources(this.LedReady, "LedReady");
         this.LedReady.ImageList = this.LedImages;
         this.LedReady.Name = "LedReady";
         // 
         // LedNotReady
         // 
         resources.ApplyResources(this.LedNotReady, "LedNotReady");
         this.LedNotReady.ImageList = this.LedImages;
         this.LedNotReady.Name = "LedNotReady";
         // 
         // LedConnect
         // 
         resources.ApplyResources(this.LedConnect, "LedConnect");
         this.LedConnect.ImageList = this.LedImages;
         this.LedConnect.Name = "LedConnect";
         // 
         // NumberLbl
         // 
         resources.ApplyResources(this.NumberLbl, "NumberLbl");
         this.NumberLbl.Name = "NumberLbl";
         // 
         // MainControlClient
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.NumberLbl);
         this.Controls.Add(this.ClearLogBtn);
         this.Controls.Add(this.TableGrid);
         this.Controls.Add(this.StopPlanBtn);
         this.Controls.Add(this.PausePlanBtn);
         this.Controls.Add(this.StartPlanBtn);
         this.Controls.Add(this.SendDataBtn);
         this.Controls.Add(this.GetStateBtn);
         this.Controls.Add(this.ClearPlanBtn);
         this.Controls.Add(this.LoadPlanBtn);
         this.Controls.Add(this.LedFinish);
         this.Controls.Add(this.LedPause);
         this.Controls.Add(this.LedProcess);
         this.Controls.Add(this.LedReady);
         this.Controls.Add(this.LedNotReady);
         this.Controls.Add(this.LedConnect);
         this.Controls.Add(this.QuitBtn);
         this.Controls.Add(this.MessagesLB);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.ConnectBtn);
         this.Controls.Add(this.PortNum);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.SrvName);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Name = "MainControlClient";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
         this.Load += new System.EventHandler(this.OnLoad);
         ((System.ComponentModel.ISupportInitialize)(this.PortNum)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.TableGrid)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox SrvName;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.NumericUpDown PortNum;
      private System.Windows.Forms.Button ConnectBtn;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.ListBox MessagesLB;
      private System.Windows.Forms.Button QuitBtn;
      private System.Windows.Forms.Label LedConnect;
      private System.Windows.Forms.Label LedNotReady;
      private System.Windows.Forms.Label LedReady;
      private System.Windows.Forms.Label LedProcess;
      private System.Windows.Forms.Label LedPause;
      private System.Windows.Forms.Label LedFinish;
      private System.Windows.Forms.Button LoadPlanBtn;
      private System.Windows.Forms.Button ClearPlanBtn;
      private System.Windows.Forms.Button GetStateBtn;
      private System.Windows.Forms.Button SendDataBtn;
      private System.Windows.Forms.Button StartPlanBtn;
      private System.Windows.Forms.Button PausePlanBtn;
      private System.Windows.Forms.Button StopPlanBtn;
      private System.Windows.Forms.DataGridView TableGrid;
      private System.Windows.Forms.ImageList LedImages;
      private System.Windows.Forms.DataGridViewTextBoxColumn Id;
      private System.Windows.Forms.DataGridViewTextBoxColumn Xangle;
      private System.Windows.Forms.DataGridViewTextBoxColumn Yangle;
      private System.Windows.Forms.DataGridViewTextBoxColumn Emev;
      private System.Windows.Forms.DataGridViewTextBoxColumn Intensity;
      private System.Windows.Forms.DataGridViewTextBoxColumn Result;
      private System.Windows.Forms.DataGridViewTextBoxColumn ResultX;
      private System.Windows.Forms.DataGridViewTextBoxColumn ResultZ;
      private System.Windows.Forms.DataGridViewTextBoxColumn ResultIntensity;
      private System.Windows.Forms.Button ClearLogBtn;
      private System.Windows.Forms.Label NumberLbl;
   }
}

