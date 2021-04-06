// Copyright (C) 2020 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using TM;
using TMPlan;
using TMSrv;

namespace MainCClient.NET
{
   /// <summary>
   ///    Class MainControlClient.
   ///    Implements the <see cref="System.Windows.Forms.Form" />
   /// </summary>
   /// <seealso cref="System.Windows.Forms.Form" />
   public partial class MainControlClient : Form
   {
      #region Constructors and destructors

      /// <summary>
      ///    Initializes a new instance of the <see cref="MainControlClient" /> class.
      /// </summary>
      public MainControlClient()
      {
         InitializeComponent();
         Init();
      }

      #endregion

      #region  Fields

      /// <summary>
      ///    The IP address
      /// </summary>
      public string IPaddress;

      /// <summary>
      ///    The plan data
      /// </summary>
      public List<Spot> PlanData;

      /// <summary>
      ///    The plan file
      /// </summary>
      public string PlanFile;

      /// <summary>
      ///    The port
      /// </summary>
      public int Port;

      /// <summary>
      ///    The console writer to redirect Console output to Log window
      /// </summary>
      private ConsoleWriter fConsoleWriter;

      #endregion

      #region Public properties

      /// <summary>
      ///    The plan client.
      /// </summary>
      /// <value>The PlanClient.</value>
      public PlanClient Client
      {
         get;
         private set;
      }

      /// <summary>
      /// True if server is connected
      /// </summary>
      public bool IsConnected
      {
         get
         {
            return (Client != null) && Client.IsConnected;
         }
      }

      /// <summary>
      ///  True if plan processing is finished
      /// </summary>
      public bool IsPlanFinished
      {
         get
         {
            return (Client != null) && 
                   (Client.SpotsTotal == Client.SpotsPassed) && 
                   (Client.SpotsPassed > 0);
         }
      }

      /// <summary>
      /// True if plan data sent to server
      /// </summary>
      public bool IsPlanSent
      {
         get;
         private set;
      }

      /// <summary>
      /// True if plan processing is ON
      /// </summary>
      public bool IsPlanProcessing
      {
         get
         {
            return (Client != null) && 
                   (Client.SpotsPassed < Client.SpotsTotal);
         }
      }

      /// <summary>
      ///    Gets or sets a value indicating whether [plan loaded].
      /// </summary>
      /// <value><c>true</c> if [plan loaded]; otherwise, <c>false</c>.</value>
      public bool PlanLoaded
      {
         get;
         set;
      }

      /// <summary>
      ///   true - if language is Russian set in app.config 
      /// </summary>
      public bool ru 
      {
         get {
            return Program.Language == "ru";
         }
      }
      #endregion

      #region Public methods

      /// <summary>
      ///    Shows the plan.
      /// </summary>
      /// <param name="plan">The plan.</param>
      public void ShowPlan(List<Spot> plan)
      {
         try {
            foreach (var spot in plan) {
               var row = new[] {spot.id.ToString(), spot.xangle.ToString(), 
                                spot.zangle.ToString(), spot.energy.ToString(), 
                                spot.pcount.ToString(), "", "0", "0", "0"};
               TableGrid.Rows.Add(row);
            }
         } catch {
            // ignored
         }
      }

      #endregion

      #region Private methods

      /// <summary>
      ///    Adds the message.
      /// </summary>
      /// <param name="txt">The text.</param>
      private void AddMessage(string txt)
      {
         txt = txt.Replace("\r\n", "");
         txt = txt.Replace("\0", "0");

         if (InvokeRequired) {
            MessagesLB.Invoke((MethodInvoker) delegate
            {
               MessagesLB.Items.Add(txt);
               MessagesLB.SelectedIndex = MessagesLB.Items.Count - 1;
            });
         } else {
            MessagesLB.Items.Add(txt);
            MessagesLB.SelectedIndex = MessagesLB.Items.Count - 1;
         }
      }

