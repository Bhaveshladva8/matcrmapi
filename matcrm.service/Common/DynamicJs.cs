using Newtonsoft.Json.Linq;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Dynamic;

namespace matcrm.service.Common
{
    public class DynamicJs
    {

        public static void CreateJS(CreateJSVM Model)
        {
            try
            {
                var apiUrl = OneClappContext.ValidIssuer;
                string LoadWidget = "\n" +
                "var bodyData = document.createElement('div');" + "\n" +
                "var promises = [];" + "\n" +
                "promises.push(loadjscssfile(" + "\"" + OneClappContext.ValidIssuer + "/DynamicFormCss/dynamicform.css" + "\"," + "\"css\"," + "\"head\"" + "));" + "\n" +
                "promises.push(loadjscssfile(" + "\"" + OneClappContext.ValidIssuer + "/DynamicFormCss/jquery-3.3.1.js" + "\"," + "\"js\"," + "\"head\"" + "));" + "\n" +
                "promises.push(loadjscssfile(" + "\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.2/dist/css/bootstrap.min.css" + "\"," + "\"css\"," + "\"head\"" + "));" + "\n" +
                "promises.push(loadjscssfile(" + "\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.2/dist/js/bootstrap.bundle.min.js" + "\"," + "\"js\"," + "\"head\"" + "));" + "\n" +
                "bodyData.innerHTML = Form.FormHTMLString;" + "\n" +
                "document.onreadystatechange = () =>" + "\n" +
                "{" + "\n" +
                    "if (document.readyState === \"complete\")" + "\n" +
                    "{" + "\n" +
                        "var _doc = document; _doc.body.appendChild(bodyData);" + "\n" +
                        "_doc.body.style.cssText = Form.LayoutBackground;" + "\n" +
                        "_doc.getElementById('oneClapp-form-submit').addEventListener('click', submitForm);" + "\n" +
                        "var currenturl = document.location.href.toLocaleLowerCase();" + "\n" +
                        "var els = document.getElementsByClassName('text-danger');" + "\n" +
                            "for (var i = 0; i < els.length; i++)" + "\n" +
                            "{" + "\n" +
                            "els[i].style.display = 'none';" + "\n" +
                            "}" + "\n" +
                        "Promise.all(promises)" + "\n" +
                            ".then(function(result) {" + "\n" +
                               "openToggle();" + "\n" +
                        "});" + "\n" +
                    "};" + "\n" +
                "}" + "\n" +

        "function openToggle()" + "\n" +
                "{" + "\n" +
            "$('#toggle-btn').click(function() {" + "\n" +
                "$('#sidebar-contact').toggleClass('active');" + "\n" +
                "$('#toggle-btn').toggleClass('active');" + "\n" +
                    "})" + "\n" +

                    "$('#close-btn').click(function() {" + "\n" +
                "$('#sidebar-contact').toggleClass('active');" + "\n" +
                "$('#close-btn').toggleClass('active');" + "\n" +
                    "})" + "\n" +
        "}" + "\n" +

        "function loadjscssfile(filename, filetype, pos) {" + "\n" +
                           "var fileref;" + "\n" +

                           "if (filetype === " + "\"js\"" + ")" + "\n" +
                           "{ //if filename is a external JavaScript file" + "\n" +
                               "fileref = document.createElement(" + "\"script\"" + "); " + "\n" +
                               "fileref.setAttribute(" + "\"type\"" + ", " + "\"text/javascript\"" + ");" + "\n" +
                               "fileref.setAttribute(" + "\"src\"" + ", filename);" + "\n" +
                           "}" + "\n" +
                           "else if (filetype === " + "\"css\"" + ")" + "\n" +
                           "{ //if filename is an external CSS file" + "\n" +
                               "fileref = document.createElement(" + "\"link\"" + ");" + "\n" +
                               "fileref.setAttribute(" + "\"rel\"" + ", " + "\"stylesheet\"" + ");" + "\n" +
                               "fileref.setAttribute(" + "\"type\"" + ", " + "\"text/css\"" + ");" + "\n" +
                               "fileref.setAttribute(" + "\"href\"" + ", filename);" + "\n" +
                           "}" + "\n" +

                           "if (fileref)" + "\n" +
                           "{" + "\n" +
                               "document.getElementsByTagName(pos)[0].appendChild(fileref);" + "\n" +
                           "}" + "\n" +
                           "}" + "\n" +

                "function ValidateForm()" + "\n" +
                "{" + "\n" +
                    "var Invalid = 0;" + "\n" +
                    "var Allfields = $('#oneClapp-form .oneClapp-form-control');" + "\n" +
                    "Allfields.each(function() {" + "\n" +
                        "var currentField = $(this).attr('id');" + "\n" +
                        "if (this.required == true)" + "\n" +
                        "{" + "\n" +
                            "if (this.type == 'text' || this.type == 'textarea' || this.type == 'date' || this.type == 'select-one')" + "\n" +
                            "{" + "\n" +
                                "if (this.value == '')" + "\n" +
                                "{" + "\n" +
                                "$(this).siblings('.text-danger').show();" + "\n" +
                                    "Invalid++;" + "\n" +
                                "}" + "\n" +
                                "else" + "\n" +
                                "{" + "\n" +
                                "$(this).siblings('.text-danger').hide();" + "\n" +
                                "}" + "\n" +
                            "}" + "\n" +
                            "else if (this.type == 'checkbox' || this.type == 'radio')" + "\n" +
                            "{" + "\n" +
                                "var optionsName = this.name;" + "\n" +
                                "var options = document.getElementsByName(optionsName);" + "\n" +
                                "var ischecked = false;" + "\n" +
                                "for (let j = 0; j < options.length; j++)" + "\n" +
                                "{" + "\n" +
                                    "const element1 = options[j];" + "\n" +
                                    "if (element1.checked) {" + "\n" +
                                        "ischecked = true;" + "\n" +
                                        "}" + "\n" +
                                "}" + "\n" +
                            "if (ischecked) {" + "\n" +
                                // "$(this).siblings('.text-danger').hide();" + "\n" +
                                "$(this).parent().siblings('.text-danger').hide();" + "\n" +
                                "} else {" + "\n" +
                                    "Invalid++;" + "\n" +
                                    // "$(this).siblings('.text-danger').show();" + "\n" +
                                    "$(this).parent().siblings('.text-danger').show();" + "\n" +
                                    "}" + "\n" +
                                "}" + "\n" +
                            "}" + "\n" +
                            "});" + "\n" +
                        "if (Invalid > 0)" + "\n" +
                        "{" + "\n" +
                            "return false;" + "\n" +
                        "}" + "\n" +
                        "else" + "\n" +
                        "{" + "\n" +
                            "return true;" + "\n" +
                        "}" + "\n" +
                        "};" + "\n" +

                        "function GetFormFields()" + "\n" +
                        "{" + "\n" +
                            "var inputs = $('#oneClapp-form .oneClapp-form-control');" + "\n" +
                            "let formFieldValues = [];" + "\n" +
                            "var model = $.map(inputs, function(x, y) {" + "\n" +
                                    "let Obj = { };" + "\n" +
                                    "let htmlControl = x;" + "\n" +
                                    "let type = x.type;" + "\n" +
                                    "let value = x.value;" + "\n" +
                                    // "let oneClappFormFieldId = $(x).siblings('.oneclapp-form-field')[0].id;" + "\n" +
                                    "let oneClappFormFieldId;" + "\n" +
                                    "if (type == 'checkbox' || type == 'radio'){" + "\n" +
                                     "oneClappFormFieldId = $(x).parents().siblings('.oneclapp-form-field')[0].id;" + "\n" +
                                    "} else {" + "\n" +
                                    "oneClappFormFieldId = $(x).siblings('.oneclapp-form-field')[0].id;" + "\n" +
                                    "}" + "\n" +
                                    "let customFieldObj = Form.CustomFields.find(t => t.OneClappFormFieldId == +oneClappFormFieldId);" + "\n" +
                                    "let customFieldId = null;" + "\n" +
                                    "if (customFieldObj)" + "\n" +
                                    "{" + "\n" +
                                        "customFieldId = customFieldObj.CustomFieldId;" + "\n" +
                                     "}" + "\n" +
                                    "let oneClappFormId = Form.FormId;" + "\n" +
                                    "if (type == 'text' || type == 'textarea' || type == 'date')" + "\n" +
                                    "{" + "\n" +
                                        "Obj = {" + "\n" +
                                                    "oneClappFormId: oneClappFormId," + "\n" +
                                                    "oneClappFormFieldId: Number(oneClappFormFieldId)," + "\n" +
                                                    "customFieldId: customFieldId," + "\n" +
                                                    "Value: value" + "\n" +
                                                "}" + "\n" +
                                        "formFieldValues.push(Obj);" + "\n" +
                                    "}" + "\n" +
                                    "else if (type == 'select-one')" + "\n" +
                                        "{" + "\n" +
                                            "Obj = {" + "\n" +
                                                        "oneClappFormId: oneClappFormId," + "\n" +
                                                        "oneClappFormFieldId: Number(oneClappFormFieldId)," + "\n" +
                                                        "customFieldId: customFieldId," + "\n" +
                                                        "optionId: Number(value)" + "\n" +
                                                   "}" + "\n" +
                                            "formFieldValues.push(Obj);" + "\n" +
                                            "}" + "\n" +
                        "else if (type == 'checkbox' || type == 'radio')" + "\n" +
                            "{" + "\n" +
                                "var optionsName = htmlControl.name;" + "\n" +
                                "var options = document.getElementsByName(optionsName);" + "\n" +
                                "for (let j = 0; j < options.length; j++)" + "\n" +
                                    "{" + "\n" +
                                        "let element1 = options[j];" + "\n" +
                                        "if (element1.checked) {" + "\n" +
                                            "let optionId = element1.nextSibling.id;" + "\n" +
                                            "Obj = {" + "\n" +
                                                        "oneClappFormId: oneClappFormId," + "\n" +
                                                        "oneClappFormFieldId: Number(oneClappFormFieldId)," + "\n" +
                                                        "customFieldId: customFieldId," + "\n" +
                                                        "optionId: Number(optionId)" + "\n" +
                                                    "}" + "\n" +
                                            "let isExistData = formFieldValues.find(t => t.oneClappFormId == Obj.oneClappFormId && t.oneClappFormFieldId == Obj.oneClappFormFieldId" + "\n" +
                                            "&& t.optionId == Obj.optionId);" + "\n" +
                                            "if (isExistData == null)" + "\n" +
                                            "{" + "\n" +
                                                "formFieldValues.push(Obj);" + "\n" +
                                            "}" + "\n" +
                                         "}" + "\n" +
                                        "}" + "\n" +
                                        "}" + "\n" +
                                    "});" + "\n" +
                                    "return formFieldValues;" + "\n" +
                                    "}" + "\n" +

                        "function submitForm(){" + "\n" +
        "if (ValidateForm())" + "\n" +
        "{" + "\n" +
            "var data = { };" + "\n" +
            "var formFieldValues = GetFormFields();" + "\n" +
            "console.log('formFieldValues:----', formFieldValues);" + "\n" +
            "data.OneClappFormId = Form.FormId;" + "\n" +
            "data.FormFieldValues = formFieldValues;" + "\n" +
            "let isFirstSubmit = true;" + "\n" +
            "if (isFirstSubmit)" + "\n" +
            "{" + "\n" +
                        "$.ajax({" + "\n" +
                         "url:'" + apiUrl + "OneClappRequestForm/AddUpdate'," + "\n" +
                        "type: 'POST'," + "\n" +
                        "data: JSON.stringify(data)," + "\n" +
                        "contentType: 'application/json; charset=utf-8'," + "\n" +
                        "dataType: 'json'," + "\n" +
                        "async: false," + "\n" +
                        "success: function(response) {" + "\n" +
                            "console.log(response);" + "\n" +
                    "if (response.success)" + "\n" +
                    "{" + "\n" +
                        "alert('Form submitted successfully!');" + "\n" +
                        "location.href = Form.RedirectUrl;" + "\n" +
                    "}" + "\n" +
                    "else" + "\n" +
                    "{" + "\n" +
                        "alert(response.errorMessage);" + "\n" +
                    "}" + "\n" +
                "}," + "\n" +
                        "error: function(e) {" + "\n" +
                    "console.log('There was an error: ' + JSON.stringify(e));" + "\n" +
                "}" + "\n" +
            "});" + "\n" +
                "}" + "\n" +
            "}" + "\n" +
        "}" + "\n";


                string ScriptToWrite = String.Concat(Model.Contents, LoadWidget);

                System.IO.File.WriteAllText(Model.JSPath, ScriptToWrite);
                //foreach (string s in contents)    
                //{
                //sw.WriteLine();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetEmbededCode(FormModalPopUP Model)
        {
            var formStyle = "";
            var headerStyle = "";
            var layoutStyle = "";
            var headerImageStyle = "";
            var isEnabled = false;
            var isTextAligned = false;
            var alignStyle = "text-align:left;";
            if (Model.FormStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.FormStyle.ToString());
                var borderTopLeft = myObject["border-top-left-radius"];
                if (borderTopLeft != null)
                {
                    headerImageStyle = "border-top-left-radius:" + borderTopLeft.ToString() + ";";
                }
                var borderTopRight = myObject["border-top-right-radius"];
                if (borderTopRight != null)
                {
                    headerImageStyle = headerImageStyle + "border-top-right-radius:" + borderTopRight.ToString() + ";";
                }
                if (string.IsNullOrEmpty(headerImageStyle))
                {
                    headerImageStyle = "border-radius: 4px 4px 0 0;";
                }

                formStyle = Model.FormStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
            }
            if (Model.HeaderStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.HeaderStyle.ToString());
                isEnabled = myObject.isEnabled;
                var headerWidth = myObject.width;
                headerStyle = Model.HeaderStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                if (headerWidth != null)
                {
                    headerStyle = headerStyle + "width:" + headerWidth.ToString() + "%;";
                }
                if (headerWidth.ToString() != "100")
                {
                    headerImageStyle = "";
                }

                JObject dataobject = JObject.Parse(Model.HeaderStyle.ToString());
                var HeaderStyleProperties = dataobject.ToObject<Dictionary<string, object>>();
                headerStyle = "";
                foreach (var item in HeaderStyleProperties)
                {
                    if (item.Key == "width")
                    {
                        headerStyle = headerStyle + item.Key + ":" + item.Value + "%;";
                    }
                    else
                    {
                        headerStyle = headerStyle + item.Key + ":" + item.Value + ";";
                    }
                }
            }
            if (Model.LayoutStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.LayoutStyle.ToString());
                if (myObject.textalign != null)
                {
                    isTextAligned = myObject.textalign;
                }
                if (isTextAligned == true)
                {
                    alignStyle = "text-align:right;";
                }
                layoutStyle = Model.LayoutStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");

                JObject dataobject = JObject.Parse(Model.LayoutStyle.ToString());
                var LayoutStyleProperties = dataobject.ToObject<Dictionary<string, object>>();
                layoutStyle = "";
                foreach (var item in LayoutStyleProperties)
                {
                    layoutStyle = layoutStyle + item.Key + ":" + item.Value + ";";
                }
            }

