using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace matcrm.service.Common
{
    public class OperationResult
    {
        public OperationResult() { }

        public OperationResult(bool success) : this(success, null) { }

        public OperationResult(bool success, string errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }
        public OperationResult(bool success, HttpStatusCode httpStatusCode, string errorMessage, long? Id)
        {
            this.Success = success;
            this.StatusCode = httpStatusCode;
            this.StatusMessage = httpStatusCode.ToString();
            this.ErrorMessage = errorMessage;
            this.Id = Id;
        }

        // public OperationResult(bool success, HttpStatusCode httpStatusCode, string errorMessage, long? Id)
        // {
        //     this.Success = success;
        //     this.StatusCode = httpStatusCode;
        //     this.StatusMessage = httpStatusCode.ToString();
        //     this.ErrorMessage = errorMessage;
        //     this.Id = Id;
        // }



        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public long? Id { get; private set; }
    }

    public class OperationResult<T> where T : class
    {
        public OperationResult() { }

        public OperationResult(bool success)
        {
            this.Success = success;
        }

        public OperationResult(bool success, HttpStatusCode httpStatusCode, string errorMessage)
        {
            this.Success = success;
            this.StatusCode = httpStatusCode;
            this.StatusMessage = httpStatusCode.ToString();
            this.ErrorMessage = errorMessage;
        }

        public OperationResult(bool success, string errorMessage, T model)
        {
            this.Model = model;
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public OperationResult(bool success, HttpStatusCode httpStatusCode, string errorMessage, IEnumerable<T> stateList)
        {
            this.StateList = stateList;
            this.Success = success;
            this.StatusCode = httpStatusCode;
            this.StatusMessage = httpStatusCode.ToString();
            this.ErrorMessage = errorMessage;
        }

        public OperationResult(bool success, HttpStatusCode httpStatusCode, string errorMessage, T model)
        {
            this.Success = success;
            this.StatusCode = httpStatusCode;
            this.StatusMessage = httpStatusCode.ToString();
            this.ErrorMessage = errorMessage;
            Model = model;
        }

        public OperationResult(bool success, HttpStatusCode httpStatusCode, string errorMessage, T model,long totalCount)
        {
            this.Success = success;
            this.StatusCode = httpStatusCode;
            this.StatusMessage = httpStatusCode.ToString();
            this.ErrorMessage = errorMessage;
            Model = model;
            TotalCount = totalCount;
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public T Model { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public IEnumerable<T> StateList { get; private set; }
        public long TotalCount { get; set; }
    }
    public class DataTableResult<T> where T : class
    {
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public int draw { get; set; }

        public DataTableResult() { }

        public DataTableResult(bool success, string errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public DataTableResult(bool success, string errorMessage, IEnumerable<T> stateList, int draw)
        {
            this.data = stateList;
            this.draw = draw;
        }

        public int recordsTotal
        {
            get
            {
                if (data == null)
                    return 0;
                return data.Count();
            }
        }

        public int recordsFiltered
        {
            get
            {
                if (data == null)
                    return 0;
                return data.Count();
            }
        }

        public IEnumerable<T> data { get; private set; }
    }
    public class Result<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Message { get; set; }
        public T Model { get; set; }

        public Result()
        {
            IsSuccess = true;
            StatusCode = HttpStatusCode.OK;
            StatusMessage = nameof(HttpStatusCode.OK);
        }

        public Result(bool isSuccess, HttpStatusCode httpStatusCode)
        {
            IsSuccess = isSuccess;
            StatusCode = httpStatusCode;
            StatusMessage = httpStatusCode.ToString();
        }

        public Result(bool isSuccess, HttpStatusCode httpStatusCode, string message)
        {
            IsSuccess = isSuccess;
            StatusCode = httpStatusCode;
            StatusMessage = httpStatusCode.ToString();
            Message = message;
        }

        public Result(bool isSuccess, HttpStatusCode httpStatusCode, string message, T model)
        {
            IsSuccess = isSuccess;
            StatusCode = httpStatusCode;
            StatusMessage = httpStatusCode.ToString();
            Message = message;
            Model = model;
        }

        public Result(bool isSuccess, HttpStatusCode httpStatusCode, T model)
        {
            IsSuccess = isSuccess;
            StatusCode = httpStatusCode;
            StatusMessage = httpStatusCode.ToString();
            Model = model;
        }
    }
}