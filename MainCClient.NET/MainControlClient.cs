// Copyright (C) 2020 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reactive.Linq;
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
      ///    True - plan data sent to server
      /// </summary>
      public bool IsPlanSent
      {
         get;
         private set;
      }

      /// <summary>
      ///    true - if language is Russian set in app.config
      /// </summary>
      public bool ru
      {
         get
         {
            return Program.Language == "ru";
         }
      }

      #endregion

      #region Public methods

      /// <summary>
      ///    True if server is connected
      /// </summary>
      public bool IsReady()
      {
         var ret = (Client != null) && Client.IsConnected && Client.IsReady;
         var msg = ru ? "Сервер не готов или не подключен" : "Server is not ready or not connected";
         if (!ret) {
            Console.WriteLine(msg);
         }

         return ret;
      }

      /// <summary>
      ///    Shows the plan.
      /// </summary>
      /// <param name="plan">The plan.</param>
      public void ShowPlan(List<Spot> plan)
      {
         try {
            foreach (var spot in plan) {
               var row = new[]
               {
                  spot.id.ToString(), spot.xangle.ToString(),
                  spot.zangle.ToString(), spot.energy.ToString(),
                  spot.pcount.ToString(), "", "0", "0", "0"
               };
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
         var msg = ru ? "Сервер не подключен" : "Server is not connected";
         if (!Client.IsConnected) {
            Console.WriteLine(msg);
            return false;
         }

         msg = ru ? "План не загружен на сервер" : "Plan is not loaded to the server";
         if (!Client.IsPlanLoaded || !IsPlanSent) {
            Console.WriteLine(msg);
            return false;
         }

         msg = ru ? "Исполнение плана завершено" : "Plan processing is finished";
         if (Client.IsFinished) {
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
         TableGrid.Rows.Clear();
         Client.Clear();
         IsPlanSent = false;
      }

      /// <summary>
      ///    Handles the Click event of the ConnectBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void ConnectBtn_Click(object sender, EventArgs e)
      {
         var msg = ru ? "Соединиться" : "Connect";

         if (Client.IsConnected) {
            Client.Disconnect();
            LedConnect.ImageIndex = 0;
            ConnectBtn.Invk(t => t.Text = msg);
            return;
         }

         IPaddress = SrvName.Text;

         try {
            Port = decimal.ToInt32(PortNum.Value);
         } catch (Exception ex) {
            Port = Globals.Port;
         }

         Client.Connect(IPaddress, Port);

         if (Client.IsConnected) {
            LedConnect.ImageIndex = 1;
            ConnectBtn.Invk(t => t.Text = ru ? "Отсоединиться" : "Disconnect");

            Client.SendInfo("I'm test client for MainC");
            Client.AskServerState();
            msg += ru ? " - соединение успешно" : " is connected";
            Console.WriteLine(msg);
         } else {
            msg += " не доступен";
            Console.WriteLine(msg);
            MessageBox.Show(msg, ru ? "Предупреждение" : "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if ((row.Cells[0].Value == null) || !row.Cells[0].Value.ToString().Equals(id.ToString())) {
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
         Client.AskServerState();
      }

      /// <summary>
      ///    Initializes this instance.
      /// </summary>
      private void Init()
      {
         SetConsoleOutput();
         Client = new PlanClient();
         IsPlanSent = false;

         Client.ServerStateChanged += OnStateChanged;
         Client.ServerDisconnected += OnDisconnected;
         Client.PlanResultsProcessed += UpdatePlanTable;
         Globals.Debug = true;
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
            Client.Load(PlanFile);
         } catch {
            return;
         }

         ShowPlan(Client.Plan);
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

         // test double click
         var mouseDown = Observable.FromEventPattern<MouseEventArgs>(MessagesLB, "MouseDown");
         var rect = MessagesLB.ClientRectangle;

         var pattern = new[] {1, 2, 1};
         var triple = pattern.ToObservable();
         var src = new[] {1, 1, 2, 1, 1, 4, 4, 1, 2, 1, 4, 4, 1, 2, 1, 32, 2, 3, 3, 1, 2, 1, 3, 3, 4};
         var li = src.PatternAt(pattern);

         var clicks = (from mouse in mouseDown 
                       select new {mouse.EventArgs.X, mouse.EventArgs.Y, mouse.EventArgs.Clicks}).
                      TimeInterval().
                      Where(ev => (ev.Interval.TotalMilliseconds < 510) &&
                                  rect.Contains(ev.Value.X, ev.Value.Y));

         var tt = (from click in clicks select click.Value.Clicks).SequenceEqual(pattern);
         clicks.Subscribe(ev =>
         {
            //Console.WriteLine("Triple click: X={0}, Y={1}", ev.Value.X, ev.Value.Y);
            if (ev.Value.Clicks == 1)
            { //Triple click
               MessagesLB.Items.Clear();
            }
         });
      }

      /// <summary>
      ///    Quit the program
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
               break;
            case EPlanState.READY:
               LedReady.ImageIndex = 2;
               LedReady.Invk(t => t.Text = ru ? "     Готов" : "     Ready");
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;
               LedReady.ImageIndex = 1;
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
      ///    Handles the Click event of the PausePlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void PausePlanBtn_Click(object sender, EventArgs e)
      {
         if (!CanProcessPlan()) {
            return;
         }

         Client.Pause();
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
         var msg = ru ? "План не загружен" : "Plan not loaded";
         if (!Client.IsPlanLoaded) {
            Console.WriteLine(msg);
            return;
         }

         msg = ru ? "План в процессе выполнения" : "Plan processing is ON";
         if (Client.IsProcessing) {
            Console.WriteLine(msg);
            return;
         }

         IsPlanSent = Client.Send();
      }

      /// <summary>
      ///    Re-translate Console.Write, Console.WriteLine output to Log window
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

         Client.Start();
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

         Client.Stop();
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
