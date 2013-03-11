
namespace Sackle.ComponentModel.Messaging
{
    /// <summary>
    /// メッセージの基本クラス
    /// </summary>
    public abstract class MessageBase
    {
        /// <summary>
        /// メッセージ送信者を取得します。
        /// </summary>
        public object Sender { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender">メッセージの送信者</param>
        public MessageBase(object sender)
        {
            this.Sender = sender;
        }
    }
}
