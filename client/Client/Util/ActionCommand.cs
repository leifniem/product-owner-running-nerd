using System;
using System.Windows.Input;

namespace LoadRunnerClient
{
    /*
     * Standard-Basisklasse für Action-Commands
     */

    public class ActionCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Predicate<object> _canExec;

        public ActionCommand(Action<object> exec) : this(exec, null)
        {
        }

        public ActionCommand(Action<object> exec, Predicate<object> canExec)
        {
            if (exec == null)
            {
                throw new ArgumentNullException("execute");
            }
            _exec = exec;
            _canExec = canExec;
        }

        /// <summary>
        /// Checks if the command can be executed
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool CanExecute(object param)
        {
            return _canExec == null ? true : _canExec(param);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object param)
        {
            _exec(param);
        }
    }
}