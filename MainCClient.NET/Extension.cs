using System;
using System.Windows.Forms;

namespace MainCClient.NET
{
   public static class Extensions
   {
      #region Public methods
      
      /// <summary>
      ///  textbox.Invoke(t => t.Text = "A");
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="control"></param>
      /// <param name="del"></param>
      public static void Invoke<T>(this T control, Action<T> del) where T : Control
      {
         if (control.InvokeRequired) {
            control.Invoke(new Action(() => del(control)));
         } else {
            del(control);
         }
      }

      #endregion
   }
}
