using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Xaml.Effects.Toolkit.Common;
using Xaml.Effects.Toolkit.Controls;
using Xaml.Effects.Toolkit.Uitity;

namespace Xaml.Effects.Toolkit.Adorners
{
    /// <summary>
    /// 拖拽预览装饰器
    /// </summary>
    public class DragPreviewAdorner : Adorner
    {
        /// <summary>
        /// 把拖拽预览Adorner 附加到可放置的对象上
        /// </summary>
        /// <param name="AdornedElement">当前可放置的对象</param>
        /// <param name="dataObject">拖拽的数据</param>
        /// <returns></returns>
        public static DragPreviewAdorner Attach(UIElement AdornedElement, IDataObject dataObject)
        {
            var dragObject = dataObject.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null)
            {
                var adorner = new DragPreviewAdorner(AdornedElement, dragObject);
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null)
                {
                    adornerLayer.Add(adorner);
                }
                return adorner;
            }
            return null;
        }


        private DragPreviewAdorner(UIElement AdornedElement, DragObject dragObject)
            : base(AdornedElement)
        {
            this.DragObject = dragObject;
            this.IsHitTestVisible = false;
            AdornedElement.DragLeave += AdornedElement_DragLeave;
            AdornedElement.Drop += AdornedElement_Drop;
            this.Unloaded += DragControlAdorner_Unloaded;
            DragSource = dragObject.DragSource as FrameworkElement;
            if (DragSource != null)
            {
                DragSource.QueryContinueDrag += DragControlAdorner_QueryContinueDrag;
            }
            DropRectangle = new Rect(0, 0, 48, 32);
        }

        /// <summary>
        /// 用于在拖拽时捕获鼠标所在对象的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragControlAdorner_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            var screenPos = Win32Api.GetCursorPos();
            var dragpoint = AdornedElement.PointFromScreen(new Point(screenPos.X, screenPos.Y));
            if (dragpoint != DragPoint)
            {
                DragPoint = dragpoint;
                DropRectangle.X = dragpoint.X - DropRectangle.Width / 2;
                DropRectangle.Y = dragpoint.Y - DropRectangle.Height / 2;
                this.InvalidateVisual();
            }
        }

        /// <summary>
        /// 拖拽完成或取消后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragControlAdorner_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DragSource != null)
            {
                DragSource.QueryContinueDrag -= DragControlAdorner_QueryContinueDrag;
            }
            this.Unloaded -= DragControlAdorner_Unloaded;
            AdornedElement.Drop -= AdornedElement_Drop;
            AdornedElement.DragLeave -= AdornedElement_DragLeave;
            this.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 拖拽完成时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdornedElement_Drop(object sender, DragEventArgs e)
        {
            this.Detach();
            e.Handled = true;




            //生成对象
            var dItem = new DesignerItem();
            dItem.Left = DropRectangle.Left;
            dItem.Top = DropRectangle.Top;
            dItem.Width = DropRectangle.Width;
            dItem.Height = DropRectangle.Height;
            dItem.BackgroundStyle = BackgroundStyle.Image;

            //放置对象
            if (this.AdornedElement is DesignerCanvas canvas)
            {
                canvas.Children.Add(dItem);
            }
            else if (this.AdornedElement is DesignerItem item)
            {
                item.Items.Add(dItem);
            }
        }

        /// <summary>
        /// 离开当前拖拽目标，分离Adorner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdornedElement_DragLeave(object sender, DragEventArgs e)
        {
            this.Detach();
        }


        /// <summary>
        /// 分离Adorner
        /// </summary>
        public void Detach()
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }


        /// <summary>
        /// 拖拽Adorner 预览渲染
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var DESIGNER_DROP_PARENT_BORDER = (Pen)this.FindResource("DESIGNER_DROP_PARENT_BORDER");
            drawingContext.DrawRectangle(null, DESIGNER_DROP_PARENT_BORDER, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            if (DragPoint.HasValue)
            {
                var DESIGNER_DROP_TARGET_BACKGROUND = (Brush)this.FindResource("DESIGNER_DROP_TARGET_BACKGROUND");
                var DESIGNER_DROP_TARGET_BORDER = (Pen)this.FindResource("DESIGNER_DROP_TARGET_BORDER");
                drawingContext.DrawRectangle(DESIGNER_DROP_TARGET_BACKGROUND, DESIGNER_DROP_TARGET_BORDER, this.DropRectangle);
            }
            base.OnRender(drawingContext);
        }



        #region Drag Infomation

        /// <summary>
        /// 放置位置
        /// </summary>
        private Rect DropRectangle;


        /// <summary>
        /// 发起人
        /// </summary>
        public FrameworkElement DragSource { get; private set; }

        /// <summary>
        /// 当前拖放的坐标
        /// </summary>
        public Point? DragPoint { get; private set; }

        /// <summary>
        /// 拖放的数据
        /// </summary>
        public DragObject DragObject { get; private set; }

        #endregion

    }
}
