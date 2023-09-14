using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Input;

namespace Xaml.Effects.Toolkit.Model
{

    public class WindowDestroyArgs : RoutedEventArgs
    {
        /// <summary>
        /// Window.DialogResult 返回值
        /// </summary>
        public Boolean DialogResult { get; set; }


        /// <summary>
        /// 应用此时间 关闭模态窗口或普通窗口
        /// </summary>
        /// <param name="window"></param>
        public void Apply(Window window)
        {
            if (window.IsModal())
            {
                window.DialogResult = DialogResult;
            }
            else
            {
                window.Close();
            }
        }



    }


    public class DialogModel: ObservableObject
    {
        public DialogModel()
        {
            Title = "";
            this.SubmitCommand = new RelayCommand(Execute_Submit, Can_Submit);
            this.ApplyCommand = new RelayCommand(Execute_Apply, Can_Apply);
            this.CancelCommand = new RelayCommand(Execute_Cancel, Can_Cancel);
            this.ExitCommand = new RelayCommand(Execute_Exit, Can_Exit);

        }

        /// <summary>
        /// 窗口标题
        /// </summary>
        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                base.SetProperty(ref this.title, value);
            }
        }
        protected String title;



        #region dialog event

        /// <summary>
        /// 提交按钮事件
        /// </summary>
        protected virtual void Execute_Submit()
        {
            this.DialogResult = true;
        }

        protected virtual Boolean Can_Submit()
        {
            return true;
        }

        /// <summary>
        /// 应用按钮事件
        /// </summary>
        protected virtual void Execute_Apply()
        {

        }

        protected virtual Boolean Can_Apply()
        {
            return true;
        }
        /// <summary>
        /// 取消按钮事件
        /// </summary>
        protected virtual void Execute_Cancel()
        {
            this.DialogResult = false;
        }

        protected virtual Boolean Can_Cancel()
        {
            return true;
        }
        /// <summary>
        /// 取消按钮事件
        /// </summary>
        protected virtual void Execute_Exit()
        {
            this.DialogResult = true;
        }

        protected virtual Boolean Can_Exit()
        {
            return true;
        }
        /// <summary>
        /// 确定选择,需要时在派生类中重写
        /// </summary>
        public ICommand SubmitCommand { get; protected set; }

        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand ApplyCommand { get; protected set; }

        /// <summary>
        /// 取消选择,需要时在派生类中重写
        /// </summary>
        public ICommand CancelCommand { get; protected set; }

        /// <summary>
        /// 取消选择,需要时在派生类中重写
        /// </summary>
        public ICommand ExitCommand { get; protected set; }


        /// <summary>
        /// 关闭当前对话框并返回结果
        /// </summary>
        protected Boolean DialogResult
        {
            set
            {
                if (OnClose != null)
                {
                    OnClose.Invoke(this, new WindowDestroyArgs() { DialogResult = value });
                }
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event EventHandler<WindowDestroyArgs> OnClose;
        #endregion
    }
}
