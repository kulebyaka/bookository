using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace BookProgress.Web.Helpers
{
    public static class UtilityHelpers
    {
        public const string SubformPrefixKey = "subformPrefix";

        public static MvcHtmlString PartialSubform<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, string partialViewName, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            String expressionText = ExpressionHelper.GetExpressionText(expression);

            ViewDataDictionary newViewData = new ViewDataDictionary(htmlHelper.ViewData);
            newViewData.Model = null;

            if (htmlHelper.ViewData.ContainsKey(SubformPrefixKey))
                newViewData[SubformPrefixKey] = CombineHtmlExpressionTexts((string)htmlHelper.ViewData[SubformPrefixKey], expressionText);
            else
                newViewData.Add(SubformPrefixKey, expressionText);

            return htmlHelper.Partial(partialViewName, modelMetadata.Model, newViewData);
        }

        public static Subform BeginSubForm<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, string actionName, string controllerName, Expression<Func<TModel, TProperty>> expression, string tagName = "div", bool forceNewSnippetId = false)
        {
            string subformPrefix = GetExpressionPrefix(htmlHelper, expression);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var url = urlHelper.Action(actionName, controllerName, new { subformPrefix = subformPrefix });
            return new Subform(htmlHelper.ViewContext, url, tagName);
        }

        public static MvcHtmlString HiddensForProperties<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string expressionPrefix = GetExpressionPrefix<TModel, TProperty>(htmlHelper, expression);

            string response = "";
            foreach (var property in modelMetadata.Properties)
            {
                if (property.ModelType == typeof(PicklistField))
                {
                    if (property.Model != null)
                    {
                        var model = (PicklistField)property.Model;
                        response += htmlHelper.Hidden(CombineHtmlExpressionTexts(expressionPrefix, property.PropertyName + ".Id"), model.Id);
                        response += htmlHelper.Hidden(CombineHtmlExpressionTexts(expressionPrefix, property.PropertyName + ".Label"), model.Label);
                    }
                }
                else if (!property.IsComplexType)
                {
                    response += htmlHelper.Hidden(CombineHtmlExpressionTexts(expressionPrefix, property.PropertyName), property.Model);
                }
            }

            return MvcHtmlString.Create(response);
        }

        private static string GetExpressionPrefix<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string expressionPrefix = ((string)htmlHelper.ViewData[SubformPrefixKey] ?? "");
            return CombineHtmlExpressionTexts(expressionPrefix, expressionText);
        }

        public static string CombineHtmlExpressionTexts(string prefix, string text)
        {
            bool combineWithDot = true;
            if (prefix.EndsWith(".") || text.StartsWith("."))
                combineWithDot = false;
            if (text.StartsWith("["))
                combineWithDot = false;
            if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(text))
                combineWithDot = false;
            return prefix + (combineWithDot ? "." : "") + text;
        }

        public static string ToQueryString(object values)
        {
            var dictionary = new RouteValueDictionary(values);
            var queryString = new StringBuilder();
            foreach (var key in dictionary.Keys)
            {
                if (queryString.Length > 0)
                {
                    queryString.Append("&");
                }
                queryString.Append(key);
                queryString.Append("=");
                queryString.Append(dictionary[key]);
            }
            return queryString.ToString();
        }

        public static HtmlString ApplicationVersion(this HtmlHelper helper)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var version = asm.GetName().Version;
            var product = asm
                .GetCustomAttributes(typeof (System.Reflection.AssemblyProductAttribute), true)
                .FirstOrDefault() as System.Reflection.AssemblyProductAttribute;

            if (version != null && product != null)
            {
                return new HtmlString(string.Format("<span>{0} v{1}.{2}.{3}.{4}</span>",
                    product.Product,
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision));
            }
            else
            {
                return new HtmlString("");
            }
        }
    }

}