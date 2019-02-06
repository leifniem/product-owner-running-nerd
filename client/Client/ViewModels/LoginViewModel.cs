using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LoadRunnerClient
{
    /// <summary>
    /// LoginViewModel to login into the sevrer
    /// </summary>
    public class LoginViewModel : ObservableViewModelBase
    {
        /// <summary>
        /// UISTateModel
        /// </summary>
        private UiStateModel uiStateModel;

        /// <summary>
        /// Model that contains logic
        /// </summary>
        private LoginModel _model;   
        
		/// <summary>
		/// Username Variable
		/// </summary>
        private string _username = "";

		/// <summary>
		/// IP holding the server address
		/// </summary>
		private string _serverAddress = "";

		public string serverAddress
		{
			get => _model.serverAddress;
			set => this._model.serverAddress = value;
		}

		public string username
        {
            get => _model.username;
            set => this._model.username = value;
        }

        /// <summary>
        /// constructor of the VM
        /// Initializes the model and listens to the PropertyChanged method of the model
        /// </summary>
        /// <param name="uiStateModel"> uiStateModel from the MainViewModel </param>
        public LoginViewModel(UiStateModel uiStateModel)
        {
            this.uiStateModel = uiStateModel;
            this._model = new LoginModel();
            this._model.PropertyChanged += _model_PropertyChanged;
        }

        /// <summary>
        /// PropertyChanged listener which changes the view to serverlist when the loggedIn property of  the model is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> Property which is changed</param>
        private void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "loggedIn":
                    uiStateModel.State = "ServerList";
                    return;
            }
        }

        private ICommand _loginCommand;

        /// <summary>
        /// LoginCommand which is bound to the LoginButton
        /// calls the Login Mehtod when pressed and the username is not empty
        /// </summary>
        public ICommand LoginIntoServerCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new ActionCommand(e => { Login(); });
                }
                return _loginCommand;
            }
        }

        /// <summary>
        /// Login method which gives the username to the clienthcannelhandler and sends a login message to the sevrer
        /// </summary>
        public void Login()
        {			
			_model.Login();      
        }
    }
}