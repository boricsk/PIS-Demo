using System.Windows.Input;

namespace ProdInfoSys.CommandRelay
{
    /// <summary>
    /// Provides a generic implementation of the ICommand interface that relays command execution and query logic to
    /// delegates.
    /// </summary>
    /// <remarks>Use this class to create commands for UI elements by specifying the execution logic and,
    /// optionally, the logic that determines whether the command can execute. This is commonly used in MVVM patterns to
    /// bind UI actions to view model methods without creating separate command classes for each action.</remarks>
    internal class ProjectCommandRelay : ICommand
    {
        /// <summary>
        /// Represents the action to execute when the command is invoked.
        /// </summary>
        private readonly Action<object?> _execute;

        private readonly Func<object?, bool>? _canExecute;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">An optional parameter to evaluate whether the command can execute. The value and its interpretation depend
        /// on the specific command implementation.</param>
        /// <returns>true if the command can execute; otherwise, false.</returns>
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        /// <summary>
        /// Executes the associated command logic using the specified parameter.
        /// </summary>
        /// <param name="parameter">An optional parameter to pass to the command. The meaning and type of this parameter depend on the command
        /// implementation and may be null.</param>
        public void Execute(object? parameter) => _execute(parameter);

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        /// <remarks>This event is typically raised by the command source to indicate that the result of
        /// the CanExecute method may have changed. Subscribers should re-query the command's ability to execute when
        /// this event is raised. In this implementation, the event is linked to CommandManager.RequerySuggested, which
        /// automatically raises the event when conditions change that might affect command execution.</remarks>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Initializes a new instance of the ProjectCommandRelay class with the specified execute action and optional
        /// can-execute predicate.
        /// </summary>
        /// <param name="execute">The action to execute when the command is invoked. Cannot be null.</param>
        /// <param name="canExecute">An optional predicate that determines whether the command can execute with the given parameter. If null, the
        /// command is always considered executable.</param>
        public ProjectCommandRelay(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
    }
}

