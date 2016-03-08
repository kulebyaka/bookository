using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using BookProgress.Web.Helpers.Utils;

namespace BookProgress.Web.Helpers
{
    /// <summary>
    /// Helpers for decorating various elements with bootstrap markup.
    /// </summary>
    public static class BootstrapHelpers
    {
        private const int DefaltLabelCols = 3;

        public static BtTable BtTable(this HtmlHelper html)
        {
            return new BtTable(html);
        }

        public static BtPaging BtPaging(this HtmlHelper html)
        {
            return new BtPaging(html);
        }

        
        /// <summary>
        /// Renders one bootstrap form-group div element.
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <param name="html">Html helper</param>
        /// <param name="label">Contents of the label part of the form-group</param>
        /// <param name="input">Contents of the input part of the form-group</param>
        /// <returns>string with HTML markup</returns>
        public static MvcHtmlString BtInputGroup<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, MvcHtmlString label,
            MvcHtmlString input)
        {
            var validationError = "";
            if (!html.ViewData.ModelState.IsValidField(ExpressionHelper.GetExpressionText(expression)))
            {
                validationError = html.ValidationMessageFor(expression).ToHtmlString();
            }

            // enclosing div container:
            var groupDiv = new TagBuilder("div");
            groupDiv.MergeAttribute("class", "form-group");

            // label
            if (label != null)
            groupDiv.InnerHtml += label.ToHtmlString();

            // input
            var inputdiv = new TagBuilder("div");
            inputdiv.AddCssClass("col-sm-" + (12 - DefaltLabelCols));
            if (!String.IsNullOrEmpty(validationError))
            {
                inputdiv.AddCssClass("input-validation-error");
            }
            inputdiv.InnerHtml += input.ToHtmlString();
            inputdiv.InnerHtml += validationError;
            groupDiv.InnerHtml += inputdiv.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(groupDiv.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString BtInputGroup<TModel>(this HtmlHelper<TModel> html, MvcHtmlString input)
        {
            // enclosing div container:
            var groupDiv = new TagBuilder("div");
            groupDiv.MergeAttribute("class", "input-group");
            groupDiv.InnerHtml += input.ToHtmlString();

            return new MvcHtmlString(groupDiv.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Span<TModel>(this HtmlHelper<TModel> helper, MvcHtmlString input, object htmlAttributes=null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var span = new TagBuilder("span");
            span.MergeAttributes(attributes);
            span.InnerHtml += input.ToHtmlString();
            return MvcHtmlString.Create(span.ToString());
        }


        public static BtForm BtForm(this HtmlHelper html)
        {
            return new BtForm(html.ViewContext);
        }


        /// <summary>
        /// Renders input label with bootstrap markup
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TProperty">Model property</typeparam>
        /// <param name="html">Html</param>
        /// <param name="expression">Model property for the input</param>
        /// <returns>HTML markup</returns>
        public static MvcHtmlString BtLabel<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression)
        {
            var cssClasses = new List<string> {"col-sm-" + DefaltLabelCols, "control-label"};

            // labels of required have "required" css class in addition:
            bool required = ModelMetadata.FromLambdaExpression(expression, html.ViewData).IsRequired;
            if (required) cssClasses.Add("required");

            return html.LabelFor(expression, new {@class = string.Join(" ", cssClasses)});
        }

        public static MvcHtmlString BtInputText<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression)
        {
            var label = html.BtLabel(expression);
            var input = html.TextBoxFor(expression, new { @class = "form-control" });

            return html.BtInputGroup(expression, label, input);
        }
        
        public static MvcHtmlString BtInputTextWithoutLabel<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attributes.Add("class", "form-control");
            return html.TextBoxFor(expression, attributes);
        }

        public static MvcHtmlString BtTwoInputText<TModel, T1Property, T2Property>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, T1Property>> firstExp, Expression<Func<TModel, T2Property>> secondExp)
        {
            // enclosing div container:
            var groupDiv = new TagBuilder("div");
            groupDiv.MergeAttribute("class", "form-group");
            html.TwoColumnDivHelper(ref groupDiv, firstExp);
            html.TwoColumnDivHelper(ref groupDiv, secondExp);
            return new MvcHtmlString(groupDiv.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Renders input label with bootstrap markup and JavaScript for select datetime
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TProperty">Model property</typeparam>
        /// <param name="html">Html</param>
        /// <param name="expression">Model property for the input</param>
        /// <returns>HTML markup</returns>
        public static MvcHtmlString BtDatetime<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BtDatetimeWidget(html, expression, WebConfigurationManager.AppSettings["FormatDateTime"], "hour", "year");
        }

        /// <summary>
        /// Renders input label with bootstrap markup and JavaScript for select date
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TProperty">Model property</typeparam>
        /// <param name="html">Html</param>
        /// <param name="expression">Model property for the input</param>
        /// <returns>HTML markup</returns>
        public static MvcHtmlString BtDate<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression) 
        {
            return BtDatetimeWidget(html, expression, WebConfigurationManager.AppSettings["FormatDate"], "month", null);
        }

