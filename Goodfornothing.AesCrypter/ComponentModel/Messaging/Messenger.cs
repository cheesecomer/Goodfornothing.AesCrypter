using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Sackle.ComponentModel.Messaging
{
    /// <summary>
    /// ビューモデルからビューに状態を通知するための機能を提供するクラス。
    /// </summary>
    public class Messenger
    {
        private static Messenger _Default = new Messenger();

        /// <summary>
        /// 規定のメッセンジャー
        /// </summary>
        public static Messenger Default
        {
            get { return Messenger._Default = Messenger._Default ?? new Messenger(); }
        }

        /// <summary>
        /// メッセージの動作情報リスト
        /// </summary>
        private List<ActionInfo> list = new List<ActionInfo>();

        /// <summary>
        /// メッセージハンドラを登録します。
        /// </summary>
        /// <typeparam name="T">メッセージのタイプ</typeparam>
        /// <param name="recipient">登録対象</param>
        /// <param name="action">メッセージハンドラ</param>
        public void Register<T>(FrameworkElement recipient, Action<T> action) where T : MessageBase
        {
            list.Add(new ActionInfo { MesssageType = typeof(T), Receiver = recipient, Action = action, });
        }

        /// <summary>
        /// メッセージを送信します。
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        public void Send(MessageBase message)
        {
            var query = list.Where(
                o => 
                    o.Receiver.DataContext == message.Sender && 
                    o.MesssageType == message.GetType()
                    )
                    .Select(o => o.Action);
            foreach (var action in query.ToArray())
            {
                action.DynamicInvoke(message);
            }
        }

        /// <summary>
        /// メッセージと動作の紐付け
        /// </summary>
        private class ActionInfo
        {
            public Type MesssageType { get; set; }
            public FrameworkElement Receiver { get; set; }
            public Delegate Action { get; set; }
        }

    }
}
