using System;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Win32;
using Prism.Interactivity.InteractionRequest;

namespace TrumpSoftware.Wpf.TriggerActions
{
    public class OpenFileDialogTriggerAction : TriggerAction<FrameworkElement>
    {
        #region Properties

        #region Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (OpenFileDialogTriggerAction));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region Filter

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof (string), typeof (OpenFileDialogTriggerAction));

        public string Filter
        {
            get { return (string) GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        #endregion

        #endregion

        #region Overridden members

        protected override void Invoke(object parameter)
        {
            var e = parameter as InteractionRequestedEventArgs;
            if (e == null)
                return;

            var dialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter,
            };

            var dialogResult = dialog.ShowDialog();

            var confirmation = e.Context as IConfirmation;
            if (confirmation != null)
            {
                confirmation.Confirmed = (dialogResult == true);
            }

            if (dialogResult == true)
            {
                e.Context.Content = dialog.FileName;
            }

            var callback = e.Callback;
            if (callback != null)
                callback.Invoke();
        }

        #endregion
    }
}
