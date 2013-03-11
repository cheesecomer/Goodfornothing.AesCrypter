using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq;

namespace Goodfornothing.AesCrypter.ComponentModel
{
    /// <summary>
    /// 変更通知及びデータエラー通知機能を有した基底のクラス
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// ディスパッチ
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// プロパティ値が変更されたときに発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase(Dispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        /// <summary>
        /// <see cref="PropertyChanged"/>イベントを発行します。
        /// </summary>  
        /// <typeparam name="TResult">プロパティの型</typeparam>
        /// <param name="property">プロパティ名を表すExpression。() => Nameのように指定する。</param>
        protected void OnPropertyChanged<TResult>(Expression<Func<TResult>> property)
        {
            if (this.PropertyChanged == null) return;

            // ラムダ式のBodyを取得する。MemberExpressionじゃなかったら駄目
            var memberEx = property.Body as MemberExpression;
            if (memberEx == null) throw new ArgumentException();

            // () => NameのNameの部分の左側に暗黙的に存在しているオブジェクトを取得する式をゲット
            var senderExpression = memberEx.Expression as ConstantExpression;

            // ConstraintExpressionじゃないと駄目
            if (senderExpression == null) throw new ArgumentException();

            // 定数なのでValueプロパティからsender用のインスタンスを得る
            var sender = senderExpression.Value;

            this.OnPropertyChanged(memberEx.Member.Name);
        }

        /// <summary>
        /// <see cref="PropertyChanged"/>イベントを発行します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.RaisePropertyChanged(propertyName);
                this.RaisePropertyChanged("HasErrors");
            }));
        }

        /// <summary>
        /// <see cref="PropertyChanged"/>イベントを発行します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null) return;
            this.Dispatcher.BeginInvoke(
                (Action)(() => this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName))));
        }

        #region IDataErrorInfo Members

        /// <summary>
        /// オブジェクトに関する間違いを示すエラー メッセージを取得します。
        /// </summary>
        public string Error
        {
            get
            {
                var dummy = new List<ValidationResult>();
                var hasErrors = !Validator.TryValidateObject(this, new ValidationContext(this, null, null), dummy, true);
                return hasErrors ? "Object has errors." : null;
            }
        }

        /// <summary>
        /// プロパティのエラー情報を取得します。
        /// </summary>
        /// <param name="columnName">プロパティ名</param>
        /// <returns>エラー情報</returns>
        /// <remarks>内部でTryValidatePropertyが実行される為、プロパティには値が設定されている必要があります。</remarks>
        public string this[string columnName]
        {
            get
            {
                try
                {
                    // TryValidatePropertyで、プロパティを検証
                    var results = new List<ValidationResult>();
                    if (Validator.TryValidateProperty(
                        GetType().GetProperty(columnName).GetValue(this, null),
                        new ValidationContext(this, null, null) { MemberName = columnName },
                        results))
                    {
                        return null;
                    }

                    // エラーがあれあ最初のエラーを返す
                    return results.First().ErrorMessage;
                }
                finally
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        #endregion
    }
}
