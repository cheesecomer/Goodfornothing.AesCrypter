using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Goodfornothing.AesCrypter.ComponentModel
{
    /// <summary>
    /// デリゲートコマンド
    /// </summary>
    public sealed class DelegateCommand : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute"></param>
        public DelegateCommand(Action execute)
            : this(execute, () => true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            return _canExecute();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            _execute();
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #region ICommand
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }
        void ICommand.Execute(object parameter)
        {
            Execute();
        }
        #endregion
    }

    /// <summary>
    /// デリゲートコマンド
    /// </summary>
    /// <typeparam name="T">Parameterの型</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        private static readonly bool IS_VALUE_TYPE;

        static DelegateCommand()
        {
            IS_VALUE_TYPE = typeof(T).IsValueType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        public DelegateCommand(Action<T> execute)
            : this(execute, o => true)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(T parameter)
        {
            return this._canExecute(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(T parameter)
        {
            this._execute(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #region ICommand
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(Cast(parameter));
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(Cast(parameter));
        }
        #endregion

        /// <summary>
        /// convert parameter value
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private T Cast(object parameter)
        {
            if (parameter == null && IS_VALUE_TYPE)
            {
                return default(T);
            }

            return (T)parameter;
        }
    }
}
