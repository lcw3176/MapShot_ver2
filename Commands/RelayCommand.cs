﻿using System;
using System.Windows.Input;

namespace MapShot_ver2.Commands
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> executeMethod;

        public RelayCommand(Action<object> executeMethod)
        {
            this.executeMethod = executeMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }
    }
}
