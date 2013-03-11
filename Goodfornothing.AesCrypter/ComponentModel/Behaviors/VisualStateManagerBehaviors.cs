using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Sackle.ComponentModel.Behaviors
{
    /// <summary>
    /// VisualStateManager添付ビヘイビア
    /// </summary>
    public class VisualStateManagerBehaviors
    {
        /// <summary>
        /// 指定された <see cref="DependencyObject"/> から VisualStateProperty 添付プロパティの値を取得します。
        /// </summary>
        /// <param name="obj">プロパティ値の読み取り元の要素。</param>
        /// <returns>VisualStateProperty 添付プロパティの値</returns>
        public static string GetVisualStateProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(VisualStatePropertyProperty);
        }

        /// <summary>
        /// VisualStateProperty 添付プロパティの値を <see cref="DependencyObject"/> に設定します。
        /// </summary>
        /// <param name="obj">VisualStateProperty 添付プロパティを設定する要素。</param>
        /// <param name="value">設定するプロパティ値。</param>
        public static void SetVisualStateProperty(DependencyObject obj, string value)
        {
            obj.SetValue(VisualStatePropertyProperty, value);
        }

        /// <summary>
        /// VisualStateProperty 依存関係プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty VisualStatePropertyProperty =
         DependencyProperty.RegisterAttached(
         "VisualStateProperty",
         typeof(string),
         typeof(VisualStateManagerBehaviors),
         new PropertyMetadata((s, e) =>
         {
             var propertyName = (string)e.NewValue;
             var control = s as Control;
             if (control == null)
                 throw new InvalidOperationException("This attached property only supports types derived from Control.");
             System.Windows.VisualStateManager.GoToState(control, (string)e.NewValue, true);
         }));
    }
}
