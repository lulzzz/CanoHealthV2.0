using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.CommonTools.ModelState
{
    public class ConvertModelState : IConvertModelState
    {
        public string GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
            string errors = string.Join(" ", errorList.ToArray());
            return errors;
        }
    }
}