        /// <summary>
        /// Renders input label with bootstrap markup and JavaScript for select time
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TProperty">Model property</typeparam>
        /// <param name="html">Html</param>
        /// <param name="expression">Model property for the input</param>
        /// <returns>HTML markup</returns>
        public static MvcHtmlString BtTime<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression)
        {
            return BtDatetimeWidget(html, expression, WebConfigurationManager.AppSettings["FormatTime"], "hour", "hour");
        }

        /// <summary>
        /// Renders input label with bootstrap markup and JavaScript for select datetime
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TProperty">Model property</typeparam>
        /// <typeparam name="string">string</typeparam>
        /// <typeparam name="string">string</typeparam>
        /// <typeparam name="string">string</typeparam>
        /// <param name="html">Html</param>
        /// <param name="expression">Model property for the input</param>
        /// <param name="format">Format of datetime (http://www.malot.fr/bootstrap-datetimepicker/)</param>
        /// <param name="minView">The lowest view that the datetimepicker should show. (http://www.malot.fr/bootstrap-datetimepicker/)</param>
        /// <param name="maxView">The highest view that the datetimepicker should show. (http://www.malot.fr/bootstrap-datetimepicker/)</param>
        /// <returns>HTML markup</returns>
        public static MvcHtmlString BtDatetimeWidget<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression, string dateTimeFormat, string minView, string maxView)
        {
            var fieldName = ExpressionHelper.GetExpressionText(expression);

            var inputGroupAddon = CreateTag("span", 
                new Dictionary<string, string>() { { "class", "input-group-addon" } },
                CreateTag("span", 
                    new Dictionary<string, string>() { 
                    { "class", maxView != null && maxView.Equals("hour") ? "glyphicon glyphicon-time" : "glyphicon glyphicon-calendar" } }, 
                    null)
                );

            var div = CreateTag("div",
                new Dictionary<string, string>() { 
                    { "data-link-field", fieldName }, 
                    { "class", "input-group date form-datetime-" + fieldName } 
                },
                CreateTag("input", 
                    new Dictionary<string, string>() { 
                        { "type", "text" }, 
                        { "class", "form-control" }, 
                        { "readonly", "readonly" } 
                    }, 
                    null) + inputGroupAddon);

            var minViewParam = "hour";
            var maxViewParam = "decade";
            var startViewParam = 2;
            if (!String.IsNullOrEmpty(minView))
            {
                minViewParam = minView;
            }
            if (!String.IsNullOrEmpty(maxView))
            {
                maxViewParam = maxView;
            }
            if (maxView != null && maxView.Equals("hour"))
            {
                startViewParam = 1;
            }

            var parameters = new { 
                format = dateTimeFormat,
                minView = minViewParam,
                maxView = maxViewParam,
                startView = startViewParam,
                minuteStep = 1, 
                weekStart = 1, 
                todayBtn = 0, 
                autoclose = 1, 
                todayHighlight = 1, 
                forceParse = 0, 
                showMeridian = 0, 
            };

            var jsInnerHtml = "$(function(){";
            jsInnerHtml += "$('div[data-link-field=\"" + fieldName + "\"]').datetimepicker(";
            jsInnerHtml += new JavaScriptSerializer().Serialize(parameters);
            jsInnerHtml += ");";
            jsInnerHtml += "});";

            var script = CreateTag("script",
                    new Dictionary<string, string>() { { "type", "text/javascript" } },
                    jsInnerHtml);

            return html.BtInputGroup(expression, html.BtLabel(expression), 
                new MvcHtmlString(div + 
                    html.HiddenFor(expression, new { @class = "form-control" }).ToString() +
                    script));
        }


        public static MvcHtmlString BtInputTextarea<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression)
        {
            var label = html.BtLabel(expression);
            var input = html.TextAreaFor(expression, new {@class = "form-control"});

            return html.BtInputGroup(expression, label, input);
        }

        public static MvcHtmlString BtInputDropDown<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, bool modifiable = true)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);

            var inputAttributes = new Dictionary<string, object>();
            inputAttributes.Add("class", "form-control");

            var inputValueHidden = MvcHtmlString.Empty;
            if (!modifiable)
            {
                inputAttributes.Add("disabled", "disabled");
                inputValueHidden = html.HiddenFor(expression);
            }

            var label = html.BtLabel(expression);
            var input = html.DropDownListFor(expression, AddEmptyValue(selectList), inputAttributes);

            return html.BtInputGroup(expression, label, new MvcHtmlString(input.ToString() + inputValueHidden.ToString()));
        }


        public static MvcHtmlString BtInputBooleanDropDown<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression)
        {
            var selectList = new List<SelectListItem>() 
            {
                new SelectListItem() { Value = "true", Text = "Yes" },
                new SelectListItem() { Value = "false", Text = "No" }
            };

            return html.BtInputDropDown(expression, selectList);
        }

        public static MvcHtmlString BtSearchDropDown<TModel, TPicklistType>(this HtmlHelper<TModel> html, Expression<Func<TModel, PicklistField>> expression, PicklistType<TPicklistType> type)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            expressionText = UtilityHelpers.CombineHtmlExpressionTexts(expressionText, "Id");
            return BtSearchDropDownInternal(html, expressionText, type);
        }

        public static MvcHtmlString BtSearchDropDown<TModel, TPicklistType>(this HtmlHelper<TModel> html, Expression<Func<TModel, int?>> expression, PicklistType<TPicklistType> type) 
        {
            return BtSearchDropDownInternal(html, ExpressionHelper.GetExpressionText(expression), type);
        }

        private static MvcHtmlString BtSearchDropDownInternal<TModel, TPicklistType>(this HtmlHelper<TModel> html, String propertyPath, PicklistType<TPicklistType> type) 
        {
            var selectList = type.GetPickList();
            var input = html.DropDownList(propertyPath, AddEmptyValue(selectList), new { @class = "form-control picklist-dropdown" });
            return new MvcHtmlString(input.ToString());
        }

        public static MvcHtmlString BtInputDropDown<TModel, TPicklistType>(this HtmlHelper<TModel> html, Expression<Func<TModel, PicklistField>> expression, PicklistType<TPicklistType> type, object htmlAttributes = null, bool modifiable = true)
        {
            var selectList = type.GetPickList();
            return BtInputDropDown(html, expression, selectList, htmlAttributes, modifiable);
        }

        public static MvcHtmlString BtInputDropDown<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, PicklistField>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes = null, bool modifiable = true)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, PicklistField>(expression, html.ViewData);

            PicklistField model = (PicklistField)modelMetadata.Model;
            
            var inputAttributes = new Dictionary<string, object>() 
            {
                { "class", "form-control picklist-dropdown" }
            };

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var attribute in attributes)
            {
                inputAttributes.Add(attribute.Key,attribute.Value);
            }
            
            var inputValueHidden = MvcHtmlString.Empty;
            if (!modifiable)
            {
                inputAttributes.Add("disabled", "disabled");
                inputValueHidden = html.Hidden(UtilityHelpers.CombineHtmlExpressionTexts(expressionText, "Id"), model != null ? model.Id.ToString() : "");
            }
            var label = html.BtLabel(expression);
            var input = html.DropDownList(UtilityHelpers.CombineHtmlExpressionTexts(expressionText, "Id"), AddEmptyValue(selectList), inputAttributes);
            var hidden = html.Hidden(UtilityHelpers.CombineHtmlExpressionTexts(expressionText, "Label"), model != null ? model.Label : "");

            return html.BtInputGroup(expression, label, new MvcHtmlString(input.ToString() + hidden.ToString() + inputValueHidden.ToString()));
        }

        private static IEnumerable<SelectListItem> AddEmptyValue(IEnumerable<SelectListItem> values)
        {
            List<SelectListItem> withEmptyValue = new List<SelectListItem>(values);
            withEmptyValue.Insert(0, new SelectListItem() { Text = "", Value = "" });
            return withEmptyValue;
        }

        public static MvcHtmlString BtInputEditableDropDown<TModel, TProperty1, TPropert2>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty1>> expressionForText, Expression<Func<TModel, TPropert2>> expressionForHidden, IEnumerable<SelectListItem> selectList)
        {
            var label = html.BtLabel(expressionForText);
            var hidden = html.HiddenFor(expressionForHidden);

            TagBuilder caret = new TagBuilder("span");
            caret.MergeAttribute("class", "caret");

            TagBuilder button = new TagBuilder("button");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("class", "btn btn-default dropdown-for-input-toggle");
            button.MergeAttribute("data-toggle", "dropdown");
            button.MergeAttribute("id", "test");
            button.InnerHtml = caret.ToString();

            TagBuilder inputGroupBtn = new TagBuilder("div");
            inputGroupBtn.MergeAttribute("class", "input-group-btn");
            inputGroupBtn.InnerHtml = button.ToString();

            TagBuilder inputGroup = new TagBuilder("div");
            inputGroup.MergeAttribute("class", "input-group");
            inputGroup.InnerHtml = html.TextBoxFor(expressionForText, new { @class = "form-control" }) + "" + BtDropDownMenu(selectList) + hidden + inputGroupBtn.ToString();
            return html.BtInputGroup(expressionForText, label, new MvcHtmlString(inputGroup.ToString()));
        }

        public static MvcHtmlString BtValidationSummary<TModel>(this HtmlHelper<TModel> html)
        {
            if (!html.ViewData.ModelState.IsValid &&  html.ViewData.ModelState.ContainsKey(string.Empty)) 
            {
                TagBuilder btAlert = new TagBuilder("div");
                btAlert.MergeAttribute("class", "alert alert-danger");
                btAlert.InnerHtml = html.ValidationSummary(true).ToString();
                return MvcHtmlString.Create(btAlert.ToString());
            }
            else
            {
                return MvcHtmlString.Empty;
            }
            
        }

        /// <summary>
        /// Renders checkbox wrapped with label element
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <param name="html">Html</param>
        /// <param name="expression">Model property for the input</param>
        /// <returns>HTML markup</returns>
        public static MvcHtmlString ChekboxWithLabelFor<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, bool>> expression)
        {
            var checkbox = html.CheckBoxFor(expression);
            var checkboxName = ExpressionHelper.GetExpressionText(expression);
            var labelTag = new TagBuilder("label")
            {
                InnerHtml = checkbox + "&nbsp;" +
                            LabelHelper(ModelMetadata.FromLambdaExpression(expression, html.ViewData), checkboxName)
            };
            return new MvcHtmlString(labelTag.ToString());
        }


        public static BtPanel BtPanel(this HtmlHelper html, string type)
        {
            return new BtPanel(html.ViewContext, type);
        }

        #region Buttons

        public static MvcHtmlString BtSubmitButton(this HtmlHelper html, string label,
            ButtonType buttonType = null, bool disabled = false, object htmlAttributes = null)
        {
            TagBuilder button = new TagBuilder("button");
            button.MergeAttribute("type", "submit");
            return html.BtButton(label, buttonType, htmlAttributes, button, disabled);
        }

        public static MvcHtmlString BtActionLink(this HtmlHelper html, string label,
            ButtonType buttonType = null, string link = "javascript://nop/", bool disabled = false, object htmlAttributes = null)
        {
            TagBuilder button = new TagBuilder("a");
            button.MergeAttribute("href", link);

            return html.BtButton(label, buttonType, htmlAttributes, button, disabled);
        }

        public static MvcHtmlString BtConfirmActionLink(this HtmlHelper html, string label, string confirmMessage,
            ButtonType buttonType = null, string okCallback = "", string okRedirect = "", string link = "javascript://nop/", bool disabled = false, object htmlAttributes = null)
        {
            TagBuilder button = new TagBuilder("a");
            button.MergeAttribute("href", link);
            button.AddCssClass("confirm-trigger");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("data-confirm-ok-callback", okCallback);
            button.MergeAttribute("data-confirm-ok-redirect", okRedirect);
            button.MergeAttribute("data-confirm-message", confirmMessage);

            return html.BtButton(label, buttonType, htmlAttributes, button, disabled);
        }

        public static MvcHtmlString BtButton(this HtmlHelper html, string label, ButtonType buttonType = null, string id = null, bool disabled = false, object htmlAttributes = null)
        {
            TagBuilder button = new TagBuilder("button");
            button.MergeAttribute("type", "button");
            if (id!=null) button.MergeAttribute("id", id);
            return html.BtButton(label, buttonType, htmlAttributes, button, disabled);
        }

        public static MvcHtmlString BtConfirmButton(this HtmlHelper html, string label, string confirmMessage,
            ButtonType buttonType, string okCallback = "", string okRedirect = "", bool disabled = false, object htmlAttributes = null)
        {
            TagBuilder button = new TagBuilder("button");
            button.AddCssClass("confirm-trigger");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("data-confirm-ok-callback", okCallback);
            button.MergeAttribute("data-confirm-ok-redirect", okRedirect);
            button.MergeAttribute("data-confirm-message", confirmMessage);
            
            return html.BtButton(label, buttonType, htmlAttributes, button, disabled);
        }

        #endregion

        public static MvcHtmlString BtSearchFieldFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, PicklistField>> fieldExpression, SearchType entityType, object defaultFilter = null, string onSelectedCallbackName = null, bool modifiable = false)
        {
            var valuePath = ExpressionHelper.GetExpressionText(fieldExpression) + ".Id";
            var labelPath = ExpressionHelper.GetExpressionText(fieldExpression) + ".Label";
            return BtSearchFieldInternal(html, fieldExpression, labelPath, valuePath, entityType, defaultFilter, onSelectedCallbackName, modifiable);
        }

        public static MvcHtmlString BtSearchFieldFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> fieldExpression, SearchType entityType, object defaultFilter = null, string onSelectedCallbackName = null, bool modifiable = false)
        {
            var valuePath = ExpressionHelper.GetExpressionText(fieldExpression);
            return BtSearchFieldInternal(html, fieldExpression, valuePath, "temp", entityType, defaultFilter, onSelectedCallbackName, modifiable);
        }

        public static MvcHtmlString BtSearchFieldInternal<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> fieldExpression, string labelPath, string valuePath, SearchType entityType, object defaultFilter = null, string onSelectedCallbackName = null, bool modifiable = false)
        {
            var label = html.BtLabel(fieldExpression);

            var searchButtonAttributes = new
            {
                @class = "modal-trigger",
                data_modal_content_url = entityType.Route + "?" + UtilityHelpers.ToQueryString(defaultFilter),
                data_on_selected_callback_name = onSelectedCallbackName
            };

            var removeButtonAttributes = new
            {
                @class = "remove-searched-data-trigger"
            };

            var searchButton = html.BtButton("", ButtonType.SearchField, null, false, searchButtonAttributes);
            var removeButton = html.BtButton("", ButtonType.Remove, null, false, removeButtonAttributes);
            var buttons = MvcHtmlString.Create(searchButton.ToString() + removeButton.ToString());
            var labelValue = ModelMetadata.FromStringExpression(labelPath, html.ViewData).Model;
            var value = ModelMetadata.FromStringExpression(valuePath, html.ViewData).Model;

            var textBoxAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = "form-control" });
            if (!modifiable) 
            {
                textBoxAttributes.Add("readonly", "readonly");
            }

            var a = MvcHtmlString.Create(
                html.TextBox(labelPath, labelValue, textBoxAttributes).ToHtmlString() +
                html.Hidden(valuePath, value, new { @class = "form-control" }).ToHtmlString() +
                html.Span(buttons, new { @class = "input-group-btn" }).ToHtmlString()
            );

            return html.BtInputGroup(fieldExpression, label, html.BtInputGroup(a));

        }

        #region PrivateMembers

        /// <summary>
        /// Renders button with bootstrap.
        /// </summary>
        /// <param name="html">HTML.</param>
        /// <param name="label">Label</param>
        /// <param name="buttonType">Add/Default/Deactivate</param>
        /// <param name="htmlAttributes">additional HTML attributes</param>
        /// <param name="button"></param>
        /// <param name="disabled"></param>
        /// <returns>Button with HTML markup.</returns>
        private static MvcHtmlString BtButton(this HtmlHelper html, string label, ButtonType buttonType,
            object htmlAttributes, TagBuilder button, bool disabled)
        {
            if (buttonType == null)
                buttonType = ButtonType.Default;
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (attributes.ContainsKey("class"))
                button.AddCssClass("btn btn-" + buttonType.Class + " " + attributes["class"]);
            else
                button.AddCssClass("btn btn-" + buttonType.Class);

            if (disabled) 
                button.MergeAttribute("disabled", "disabled");
            button.MergeAttributes(attributes);

            var htmlEncode = WebUtility.HtmlEncode(label);
            button.InnerHtml = (buttonType.Icon == null)
                ? htmlEncode
                : "<i class=\"glyphicon glyphicon-" + buttonType.Icon + "\"></i>" + (htmlEncode.Length > 0 ? "&nbsp;&nbsp;" : "") + htmlEncode;

            return new MvcHtmlString(button.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Get label for property
        /// </summary>
        /// <param name="metadata">Metadata for property</param>
        /// <param name="fieldName">For the default value</param>
        /// <returns>Dipslay name</returns>
        private static MvcHtmlString LabelHelper(ModelMetadata metadata, string fieldName)
        {
            string labelText;
            var displayName = metadata.DisplayName;

            if (displayName == null)
            {
                var propertyName = metadata.PropertyName;
                labelText = propertyName ?? fieldName.Split('.').Last();
            }
            else
            {
                labelText = displayName;
            }

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(labelText);
        }

        private static void TwoColumnDivHelper<TModel, TProperty>(this HtmlHelper<TModel> html, ref TagBuilder groupDiv,
            Expression<Func<TModel, TProperty>> exp)
        {
            var firstLabel = html.BtLabel(exp);
            var inputClasses = "form-control";
            var validationError = "";

            if (!html.ViewData.ModelState.IsValidField(ModelMetadata.FromLambdaExpression(exp, html.ViewData).PropertyName))
            {
                validationError = html.ValidationMessageFor(exp).ToHtmlString();
                inputClasses += " input-validation-error";
            }
            var firstInput = html.TextBoxFor(exp, new {@class = inputClasses});

            // label
            groupDiv.InnerHtml += firstLabel.ToHtmlString();

            // input
            var inputfirstdiv = new TagBuilder("div");
            inputfirstdiv.MergeAttribute("class", "col-sm-" + (6 - DefaltLabelCols));
            inputfirstdiv.InnerHtml += firstInput.ToHtmlString();
            inputfirstdiv.InnerHtml += validationError;
            
            groupDiv.InnerHtml += inputfirstdiv.ToString(TagRenderMode.Normal);
        }

        private static MvcHtmlString BtDropDownMenu(IEnumerable<SelectListItem> items)
        {
            TagBuilder ul = new TagBuilder("ul");
            ul.MergeAttribute("class", "dropdown-menu dropdown-for-input dropdown-menu-left");
            ul.MergeAttribute("role", "menu");
            ul.MergeAttribute("aria-labelledby", "test");

            foreach (SelectListItem listItem in items) 
            {
                TagBuilder a = new TagBuilder("a");
                a.MergeAttribute("href", "javascript://nop/");
                a.MergeAttribute("data-id", listItem.Value);
                a.MergeAttribute("class", "editable-dropdown-list-item");
                a.InnerHtml = listItem.Text;

                TagBuilder li = new TagBuilder("li");
                li.InnerHtml = a.ToString();

                ul.InnerHtml += li.ToString();
            }

            return new MvcHtmlString(ul.ToString());
        }

        private static string CreateTag(string tag, Dictionary<string, string> attributes, string innerHtml)
        {
            TagBuilder htmlTag = new TagBuilder(tag);

            foreach (KeyValuePair<string, string> item in attributes)
            {
                htmlTag.MergeAttribute(item.Key, item.Value);
            }
            if (innerHtml != null)
            {
                htmlTag.InnerHtml = innerHtml;
            }

            return htmlTag.ToString(TagRenderMode.Normal);
        }

        #endregion
    }

    
    
}