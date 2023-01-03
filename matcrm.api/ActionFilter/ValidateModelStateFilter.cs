using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using matcrm.data.Models;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.api.ActionFilter
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Validate model state
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(
                    BadRequest(FetchModelError(context.ModelState)
                    ));
            }
        }

        private Result<ErrorLog> BadRequest(string message)
        {
            return new Result<ErrorLog>(matcrm.data.Models.Constant.Status.Failed, System.Net.HttpStatusCode.BadRequest, message);
        }

        private string FetchModelError(ModelStateDictionary modelState)
        {
            return string.Join(Environment.NewLine, modelState.Values
                         .SelectMany(x => x.Errors)
                         .Select(x => x.ErrorMessage));
        }
    }
}
