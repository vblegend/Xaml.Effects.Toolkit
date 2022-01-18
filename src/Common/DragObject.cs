using System;
using System.Windows;

namespace Xaml.Effects.Toolkit.Common
{

    internal static class DragTypes
    {
        public static String Control = "Xaml.Effects.Toolkit.Control";
        public static String Image = "Xaml.Effects.Toolkit.Image";
    }







    /// <summary>
    /// 拖拽的数据对象
    /// </summary>
    public class DragObject
    {
        /// <summary>
        /// 创建一个拖拽的数据
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="data">数据内容</param>
        public DragObject(DependencyObject source, String type, Object data)
        {
            this.DragSource = source;
            this.Type = type;
            this.Data = data;
        }

        public void SetData(Object data)
        {
            this.Data = data;
        }

        /// <summary>
        /// 发起者
        /// </summary>
        public DependencyObject DragSource { get; private set; }

        /// <summary>
        /// 拖拽的数据类型
        /// </summary>
        public String Type { get; private set; }

        /// <summary>
        /// 拖拽的数据内容
        /// </summary>
        public Object Data { get; private set; }
    }
}
