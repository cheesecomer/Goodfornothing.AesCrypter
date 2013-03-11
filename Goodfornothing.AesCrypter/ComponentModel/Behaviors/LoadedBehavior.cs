using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sackle.ComponentModel.Behaviors
{
    /// <summary>
    /// <see cref="System.Windows.FrameworkElement.Loaded"/> に <see cref="System.Windows.Input.ICommand"/> を割り当てる為の添付ビヘイビア
    /// </summary>
    public class LoadedBehavior
    {
        /// <summary>
        /// CommandParameter 依存関係プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(LoadedBehavior), new System.Windows.PropertyMetadata(0));

        /// <summary>
        /// Command 依存関係プロパティを識別します。
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(LoadedBehavior), new System.Windows.PropertyMetadata(null, LoadedBehavior.OnCommandPropertyChanged));

        /// <summary>
        /// 指定された <see cref="DependencyObject"/> から Command 添付プロパティの値を取得します。
        /// </summary>
        /// <param name="obj">プロパティ値の読み取り元の要素。</param>
        /// <returns>Command 添付プロパティの値</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadedBehavior.CommandProperty);
        }

        /// <summary>
        /// Command 添付プロパティの値を <see cref="DependencyObject"/> に設定します。
        /// </summary>
        /// <param name="obj">Command 添付プロパティを設定する要素。</param>
        /// <param name="value">設定するプロパティ値。</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedBehavior.CommandProperty, value);
        }

        /// <summary>
        /// 指定された <see cref="DependencyObject"/> から CommandParameter 添付プロパティの値を取得します。
        /// </summary>
        /// <param name="obj">プロパティ値の読み取り元の要素。</param>
        /// <returns>CommandParameter 添付プロパティの値</returns>
        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(LoadedBehavior.CommandParameterProperty);
        }

        /// <summary>
        /// CommandParameter 添付プロパティの値を <see cref="DependencyObject"/> に設定します。
        /// </summary>
        /// <param name="obj">CommandParameter 添付プロパティを設定する要素。</param>
        /// <param name="value">設定するプロパティ値。</param>
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LoadedBehavior.CommandParameterProperty, value);
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
                control.Loaded -= new RoutedEventHandler(control_Loaded);
            }

            var newCcommand = args.NewValue as ICommand;
            if (newCcommand != null)
            {
                control.Loaded += new RoutedEventHandler(control_Loaded);
            }
        }

        private static void control_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            var command = LoadedBehavior.GetCommand(control);
            var parameter = LoadedBehavior.GetCommandParameter(control);

            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
