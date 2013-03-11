using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Goodfornothing.AesCrypter.Data
{
    /// <summary>
    /// Booleanを反転する機能を提供する
    /// </summary>
    public class BooleanTurnOverConverter : IValueConverter
    {
        /// <summary>
        /// 値を変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値。</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型。</param>
        /// <param name="parameter">使用するコンバーター パラメーター。</param>
        /// <param name="culture">コンバーターで使用するカルチャ。</param>
        /// <returns>変換された値。</returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var paramDatas = new[]{
                new { TargetType = typeof(bool), ParamName = "targetType", ValueType = targetType},
                new { TargetType = typeof(bool), ParamName = "value", ValueType = value.GetType() },
            };

            paramDatas
                .Where(o => o.TargetType != o.ValueType)
                .ToList()
                .ForEach(o => { throw new ArgumentException(string.Format("パラメータ {0} に指定された型が {1} ではありません。", o.ParamName, o.TargetType.FullName)); });

            return !((bool)value);
        }

        /// <summary>
        /// 値を変換します。
        /// </summary>
        /// <param name="value">バインディング ターゲットによって生成される値。</param>
        /// <param name="targetType">変換後の型。</param>
        /// <param name="parameter">使用するコンバーター パラメーター。</param>
        /// <param name="culture">コンバーターで使用するカルチャ。</param>
        /// <returns>変換された値。</returns>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var paramDatas = new[]{
                new { TargetType = typeof(bool), ParamName = "targetType", ValueType = targetType},
                new { TargetType = typeof(bool), ParamName = "value", ValueType = value.GetType() },
            };

            paramDatas
                .Where(o => o.TargetType != o.ValueType)
                .ToList()
                .ForEach(o => { throw new ArgumentException(string.Format("パラメータ {0} に指定された型が {1} ではありません。", o.ParamName, o.TargetType.FullName)); });

            return !((bool)value);
        }
    }
}
