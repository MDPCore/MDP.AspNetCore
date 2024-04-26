using MDP.Registration;
using System;

namespace MyLab.Module
{
    [Service<WorkService>()]
    public class MessageService : WorkService
    {
        // Fields
        private readonly string _message;


        // Constructors
        public MessageService(string message = "Hello World")
        {
            #region Contracts

            if (string.IsNullOrEmpty(message) == true) throw new ArgumentException(nameof(message));

            #endregion

            // Default
            _message = message;
        }


        // Methods
        public string GetValue()
        {
            // Return
            return _message;
        }
    }
}
