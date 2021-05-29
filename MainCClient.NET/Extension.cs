using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace MainCClient.NET
{
   public static class Extensions
   {
      #region Public methods

      /// <summary>
      ///    NumberLbl.Invk(t => t.Text = "0/0");
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="control"></param>
      /// <param name="del"></param>
      public static void Invk<T>(this T control, Action<T> del) where T : Control
      {
         if (control.InvokeRequired) {
            control.Invoke(new Action(() => del(control)));
         } else {
            del(control);
         }
      }

      public static IEnumerable<int> PatternAt(this int[] source, int[] pattern)
      {
         for (var i = 0; i < source.Length; i++) {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern)) {
               yield return i;
            }
         }
      }

      #endregion
   }
}
