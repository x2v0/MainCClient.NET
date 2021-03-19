// $Id: Program.cs 7760 2020-03-04 05:58:07Z onuchin $
//
// Copyright (C) 2020 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using TM;

namespace MainCClient.NET
{
   internal static class Program
   {
      #region Constructors and destructors

      /// <summary>
      ///    Initializes static members of the <see cref="Program" /> class.
      /// </summary>
      static Program()
      {
         ReadConfig();
         Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(Language);
         Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Language);

         var customCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
         customCulture.NumberFormat.NumberDecimalSeparator = ".";

         Thread.CurrentThread.CurrentCulture = customCulture;
      }

      #endregion

      #region Public properties

      public static string Language
      {
         get;
         set;
      }

      #endregion

      #region Private methods

      /// <summary>
      ///    Handle unhandled exceptions
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
      private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         try {
            var ex = (Exception) e.ExceptionObject;
         } catch (Exception ex) {
            Console.WriteLine("UnhandledException : " + e.ExceptionObject.GetType() + " : " + ex.Message);

            try {
            } finally {
               Environment.Exit(1);
            }
         }
      }

      /// <summary>
      ///    The main entry point for the application.
      /// </summary>
      [STAThread]
      private static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

         // Add handler for UI thread exceptions
         Application.ThreadException += UIThreadException;

         // Force all WinForms errors to go through handler
         Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

         // This handler is for catching non-UI thread exceptions
         AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

         Application.Run(new MainControlClient());
      }

      /// <summary>
      ///    Clean up after yourself and save preferences
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private static void OnProcessExit(object sender, EventArgs e)
      {
      }

      private static void ReadConfig()
      {
         string setting;

         if ((setting = ConfigurationManager.AppSettings["Language"]) != null) {
            Language = setting;
         }
      }

      /// <summary>
      ///    UIs the thread exception.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="t">The <see cref="ThreadExceptionEventArgs" /> instance containing the event data.</param>
      private static void UIThreadException(object sender, ThreadExceptionEventArgs e)
      {
         var ex = e.Exception;
         Console.WriteLine("UIThreadException : " + " object - " + sender.GetType());
         ex.Message.WriteMultiLine();
      }

      #endregion
   }
}
