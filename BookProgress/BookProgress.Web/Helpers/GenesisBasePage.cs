using System.Web.Mvc;

namespace BookProgress.Web.Helpers
{
    public abstract class GenesisBasePage<TModel> : WebViewPage<TModel>
    {
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            base.SetViewData(viewData);
            if (!viewData.ContainsKey(UtilityHelpers.SubformPrefixKey) && Context.Request.Params[UtilityHelpers.SubformPrefixKey] != null)
            {
                viewData.Add(UtilityHelpers.SubformPrefixKey, Context.Request.Params[UtilityHelpers.SubformPrefixKey]);
            }
        }

        public string NonSubmitLayout(string layoutPath)
        {
            if (Context.Request.Params["modalFormSubmit"] != null)
                return null;
            else
                return layoutPath;
        }
    }
}