using System;
using System.Windows.Input;
using Xaml.Effects.Toolkit.Model;
namespace Assets.Editor.Models
{

    public class PasswordInputModel : DialogModel
    {
        public ICommand SelectFileCommand { get; protected set; }


        public PasswordInputModel()
        {
            this.Title = "输入资源包密码";
            this.Password = "";
        }


        /// <summary>
        /// 提交按钮事件
        /// </summary>
        protected override void Execute_Submit()
        {
            this.DialogResult = true;
        }


        protected override Boolean Can_Submit()
        {
            return true;
        }





        public String Password
        {
            get
            {
                return this.password;
            }
            set
            {
                base.SetProperty(ref this.password, value);
            }
        }

        private String password;


    }
}
