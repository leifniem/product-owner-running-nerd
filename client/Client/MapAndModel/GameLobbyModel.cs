using LoadRunnerClient.DTOs;
using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
    /// <summary>
    /// Model for GameLobby View
    /// </summary>
    public class GameLobbyModel : ObservableModelBase
    {
        private ClientChannelHandler _clientChannelHandler;

        public ClientChannelHandler ClientChannelHandler
        {
            get => _clientChannelHandler;
            set => _clientChannelHandler = value;
        }

		/// <summary>
		/// GameLobbyKick EventHandler whichs handles a kickevent
		/// </summary>
		public event EventHandler<EventArgs> GameLobbyKick;
		/// <summary>
		/// ObservableCollection that stores SectionModel
		/// </summary>
		private readonly ObservableCollection<SectionModel> _sections = new ObservableCollection<SectionModel>();
        public ObservableCollection<SectionModel> Sections { get { return _sections; } }

		/// <summary>
		/// Section that is currently selected by the user
		/// </summary>
		private int _selectedSection = -1;
        public int selectedSection
        {
            get => _selectedSection;
            set => _selectedSection = value;
        }

		/// <summary>
		/// Property determining if session is to be started
		/// </summary>
        private bool _startProperty = false;

        public bool startProperty
        {
            get => _startProperty;
            set
            {
                if (_startProperty != value)
                {
                    _startProperty = value;
                    OnPropertyChanged("startProperty");
                }
            }
        }

		/// <summary>
		/// DTO of the Session currently in the Lobby
		/// </summary>
        private SessionDTO _session;
        public SessionDTO session { get => _session; set => _session = value; }

        #region Constructor

		/// <summary>
		/// Constructor adding the ClientChannelHandler and calculating the Buttons to select each session
		/// </summary>
		/// <param name="session">Session to display Lobby for</param>
        public GameLobbyModel(SessionDTO session)
        {
            this.ClientChannelHandler = ClientChannelHandler.getInstance();
            int numberofsections = session.mapMetaDTO.numberOfSections;
            for (int i = 1; i <= numberofsections; i++)
            {
                Sections.Add(new SectionModel(i.ToString(), (600 / numberofsections), (600/numberofsections), this));
            }
        }

		public void ExitLobby()
		{
			Sections.Clear();
			RemoveListener();
		}
        #endregion Constructor

        #region Listener
		/// <summary>
		/// Adds handlers to multiple channels
		/// </summary>
        public void AddListener()
        {
            ClientChannelHandler.sessionChannel.OnMessageReceived += listener;
            ClientChannelHandler.clientSessionsChannel.OnMessageReceived += listener;
        }

		/// <summary>
		/// Removes Lobby handlers from channels 
		/// </summary>
        public void RemoveListener()
        {
            ClientChannelHandler.sessionChannel.OnMessageReceived -= listener;
            ClientChannelHandler.clientSessionsChannel.OnMessageReceived -= listener;
        }

		#endregion Listener

		#region SectionSelection

		/// <summary>
		/// Selects a section
		/// </summary>
		/// <param name="keyword">selected Section-Button</param>
		public void SelectSection(string keyword)
        {
            /// loop through all SectionModel
            foreach (SectionModel section in Sections)
            {
                /// if content == button content we can select the section or deselect a section
                if (section.Content == keyword)
                {
                    /// if current section is the section we selected we need to deselect the sectionmodel
                    if (_selectedSection.ToString() == keyword)
                    {
                        section.pressed = false;
                        sendDeselectSectionMessage(_selectedSection);
                        _selectedSection = 0;
                    }
                    /// if the section is already selected nothing happens
                    else if (section.pressed)
                    {
                        return;
                    }
                    /// anything else will be selected and send to the server
                    else
                    {
                        section.pressed = true;
                        Int32.TryParse(keyword, out _selectedSection);
                        ClientChannelHandler.sendSelectSectionMessage(selectedSection);
                    }
                }
            }
        }

		/// <summary>
		/// Sends message if section was deselected by user
		/// </summary>
		/// <param name="section"></param>
        public void sendDeselectSectionMessage(int section)
        {
            ClientChannelHandler.sendDeselectSectionMessage(section);
        }

        #endregion SectionSelection

        #region MessageHandling
		/// <summary>
		/// Handler method for Lobby related messaging
		/// </summary>
		/// <param name="type">Type of Message</param>
		/// <param name="msg">Message Contents</param>
        public void listener(string type, string msg)
        {
            switch (type)
            {
                case SelectSectionMessage.TYPE:
                    ParseSelectSectionMessage(msg);
                    return;

                case DenySelectSectionMessage.TYPE:
                    return;

                case StartGameMessage.TYPE:
                    startProperty = true;
                    return;
				case KickMessage.TYPE:
					ParseKickMessage(msg);
					return;
            }
        }

		/// <summary>
		/// Parses messages of selected sessions and updates their models accordingly
		/// </summary>
		/// <param name="msg"></param>
        private void ParseSelectSectionMessage(string msg)
        {
            SelectSectionMessage selectSectionMessage = Serializer.Deserialize<SelectSectionMessage>(msg);

            foreach (SectionModel model in Sections)
            {
                if (selectSectionMessage.sections.ContainsKey(Int32.Parse(model.Content)))
                {
                    foreach (KeyValuePair<int, List<string>> keyValuePair in selectSectionMessage.sections)
                    {
                        if (keyValuePair.Key == Int32.Parse(model.Content))
                        {
                            model.PressedProperty = true;
                            model.User = keyValuePair.Value[0];
                            model.BorderBrush = Application.Current.Resources["COLOR_" + keyValuePair.Value[1]] as Brush;
                        }
                    }
                }
                else
                {
                    model.PressedProperty = false;
                    model.User = " ";
                }
            }
        }

		/// <summary>
		/// Parses and Executes a KickedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseKickMessage(string msg)
		{
			//KickMessage message = Serializer.Deserialize<KickMessage>(msg);
			EventHandler<EventArgs> handler = GameLobbyKick;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}
			else
			{
				Console.Error.WriteLine("GameLobby Handler not found");
			}
		}
		#endregion MessageHandling
	}

    /// <summary>
    /// Model for a section which can be selected thorugh its button representation
    /// </summary>
    public class SectionModel : ObservableViewModelBase
    {
        public string Content { get; set; }
        public bool pressed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private string _user = " ";

        public string User
        {
            get => _user; set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged("User");
                }
            }
        }

        public GameLobbyModel model { get; set; }

        /// property that is used to change the color of the border of the button
        public bool PressedProperty
        {
            get { return pressed; }
            set
            {
                pressed = value;
                OnPropertyChanged("PressedProperty");
                /// Because Background is dependant on this property.
                OnPropertyChanged("BorderBrush");
            }
        }

        private Brush _borderBrush;
        public Brush BorderBrush
        {
            get
            {
                return PressedProperty ? _borderBrush : Brushes.Green;
            }
            set
            {
                _borderBrush = value;
                OnPropertyChanged("BorderBrush");
            }
        }

        private ICommand _selectSectionCommand;

        public ICommand SelectSectionCommand
        {
            get
            {
                if (_selectSectionCommand == null)
                {
                    _selectSectionCommand = new ActionCommand(e => model.SelectSection(Content));
                }
                return _selectSectionCommand;
            }
        }

        public SectionModel(string content, int width, int height, GameLobbyModel model)
        {
            Content = content;
            Width = width;
            Height = height;
            this.model = model;
        }
    }
}