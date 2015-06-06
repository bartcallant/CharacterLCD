using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Callant;
using Windows.Devices.AllJoyn;
using Windows.ApplicationModel.Core;
using System;

namespace CharacterLCD.AllJoynClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }
        private string _status;
        public string Status
        {
            get { return _status; }
            set { Set("Status", ref _status, value); }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set { Set("Message", ref _message, value); }
        }
        private CharacterLCDConsumer _consumer;
        public CharacterLCDConsumer Consumer
        {
            get { return _consumer; }
            set { Set("Consumer", ref _consumer, value); }
        }

        public ICommand ConnectServer { get { return new RelayCommand(Connect); } }
        public void Connect()
        {
            Status = "Searching";
            CharacterLCDWatcher watcher = new CharacterLCDWatcher(new AllJoynBusAttachment());
            watcher.Added += Watcher_Added;
            watcher.Start();
        }

        private async void Watcher_Added(CharacterLCDWatcher sender, AllJoynServiceInfo args)
        {
            CharacterLCDJoinSessionResult joinSessionResult = await CharacterLCDConsumer.JoinSessionAsync(args, sender);

            if (joinSessionResult.Status == AllJoynStatus.Ok)
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Consumer = joinSessionResult.Consumer;
                    Consumer.SessionLost += Consumer_SessionLost;
                    Consumer.SessionMemberAdded += Consumer_SessionMemberAdded;
                    Consumer.SessionMemberRemoved += Consumer_SessionMemberRemoved;
                    Status = "Connected";
                });
            }
        }

        private async void Consumer_SessionMemberRemoved(CharacterLCDConsumer sender, AllJoynSessionMemberRemovedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Session member removed!";
            });
        }

        private async void Consumer_SessionMemberAdded(CharacterLCDConsumer sender, AllJoynSessionMemberAddedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Session member added!";
            });
        }

        private async void Consumer_SessionLost(CharacterLCDConsumer sender, AllJoynSessionLostEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Session lost!";
            });
        }

        public ICommand GetResult { get { return new RelayCommand(GetServerResult); } }
        public async void GetServerResult()
        {
            CharacterLCDSendResult result = await Consumer.SendAsync(Message);

            if (result.Status == AllJoynStatus.Ok)
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Status = "Message send!";
                });
            }
            else
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Status = "Failed to send message!";
                });
            }
        }
    }
}