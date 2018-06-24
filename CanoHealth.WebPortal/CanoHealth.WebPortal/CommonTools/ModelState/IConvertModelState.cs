using System.Web.Mvc;

namespace CanoHealth.WebPortal.CommonTools.ModelState
{
    public interface IConvertModelState
    {
        string GetErrorsFromModelState(ModelStateDictionary modelState);
    }
}