            // var embed = "<div class='py-4 lightgraybg dynemic-form-height dynamic-form-bg-img' style='background-size: cover;" + layoutStyle + "'>";
            var embed = "<div class='container'>";
            embed += "<div class='card m-auto' style='" + formStyle + "'>";

            embed += "<div>";
            if (isEnabled == true)
            {
                embed += "<div class='dynamic-form-bg-img ng-star-inserted' style='height: 100px;background-position: center;margin: auto;" + headerStyle + headerImageStyle + "' >'";
                embed += "</div>";
            }

            embed += "<div class='mt-3'>";
            // embed += "<img src='./header-img.png' class='w-100'> ";
            embed += "</div>";
            embed += "</div>";
            embed += "<div class='card-body' >";
            embed += "<h4 class='modal-title'>" + Model.FormName + "</h4>";
            embed += "<hr class='mb-0'>";
            embed += "<form id='oneClapp-form'>";

            if (Model.CustomFields != null && Model.CustomFields.Count != 0)
            {
                var index = 1;
                foreach (var itm in Model.CustomFields)
                {
                    var styleObj = "";
                    var fieldColorStyle = "";
                    if (itm.FormFieldStyle != null)
                    {
                        dynamic myObject = JObject.Parse(itm.FormFieldStyle.ToString());
                        if (myObject.textalign != null)
                        {
                            isTextAligned = myObject.textalign;
                        }
                        // styleObj = itm.FormFieldStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                        JObject dataobject = JObject.Parse(itm.FormFieldStyle.ToString());
                        var ShipmentObjProperties = dataobject.ToObject<Dictionary<string, object>>();

                        foreach (var item in ShipmentObjProperties)
                        {
                            styleObj = styleObj + item.Key + ":" + item.Value + ";";
                        }
                        Console.WriteLine(styleObj);
                    }
                    var styleObj2 = "";
                    if (itm.TypographyStyle != null)
                    {
                        // styleObj2 = itm.TypographyStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");

                        JObject dataobject = JObject.Parse(itm.TypographyStyle.ToString());
                        var ShipmentObjProperties = dataobject.ToObject<Dictionary<string, object>>();
                        foreach (var item in ShipmentObjProperties)
                        {
                            if (item.Key.ToLower() == "color")
                            {
                                fieldColorStyle = item.Key + ":" + item.Value + ";";
                            }
                            styleObj2 = styleObj2 + item.Key + ":" + item.Value + ";";
                        }
                    }
                    styleObj = styleObj2 + styleObj;
                    if (itm.CustomControl != null)
                    {
                        embed += "<div class='mb-3 mt-3'>";

                        if (!string.IsNullOrEmpty(itm.LabelName))
                        {
                            embed += "<dt class='" + itm.LabelName + "' style='" + alignStyle + fieldColorStyle + "'>" + itm.LabelName;
                            if (itm.IsRequired)
                            {
                                // embed += "<span class='required text-danger'>*</span>";
                                embed += "<span class='required'>*</span>";
                            }
                            embed += "</dt>";
                        }
                        embed += "<dd>";
                        embed += "<label class='oneclapp-form-field' style='display:none' id='" + itm.OneClappFormFieldId + "'>" + itm.OneClappFormFieldId + "</label>";
                        if (itm.CustomControl.Name == "TextBox")
                        {
                            // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                            if (itm.IsRequired)
                            {
                                embed += "<input type='text' style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' required >";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<input type='text' style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' >";
                            }

                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "TextArea")
                        {
                            if (itm.IsRequired)
                            {
                                embed += "<textarea style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' required ></textarea>";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<textarea style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' ></textarea>";
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "DropDown")
                        {
                            // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                            if (itm.IsRequired)
                            {
                                embed += "<select style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control oneClapp-form-control' required >";
                            }
                            else
                            {
                                embed += "<select style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control oneClapp-form-control' >";
                            }

                            embed += "<option value='' >" + "Select" + "</option>";

                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                foreach (var ddlItem in itm.CustomControlOptions)
                                {
                                    embed += "<option value='" + ddlItem.Id + "'>" + ddlItem.Option + "</option>";
                                }
                            }
                            embed += "</select>";
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Checkbox")
                        {
                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                foreach (var checkItem in itm.CustomControlOptions)
                                {
                                    embed += "<div style='" + alignStyle + "'>";
                                    if (itm.IsRequired)
                                    {
                                        embed += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control me-2 form-check-input' required >";
                                    }
                                    else
                                    {
                                        embed += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control me-2 form-check-input' >";
                                    }

                                    embed += "<label class='oneclapp-form-field-option' style='display:none' id='" + checkItem.Id + "'>" + checkItem.Id + "</label>";

                                    embed += "<label class='form-check-label' for='" + checkItem.Option + itm.Id + "'>" + checkItem.Option + "</label>";
                                    embed += "</div>";
                                }

                            }
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Radio")
                        {
                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                var radioIndex = 0;
                                foreach (var radioItem in itm.CustomControlOptions)
                                {
                                    embed += "<div  style='" + alignStyle + "'>";
                                    if (itm.IsRequired)
                                    {
                                        embed += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-check-input me-2' required>";
                                    }
                                    else
                                    {
                                        embed += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-check-input me-2'>";
                                    }

                                    embed += "<label class='oneclapp-form-field-option' style='display:none' id='" + radioItem.Id + "'>" + radioItem.Id + "</label>";
                                    // if (radioIndex == 0)
                                    // {
                                    embed += "<label class='form-check-label' for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label>";
                                    embed += "</div>";
                                    // }
                                    // else
                                    // {
                                    //     embed += "<label class='form-check-label' for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label>";
                                    // }
                                    radioIndex++;
                                }
                            }
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Date")
                        {
                            if (itm.IsRequired)
                            {
                                embed += "<input style ='" + styleObj + alignStyle + "' type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-control' required>";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<input style ='" + styleObj + alignStyle + "' type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-control'>";
                            }
                            embed += "</dd>";
                        }
                        embed += "</dd>";
                    }
                    index++;
                }
            }

            if (string.IsNullOrEmpty(Model.ButtonText))
            {
                Model.ButtonText = "Save";
            }

            embed += "<hr>";
            embed += "<div class='d-flex justify-content-end'>";
            embed += "<button id='oneClapp-form-submit' type='submit' class='btn btn-primary'>" + Model.ButtonText + "</button>";
            // embed += "<button type='button' class='btn btn-danger'>Close</button>";
            embed += "</div>";
            embed += "</form>";
            embed += "</div>";
            embed += "</div>";
            embed += "</div>";
            embed += "</div>";

            return embed;
        }

