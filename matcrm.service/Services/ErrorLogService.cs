using System;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class ErrorLogService : Service<ErrorLog>, IErrorLogService
    {
        private IUnitOfWork _unitofWork;
        public ErrorLogService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitofWork = unitOfWork;
        }

        public void LogException(Exception ex)
        {
            ErrorLog errorLogObj = new ErrorLog();
            errorLogObj.CreatedTime = DateTime.UtcNow;
            if (ex.InnerException != null)
                errorLogObj.InnerException = ex.InnerException.Message;
            errorLogObj.Message = ex.Message;
            errorLogObj.Source = ex.Source;
            errorLogObj.StackTrace = ex.StackTrace;
            _unitofWork.ErrorLogRepository.AddAsync(errorLogObj);
            _unitofWork.CommitAsync();
        }

        public void LogMessage(string Message)
        {
            ErrorLog errorLogObj = new ErrorLog();
            errorLogObj.CreatedTime = DateTime.Now;
            errorLogObj.Message = Message;
            _unitofWork.ErrorLogRepository.AddAsync(errorLogObj);
            _unitofWork.CommitAsync();
        }
    }
    public partial interface IErrorLogService : IService<ErrorLog>
    {
        void LogException(Exception ex);
        void LogMessage(string Message);

    }
}