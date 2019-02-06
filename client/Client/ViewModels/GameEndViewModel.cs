using LoadRunnerClient.MapAndModel;
using LoadRunnerClient.Network;
using LoadRunnerClient.Network.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LoadRunnerClient
{
	/// <summary>
	/// GameEndViewModel
	/// </summary>
	public class GameEndViewModel : ObservableViewModelBase
	{
		private TaskFactory taskFactory;
		private UiStateModel uiStateModel;
		private static GameEndViewModel _instance;
		private bool someoneWon = false;

		/// <summary>
		/// getInstance method
		/// </summary>
		/// <param name="uiStateModel"></param>
		/// <returns>Instance of a GameEndViewModel</returns>
		public static GameEndViewModel getInstance(UiStateModel uiStateModel)
		{
			if (_instance == null)
			{
				_instance = new GameEndViewModel(uiStateModel);
			}
			return _instance;
		}

		/// <summary>
		/// Standart constrcutor which adds a listener to PropertyChanged
		/// </summary>
		/// <param name="uiStateModel"></param>
		private GameEndViewModel(UiStateModel uiStateModel)
		{
			taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
			this.uiStateModel = uiStateModel;
			PropertyChanged += shown;
		}

		/// <summary>
		/// Method which listens to the PropertyChanged of the class and checks if the View is shown, if so
		/// the initView is called
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void shown(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "show")
			{
				if (show)
				{
					initView();
				}
			}
		}

		private ObservableCollection<ScoreModel> _scoreModels = new ObservableCollection<ScoreModel>();
		/// <summary>
		/// ObservableCollection which stores ScoreModels 
		/// </summary>
		public ObservableCollection<ScoreModel> ScoreModels
		{
			get => _scoreModels;
			set => _scoreModels = value;
		}

		private Dictionary<string, int> _scores;
		public Dictionary<string, int> Scores
		{
			get => _scores;
			set => _scores = value;
		}

		private string _winner;
		/// <summary>
		/// WinnerProperty which contains the winner as a string
		/// </summary>
		public string Winner
		{
			get => _winner;
			set
			{
				_winner = value;
				OnPropertyChanged("Winner");
			}
		}

		private string _state;

		/// <summary>
		/// Color of Game winner
		/// </summary>
		private string winnerColor;

		/// <summary>
		/// StateProperty which checks if the game ist lost or won
		/// </summary>
		public string State
		{
			get => _state;
			set
			{
				if (value.Equals(GameEndMessage.LOSS_STATUS)) {
					_state = "GAME OVER";
					someoneWon = false;
				} else if (value.Equals(GameEndMessage.QUIT_STATUS)) {
					_state = "GAME ABORTED";
					someoneWon = false;
				} else if (value.Equals(GameEndMessage.WIN_STATUS)) {
					_state = "WINNER";
					SomeoneWon = true;
				}

				OnPropertyChanged("State");
			}
		}

		public bool SomeoneWon {
			get => someoneWon;
			set
			{
				someoneWon = value;
				OnPropertyChanged("SomeoneWon");
			}
		}

		public string WinnerColor {
			get => winnerColor;
			set => winnerColor = value;
		}

		public Uri WinnerSprite
		{
			get{
				return new Uri("./Resources/Images/winner-sprite-" + winnerColor.ToLower() + ".png", UriKind.Relative);
			}
		}

		private ICommand _backToLobbyCommand;
		/// <summary>
		/// ICommand to go back to the serverlist
		/// </summary>
		public ICommand BackToLobbyCommand{
			get {
				if(_backToLobbyCommand == null){
							_backToLobbyCommand = new ActionCommand(e => BackToLobby());
				}
				return _backToLobbyCommand;
			}
		}

		public void BackToLobby(){
			this.uiStateModel.State = "ServerList";
		}

		
		/// <summary>
		/// InitView method which adds a number of scoremodel to the ObservableCollection for each user which participated in the game
		/// </summary>
		private void initView()
		{		
			if (Scores != null)
			{
				taskFactory.StartNew(() =>
				{
					ScoreModels.Clear();
					Winner = Scores.First().Key;
					foreach (KeyValuePair<string, int> score in Scores)
					{
						ScoreModels.Add(new ScoreModel(score.Key, score.Value));
					}
				}).Wait();
				
			}
		}
	}

	/// <summary>
	/// ScoreModel which contains the Username and the Score of the user
	/// </summary>
	public class ScoreModel : ObservableModelBase
	{
		private string _user;
		private int _score;
		public string User
		{
			get => _user;
			set
			{
				_user = value;
				OnPropertyChanged("User");
			}
		}

		public int Score
		{
			get => _score;
			set
			{
				_score = value;
				OnPropertyChanged("Score");
			}
		}

		public ScoreModel()
		{

		}

		public ScoreModel(string user, int score)
		{
			User = user;
			Score = score;
		}
	}
}
