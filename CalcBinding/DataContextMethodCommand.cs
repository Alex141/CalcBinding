using System;
using System.Windows;
using System.Windows.Input;

namespace CalcBinding
{
	public class DataContextMethodCommand : ICommand
	{
		string _methodName;
		FrameworkElement _targetObject;

		public DataContextMethodCommand(string methodName, FrameworkElement targetObject)
		{
			_methodName = methodName;
			_targetObject = targetObject;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			//get DataContext as late as possible to execute on the correct instance
			var dataContext = _targetObject.DataContext;
			var dataContextType = dataContext.GetType();
			var dataContextMethod = dataContextType.GetMethod(_methodName);
			dataContextMethod.Invoke(dataContext, null);
		}
	}
}
