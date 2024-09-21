using System;
using System.Windows.Input;

namespace SRPSimulator.ViewModel
{
    using CanExecuteHandler = Func<object, bool>;
    
    public class XAMLCommand : ICommand
    {
        public string Text 
        { get; set; }

        public string HandlerExecute
        { get; set; }

        public string HandlerCanExecute
        { get; set; }

        internal ViewModelBase Container 
        { get; set; }

        public XAMLCommand()
            : base()
        { }

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private event EventHandler execute_;
        private event CanExecuteHandler canExecute_;
        //public event System.Windows.Input.CanExecuteRoutedEventHandler CanExecuted;

        public void Execute(object parameter)
        {
            if (execute_ == null)
                execute_ = (EventHandler)CreateHandler(HandlerExecute,
                    typeof(EventHandler));
            if (execute_ != null)
                execute_(parameter, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute_ == null)
                canExecute_ = (CanExecuteHandler)CreateHandler(HandlerCanExecute, 
                    typeof(CanExecuteHandler));
            return canExecute_ != null ? canExecute_(parameter) : false;
        }

        public object CreateHandler(string handlerName, Type handlerType)
        {
            if (handlerName is null)
                return null;
            handlerName.Trim();
            if (handlerName.Length == 0)
                return null;
            var methodInfo = Container.GetType().GetMethod(handlerName);
            if (methodInfo == null)
                return null;
            return Delegate.CreateDelegate(handlerType, Container, methodInfo.Name, true, true);
        }
    }
}
