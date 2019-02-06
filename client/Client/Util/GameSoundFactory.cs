using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LoadRunnerClient.Util
{
	public class GameSoundFactory
	{
		/// MediaPlayer for each sound
		private MediaPlayer running = new MediaPlayer();
		private MediaPlayer climbing = new MediaPlayer();
		private MediaPlayer jumping = new MediaPlayer();
		private MediaPlayer digging = new MediaPlayer();
		//private MediaPlayer itemcollect = new MediaPlayer();
		private MediaPlayer energy = new MediaPlayer();
		private MediaPlayer creditpoints = new MediaPlayer();
		private MediaPlayer pizza = new MediaPlayer();
		private MediaPlayer lifelost = new MediaPlayer();

		/// <summary>
		/// Dictionary providing access to a Media Player by the AvailableSounds Enum
		/// </summary>
		private Dictionary<AvailableSounds, MediaPlayer> sounds = new Dictionary<AvailableSounds, MediaPlayer>();

		/// <summary>
		/// Enum of Available Sounds in Game
		/// </summary>
		public enum AvailableSounds{
			RUNNING, 
			CLIMBING, 
			JUMPING, 
			LIFE_LOST, 
			LIFE_GAINED, 
			CREDIT_POINTS,
			ENERGY_DRINK,
			DIGGING
		}


		public GameSoundFactory()
		{
			/// Loading sounds into the Media Players
			running.Open(new Uri("./Resources/Sounds/running.mp3", UriKind.Relative));
			climbing.Open(new Uri("./Resources/Sounds/climbing.mp3", UriKind.Relative));
			jumping.Open(new Uri("./Resources/Sounds/jump.mp3", UriKind.Relative));
			digging.Open(new Uri("./Resources/Sounds/dig.mp3", UriKind.Relative));
			energy.Open(new Uri("./Resources/Sounds/energy_drink.mp3", UriKind.Relative));
			creditpoints.Open(new Uri("./Resources/Sounds/credit_points.mp3", UriKind.Relative));
			pizza.Open(new Uri("./Resources/Sounds/pizza.mp3", UriKind.Relative));
			lifelost.Open(new Uri("./Resources/Sounds/hit.wav", UriKind.Relative));
			//itemcollect.Open(new Uri("./Resources/Sounds/.wav", UriKind.Relative));

			/// Add all sounds to dictionary under the related key
			sounds.Add(AvailableSounds.RUNNING, running);
			sounds.Add(AvailableSounds.JUMPING, jumping);
			sounds.Add(AvailableSounds.CLIMBING, climbing);
			sounds.Add(AvailableSounds.CREDIT_POINTS, creditpoints);
			sounds.Add(AvailableSounds.LIFE_GAINED, pizza);
			sounds.Add(AvailableSounds.ENERGY_DRINK, energy);
			sounds.Add(AvailableSounds.DIGGING, digging);
			sounds.Add(AvailableSounds.LIFE_LOST, lifelost);

			/// AutoReset of Sound
			foreach(MediaPlayer mp in sounds.Values){
				mp.MediaEnded += resetSound;
			}
		}

		/// <summary>
		/// EventHandler that resets sound so they can be played again when they finish playing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void resetSound(object sender, EventArgs e)
		{
			MediaPlayer x = sender as MediaPlayer;
			x.Stop();
			x.Position = TimeSpan.Zero;
		}

		/// <summary>
		/// Method handing out references to the MediaPlayer of the required sound
		/// </summary>
		/// <param name="sound">Key of AvailableSounds Enum</param>
		/// <returns>MediaPlayer reference for sound</returns>
		public MediaPlayer getSound(AvailableSounds sound)
		{
            sounds[sound].Stop();
			return sounds[sound];
		}
	}
}