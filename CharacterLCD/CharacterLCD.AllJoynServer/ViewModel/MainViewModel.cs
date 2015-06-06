using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Windows.Devices.AllJoyn;
using Callant;
using Windows.ApplicationModel.Core;
using System;

namespace CharacterLCD.AllJoynServer.ViewModel
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
        private string _status = "Not running";
        public string Status
        {
            get { return _status; }
            set { Set("Status", ref _status, value); }
        }

        private CharacterLCDProducer _producer;
        public CharacterLCDProducer Producer
        {
            get { return _producer; }
            set { Set("Producer", ref _producer, value); }
        }

        public ICommand StartServer { get { return new RelayCommand(Start); } }
        public ICommand StopServer { get { return new RelayCommand(Stop); } }

        public void Start()
        {
            CharacterLCDProducer p = new CharacterLCDProducer(new AllJoynBusAttachment());
            p.Service = new CharacterLCDService();
            Producer = p;
            Producer.Start();
            Producer.SessionLost += Producer_SessionLost;
            Producer.SessionMemberAdded += Producer_SessionMemberAdded;
            Producer.SessionMemberRemoved += Producer_SessionMemberRemoved;
            Producer.Stopped += Producer_Stopped;
            Status = "Running";
        }
        public void Stop()
        {
            Producer.Stop();
            Status = "Not running";
        }

        private async void Producer_Stopped(CharacterLCDProducer sender, AllJoynProducerStoppedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Stoped!";
            });
        }

        private async void Producer_SessionMemberRemoved(CharacterLCDProducer sender, AllJoynSessionMemberRemovedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Session member removed!";
            });
        }

        private async void Producer_SessionMemberAdded(CharacterLCDProducer sender, AllJoynSessionMemberAddedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Session member added!";
            });
        }

        private async void Producer_SessionLost(CharacterLCDProducer sender, AllJoynSessionLostEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = "Session lost!";
            });
        }
    }
}