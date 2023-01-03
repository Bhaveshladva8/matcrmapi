using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Services;

namespace matcrm.service.BusinessLogic
{
    public class ProjectModuleLogic
    {
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IMateTicketService _mateTicketService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeProjectTaskService _employeeProjectTaskService;
        public ProjectModuleLogic(IEmployeeProjectService employeeProjectService,
        IEmployeeTaskService employeeTaskService,
        IMateTicketService mateTicketService,
        IEmployeeSubTaskService employeeSubTaskService,
        IEmployeeChildTaskService employeeChildTaskService,
        IEmployeeProjectTaskService employeeProjectTaskService)
        {
            _employeeProjectService = employeeProjectService;
            _mateTicketService = mateTicketService;
            _employeeTaskService = employeeTaskService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeProjectTaskService = employeeProjectTaskService;
        }        

    }
}