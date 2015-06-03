using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Callant;
using Windows.Devices.AllJoyn;

namespace CharacterLCD.AllJoynClient
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CoreDispatcher m_dispatcher;
        private MainPage rootPage;
        private CharacterLCDConsumer _consumer = null;

        protected async void OnPropertyChanged(string name)
        {
            await m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            });
        }

        public MainViewModel()
        {
            m_dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            rootPage = MainPage.Current;

            StartSearch();
        }

        private bool _connected = false;
        public bool IsConnected
        {
            get { return _connected; }
            set
            {
                if (value != _connected)
                {
                    _connected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }
        private string _status = "Not Connected";
        public string Status
        {
            get { return _status; }
            set {
                if (value != _status)
                {
                    _status = value;
                    if(value == "Connected" || value == "Message Send!")
                    {
                        IsConnected = true;
                    }
                    else
                    {
                        IsConnected = false;
                    }
                    OnPropertyChanged("Message");
                }
            }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public ICommand SendMessage
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    Send();
                });
            }
        }

        private void StartSearch()
        {
            CharacterLCDWatcher watcher = new CharacterLCDWatcher(new AllJoynBusAttachment());
            watcher.Added += Watcher_Added;
            Status = "Searching...";
            watcher.Start();
        }

        private async void Watcher_Added(CharacterLCDWatcher sender, AllJoynServiceInfo args)
        {
            Status = "Connecting....";
            CharacterLCDJoinSessionResult joinSessionResult = await CharacterLCDConsumer.JoinSessionAsync(args, sender);
            if(joinSessionResult.Status == AllJoynStatus.Ok)
            {
                _consumer = joinSessionResult.Consumer;
                Status = "Connected";
            }
            else
            {
                Status = "Error while connecting";
            }
        }

        private async void Send()
        {
            if(IsConnected && _consumer != null)
            {
                CharacterLCDSendResult sendResult = await _consumer.SendAsync(Message);
                if(sendResult.Status == AllJoynStatus.Ok)
                {
                    Status = "Message Send!";
                }
                else
                {
                    Status = "Error while sending message";
                }
            }
        }
    }
}
