// $Id: ConsoleWriter.cs 7760 2020-03-04 05:58:07Z onuchin $
//
// Copyright (C) 2020 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.IO;
using System.Text;

namespace MainCClient.NET
{
   /// <summary>
   ///    Class ConsoleWriterEventArgs.
   ///    Implements the <see cref="System.EventArgs" />
   /// </summary>
   /// <seealso cref="System.EventArgs" />
   public class ConsoleWriterEventArgs : EventArgs
   {
      #region Constructors and destructors

      /// <summary>
      ///    Initializes a new instance of the <see cref="ConsoleWriterEventArgs" /> class.
      /// </summary>
      /// <param name="value">The value.</param>
      public ConsoleWriterEventArgs(string value)
      {
         Value = value;
      }

      #endregion

      #region Public properties

      /// <summary>
      ///    Gets the value.
      /// </summary>
      /// <value>The value.</value>
      public string Value
      {
         get;
         set;
      }

      #endregion
   }

   /// <summary>
   ///    Class ConsoleWriter.
   ///    Implements the <see cref="System.IO.TextWriter" />
   /// </summary>
   /// <seealso cref="System.IO.TextWriter" />
   public class ConsoleWriter : TextWriter
   {
      #region Public events

      /// <summary>
      ///    Occurs when [write event].
      /// </summary>
      public event EventHandler<ConsoleWriterEventArgs> WriteEvent;

      /// <summary>
      ///    Occurs when [write line event].
      /// </summary>
      public event EventHandler<ConsoleWriterEventArgs> WriteLineEvent;

      #endregion

      #region Public properties

      /// <summary>
      ///    When overridden in a derived class, returns the <see cref="T:System.Text.Encoding" /> in which the output is
      ///    written.
      /// </summary>
      /// <value>The encoding.</value>
      public override Encoding Encoding
      {
         get
         {
            return Encoding.UTF8;
         }
      }

      #endregion

      #region Public methods

      /// <summary>
      ///    Writes a string to the text stream.
      /// </summary>
      /// <param name="value">The string to write.</param>
      public override void Write(string value)
      {
         if (WriteEvent != null) {
            WriteEvent(this, new ConsoleWriterEventArgs(value));
         }

         base.Write(value);
      }

      /// <summary>
      ///    Writes a string followed by a line terminator to the text stream.
      /// </summary>
      /// <param name="value">
      ///    The string to write. If <paramref name="value" /> is null, only the line termination characters are
      ///    written.
      /// </param>
      public override void WriteLine(string value)
      {
         if (WriteLineEvent != null) {
            WriteLineEvent(this, new ConsoleWriterEventArgs(value));
         }

         base.WriteLine(value);
      }

      #endregion
   }
}
