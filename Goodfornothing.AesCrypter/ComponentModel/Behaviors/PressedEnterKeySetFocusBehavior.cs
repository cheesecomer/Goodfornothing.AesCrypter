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
    /// <see cref="System.Windows.UIElement"/> に <see cref="System.Windows.Input.Key.Enter"/> を押された際に、次コントロールにフォーカスを移動する為の添付ビヘイビア
    /// </summary>
    public class PressedEnterKeySetFocusBehavior
    {
        /// <summary>
        /// NextElementParameter 添付プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty NextElementParameter = DependencyProperty.RegisterAttached("NextElement",
            typeof(Control),
            typeof(PressedEnterKeySetFocusBehavior),
            new PropertyMetadata(null, PressedEnterKeySetFocusBehavior.OnNextElementChanged));

        /// <summary>
        /// 指定された <see cref="DependencyObject"/> から NextElementParameter 添付プロパティの値を取得します。
        /// </summary>
        /// <param name="obj">プロパティ値の読み取り元の要素。</param>
        /// <returns>NextElementParameter 添付プロパティの値</returns>
        public static Control GetNextElement(DependencyObject obj)
        {
            return (Control)obj.GetValue(PressedEnterKeySetFocusBehavior.NextElementParameter);
        }

        /// <summary>
        /// NextElementParameter 添付プロパティの値を <see cref="DependencyObject"/> に設定します。
        /// </summary>
        /// <param name="obj">Command 添付プロパティを設定する要素。</param>
        /// <param name="value">設定するプロパティ値。</param>
        public static void SetNextElement(DependencyObject obj, Control value)
        {
            obj.SetValue(PressedEnterKeySetFocusBehavior.NextElementParameter, value);
        }
        
        
        private static void OnNextElementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null)
            {
                return;
            }

            if (e.OldValue == null)
            {
                element.KeyDown += PressedEnterKeySetFocusBehavior.KeyDown;
            }
        }

        private static void KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.None) && (e.Key == Key.Enter))
            {
                PressedEnterKeySetFocusBehavior.GetNextElement(sender as DependencyObject).Focus();
            }
        }
    }
}