        public static string GetModalPopUpHtml(FormModalPopUP Model)
        {
            var formStyle = "";
            var headerStyle = "";
            var layoutStyle = "";
            var isEnabled = false;
            var headerImageStyle = "";
            var isTextAligned = false;
            var alignStyle = "text-align:left;";
            if (Model.FormStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.FormStyle.ToString());
                var borderTopLeft = myObject["border-top-left-radius"];
                if (borderTopLeft != null)
                {
                    headerImageStyle = "border-top-left-radius:" + borderTopLeft.ToString() + ";";
                }
                var borderTopRight = myObject["border-top-right-radius"];
                if (borderTopRight != null)
                {
                    headerImageStyle = headerImageStyle + "border-top-right-radius:" + borderTopRight.ToString() + ";";
                }
                if (string.IsNullOrEmpty(headerImageStyle))
                {
                    headerImageStyle = "border-radius: 4px 4px 0 0;";
                }
                formStyle = Model.FormStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                // formStyle += "margin:auto;" ;
            }
            if (Model.HeaderStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.HeaderStyle.ToString());
                isEnabled = myObject.isEnabled;
                var headerWidth = myObject.width;
                headerStyle = Model.HeaderStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                if (headerWidth != null)
                {
                    headerStyle = headerStyle + "width:" + headerWidth.ToString() + "%;";
                }
                if (headerWidth.ToString() != "100")
                {
                    headerImageStyle = "";
                }

