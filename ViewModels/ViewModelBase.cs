using Catawba.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Catawba.ViewModels
{
    /// <summary>
    /// This is the parent class that all viewModels will inherit from. It contains components necessary for navigating views,
    /// enabling and disabling the popup, raising the onPropertyChanged event for properties to notify the UI of change, and controlling
    /// accesss to certain view. Note* views require the popup UI component for the popup to function.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DispatcherTimer popupTimer;
        public NavigationService navigator { get; set; }

        protected string _popupType;
        public string popupType
        {
            get => _popupType;
            set
            {
                _popupType = value;
                OnPropertyChanged(nameof(popupType));
            }
        }

        protected bool _windowIsEnabled;
        public bool windowIsEnabled
        {
            get => _windowIsEnabled;
            set
            {
                _windowIsEnabled = value;
                OnPropertyChanged(nameof(windowIsEnabled));
            }
        }

        protected Visibility _popupVisibility;
        public Visibility popupVisibility
        {
            get => _popupVisibility;
            set
            {
                _popupVisibility = value;
                OnPropertyChanged(nameof(popupVisibility));
            }
        }
        protected string _popupMessage;
        public string popupMessage
        {
            get => _popupMessage;
            set
            {
                _popupMessage = value;
                OnPropertyChanged(nameof(popupMessage));
            }
        }

        protected ICommand _optionCommand;
        public ICommand optionCommand
        {
            get => _optionCommand;
            set
            {
                _optionCommand = value;
                OnPropertyChanged(nameof(optionCommand));
            }
        }
        public ShowPopupCommand showPopupCommand { get; set; }
        public ClosePopupCommand closePopupCommand { get; set; }
        public ClosePopupCommand popupOptionCommand { get; set; }

        public ViewModelBase()
        {
            popupTimer = new DispatcherTimer();
            windowIsEnabled = true;
            popupMessage = "";
            popupVisibility = Visibility.Collapsed;
            showPopupCommand = new ShowPopupCommand(this);
            closePopupCommand = new ClosePopupCommand(this,false);
            popupOptionCommand = new ClosePopupCommand(this, true);
        }

        /// <summary>
        /// Displays a confirmation popup with the specified message. That requires the user to click ok to proceed.
        /// </summary>
        /// <param name="message">The message to display in the popup.</param>
        public void displayConfirmationPopup(string message)
        {
            popupType = "confirm";
            windowIsEnabled = false;
            popupMessage = message;
            popupVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays a timed popup with the specified message. This current timer is set to 1.2 seconds
        /// </summary>
        /// <param name="message">The message to display in the popup.</param>
        public void displayTimedPopup(string message)
        {
            popupType = "timed";
            popupMessage = message;
            popupVisibility = Visibility.Visible;

            popupTimer.Interval = TimeSpan.FromSeconds(1.2);
            popupTimer.Tick += PopupTimer_Tick;
            popupTimer.Start();
        }

        /// <summary>
        /// Displays an option popup with the specified message and command.
        /// </summary>
        /// <param name="message">The message to display in the popup.</param>
        /// <param name="optionCommand">The command to execute when the user selects the yes option.</param>
        public void displayOptionPopup(string message, ICommand optionCommand)
        {
            popupType = "option";
            this.optionCommand = optionCommand;
            windowIsEnabled = false;
            popupMessage = message;
            popupVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Display a popup that needs to be manually closed
        /// </summary>
        /// <param name="message">The message to display in the popup.</param>
        public void displayWaitingPopup(string message)
        {
            popupType = "timed";
            windowIsEnabled = false;
            popupMessage = message;
            popupVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Closes the popup and optionally executes the option command.
        /// </summary>
        /// <param name="executeOption">Specifies whether to execute the option command.</param>
        public void closePopup(bool executeOption)
        {
            if (optionCommand != null && executeOption)
            {
                optionCommand.Execute(this);
                optionCommand = null;
            }

            windowIsEnabled = true;
            popupMessage = "";
            popupVisibility = Visibility.Collapsed;
        }

        private void PopupTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer and close the popup
            popupTimer.Stop();
            closePopup(false);
        }

        // I stole this https://stackoverflow.com/questions/37787388/how-to-force-a-ui-update-during-a-lengthy-task-on-the-ui-thread
        // This allows the ui to update while waiting for a thread to finish
        protected void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
