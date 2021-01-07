using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoRecConFFMPEG.Extras
	{
	public class RelayCommand : System.Windows.Input.ICommand
		{
		private Action<object> execute;

		private Predicate<object> pCanExecute;
		private Func<bool> fCanExecute;

		private event EventHandler CanExecuteChangedInternal;

		public RelayCommand(Action<object> execute)
			: this(execute, DefaultCanExecute)
			{
			}

		public RelayCommand(Action<object> execute, Predicate<object> _canExecute)
			{
			if (execute == null)
				{
				throw new ArgumentNullException("execute");
				}

			if (_canExecute == null)
				{
				throw new ArgumentNullException("canExecute");
				}

			this.execute = execute;
			this.pCanExecute = _canExecute;
			}

		public event EventHandler CanExecuteChanged
			{
			add
				{
				System.Windows.Input.CommandManager.RequerySuggested += value;
				this.CanExecuteChangedInternal += value;
				}

			remove
				{
				System.Windows.Input.CommandManager.RequerySuggested -= value;
				this.CanExecuteChangedInternal -= value;
				}
			}

		public RelayCommand(Action<object> _execute, Func<bool> fcanExecute)
			{
			if (_execute == null)
				throw new ArgumentNullException(string.Format("{0} not implemented!", nameof(_execute)));

			execute = _execute;
			fCanExecute = fcanExecute;
			}

		public bool CanExecute(object parameter)
			{
			if (fCanExecute != null)
				{
				return this.fCanExecute();
				}

			return this.pCanExecute != null && this.pCanExecute(parameter);
			}

		public void Execute(object parameter)
			{
			this.execute(parameter);
			}

		public void OnCanExecuteChanged()
			{
			EventHandler handler = this.CanExecuteChangedInternal;
			if (handler != null)
				{
				//DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
				handler.Invoke(this, EventArgs.Empty);
				}
			}

		public void Destroy()
			{
			this.pCanExecute = _ => false;
			this.fCanExecute = null;
			this.execute = _ => { return; };
			}

		private static bool DefaultCanExecute(object parameter)
			{
			return true;
			}
		}
	}
