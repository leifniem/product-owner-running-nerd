using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadRunnerClient.Util
{
	/// <summary>
	/// <see cref="EventArgs"/> that contains a <see cref="message"/> for further information
	/// </summary>
	public class ErrorMessageEventArgs : EventArgs
	{
		public string message;
	}
}