      /// <summary>
      /// </summary>
      /// <returns></returns>
      private bool CanProcessPlan()
      {
         var msg = ru ? "Сервер не готов или не подключен" : 
                        "Server is not ready or not connected";
         if (!IsConnected || !IsServerReady) {
            Console.WriteLine(msg);
            return false;
         }

         msg = ru ? "План не загружен на сервер" : 
                    "Plan is not loaded to the server";
         if (!PlanLoaded ||
             !IsPlanSent) {
            Console.WriteLine(msg);
            return false;
         }

         msg = ru ? "Исполнение плана завершено" : 
                    "Plan processing is finished";
         if (IsPlanFinished) {
            Console.WriteLine(msg);
            return false;
         }

         return true;
      }

      /// <summary>
      ///    Handles the Click event of the ClearLogBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void ClearLogBtn_Click(object sender, EventArgs e)
      {
         MessagesLB.Items.Clear();
      }

      /// <summary>
      ///    Handles the Click event of the ClearPlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void ClearPlanBtn_Click(object sender, EventArgs e)
      {
         var msg = ru ? "Сервер не готов или не подключен" : 
                        "Server is not ready or not connected";
         if (!IsConnected || !IsServerReady) {
            Console.WriteLine(msg);
            return;
         }

         msg = ru ? "План не загружен на сервер" : 
                    "Plan is not loaded to the server";
         if (!PlanLoaded ||
             !IsPlanSent) {
            Console.WriteLine(msg);
            return;
         }

         msg = ru ? "Ошибка очистки плана на сервере" : 
                    "Failed to clear plan on server";
         if (!Client.Clear()) {
            Console.WriteLine(msg);
            return;
         }

         msg = ru ? "Запрос на очистку плана послан на сервер" : 
                    "Clear request sent to server";
         Console.WriteLine(msg);
         IsPlanSent = false;
         TableGrid.Rows.Clear();
         PlanLoaded = false;
      }

