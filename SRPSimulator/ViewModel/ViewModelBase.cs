using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SRPSimulator.ViewModel
{
    internal class ViewModelBase : UIElement
    {
        internal class CommandsCollection : Dictionary<string, XAMLCommand>, IDictionary
        {
            ViewModelBase model_;

            public CommandsCollection(ViewModelBase model)
                : base()
            {
                model_ = model;
            }

            public void Add(object key, object value)
            {
                base.Add((string)key, (XAMLCommand)value);
                ((XAMLCommand)value).Container = model_;
            }
        }

        public ViewModelBase()
        {
            Commands = new(this);
        }

        public CommandsCollection Commands
        { get; set; }
    }
}
