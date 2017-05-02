using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Galateia.Infra.Config.Attributes;

namespace Galateia.Infra.Config
{
    /// <summary>
    ///     プロパティに設定された属性値をもとに，設定用のUIを生成します．
    /// </summary>
    public static class ConfigTools
    {
        /// <summary>
        ///     Config.ConfigurableObjectAttribute 属性のついた型の設定を行うためのUIを生成します．
        /// </summary>
        /// <param name="type">型</param>
        /// <param name="defaultMargin">コンポーネントの既定のマージン</param>
        /// <returns>DataContextに<paramref name="type" />型オブジェクトのインスタンスを指定すると，設定の編集が可能になるように生成されたUserControlのインスタンス．</returns>
        public static UserControl GenerateUserControl(Type type, Thickness defaultMargin)
        {
            var layout =
                Attribute.GetCustomAttribute(type, typeof (ConfigurableObjectAttribute)) as ConfigurableObjectAttribute;
            if (layout == null)
                throw new ArgumentException(@"CofigurableObject属性がありません．", "type");
            var numberOfColumns = layout.NumberOfColumns;

            var configElements = new List<ConfigElement>();
            var groupedElements = new Dictionary<string, List<ConfigElement>>();

            // コントロールのインスタンス化
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var ignore = Attribute.GetCustomAttribute(propertyInfo, typeof (ConfigIgnoreAttribute)) != null;
                var configurable =
                    Attribute.GetCustomAttribute(propertyInfo, typeof (ConfigurableAttribute)) as ConfigurableAttribute;
                if (ignore || configurable == null) continue;
                // Configurableとしてマークされている
                var element = GenerateElement(propertyInfo, configurable, defaultMargin);
                if (element.Span > numberOfColumns)
                    throw new ArgumentException("The span of " + propertyInfo.Name +
                                                " exceeds the number of columns: " + numberOfColumns + ".");

                // エレメントをキューに追加
                if (configurable.Group == null)
                    configElements.Add(element);
                else
                {
                    if (!groupedElements.ContainsKey(configurable.Group))
                        groupedElements.Add(configurable.Group, new List<ConfigElement>());
                    groupedElements[configurable.Group].Add(element);
                }
            }

            // グリッドへの配置
            var grid = GenerateGrid(configElements, numberOfColumns);
            foreach (var group in groupedElements)
            {
                var row = grid.RowDefinitions.Count;
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});