      /// <summary>
      ///    Handles the Click event of the ConnectBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void ConnectBtn_Click(object sender, EventArgs e)
      {
         var msg = ru ? "Соединиться" : "Connect";
         if (IsConnected) {
            Client.Disconnect();
            LedConnect.ImageIndex = 0;
            ConnectBtn.Text = msg;
            return;
         }

         IPaddress = SrvName.Text;

         try {
            Port = decimal.ToInt32(PortNum.Value);
         } catch (Exception ex) {
            Port = Globals.Port;
         }

         Client.Connect(IPaddress, Port);
         msg = ru ? "Сервер = " + IPaddress + " Порт = " + Port : 
                    "Server = " + IPaddress + " Port = " + Port;

         if (IsConnected) {
            LedConnect.ImageIndex = 1;
            ConnectBtn.Text = ru ? "Отсоединиться" : "Disconnect";

            Client.SendInfo("I'm test client for MainC");
            Client.SendCommand(EPlanCommand.GETSTATE);
            msg += ru ? " - соединение успешно" : " is connected";
            Console.WriteLine(msg);
         } else {
            msg += " не доступен";
            Console.WriteLine(msg);
            MessageBox.Show(msg, ru ? "Предупреждение" : "Warning", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }

      /// <summary>
      ///    Gets the row by identifier.
      /// </summary>
      /// <param name="id">The spot identifier.</param>
      /// <returns>System.Int32.</returns>
      private int GetRowById(int id)
      {
         var rows = TableGrid.Rows;
         var rowIndex = -1;

         foreach (DataGridViewRow row in rows) {
            if ((row.Cells[0].Value == null) ||
                !row.Cells[0].Value.ToString().Equals(id.ToString())) {
               continue;
            }

            rowIndex = row.Index;
            break;
         }

         return rowIndex;
      }

      /// <summary>
      ///    Handles the Click event of the GetStateBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void GetStateBtn_Click(object sender, EventArgs e)
      {
         if (!IsConnected) {
            Console.WriteLine(ru ? "Нет соединения с сервером" : 
                                   "Server is not connected");
            return;
         }

         Console.WriteLine(ru ? "Запрос о статусе послан на сервер" : 
                                "Request for status sent to server");
         Client.AskServerState();
      }

      /// <summary>
      ///    Initializes this instance.
      /// </summary>
      private void Init()
      {
         SetConsoleOutput();
         Client = new PlanClient();

         Client.ServerStateChanged += OnStateChanged;
         Client.ServerDisconnected += OnDisconnected;
         Client.PlanResultsProcessed += UpdatePlanTable;
      }

      /// <summary>
      ///    Handles the Click event of the LoadPlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void LoadPlanBtn_Click(object sender, EventArgs e)
      {
         var fd = new OpenFileDialog();
         fd.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
         fd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

         if (fd.ShowDialog() == DialogResult.Cancel) {
            return;
         }

         PlanFile = fd.FileName;

         try {
            PlanData = Client.Load(PlanFile);
         } catch {
            PlanLoaded = false;
            return;
         }

         PlanLoaded = true;
         ShowPlan(PlanData);
         Console.WriteLine(ru ? "План загружен. Число выстрелов = " : 
                                "Plan loaded. Entries = " + PlanData.Count);
      }

      /// <summary>
      ///    Handles the Closed event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="FormClosedEventArgs" /> instance containing the event data.</param>
      private void OnClosed(object sender, FormClosedEventArgs e)
      {
         Quit();
      }

      /// <summary>
      ///    Handles the WriteEvent event of the ConsoleWriter control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="ConsoleWriterEventArgs" /> instance containing the event data.</param>
      private void OnConsoleWrite(object sender, ConsoleWriterEventArgs e)
      {
         var txt = "\t" + DateTime.Now + " ::  " + e.Value;
         AddMessage(txt);
      }

      /// <summary>
      ///    Handles the WriteLineEvent event of the ConsoleWriter control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="ConsoleWriterEventArgs" /> instance containing the event data.</param>
      private void OnConsoleWriteLine(object sender, ConsoleWriterEventArgs e)
      {
         var txt = "\t" + DateTime.Now + " ::  " + e.Value;
         AddMessage(txt);
      }

      /// <summary>
      ///    Called when [disconnected].
      /// </summary>
      private void OnDisconnected()
      {
         Console.WriteLine(ru ? "Отсоединение с сервером " + IPaddress + ":" + Port : 
                                "Disconnected from " + IPaddress + ":" + Port);

         if (InvokeRequired) {
            Invoke((MethodInvoker) Reset);
         } else {
            Reset();
         }
      }

      /// <summary>
      ///    Handles the <see cref="E:Load" /> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void OnLoad(object sender, EventArgs e)
      {
         string setting;

         SrvName.Text = (setting = ConfigurationManager.AppSettings["IP"]) != null ? setting : Globals.IP;

         if ((setting = ConfigurationManager.AppSettings["Port"]) != null) {
            decimal val;
            PortNum.Value = decimal.TryParse(setting, out val) ? val : Globals.Port;
         }
      }

      /// <summary>
      ///  Quit the program
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void OnQuitClick(object sender, EventArgs e)
      {
         Quit();
      }

      /// <summary>
      ///    Called when [state changed].
      /// </summary>
      /// <param name="data">The state data.</param>
      private void OnStateChanged(StateData data)
      {
         var state = (EPlanState) data.state;

         if (state != EPlanState.INPROCESS) {
            Console.WriteLine(state);
         }

         switch (state) {
            case EPlanState.NOTREADY:
               LedReady.ImageIndex = 0;
               LedReady.Invk(t => t.Text = ru ? "     Не готов" : "     Not ready");
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;
               IsServerReady = false;
               break;
            case EPlanState.READY:
               LedReady.ImageIndex = 2;
               LedReady.Invk(t => t.Text = ru ? "     Готов" : "     Ready");
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;
               LedReady.ImageIndex = 1;
               IsServerReady = true;
               break;
            case EPlanState.INPROCESS:
               LedReady.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;
               LedProcess.ImageIndex = 1;
               NumberLbl.Invk(t => t.Text = Client.SpotsPassed + "/" + Client.SpotsTotal);
               break;
            case EPlanState.PAUSED:
               LedReady.ImageIndex = 0;
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedPause.ImageIndex = 1;
               IsPlanPaused = true;
               break;
            case EPlanState.FINISHED:
               LedReady.ImageIndex = 0;
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 1;
               break;
         }
      }

      /// <summary>
      ///  true if plan is paused
      /// </summary>
      public bool IsPlanPaused
      {
         get;
         private set;
      }

      /// <summary>
      ///  true if server is ready
      /// </summary>
      public bool IsServerReady
      {
         get;
         private set;
      }

      /// <summary>
      ///    Handles the Click event of the PausePlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void PausePlanBtn_Click(object sender, EventArgs e)
      {
         if (!CanProcessPlan()) {
            return;
         }

         if (Client.Pause()) {
            Console.WriteLine(ru ? "Запрос на паузу послан на сервер" : 
                                   "Pause request sent to serverd");
         }
      }

      /// <summary>
      ///    Exit application
      /// </summary>
      private void Quit()
      {
         Client.ServerStateChanged -= OnStateChanged;
         Client.ServerDisconnected -= OnDisconnected;
         Client.PlanResultsProcessed -= UpdatePlanTable;
         Client = null;
         Console.WriteLine("Quit");
         Environment.Exit(1);
      }

      /// <summary>
      ///    Resets this instance.
      /// </summary>
      private void Reset()
      {
         ConnectBtn.Text = ru ? "Соединиться" : "Connect";
         LedConnect.ImageIndex = 0;
         LedReady.ImageIndex = 0;
         LedProcess.ImageIndex = 0;
         LedPause.ImageIndex = 0;
         LedFinish.ImageIndex = 0;
         NumberLbl.Invk(t => t.Text = "0/0");
         Console.WriteLine("Reset");
      }

      /// <summary>
      ///    Handles the Click event of the SendDataBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void SendDataBtn_Click(object sender, EventArgs e)
      {
         var msg = ru ? "Сервер не готов или не подключен" : "Server is not ready or not connected";
         if (!IsConnected || !IsServerReady) {
            Console.WriteLine(msg);
            return;
         }

         msg = ru ? "План в процессе выполнения" : "Plan processing is ON";
         if (IsPlanProcessing) {
            Console.WriteLine(msg);
            return;
         }

         msg = ru ? "План не загружен" : "Plan not loaded";
         if (!PlanLoaded || PlanData.Count == 0) {
            Console.WriteLine(msg);
            return;
         }

         IsPlanSent = Client.Send(PlanData);

         msg = ru ? "План загружен на сервер" : "Plan data sent to server";
         if (IsPlanSent) {
            Console.WriteLine(msg);
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void SetConsoleOutput()
      {
         fConsoleWriter = new ConsoleWriter();
         fConsoleWriter.WriteEvent += OnConsoleWrite;
         fConsoleWriter.WriteLineEvent += OnConsoleWriteLine;
         Console.SetOut(fConsoleWriter);
      }

      /// <summary>
      ///    Handles the Click event of the StartPlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void StartPlanBtn_Click(object sender, EventArgs e)
      {
         if (!CanProcessPlan()) {
            return;
         }

         if (Client.Start()) {
            var msg = ru ? "Запрос на старт плана послан на сервер" : "Start request sent to server";
            Console.WriteLine(msg);
         }
      }

      /// <summary>
      ///    Handles the Click event of the StopPlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void StopPlanBtn_Click(object sender, EventArgs e)
      {
         if (!CanProcessPlan()) {
            return;
         }

         if (Client.Stop()) {
            var msg = ru ? "Запрос на остановку плана послан на сервер" : "Stop request sent to server";
            Console.WriteLine(msg);
         }
      }

      /// <summary>
      ///    Updates the plan table.
      /// </summary>
      /// <param name="results">The results.</param>
      private void UpdatePlanTable(List<SpotResult> results)
      {
         var rows = TableGrid.Rows;

         foreach (var spot in results) {
            var id = spot.id;
            var rowIndex = GetRowById(id);
            if (rowIndex < 0) {
               continue;
            }

            var row = rows[rowIndex];
            row.Cells[5].Value = "OK";
            row.Cells[6].Value = spot.result_xangle.ToString();
            row.Cells[7].Value = spot.result_zangle.ToString();
            row.Cells[8].Value = spot.result_pcount.ToString();
            row.Cells[8].Value = spot.result_pcount.ToString();
         }
      }

      #endregion
   }
}