                JObject dataobject = JObject.Parse(Model.HeaderStyle.ToString());
                var HeaderStyleProperties = dataobject.ToObject<Dictionary<string, object>>();
                headerStyle = "";
                foreach (var item in HeaderStyleProperties)
                {
                    if (item.Key == "width")
                    {
                        headerStyle = headerStyle + item.Key + ":" + item.Value + "%;";
                    }
                    else
                    {
                        headerStyle = headerStyle + item.Key + ":" + item.Value + ";";
                    }
                }
            }
            if (Model.LayoutStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.LayoutStyle.ToString());
                if (myObject.textalign != null)
                {
                    isTextAligned = myObject.textalign;
                }
                if (isTextAligned == true)
                {
                    alignStyle = "text-align:right;";
                }
                layoutStyle = Model.LayoutStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");

                JObject dataobject = JObject.Parse(Model.LayoutStyle.ToString());
                var LayoutStyleProperties = dataobject.ToObject<Dictionary<string, object>>();
                layoutStyle = "";
                foreach (var item in LayoutStyleProperties)
                {
                    layoutStyle = layoutStyle + item.Key + ":" + item.Value + ";";
                }
            }
            var embed = "<div class='py-4 lightgraybg dynemic-form-height dynamic-form-bg-img' style='background-size: cover;" + layoutStyle + "'>";
            embed += "<div class='container mt-3'>";

            embed += "<button type='button' class='btn btn-primary' data-bs-toggle='modal' data-bs-target='#myModal'>";
            embed += Model.FormName;
            embed += "</button>";
            embed += "</div>";

            embed += "<div class='modal' id='myModal'>";
            embed += "<div class='modal-dialog'>";
            embed += "<div class='modal-content' style='margin: auto !important;" + formStyle + "'>";
            // embed += "<div class='py-4 lightgraybg dynemic-form-height dynamic-form-bg-img' style='background-size: cover;" + layoutStyle + "'>";
            embed += "<div class='card'>";

            embed += "<div>";
            if (isEnabled == true)
            {
                embed += "<div class='dynamic-form-bg-img ng-star-inserted' style='height: 100px;border-radius: 4px 4px 0 0;background-position: center;margin: auto;" + headerStyle + headerImageStyle + "' >'";
                embed += "</div>";
            }

            embed += "<div class='mt-3'>";
            // embed += "<img src='./header-img.png' class='w-100'> ";
            embed += "</div>";
            embed += "</div>";
            embed += "<div class='card-body' >";
            embed += " <div class='modal-header'>";
            embed += "<h4 class='modal-title'>" + Model.FormName + "</h4>";
            // embed += " <button type='button' class='btn-close' data-bs-dismiss='modal'></button> -->";
            embed += "</div>";


            embed += "<div class='modal-body custom-scroll'>";
            embed += "<form id='oneClapp-form' >";

            if (Model.CustomFields != null && Model.CustomFields.Count != 0)
            {
                var index = 1;
                foreach (var itm in Model.CustomFields)
                {
                    var styleObj = "";
                    var fieldColorStyle = "";
                    if (itm.FormFieldStyle != null)
                    {
                        dynamic myObject = JObject.Parse(itm.FormFieldStyle.ToString());
                        if (myObject.textalign != null)
                        {
                            isTextAligned = myObject.textalign;
                        }
                        // styleObj = itm.FormFieldStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                        JObject dataobject = JObject.Parse(itm.FormFieldStyle.ToString());
                        var ShipmentObjProperties = dataobject.ToObject<Dictionary<string, object>>();

                        foreach (var item in ShipmentObjProperties)
                        {
                            styleObj = styleObj + item.Key + ":" + item.Value + ";";
                        }
                        Console.WriteLine(styleObj);
                    }
                    var styleObj2 = "";
                    if (itm.TypographyStyle != null)
                    {
                        // styleObj2 = itm.TypographyStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");

                        JObject dataobject = JObject.Parse(itm.TypographyStyle.ToString());
                        var ShipmentObjProperties = dataobject.ToObject<Dictionary<string, object>>();
                        foreach (var item in ShipmentObjProperties)
                        {
                            if (item.Key.ToLower() == "color")
                            {
                                fieldColorStyle = item.Key + ":" + item.Value + ";";
                            }
                            styleObj2 = styleObj2 + item.Key + ":" + item.Value + ";";
                        }
                    }
                    styleObj = styleObj2 + styleObj;
                    if (itm.CustomControl != null)
                    {
                        embed += "<div class='mb-3 mt-3'>";

                        if (!string.IsNullOrEmpty(itm.LabelName))
                        {
                            embed += "<dt class='" + itm.LabelName + "' style='" + alignStyle + fieldColorStyle + "'>" + itm.LabelName;
                            if (itm.IsRequired)
                            {
                                // embed += "<span class='required text-danger'>*</span>";
                                embed += "<span class='required'>*</span>";
                            }
                            embed += "</dt>";
                        }
                        embed += "<dd>";
                        embed += "<label class='oneclapp-form-field' style='display:none' id='" + itm.OneClappFormFieldId + "'>" + itm.OneClappFormFieldId + "</label>";
                        if (itm.CustomControl.Name == "TextBox")
                        {
                            // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                            if (itm.IsRequired)
                            {
                                embed += "<input type='text' style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' required >";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<input type='text' style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' >";
                            }

                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "TextArea")
                        {
                            if (itm.IsRequired)
                            {
                                embed += "<textarea style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' required ></textarea>";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<textarea style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' ></textarea>";
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "DropDown")
                        {
                            // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                            if (itm.IsRequired)
                            {
                                embed += "<select style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control oneClapp-form-control' required >";
                            }
                            else
                            {
                                embed += "<select style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control oneClapp-form-control' >";
                            }

                            embed += "<option value='' >" + "Select" + "</option>";

                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                foreach (var ddlItem in itm.CustomControlOptions)
                                {
                                    embed += "<option value='" + ddlItem.Id + "'>" + ddlItem.Option + "</option>";
                                }
                            }
                            embed += "</select>";
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Checkbox")
                        {
                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                foreach (var checkItem in itm.CustomControlOptions)
                                {
                                    embed += "<div style='" + alignStyle + "'>";
                                    if (itm.IsRequired)
                                    {
                                        embed += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control me-2 form-check-input' required >";
                                    }
                                    else
                                    {
                                        embed += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control me-2 form-check-input' >";
                                    }

                                    embed += "<label class='oneclapp-form-field-option' style='display:none' id='" + checkItem.Id + "'>" + checkItem.Id + "</label>";

                                    embed += "<label class='form-check-label' for='" + checkItem.Option + itm.Id + "'>" + checkItem.Option + "</label>";
                                    embed += "</div>";
                                }

                            }
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Radio")
                        {
                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                var radioIndex = 0;
                                foreach (var radioItem in itm.CustomControlOptions)
                                {
                                    embed += "<div  style='" + alignStyle + "'>";
                                    if (itm.IsRequired)
                                    {
                                        embed += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-check-input me-2' required>";
                                    }
                                    else
                                    {
                                        embed += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-check-input me-2'>";
                                    }

                                    embed += "<label class='oneclapp-form-field-option' style='display:none' id='" + radioItem.Id + "'>" + radioItem.Id + "</label>";
                                    // if (radioIndex == 0)
                                    // {
                                    embed += "<label class='form-check-label' for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label>";
                                    embed += "</div>";
                                    // }
                                    // else
                                    // {
                                    //     embed += "<label class='form-check-label' for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label>";
                                    // }
                                    radioIndex++;
                                }
                            }
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Date")
                        {
                            if (itm.IsRequired)
                            {
                                embed += "<input style ='" + styleObj + alignStyle + "' type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-control' required>";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<input style ='" + styleObj + alignStyle + "' type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-control'>";
                            }
                            embed += "</dd>";
                        }
                        embed += "</dd>";
                    }
                    index++;
                }
            }

            embed += "</form>";
            embed += "</div>";
            embed += "</div>";
            embed += "</div>";
            embed += "</div>";

            if (string.IsNullOrEmpty(Model.ButtonText))
            {
                Model.ButtonText = "Save";
            }

            embed += "<div class='modal-footer px-0'>";
            // embed += "<div class='d-flex justify-content-end flex-wrap'>";
            embed += "<button  id='oneClapp-form-submit' type = 'submit' class='btn btn-primary mx-2 mb-1'>" + Model.ButtonText + "</button>";
            embed += "<button type='button' data-bs-dismiss='modal'class='btn btn-danger mx-2 mb-1'>Close</button>";
            embed += "</div>";

            embed += "</div>";
            embed += "</div>";
            embed += "</div>";
            return embed;
        }

        public static string GetSlidingFormHtml(FormModalPopUP Model)
        {
            var formStyle = "";
            var headerStyle = "";
            var layoutStyle = "";
            var isEnabled = false;
            var headerImageStyle = "";
            var isTextAligned = false;
            var alignStyle = "text-align:left;";
            if (Model.FormStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.FormStyle.ToString());
                var borderTopLeft = myObject["border-top-left-radius"];
                if (borderTopLeft != null)
                {
                    headerImageStyle = "border-top-left-radius:" + borderTopLeft.ToString() + ";";
                }
                var borderTopRight = myObject["border-top-right-radius"];
                if (borderTopRight != null)
                {
                    headerImageStyle = headerImageStyle + "border-top-right-radius:" + borderTopRight.ToString() + ";";
                }
                if (string.IsNullOrEmpty(headerImageStyle))
                {
                    headerImageStyle = "border-radius: 4px 4px 0 0;";
                }
                formStyle = Model.FormStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                // formStyle += "margin:auto;" ;
            }
            if (Model.HeaderStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.HeaderStyle.ToString());
                isEnabled = myObject.isEnabled;
                var headerWidth = myObject.width;
                headerStyle = Model.HeaderStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                if (headerWidth != null)
                {
                    headerStyle = headerStyle + "width:" + headerWidth.ToString() + "%;";
                }
                if (headerWidth.ToString() != "100")
                {
                    headerImageStyle = "";
                }

                JObject dataobject = JObject.Parse(Model.HeaderStyle.ToString());
                var HeaderStyleProperties = dataobject.ToObject<Dictionary<string, object>>();
                headerStyle = "";
                foreach (var item in HeaderStyleProperties)
                {
                    if (item.Key == "width")
                    {
                        headerStyle = headerStyle + item.Key + ":" + item.Value + "%;";
                    }
                    else
                    {
                        headerStyle = headerStyle + item.Key + ":" + item.Value + ";";
                    }
                }
            }
            if (Model.LayoutStyle != null)
            {
                dynamic myObject = JObject.Parse(Model.LayoutStyle.ToString());
                if (myObject.textalign != null)
                {
                    isTextAligned = myObject.textalign;
                }
                if (isTextAligned == true)
                {
                    alignStyle = "text-align:right;";
                }
                layoutStyle = Model.LayoutStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");

                JObject dataobject = JObject.Parse(Model.LayoutStyle.ToString());
                var LayoutStyleProperties = dataobject.ToObject<Dictionary<string, object>>();
                layoutStyle = "";
                foreach (var item in LayoutStyleProperties)
                {
                    layoutStyle = layoutStyle + item.Key + ":" + item.Value + ";";
                }
            }
            // var embed = "<div class='py-4 lightgraybg dynemic-form-height dynamic-form-bg-img' style='background-size: cover;" + layoutStyle + "'>";
            var embed = "<div id='sidebar-contact' class='sidebar-contact'>";
            embed += "<div id='toggle-btn' class='toggle'>"+ Model.FormName +"</div>";
            embed += "<div class='' style='background-size: cover;" + layoutStyle + "'>";
            embed += "<div class='card m-auto' style='" + formStyle + "'>";
            embed += "<div>";
            if (isEnabled == true)
            {
                embed += "<div class='dynamic-form-bg-img ng-star-inserted' style='height: 100px;border-radius: 4px 4px 0 0;background-position: center;margin: auto;" + headerStyle + headerImageStyle + "' >'";
                embed += "</div>";
            }
            embed += "<h2 class='p-4 mb-0'>" + Model.FormName + "</h2>";
            embed += "<hr class='my-0'>";
            embed += "<div class='scroll custom-scroll'>";

            embed += "<form id='oneClapp-form' class='hpx-450 p-4'>";

             if (Model.CustomFields != null && Model.CustomFields.Count != 0)
            {
                var index = 1;
                foreach (var itm in Model.CustomFields)
                {
                    var styleObj = "";
                    var fieldColorStyle = "";
                    if (itm.FormFieldStyle != null)
                    {
                        dynamic myObject = JObject.Parse(itm.FormFieldStyle.ToString());
                        if (myObject.textalign != null)
                        {
                            isTextAligned = myObject.textalign;
                        }
                        // styleObj = itm.FormFieldStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                        JObject dataobject = JObject.Parse(itm.FormFieldStyle.ToString());
                        var ShipmentObjProperties = dataobject.ToObject<Dictionary<string, object>>();

                        foreach (var item in ShipmentObjProperties)
                        {
                            styleObj = styleObj + item.Key + ":" + item.Value + ";";
                        }
                        Console.WriteLine(styleObj);
                    }
                    var styleObj2 = "";
                    if (itm.TypographyStyle != null)
                    {
                        // styleObj2 = itm.TypographyStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");

                        JObject dataobject = JObject.Parse(itm.TypographyStyle.ToString());
                        var ShipmentObjProperties = dataobject.ToObject<Dictionary<string, object>>();
                        foreach (var item in ShipmentObjProperties)
                        {
                            if (item.Key.ToLower() == "color")
                            {
                                fieldColorStyle = item.Key + ":" + item.Value + ";";
                            }
                            styleObj2 = styleObj2 + item.Key + ":" + item.Value + ";";
                        }
                    }
                    styleObj = styleObj2 + styleObj;
                    if (itm.CustomControl != null)
                    {
                        embed += "<div class='mb-3 mt-3'>";

                        if (!string.IsNullOrEmpty(itm.LabelName))
                        {
                            embed += "<dt class='" + itm.LabelName + "' style='" + alignStyle + fieldColorStyle + "'>" + itm.LabelName;
                            if (itm.IsRequired)
                            {
                                // embed += "<span class='required text-danger'>*</span>";
                                embed += "<span class='required'>*</span>";
                            }
                            embed += "</dt>";
                        }
                        embed += "<dd>";
                        embed += "<label class='oneclapp-form-field' style='display:none' id='" + itm.OneClappFormFieldId + "'>" + itm.OneClappFormFieldId + "</label>";
                        if (itm.CustomControl.Name == "TextBox")
                        {
                            // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                            if (itm.IsRequired)
                            {
                                embed += "<input type='text' style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' required >";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<input type='text' style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' >";
                            }

                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "TextArea")
                        {
                            if (itm.IsRequired)
                            {
                                embed += "<textarea style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' required ></textarea>";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<textarea style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control oneClapp-form-control' placeholder='" + itm.PlaceHolder + "' ></textarea>";
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "DropDown")
                        {
                            // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                            if (itm.IsRequired)
                            {
                                embed += "<select style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control oneClapp-form-control' required >";
                            }
                            else
                            {
                                embed += "<select style ='" + styleObj + alignStyle + "' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control oneClapp-form-control' >";
                            }

                            embed += "<option value='' >" + "Select" + "</option>";

                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                foreach (var ddlItem in itm.CustomControlOptions)
                                {
                                    embed += "<option value='" + ddlItem.Id + "'>" + ddlItem.Option + "</option>";
                                }
                            }
                            embed += "</select>";
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Checkbox")
                        {
                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                foreach (var checkItem in itm.CustomControlOptions)
                                {
                                    embed += "<div style='" + alignStyle + "'>";
                                    if (itm.IsRequired)
                                    {
                                        embed += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control me-2 form-check-input' required >";
                                    }
                                    else
                                    {
                                        embed += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control me-2 form-check-input' >";
                                    }

                                    embed += "<label class='oneclapp-form-field-option' style='display:none' id='" + checkItem.Id + "'>" + checkItem.Id + "</label>";

                                    embed += "<label class='form-check-label' for='" + checkItem.Option + itm.Id + "'>" + checkItem.Option + "</label>";
                                    embed += "</div>";
                                }

                            }
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Radio")
                        {
                            if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                            {
                                var radioIndex = 0;
                                foreach (var radioItem in itm.CustomControlOptions)
                                {
                                    embed += "<div  style='" + alignStyle + "'>";
                                    if (itm.IsRequired)
                                    {
                                        embed += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-check-input me-2' required>";
                                    }
                                    else
                                    {
                                        embed += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-check-input me-2'>";
                                    }

                                    embed += "<label class='oneclapp-form-field-option' style='display:none' id='" + radioItem.Id + "'>" + radioItem.Id + "</label>";
                                    // if (radioIndex == 0)
                                    // {
                                    embed += "<label class='form-check-label' for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label>";
                                    embed += "</div>";
                                    // }
                                    // else
                                    // {
                                    //     embed += "<label class='form-check-label' for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label>";
                                    // }
                                    radioIndex++;
                                }
                            }
                            if (itm.IsRequired)
                            {
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            embed += "</dd>";
                        }
                        else if (itm.CustomControl.Name == "Date")
                        {
                            if (itm.IsRequired)
                            {
                                embed += "<input style ='" + styleObj + alignStyle + "' type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-control' required>";
                                if (!string.IsNullOrEmpty(itm.LabelName))
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>" + itm.LabelName + " is required.</span>";
                                }
                                else
                                {
                                    embed += "<span class='text-danger' style='" + alignStyle + "'>This field is required.</span>";
                                }
                            }
                            else
                            {
                                embed += "<input style ='" + styleObj + alignStyle + "' type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='oneClapp-form-control form-control'>";
                            }
                            embed += "</dd>";
                        }
                        embed += "</dd>";
                    }
                    index++;
                }
            }

            if (string.IsNullOrEmpty(Model.ButtonText))
            {
                Model.ButtonText = "Save";
            }
            embed += "</form>";
            embed += "<hr class='mt-0'>";
            embed += "<div class='d-flex justify-content-end flex-wrap'>";
            embed += "<button  id='oneClapp-form-submit' type = 'submit' class='btn btn-primary mx-2 mb-1'>" + Model.ButtonText + "</button>";
            embed += "<button id='close-btn' type ='button' class='btn btn-danger mx-2 mb-1'>Close</button>";
            embed += "</div>";
            embed += "</div>";
            embed += "</div>";
            embed += "</div>";
            return embed;
        }

    }
}