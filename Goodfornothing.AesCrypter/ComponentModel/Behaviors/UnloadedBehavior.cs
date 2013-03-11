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
    /// <see cref="System.Windows.FrameworkElement.Unloaded"/> に <see cref="System.Windows.Input.ICommand"/> を割り当てる為の添付ビヘイビア
    /// </summary>
    public class UnloadedBehavior
    {
        /// <summary>
        /// CommandParameter 添付プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty CommandParameter =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(UnloadedBehavior),
            new System.Windows.PropertyMetadata(0));

        /// <summary>
        /// Command 添付プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(UnloadedBehavior),
            new System.Windows.PropertyMetadata(null, UnloadedBehavior.OnCommandPropertyChanged));

        /// <summary>
        /// 指定された <see cref="DependencyObject"/> から Command 添付プロパティの値を取得します。
        /// </summary>
        /// <param name="obj">プロパティ値の読み取り元の要素。</param>
        /// <returns>Command 添付プロパティの値</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(UnloadedBehavior.CommandProperty);
        }

        /// <summary>
        /// Command 添付プロパティの値を <see cref="DependencyObject"/> に設定します。
        /// </summary>
        /// <param name="obj">Command 添付プロパティを設定する要素。</param>
        /// <param name="value">設定するプロパティ値。</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(UnloadedBehavior.CommandProperty, value);
        }

        /// <summary>
        /// 指定された <see cref="DependencyObject"/> から CommandParameter 添付プロパティの値を取得します。
        /// </summary>
        /// <param name="obj">プロパティ値の読み取り元の要素。</param>
        /// <returns>CommandParameter 添付プロパティの値</returns>
        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(UnloadedBehavior.CommandParameter);
        }

        /// <summary>
        /// CommandParameter 添付プロパティの値を <see cref="DependencyObject"/> に設定します。
        /// </summary>
        /// <param name="obj">CommandParameter 添付プロパティを設定する要素。</param>
        /// <param name="value">設定するプロパティ値。</param>
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(UnloadedBehavior.CommandParameter, value);
        }

        /// プロパティ変更イベント
        private static void OnCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as FrameworkElement;
            if (control == null)
            {
                return;
            }

            var oldCommand = args.OldValue as ICommand;
            if (oldCommand != null)
            {
                control.Unloaded -= new RoutedEventHandler(control_Unloaded);
            }

            var newCcommand = args.NewValue as ICommand;
            if (newCcommand != null)
            {
                control.Unloaded += new RoutedEventHandler(control_Unloaded);
            }
        }

        private static void control_Unloaded(object sender, RoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            var command = UnloadedBehavior.GetCommand(control);
            var parameter = UnloadedBehavior.GetCommandParameter(control);

            if (command.CanExecute(parameter)) { command.Execute(parameter); }
        }
    }
}
