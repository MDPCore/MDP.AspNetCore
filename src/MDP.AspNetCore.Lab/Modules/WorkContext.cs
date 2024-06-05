using MDP.Registration;
using System;

namespace MyLab.Module
{
    [Service<WorkContext>(singleton:true)]
    public class WorkContext
    {
        // Fields
        private readonly WorkService _workService;


        // Constructors
        public WorkContext(WorkService workService)
        {
            #region Contracts

            if (workService == null) throw new ArgumentNullException($"{nameof(workService)}=null");

            #endregion

            // Default
            _workService = workService;
        }


        // Methods
        public string GetValue()
        {
            // Return
            return _workService.GetValue();
        }
    }
}