                var box = new GroupBox {Header = group.Key, Margin = defaultMargin};
                Grid.SetRow(box, row);
                Grid.SetColumn(box, 0);
                Grid.SetColumnSpan(box, numberOfColumns << 1);
                box.Content = GenerateGrid(group.Value, numberOfColumns);
                grid.Children.Add(box);
            }

            var control = new UserControl {Content = grid};
            return control;
        }

        private static ConfigElement GenerateElement(PropertyInfo propertyInfo, ConfigurableAttribute configurable,
            Thickness margin)
        {
            var element = new ConfigElement
            {
                Label =
                    configurable.Label == null
                        ? null
                        : new TextBlock
                        {
                            Text = configurable.Label,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = margin
                        },
                Content =
                    configurable.ControlType != null
                        ? (FrameworkElement) Activator.CreateInstance(configurable.ControlType)
                        : GenerateUserControl(propertyInfo.PropertyType, margin),
                Span = configurable.Span
            };
            element.Content.HorizontalAlignment = HorizontalAlignment.Left;
            element.Content.Margin = margin;
            // バインディングの作成
            {
                var binding = new Binding(propertyInfo.Name)
                {
                    UpdateSourceTrigger = configurable.UpdateSourceTrigger,
                    Mode = configurable.Mode
                };
                if (configurable.ValidationRuleType != null)
                    binding.ValidationRules.Add(
                        (ValidationRule) Activator.CreateInstance(configurable.ValidationRuleType));
                if (configurable.StringFormat != null)
                    binding.StringFormat = configurable.StringFormat;
                if (configurable.ConverterType != null)
                    binding.Converter = (IValueConverter) Activator.CreateInstance(configurable.ConverterType);
                // バインド
                var dependancyProperty = configurable.ControlType == null
                    ? FrameworkElement.DataContextProperty
                    : (DependencyProperty)
                        (configurable.ControlType.GetField(configurable.PropertyName + "Property",
                            BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).GetValue(null));
                element.Content.SetBinding(dependancyProperty, binding);
            }
            Type controlType = configurable.ControlType ?? typeof (UserControl);
            // Binder属性の処理
            foreach (BinderAttribute binder in Attribute.GetCustomAttributes(propertyInfo, typeof (BinderAttribute)))
            {
                var binding = new Binding(binder.SrcPropertyName)
                {
                    UpdateSourceTrigger = binder.UpdateSourceTrigger,
                    Mode = binder.Mode
                };
                if (binder.ValidationRuleType != null)
                    binding.ValidationRules.Add((ValidationRule) Activator.CreateInstance(binder.ValidationRuleType));
                if (binder.StringFormat != null)
                    binding.StringFormat = binder.StringFormat;
                if (binder.ConverterType != null)
                    binding.Converter = (IValueConverter) Activator.CreateInstance(binder.ConverterType);
                // バインド
                var dependancyProperty =
                    (DependencyProperty)
                        (controlType.GetField(binder.DstPropertyName + "Property",
                            BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).GetValue(null));
                element.Content.SetBinding(dependancyProperty, binding);
            }
            // Setter属性の処理
            foreach (SetterAttribute setter in Attribute.GetCustomAttributes(propertyInfo, typeof (SetterAttribute)))
            {
                var dependancyProperty =
                    (DependencyProperty)
                        (controlType.GetField(setter.PropertyName + "Property",
                            BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).GetValue(null));
                element.Content.SetValue(dependancyProperty, setter.Value);
            }
            return element;
        }

        private static Grid GenerateGrid(IEnumerable<ConfigElement> elements, int nColumns)
        {
            var grid = new Grid();
            // カラム追加
            for (var i = 0; i < nColumns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto)});
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
            }

            // 要素の追加
            int row = 0, column = 0; // 現在位置（ただし，columnはgridのカラムではなく，レイアウト用の論理カラム）
            List<ConfigElement> list = new List<ConfigElement>(), temp = new List<ConfigElement>();
            foreach (var element in elements)
            {
                // 溜まっているものを先に処理
                temp.Clear();
                foreach (var e in list)
                {
                    if (!AddElementToGrid(grid, e, row, ref column)) continue;
                    temp.Add(e);
                    // 改行判定
                    if (column < nColumns) continue;
                    row++;
                    column = 0;
                }
                // 追加したものをキューから削除
                foreach (var e in temp)
                    list.Remove(e);
                // 通常の追加
                if (!AddElementToGrid(grid, element, row, ref column))
                    list.Add(element);
                // 改行判定
                if (column < nColumns) continue;
                row++;
                column = 0;
            }
            // まだ残ってれば，処理
            do
            {
                temp.Clear();
                foreach (var e in list)
                {
                    if (!AddElementToGrid(grid, e, row, ref column)) continue;
                    temp.Add(e);
                    // 改行判定
                    if (column < nColumns) continue;
                    row++;
                    column = 0;
                }
                // 追加したものをキューから削除
                foreach (var e in temp)
                    list.Remove(e);
                // 改行
                row++;
                column = 0;
            } while (list.Count > 0);

            // 行追加
            var nRows = column > 0 ? row + 1 : row;
            for (var i = 0; i < nRows; i++)
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});

            return grid;
        }

        private static bool AddElementToGrid(Grid grid, ConfigElement element, int row, ref int column)
        {
            var nColumns = grid.ColumnDefinitions.Count;

            // 残りの幅が不十分なら追加しない
            if (nColumns - column < element.Span)
                return false;

            var noLable = (element.Label == null);
            if (!noLable)
            {
                Grid.SetRow(element.Label, row);
                Grid.SetColumn(element.Label, column << 1);
                grid.Children.Add(element.Label);
            }
            Grid.SetRow(element.Content, row);
            Grid.SetColumn(element.Content, (column << 1) + (noLable ? 0 : 1));
            Grid.SetColumnSpan(element.Content, (element.Span << 1) - (noLable ? 0 : 1));
            grid.Children.Add(element.Content);

            // カラムを進める
            column += element.Span;
            return true;
        }

        /// <summary>
        ///     ConfigIgnore属性が指定されていないすべてのプロパティをコピーします
        /// </summary>
        /// <typeparam name="T">ConfigBaseを継承する設定保持用クラス</typeparam>
        /// <param name="src">複製の元となるインスタンス</param>
        /// <returns>複製された新しいインスタンス</returns>
        public static T CreateCopyOf<T>(T src) where T : ConfigBase
        {
            var dst = (T) Activator.CreateInstance(typeof (T));

            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var ignore = Attribute.GetCustomAttribute(propertyInfo, typeof (ConfigIgnoreAttribute)) != null;
                if (ignore) continue;
                // ConfigIgnore属性が指定されてい「ない」
                var value = propertyInfo.GetValue(src);
                propertyInfo.SetValue(dst, value);
            }

            return dst;
        }

        /// <summary>
        ///     ConfigIgnore属性が指定されていないすべてのプロパティを比較し，一致しているかどうかを判定します．
        ///     このメソッドが正しく動作するためには，比較されるプロパティのデータ型が Equals メソッドを正しく実装している必要があります．
        /// </summary>
        /// <typeparam name="T">ConfigBaseを継承する設定保持用クラス</typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns>二つのインスタンスが一致しているかどうか．</returns>
        public static bool IsEquivalent<T>(T obj1, T obj2) where T : ConfigBase
        {
            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var ignore = Attribute.GetCustomAttribute(propertyInfo, typeof (ConfigIgnoreAttribute)) != null;
                if (ignore) continue;
                // ConfigIgnore属性が指定されてい「ない」
                var value1 = propertyInfo.GetValue(obj1);
                var value2 = propertyInfo.GetValue(obj2);
                var equal =
                    (bool)
                        propertyInfo.PropertyType.InvokeMember("Equals",
                            BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance |
                            BindingFlags.InvokeMethod, null, value1, new[] {value2});
                if (!equal)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     ConfigIgnore属性が指定されていないすべてのプロパティの値を<paramref name="src" />から<paramref name="dst" />に代入し，
        ///     代入操作によって値が変更された場合には，<paramref name="dst" />のSubstitutedイベントを発生させます．
        /// </summary>
        /// <typeparam name="T">ConfigBaseを継承する設定保持用クラス</typeparam>
        /// <param name="src">値のコピー元</param>
        /// <param name="dst">値の代入先</param>
        public static void Substitute<T>(T src, T dst) where T : ConfigBase
        {
            if (IsEquivalent(src, dst))
                return;

            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var ignore = Attribute.GetCustomAttribute(propertyInfo, typeof (ConfigIgnoreAttribute)) != null;
                if (ignore) continue;
                // ConfigIgnore属性が指定されてい「ない」
                var value = propertyInfo.GetValue(src);
                if (value != propertyInfo.GetValue(dst))
                    propertyInfo.SetValue(dst, value);
            }

            dst.RaiseSubstituted();
        }

        private class ConfigElement
        {
            public FrameworkElement Content;
            public TextBlock Label;
            public int Span;
        }
    }
}