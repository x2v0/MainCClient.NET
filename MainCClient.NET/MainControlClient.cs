// $Id: MainControlClient.cs 7767 2020-03-11 16:00:49Z onuchin $
//
// Copyright (C) 2020 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
      public List<TMPlan.Spot> PlanData;

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
      ///    Gets the client.
      /// </summary>
      /// <value>The client.</value>
      public PlanClient Client
      {
         get;
         private set;
      }

      /// <summary>
      ///    Gets a value indicating whether this <see cref="MainControlClient" /> is connected.
      /// </summary>
      /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
      public bool IsConnected
      {
         get;
         private set;
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
               var row = new[] {
                  spot.id.ToString(), spot.xangle.ToString(), 
                  spot.zangle.ToString(CultureInfo.InvariantCulture), 
                  spot.energy.ToString(CultureInfo.InvariantCulture),
                  spot.pcount.ToString(CultureInfo.InvariantCulture), "", "0", "0", "0"
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
         try {
            if (InvokeRequired) {
               txt = txt.Replace("\r\n", "");
               txt = txt.Replace("\0", "0");

               MessagesLB.Invoke((MethodInvoker) delegate
               {
                  MessagesLB.Items.Add(txt);
                  MessagesLB.SelectedIndex = MessagesLB.Items.Count - 1;
               });
            } else {
               MessagesLB.Items.Add(txt);
               MessagesLB.SelectedIndex = MessagesLB.Items.Count - 1;
            }
         } catch (Exception ex) {
            // ignored
         }
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
         PlanLoaded = false;

         if (!IsConnected) {
            Console.WriteLine("Not connected");
         }

         Client.Clear();
      }

      /// <summary>
      ///    Handles the Click event of the ConnectBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void ConnectBtn_Click(object sender, EventArgs e)
      {
         if (IsConnected) {
            IsConnected = !Client.Disconnect();
            LedConnect.ImageIndex = 0;
            ConnectBtn.Text = Program.Language == "ru" ? "Соединиться" : "Connect";
            return;
         }

         IPaddress = SrvName.Text;

         try {
            Port = decimal.ToInt32(PortNum.Value);
         } catch (Exception ex) {
            Port = Globals.Port;
         }

         IsConnected = Client.Connect(IPaddress, Port);

         if (IsConnected) {
            LedConnect.ImageIndex = 1;
            ConnectBtn.Text = Program.Language == "ru" ? "Отсоединиться" : "Disconnect";

            Client.SendInfo("I'm MainCClient test client for MainC");
            Client.SendCommand(EPlanCommand.GETSTATE);
            //Client.SendCommand(EPlanCommand.CLEARPLAN);
         } else {
            var msg = "Server = " + IPaddress + " Port = " + Port + " is unavailable";
            Console.WriteLine(msg);
            MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }

      /// <summary>
      ///    Connects to server.
      /// </summary>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
      private bool ConnectToServer()
      {
         IPaddress = SrvName.Text;

         try {
            Port = decimal.ToInt32(PortNum.Value);
         } catch (Exception ex) {
            Port = Globals.Port;
         }

         IsConnected = Client.Connect(IPaddress, Port);

         if (IsConnected) {
            LedConnect.ImageIndex = 1;
            ConnectBtn.Text = Program.Language == "ru" ? "Отсоединиться" : "Disconnect";
         }

         return IsConnected;
      }

      /// <summary>
      ///    Handles the WriteEvent event of the ConsoleWriter control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="ConsoleWriterEventArgs" /> instance containing the event data.</param>
      private void ConsoleWriter_WriteEvent(object sender, ConsoleWriterEventArgs e)
      {
         var txt = "\t" + DateTime.Now + " ::  " + e.Value;
         AddMessage(txt);
      }

      /// <summary>
      ///    Handles the WriteLineEvent event of the ConsoleWriter control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="ConsoleWriterEventArgs" /> instance containing the event data.</param>
      private void ConsoleWriter_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
      {
         var txt = "\t" + DateTime.Now + " ::  " + e.Value;
         AddMessage(txt);
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
            Console.WriteLine("Not connected");
            return;
         }

         Client.ReadData = null;
         Client.SendCommand(EPlanCommand.GETSTATE);
      }

      /// <summary>
      ///    Initializes this instance.
      /// </summary>
      private void Init()
      {
         fConsoleWriter = new ConsoleWriter();
         fConsoleWriter.WriteEvent += ConsoleWriter_WriteEvent;
         fConsoleWriter.WriteLineEvent += ConsoleWriter_WriteLineEvent;
         Console.SetOut(fConsoleWriter);

         Client = new PlanClient();

         Client.ServerStateChanged += OnStateChanged;
         Client.DataBlockReceived += OnDataBlockReceived;
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
         var exedir = AppDomain.CurrentDomain.BaseDirectory;
         fd.InitialDirectory = exedir;

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
         //Client.DumpPlan(PlanData);
      }

      /// <summary>
      ///    Handles the <see cref="E:Closed" /> event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="FormClosedEventArgs" /> instance containing the event data.</param>
      private void OnClosed(object sender, FormClosedEventArgs e)
      {
         Quit();
      }

      /// <summary>
      ///    Called when [data received].
      /// </summary>
      /// <param name="data">The data.</param>
      /// <param name="bytesRead">The bytes read.</param>
      private void OnDataBlockReceived(BufferChunk data, int bytesRead)
      {
         //Client.DumpDataBlock(data, bytesRead);
         var l = bytesRead;
      }

      /// <summary>
      ///    Called when [disconnected].
      /// </summary>
      /// <param name="data">The data.</param>
      /// <param name="bytesread">The bytesread.</param>
      private void OnDisconnected()
      {
         Console.WriteLine("Disconnected from " + IPaddress + ":" + Port);

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
         var state = (EPlanState)data.state;

         if (state != EPlanState.INPROCESS) {
            Console.WriteLine(state);
         }

         switch (state) {
            case EPlanState.NOTREADY:
               LedNotReady.ImageIndex = 2;
               LedReady.ImageIndex = 0;
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;

               break;
            case EPlanState.READY:
               LedNotReady.ImageIndex = 0;
               LedReady.ImageIndex = 0;
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;
               LedReady.ImageIndex = 1;
               break;
            case EPlanState.INPROCESS:
               LedNotReady.ImageIndex = 0;
               LedReady.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedFinish.ImageIndex = 0;
               LedProcess.ImageIndex = 1;
               UpdateNumberLabel(Client.SpotsPassed, Client.SpotsTotal);
               break;
            case EPlanState.PAUSED:
               LedNotReady.ImageIndex = 0;
               LedReady.ImageIndex = 0;
               LedProcess.ImageIndex = 0;
               LedPause.ImageIndex = 0;
               LedPause.ImageIndex = 1;

               break;
            case EPlanState.FINISHED:
               LedNotReady.ImageIndex = 0;
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
         Warning();
         Client.SendCommand(EPlanCommand.PAUSEPLAN);
      }

      private void Quit()
      {
         Client.ServerStateChanged -= OnStateChanged;
         Client.DataBlockReceived -= OnDataBlockReceived;
         Client.ServerDisconnected -= OnDisconnected;
         Client.PlanResultsProcessed -= UpdatePlanTable;
         Client = null;
         Environment.Exit(1);
      }

      /// <summary>
      ///    Resets this instance.
      /// </summary>
      private void Reset()
      {
         ConnectBtn.Text = Program.Language == "ru" ? "Соединиться" : "Connect";
         LedConnect.ImageIndex = 0;
         LedNotReady.ImageIndex = 2;
         LedReady.ImageIndex = 0;
         LedProcess.ImageIndex = 0;
         LedPause.ImageIndex = 0;
         LedFinish.ImageIndex = 0;
      }

      /// <summary>
      ///    Handles the Click event of the SendDataBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void SendDataBtn_Click(object sender, EventArgs e)
      {
         Warning();
         SendPlan();
      }

      /// <summary>
      ///    Sends the plan.
      /// </summary>
      /// <param name="nblocks">The nblocks.</param>
      private void SendPlan(uint nblocks = 10)
      {
         try {
            Client.Send(PlanData, nblocks);
         } catch (Exception ex) {
            //
         }

         Console.WriteLine("PlanData sent to server.");
      }

      /// <summary>
      ///    Handles the Click event of the StartPlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void StartPlanBtn_Click(object sender, EventArgs e)
      {
         Warning();
         Client.SendCommand(EPlanCommand.STARTPLAN);
      }

      /// <summary>
      ///    Handles the Click event of the StopPlanBtn control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void StopPlanBtn_Click(object sender, EventArgs e)
      {
         Warning();
         Client.SendCommand(EPlanCommand.STOPPLAN);
      }

      /// <summary>
      ///    Updates the number label.
      /// </summary>
      /// <param name="processed">The processed.</param>
      /// <param name="total">The total.</param>
      private void UpdateNumberLabel(uint processed, uint total)
      {
         var txt = processed + "/" + total;

         if (NumberLbl.InvokeRequired) {
            NumberLbl.Invoke((MethodInvoker) delegate { NumberLbl.Text = txt; });
         } else {
            NumberLbl.Text = txt;
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

      /// <summary>
      ///    Warnings this instance.
      /// </summary>
      private void Warning()
      {
         if (!IsConnected) {
            Console.WriteLine("Not connected");
            MessageBox.Show("Not connected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
         }

         if (!PlanLoaded) {
            Console.WriteLine("Load PlanData first");
            MessageBox.Show("Load PlanData first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }

      #endregion
   }
}
