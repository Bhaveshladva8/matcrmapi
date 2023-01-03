
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using matcrm.data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace matcrm.service.Common
{
    public class Common
    {
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static string BuildUrl(string UrlName)
        {
            if (!string.IsNullOrEmpty(UrlName))
                return OneClappContext.ValidIssuer + "Test1";
            else
                return "";
        }

        public static DateTime GetStartEndTime(DateTime Date, string Time)
        {

            if (Time.Contains("PM") || Time.Contains("pm"))
            {
                string[] timeData;
                if (Time.Contains("PM"))
                {
                    timeData = Time.Split("PM");
                }
                else
                {
                    timeData = Time.Split("pm");
                }
                if (timeData.Length > 0)
                {
                    var time = timeData[0].Trim();
                    var hourMinData = time.Split(":");
                    int hour1;
                    if (hourMinData[0] != "12")
                    {
                        hour1 = Convert.ToInt16(hourMinData[0]) + 12;
                    }
                    else
                    {
                        hour1 = Convert.ToInt16(hourMinData[0]);
                    }
                    // var hour1 = Convert.ToInt16 (hourMinData[0]) + 12;
                    var minute = Convert.ToInt16(hourMinData[1]);
                    var hour2 = hour1.ToString();
                    Date = Date.AddHours(hour1).AddMinutes(minute);
                }
            }
            if (Time.Contains("AM") || Time.Contains("am"))
            {
                string[] timeData;
                if (Time.Contains("AM"))
                {
                    timeData = Time.Split("AM");
                }
                else
                {
                    timeData = Time.Split("am");
                }
                if (timeData.Length > 0)
                {
                    var time = timeData[0].Trim();
                    var hourMinData = time.Split(":");
                    var hour1 = Convert.ToInt16(hourMinData[0]);
                    var minute = Convert.ToInt16(hourMinData[1]);
                    var hour2 = hour1.ToString();
                    Date = Date.AddHours(hour1).AddMinutes(minute);
                }
            }
            return Date;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTimeMilliSec(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static bool HasProperty(dynamic obj, string name)
        {
            Type objType = obj.GetType();

            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)obj).ContainsKey(name);
            }

            return objType.GetProperty(name) != null;
        }

        public static bool IsTokenExpired(string token)
        {
            if (token == null || ("").Equals(token))
            {
                return true;
            }

            /***
             * Make string valid for FromBase64String
             * FromBase64String cannot accept '.' characters and only accepts stringth whose length is a multitude of 4
             * If the string doesn't have the correct length trailing padding '=' characters should be added.
             */
            int indexOfFirstPoint = token.IndexOf('.') + 1;
            String toDecode = token.Substring(indexOfFirstPoint, token.LastIndexOf('.') - indexOfFirstPoint);
            while (toDecode.Length % 4 != 0)
            {
                toDecode += '=';
            }

            //Decode the string
            string decodedString = Encoding.ASCII.GetString(Convert.FromBase64String(toDecode));

            //Get the "exp" part of the string
            Regex regex = new Regex("(\"exp\":)([0-9]{1,})");
            Match match = regex.Match(decodedString);
            long timestamp = Convert.ToInt64(match.Groups[2].Value);

            DateTime date = new DateTime(1970, 1, 1).AddSeconds(timestamp);
            DateTime compareTo = DateTime.UtcNow;

            int result = DateTime.Compare(date, compareTo);

            return result < 0;
        }

        // public static bool CheckFileIsImage(HttpPostedFileBase file)
        // {
        //     using (var bitmap = new System.Drawing.Bitmap(file.InputStream))
        //     {
        //         return !bitmap.Size.IsEmpty;
        //     }
        // }

        // public static void SendMailTemplate(string mFromName, string mFrom, string mTo, string mSubject, string mBody)
        // {
        //     string mailServer = ConfigurationManager.AppSettings["SmtpServer"].ToString();
        //     string mailUser = ConfigurationManager.AppSettings["mailAccount"].ToString();
        //     string mailPassword = ConfigurationManager.AppSettings["mailPassword"].ToString();
        //     string mailTo = mTo;
        //     //string mFrom = ConfigurationManager.AppSettings["mailAccount"].ToString();

        //     if (string.IsNullOrEmpty(mTo))
        //         mTo = mailTo;

        //     System.Net.Mail.MailMessage mailObject = new System.Net.Mail.MailMessage();

        //     mailObject.To.Add(mTo);
        //     mailObject.From = new MailAddress(mFrom, mFromName, System.Text.Encoding.UTF8);

        //     mailObject.Subject = mSubject;
        //     mailObject.IsBodyHtml = true;
        //     mailObject.Body = mBody;

        //     mailObject.BodyEncoding = System.Text.Encoding.UTF8;
        //     mailObject.SubjectEncoding = System.Text.Encoding.UTF8;

        //     SmtpClient smtp = new SmtpClient();

        //     smtp.Host = mailServer;
        //     smtp.Port = 587;
        //     System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(mailUser, mailPassword);
        //     smtp.UseDefaultCredentials = false;
        //     smtp.Credentials = basicAuthenticationInfo;
        //     smtp.EnableSsl = true;

        //     smtp.Send(mailObject);
        // }

        public static void CreateJS(CreateJSVM Model)
        {
            try
            {
                var apiUrl = OneClappContext.ValidIssuer;
                string LoadWidget = "\n" +
                                    "var bodyData = document.createElement('div');" + "\n" +
                                    "bodyData.innerHTML = Form.FormHTMLString;" + "\n" +
                                    "document.onreadystatechange = () => {" + "\n" +
                                    "if (document.readyState === \"complete\") {" + "\n" +
                                    "var _doc = document;_doc.body.appendChild(bodyData);" + "\n" +
                                    "_doc.getElementById('oneClapp-form-submit').addEventListener('click', submitForm);" + "\n" +
                                          "if(!isScriptAlreadyIncluded('jquery'))" + "\n" +
                                            "{" + "\n" +
                                            "loadjscssfile(" + "\"" + "https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js" + "\"," + "\"js\"," + "\"head\"" + ")" + "\n" +
                                            //"}" + "\n" +

                                            "if(!isStyleAlreadyIncluded('bootstrap'))" + "\n" +
                                            "{" + "\n" +
                                            "loadjscssfile(" + "\"" + OneClappContext.ValidIssuer + "/Content/bootstrap.min.css" + "\"," + "\"css\"," + "\"head\"" + ")" + "\n" +
                                            "}" + "\n" +

                                            "loadjscssfile(" + "\"" + OneClappContext.ValidIssuer + "/Content/Drip.css" + "\"," + "\"css\"," + "\"head\"" + ")" + "\n" +
                                            // "var currenturl = document.location.href.toLocaleLowerCase();" + "\n" +
                                            // "var widget = ShowWidget(currenturl);" + "\n" +

                                            // "function ShowWidget(Url){" + "\n" +
                                            // "var FormSpecificPages = Form.SpecificPages.split(',');" + "\n" +
                                            // "var FormExcludedPages = Form.ExcludedPages.split(',');" + "\n" +

                                            // "if(!Form.IsShowOnEveryPage && !Form.IsShowOnSpecificPage){" + "\n" +
                                            // "return true;" + "\n" +
                                            // "}" + "\n" +

                                            // "else if(Form.IsShowOnSpecificPage && FormSpecificPages.includes(Url)){" + "\n" +
                                            // "return true;" + "\n" +
                                            // "}" + "\n" +

                                            // "else if(Form.IsShowOnEveryPage && FormExcludedPages.includes(Url)){" + "\n" +
                                            // "return false;" + "\n" +
                                            // "}" + "\n" +

                                            // "else if(Form.IsShowOnEveryPage){" + "\n" +
                                            // "return true;" + "\n" +
                                            // "}" + "\n" +

                                            // "else{" + "\n" +
                                            // "return false;" + "\n" +
                                            "}" + "\n" +

                                            "}" + "\n" +

                                            // "if(widget){" + "\n" +
                                            // //"if(!isScriptAlreadyIncluded('jquery'))" + "\n" +
                                            // //"{" + "\n" +
                                            // "loadjscssfile(" + "\"" + "https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js" + "\"," + "\"js\"," + "\"head\"" + ")" + "\n" +
                                            // //"}" + "\n" +

                                            // "if(!isStyleAlreadyIncluded('bootstrap'))" + "\n" +
                                            // "{" + "\n" +
                                            // "loadjscssfile(" + "\"" + OneClappContext.ValidIssuer +"/Content/bootstrap.min.css" + "\"," + "\"css\"," + "\"head\"" + ")" + "\n" +
                                            // "}" + "\n" +

                                            // "loadjscssfile(" + "\"" + OneClappContext.ValidIssuer + "/Content/Drip.css" + "\"," + "\"css\"," + "\"head\"" + ")" + "\n" +

                                            // "var _doc = document;_doc.body.appendChild(bodyData);" + "\n" +
                                            // "if(Form.OrientationId != 5){" + "\n" +
                                            // "document.getElementById('drip-header').style.display = 'none';" + "\n" +
                                            // "document.getElementById('drip-header').style.display = 'none';" + "\n" +
                                            // "}" + "\n" +
                                            // "var els = document.getElementsByClassName('text-danger');" + "\n" +
                                            // "for (var i = 0; i < els.length; i++)" + "\n" +
                                            // "{" + "\n" +
                                            // "els[i].style.display = 'none';" + "\n" +
                                            // "}" + "\n" +
                                            // "var isFirstOpen =  localStorage.getItem('openwidgetfirst');" + "\n" +
                                            // "if(isFirstOpen == null)" + "\n" +
                                            // "isFirstOpen = 'true';" + "\n" +
                                            // "if(isFirstOpen){" + "\n" +
                                            // "localStorage.setItem('openwidgetfirst','true');" + "\n" +
                                            // "var xhttp = new XMLHttpRequest();" + "\n" +
                                            // "xhttp.open('POST', '" + Model.WidgetUrl + "', true);" + "\n" +
                                            // "xhttp.send();" + "\n" +
                                            // "localStorage.setItem('openwidgetfirst','false');" + "\n" +
                                            // "}" + "\n" +
                                            // "}" + "\n" +
                                            // "};" + "\n" +
                                            // "\n" +
                                            // "\n" +

                                            "function isScriptAlreadyIncluded(src){" + "\n" +
                                                "var scripts = document.getElementsByTagName(" + "\"script\"" + ");" + "\n" +
                                                    "for (var i = 0; i < scripts.length; i++)" + "\n" +
                                                        "if (scripts[i].getAttribute('src') != null && scripts[i].getAttribute('src').match(src)) return true;" + "\n" +
                                                        "return false;" + "\n" +
                                                        "\n" +
                                                    "}" + "\n" +

                                            "function isStyleAlreadyIncluded(href){" + "\n" +
                                            "var styles = document.getElementsByTagName(" + "\"link\"" + ");" + "\n" +
                                                "for (var i = 0; i < styles.length; i++)" + "\n" +
                                                        "if (styles[i].getAttribute('href') != null && styles[i].getAttribute('href').match(href)) return true;" + "\n" +
                                                        "return false;" + "\n" +
                                                        "\n" +
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
                                            "}" + "\n";

                string SubmitWidgetJS = "\n" +
"function ValidateForm() {" + "\n" +
    "var Invalid = 0;" + "\n" +
    "var Allfields = $('#oneClapp-form .form-control');" + "\n" +
    "Allfields.each(function () {" + "\n" +
        "var currentField = $(this).attr('id');" + "\n" +
        "if (this.required == true) {" + "\n" +
            "if (this.type == 'text' || this.type == 'textarea' || this.type == 'date' || this.type == 'select-one') {" + "\n" +
                "if (this.value == '') {" + "\n" +
                    "$(this).siblings('.text-danger').show();" + "\n" +
                    "Invalid++;" + "\n" +
                "} else {" + "\n" +
                    "$(this).siblings('.text-danger').hide();" + "\n" +
                "}" + "\n" +
            "}" + "\n" +
            "else if (this.type == 'checkbox' || this.type == 'radio') {" + "\n" +
                "var optionsName = this.name;" + "\n" +
                "var options = document.getElementsByName(optionsName);" + "\n" +
                "var ischecked = false;" + "\n" +
                "for (let j = 0; j < options.length; j++) {" + "\n" +
                    "const element1 = options[j];" + "\n" +
                    "if (element1.checked) {" + "\n" +
                        "ischecked = true;" + "\n" +
                    "}" + "\n" +
                "}" + "\n" +
                "if (ischecked) {" + "\n" +
                    "$(this).siblings('.text-danger').hide();" + "\n" +
                "} else {" + "\n" +
                    "Invalid++;" + "\n" +
                    "$(this).siblings('.text-danger').show();" + "\n" +
                "}" + "\n" +
            "}" + "\n" +
        "}" + "\n" +
    "});" + "\n" +
    "if (Invalid > 0) {" + "\n" +
        "return false;" + "\n" +
    "}" + "\n" +
    "else {" + "\n" +
        "return true;" + "\n" +
    "}" + "\n" +
"};" + "\n" +
"function GetFormFields() {" + "\n" +
    "var inputs = $('#oneClapp-form .form-control');" + "\n" +
    "let formFieldValues = [];" + "\n" +
    "var model = $.map(inputs, function (x, y) {" + "\n" +
        "let Obj = {};" + "\n" +
        "let htmlControl = x;" + "\n" +
        "let type = x.type;" + "\n" +
        "let value = x.value;" + "\n" +
        "let oneClappFormFieldId = $(x).siblings('.oneclapp-form-field')[0].id;" + "\n" +
        "let customFieldObj = Form.CustomFields.find(t => t.OneClappFormFieldId == +oneClappFormFieldId);" + "\n" +
        "let customFieldId = null;" + "\n" +
        "if (customFieldObj) {" + "\n" +
            "customFieldId = customFieldObj.CustomFieldId;" + "\n" +
        "}" + "\n" +
        "let oneClappFormId = Form.FormId;" + "\n" +
        "if (type == 'text' || type == 'textarea' || type == 'date') {" + "\n" +
            "Obj = {" + "\n" +
                "oneClappFormId: oneClappFormId," + "\n" +
                "oneClappFormFieldId: Number(oneClappFormFieldId)," + "\n" +
                "customFieldId: customFieldId," + "\n" +
                "Value: value" + "\n" +
            "}" + "\n" +
            "formFieldValues.push(Obj);" + "\n" +

        "} else if (type == 'select-one') {" + "\n" +
            "Obj = {" + "\n" +
                "oneClappFormId: oneClappFormId," + "\n" +
                "oneClappFormFieldId: Number(oneClappFormFieldId)," + "\n" +
                "customFieldId: customFieldId," + "\n" +
                "optionId: Number(value)" + "\n" +
           "}" + "\n" +
            "formFieldValues.push(Obj);" + "\n" +
        "} else if (type == 'checkbox' || type == 'radio') {" + "\n" +
            "var optionsName = htmlControl.name;" + "\n" +
            "var options = document.getElementsByName(optionsName);" + "\n" +
            "for (let j = 0; j < options.length; j++) {" + "\n" +
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
                    "if (isExistData == null) {" + "\n" +
                        "formFieldValues.push(Obj);" + "\n" +
                    "}" + "\n" +
                "}" + "\n" +
            "}" + "\n" +
        "}" + "\n" +
    "});" + "\n" +
    "return formFieldValues;" + "\n" +
"}" + "\n" +
"function submitForm() {" + "\n" +
    "if (ValidateForm()) {" + "\n" +
        "var data = {};" + "\n" +
        "var formFieldValues = GetFormFields();" + "\n" +
        "console.log('formFieldValues:----', formFieldValues);" + "\n" +
        "data.OneClappFormId = Form.FormId;" + "\n" +
        "data.FormFieldValues = formFieldValues;" + "\n" +
        "let isFirstSubmit = true;" + "\n" +
        "if (isFirstSubmit) {" + "\n" +
            "$.ajax({" + "\n" +
             "url:'" + apiUrl + "OneClappRequestForm/AddUpdate'," + "\n" +
                // "url: 'http://localhost:4000/OneClappRequestForm/AddUpdate'," + "\n"+
                "type: 'POST'," + "\n" +
                "data: JSON.stringify(data)," + "\n" +
                "contentType: 'application/json; charset=utf-8'," + "\n" +
                "dataType: 'json'," + "\n" +
                "async: false," + "\n" +
                "success: function (response) {" + "\n" +
                    "console.log(response);" + "\n" +
                    "if (response.success) {" + "\n" +
                        "alert('Form submitted successfully!');" + "\n" +
                            "location.href = Form.RedirectUrl;" + "\n" +
                    "}" + "\n" +
                    "else {" + "\n" +
                        "alert(response.errorMessage);" + "\n" +
                    "}" + "\n" +
                "}," + "\n" +
                "error: function (e) {" + "\n" +
                    // "console.log(e);" + "\n"+
                    "console.log('There was an error: ' + JSON.stringify(e));" + "\n" +
                "}" + "\n" +
            "});" + "\n" +
        "}" + "\n" +
    "}" + "\n" +
"}" + "\n";

                string widgetJS = "\n" +
                                "function ManageContainer(elem) {" +
                                // "$(elem).parents('.drip-content').css('display', 'none');" +
                                // "$(elem).parents('.drip-content').siblings('.drip-header').css('display', 'block');" +
                                // "}" +

                                // "\n" +

                                // "function ManageTabs(elem) {" +
                                // "$(elem).siblings('.drip-content').css('display', 'block');" +
                                // "$(elem).css('display', 'none');" +
                                "};";


                // string ScriptToWrite = String.Concat(Model.Contents, LoadWidget, widgetJS, SubmitWidgetJS);
                string ScriptToWrite = String.Concat(Model.Contents, LoadWidget, SubmitWidgetJS);

                System.IO.File.WriteAllText(Model.JSPath, ScriptToWrite);
                //foreach (string s in contents)    
                //{
                //sw.WriteLine();
                //}
            }
            catch (Exception ex)
            {
                // ErrorLogs.LogError(ex);
            }
        }

        // public static List<String> GetExistingFormString(FormVM FormObject)
        // {
        //     try
        //     {
        //         List<String> FormBuildString = new List<string>();
        //         FormBuildString.Add("Add a data-drip-embedded-form="+FormObject.FormId+" attribute to your tab");
        //         FormBuildString.Add("Add a hidden input as" + FormObject.FormName + " attribute to your tab");
        //         FormBuildString.Add("Set the name of " + FormObject.FormName + " input to  " + FormObject.FormName + " .");
        //         FormBuildString.Add("Set the form action attribute to " + ConfigurationSettings.AppSettings["rooturl"] + ConfigurationSettings.AppSettings["SubmitEmbedUrl"] + FormObject.FormId + ".");
        //         FormBuildString.Add("Set the form method attribute to post.");
        //         foreach (var itm in FormObject.FormFields)
        //         {
        //             FormBuildString.Add("Set the name of "+ itm.Label +" input to  " + itm.IdentifierName +" .");
        //         }

        //         FormBuildString.Add("Add data-drip-attribute='headline' to your headline tag.");
        //         FormBuildString.Add("Add data-drip-attribute = 'description' to your description tag.");

        //         return FormBuildString;
        //     }
        //     catch (Exception ex)
        //     {
        //         ErrorLogs.LogError(ex);
        //         return new List<string>();
        //     }
        // }

        public static string GetEmbededCode(FormVM FormObject)
        {
            // regex which match tags
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");

            // replace all matches with empty strin
            var headline = FormObject.HeadLine != null ? rx.Replace(HttpUtility.HtmlDecode(FormObject.HeadLine), "").Replace("\n", "<br/>") : "";
            var description = FormObject.Description != null ? rx.Replace(HttpUtility.HtmlDecode(FormObject.Description), "").Replace("\n", "<br/>") : "";

            var embed = "<form action='" + OneClappContext.ValidIssuer + OneClappContext.SubmitUrl + FormObject.FormId + "' method='post' data-drip-embedded-form='" + FormObject.FormId + "'>";
            embed += "<h3 data-drip-attribute = 'headline' > ";
            embed += headline + "</h3>";
            embed += "<div data-drip-attribute ='description'>" + description + "</div>";
            embed += "<br/>";
            embed += "<input type='hidden' name='FormName' value='" + FormObject.FormName + "' />";
            if (FormObject.FormFields != null && FormObject.FormFields.Count > 0)
            {
                foreach (var itm in FormObject.FormFields)
                {
                    if (FormObject.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                    {
                        embed += "<div>";
                        embed += "<label for='" + itm.IdentifierName + "'>" + itm.IdentifierName.ToUpper() + "</label><br/>";
                        if (itm.IdentifierName == "Email_Address")
                        {
                            embed += "<input type ='email' id='" + itm.Identifier + "' name='" + itm.IdentifierName + "' value=''/>";
                            //embed += "<input type ='email' name='fields[" + itm.IdentifierName + "]' value=''/>";
                        }
                        else
                        {
                            embed += "<input type ='text' id='" + itm.Identifier + "' name='" + itm.IdentifierName + "' value=''/>";
                            //embed += "<input type ='text' name='fields[" + itm.IdentifierName + "]' value=''/>";
                        }
                        embed += "</div>";
                    }
                    else
                    {
                        embed += "<div>";
                        if (itm.IdentifierName == "Email_Address")
                        {
                            embed += "<input type ='email' id='" + itm.Identifier + "' name='" + itm.IdentifierName + "' placeholder = '" + itm.IdentifierName + "' value=''/> <br>";
                            //embed += "<input type ='email' name='fields[" + itm.IdentifierName + "]' placeholder = '" + itm.IdentifierName + "' value=''/> <br>";
                        }
                        else
                        {
                            embed += "<input type ='text' id='" + itm.Identifier + "' name='" + itm.IdentifierName + "' placeholder = '" + itm.IdentifierName + "' value=''/> <br>";
                            //embed += "<input type ='text' name='fields[" + itm.IdentifierName + "]' placeholder = '" + itm.IdentifierName + "' value=''/> <br>";

                        }
                        embed += "</div>";
                    }
                }
            }
            else
            {
                if (FormObject.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                {
                    embed += "<div>";
                    embed += "<label for='Email_Address'>Email Address</label><br/>";
                    embed += "<input type ='email' id='3' name='Email_Address' value=''/>";
                    embed += "</div>";
                }
                else
                {
                    embed += "<div>";
                    embed += "<input type ='email' id='3' name='Email_Address' placeholder='Email Adress' value=''/>";
                    embed += "</div>";
                }

            }
            embed += "<div>";
            embed += "<input type='submit' name='submit' value='" + FormObject.ButtonText + "' data-drip-attribute='sign-up-button' />";
            embed += "</div >";
            embed += "</form>";

            return embed;
        }

        public static string GetModalPopUpHtml(FormModalPopUP Model)
        {
            try
            {
                var modalString = "";

                #region 'Light Box'
                modalString += "<div class=" + "'drip-lightbox-wrapper'" + ">";
                modalString += "<div id = " + "'drip'" + "class=" + "'drip-lightbox side-image no-image bottom right drip-in " + Model.ImagePlaceClass + "'>";
                modalString += "<div id = " + "'drip-content'" + "class='drip-content' style='" + Model.WidgetStyle + "'>";
                // modalString += "<a id='drip-close' class='drip-close' onclick='return ManageContainer(this)'><span><b>X</b></span></a>";

                modalString += "<div id = " + "'drip-form-panel'" + "class=" + "'drip-panel'" + "style=" + "'display: block;'" + ">";


                modalString += "<div class=" + "'drip-form-main'" + ">";
                modalString += "<h3 id = 'drip-content-header'>" + Model.HeadLine + "</h3>";
                modalString += "<div id = " + "'drip-scroll'" + " class=" + "'drip-scroll'" + ">";
                modalString += "<div class=" + "'drip-description'" + ">" + Model.Description + "</div>";

                modalString += "<form id ='oneClapp-form' action='" + Model.SubmitUrl + "' method='post' class='mainform'>";
                modalString += "<div style=" + "'display: none'" + ">";
                modalString += "<input type=" + "'hidden'" + " name =" + "'form_id'" + "value =" + Model.FormId + ">";
                modalString += "</div >";
                modalString += "<div class='form_max_height'>";
                modalString += "<dl class=" + "style=" + "'font-size:" + Model.FontSize + "px" + "!important;'" + ">";
                // if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                // {
                if (Model.CustomFields != null && Model.CustomFields.Count != 0)
                {
                    var index = 1;
                    foreach (var itm in Model.CustomFields)
                    {
                        if (itm.CustomControl != null)
                        {
                            if (!string.IsNullOrEmpty(itm.LabelName))
                            {
                                modalString += "<dt class='" + itm.LabelName + "'>" + itm.LabelName;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                            }
                            modalString += "<dd>";
                            modalString += "<label class='oneclapp-form-field' style='display:none' id='" + itm.OneClappFormFieldId + "'>" + itm.OneClappFormFieldId + "</label>";
                            if (itm.CustomControl.Name == "TextBox")
                            {
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                // modalString += "<input type='text' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='drip-text-field form-control'>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<input type='text' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control' placeholder='" + itm.PlaceHolder + "' required>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                else
                                {
                                    modalString += "<input type='text' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' class='drip-text-field form-control' placeholder='" + itm.PlaceHolder + "'>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "TextArea")
                            {
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                // modalString += "<textarea id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control'></textarea>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<textarea id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control' placeholder='" + itm.PlaceHolder + "' required></textarea>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                else
                                {
                                    modalString += "<textarea id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' rows='4' cols='50' placeholder='" + itm.PlaceHolder + "' class='form-control'></textarea>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "DropDown")
                            {
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<select id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control' required>";
                                }
                                else
                                {
                                    modalString += "<select id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' class='form-control'>";
                                }
                                modalString += "<option value='' >" + "Select" + "</option>";

                                if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                                {
                                    foreach (var ddlItem in itm.CustomControlOptions)
                                    {
                                        modalString += "<option value='" + ddlItem.Id + "'>" + ddlItem.Option + "</option>";
                                    }

                                }
                                modalString += "</select>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "Checkbox")
                            {
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label><br/>";
                                // modalString += "<select id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' class='form-control'>";
                                if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                                {
                                    foreach (var checkItem in itm.CustomControlOptions)
                                    {
                                        if (itm.IsRequired)
                                        {
                                            modalString += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='form-control' required>";
                                        }
                                        else
                                        {
                                            modalString += "<input type='checkbox' id = '" + checkItem.Option + "_" + checkItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='form-control'>";
                                        }
                                        modalString += "<label class='oneclapp-form-field-option' style='display:none' id='" + checkItem.Id + "'>" + checkItem.Id + "</label>";

                                        modalString += "<label for='" + checkItem.Option + itm.Id + "'>" + checkItem.Option + "</label><br/>";
                                    }
                                }
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "Radio")
                            {
                                if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                                {
                                    foreach (var radioItem in itm.CustomControlOptions)
                                    {
                                        if (itm.IsRequired)
                                        {
                                            modalString += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='form-control' required>";
                                        }
                                        else
                                        {
                                            modalString += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' value='' placeholder='" + itm.PlaceHolder + "' class='form-control'>";
                                        }
                                        modalString += "<label class='oneclapp-form-field-option' style='display:none' id='" + radioItem.Id + "'>" + radioItem.Id + "</label>";
                                        modalString += "<label for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label><br/>";
                                    }

                                }
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "Date")
                            {
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                // modalString += "<input type='date' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='form-control'>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<input type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='form-control' required>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                else
                                {
                                    modalString += "<input type='date' id = '" + Model.FormName + "_" + itm.CustomControl.Name + "_" + index + "' name = '" + itm.Name + "' value='' placeholder='" + itm.PlaceHolder + "' class='form-control'>";
                                }
                                modalString += "</dd>";
                            }
                        }
                        index++;
                    }
                }
                modalString += "</dl>";
                modalString += "</div>";
                modalString += "<div class=" + "'form-controls'" + ">";
                modalString += "<input type ='button' value='" + Model.ButtonText + "' id='oneClapp-form-submit' class='drip-submit-button' style='background-color:" + Model.ButtonColor + "!important;'/>";
                modalString += "</div>";
                modalString += "</form>";
                modalString += "</div>";
                modalString += "</div>";
                modalString += "</div>";

                // modalString += "<div id=" + "'drip-success-panel-104672'" + "class=" + "'drip-success drip-panel drip-clearfix'" + "style=" + "'display: none'" + ">";
                // modalString += "<h3>Thank you for signing up!</h3>";
                // modalString += "<p class=" + "'drip-description drip-post-submission'" + ">Please check your email and click the link provided to confirm your subscription.</p>";
                // modalString += "</div>";

                modalString += "</div>";
                modalString += "</div>";
                modalString += "</div>";
                #endregion
                // }

                return modalString;
            }
            catch (Exception ex)
            {
                // ErrorLogs.LogError(ex);
                return "Something gone wrong with design HTML";
            }

        }

        public static string GetFieldCode(OneClappFormDto Model)
        {
            try
            {
                var modalString = "";

                if (Model.Fields != null && Model.Fields.Count != 0)
                {

                    #region 'Light Box'
                    modalString += "<div class=" + "'drip-lightbox-wrapper'" + ">";
                    modalString += "<div id = " + "'drip'" + "class=" + "'drip-lightbox side-image no-image bottom right drip-in'>";
                    modalString += "<div id = " + "'drip-content'" + "class='drip-content' >";
                    // modalString += "<a id='drip-close' class='drip-close' onclick='return ManageContainer(this)'><span><b>X</b></span></a>";

                    modalString += "<div id = " + "'drip-form-panel'" + "class=" + "'drip-panel'" + "style=" + "'display: block;'" + ">";


                    modalString += "<div class=" + "'drip-form-main'" + ">";
                    modalString += "<h3 id = 'drip-content-header'>" + "HeadLine" + "</h3>";
                    modalString += "<div id = " + "'drip-scroll'" + " class=" + "'drip-scroll'" + ">";
                    modalString += "<div class=" + "'drip-description'" + ">" + "Test Description" + "</div>";

                    modalString += "<form id ='oneClapp-form' action='" + Model.RedirectUrl + "' method='post' class='mainform'>";
                    modalString += "<div style=" + "'display: none'" + ">";
                    modalString += "<input type=" + "'hidden'" + " name =" + "'form_id'" + "value =" + Model.Id + ">";
                    modalString += "</div >";
                    modalString += "<div class='form_max_height'>";
                    modalString += "<dl class=" + "style=" + "'font-size:" + 20 + "px" + "!important;'" + ">";
                    // if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                    // {
                    foreach (var itm in Model.Fields)
                    {
                        if (itm.CustomControl != null)
                        {
                            if (itm.CustomControl.Name == "TextBox")
                            {
                                modalString += "<dt class='" + itm.Name + "'>" + itm.Name;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                // modalString += "<input type='text' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='drip-text-field form-control'>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<input type='text' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='drip-text-field form-control' required>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                else
                                {
                                    modalString += "<input type='text' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='drip-text-field form-control '>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "TextArea")
                            {
                                modalString += "<dt class='" + itm.Name + "'>" + itm.Name;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                // modalString += "<textarea id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control'></textarea>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<textarea id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control' required></textarea>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                else
                                {
                                    modalString += "<textarea id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' rows='4' cols='50' class='form-control'></textarea>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "DropDown")
                            {
                                modalString += "<dt class='" + itm.Name + "'>" + itm.Name;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<select id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' class='form-control' required>";
                                }
                                else
                                {
                                    modalString += "<select id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' class='form-control'>";
                                }

                                if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                                {
                                    foreach (var ddlItem in itm.CustomControlOptions)
                                    {
                                        modalString += "<option value='" + ddlItem.Id + "'>" + ddlItem.Option + "</option>";
                                    }

                                }
                                modalString += "</select>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "Checkbox")
                            {
                                modalString += "<dt class='" + itm.Name + "'>" + itm.Name;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label><br/>";
                                // modalString += "<select id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' class='form-control'>";
                                if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                                {
                                    foreach (var checkItem in itm.CustomControlOptions)
                                    {
                                        if (itm.IsRequired)
                                        {
                                            modalString += "<input type='checkbox' id = '" + checkItem.Option + checkItem.Id + "' name = '" + checkItem.Option + checkItem.Id + "' value='' placeholder='' class='form-control' required>";
                                        }
                                        else
                                        {
                                            modalString += "<input type='checkbox' id = '" + checkItem.Option + checkItem.Id + "' name = '" + checkItem.Option + checkItem.Id + "' value='' placeholder='' class='form-control'>";
                                        }

                                        modalString += "<label for='" + checkItem.Option + itm.Id + "'>" + checkItem.Option + "</label><br/>";
                                    }

                                }
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "Radio")
                            {
                                modalString += "<dt class='" + itm.Name + "'>" + itm.Name;

                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                //  modalString += "<p>" + itm.Name + ":</p>";

                                if (itm.CustomControlOptions != null && itm.CustomControlOptions.Count > 0)
                                {
                                    foreach (var radioItem in itm.CustomControlOptions)
                                    {

                                        if (itm.IsRequired)
                                        {
                                            modalString += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + itm.Name + itm.Id + "' value='' placeholder='' class='form-control' required>";
                                        }
                                        else
                                        {
                                            modalString += "<input type='radio' id = '" + radioItem.Option + radioItem.Id + "' name = '" + itm.Name + itm.Id + "' value='' placeholder='' class='form-control'>";
                                        }
                                        modalString += "<label for='" + radioItem.Option + itm.Id + "'>" + radioItem.Option + "</label><br/>";
                                    }

                                }
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                modalString += "</dd>";
                            }
                            else if (itm.CustomControl.Name == "Date")
                            {
                                modalString += "<dt class='" + itm.Name + "'>" + itm.Name;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                // modalString += "<label for='" + itm.Name + itm.Id + "'>" + itm.Name + ":</label>";
                                // modalString += "<input type='date' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='form-control'>";
                                if (itm.IsRequired)
                                {
                                    modalString += "<input type='date' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='form-control' required>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                }
                                else
                                {
                                    modalString += "<input type='date' id = '" + itm.Name + itm.Id + "' name = '" + itm.Name + "' value='' placeholder='' class='form-control'>";
                                }
                                modalString += "</dd>";
                            }
                        }
                    }
                    modalString += "</dl>";
                    modalString += "</div>";
                    modalString += "<div class=" + "'form-controls'" + ">";
                    modalString += "<input type ='button' value='" + Model.ButtonText + "' id='oneClapp-form-submit' class='drip-submit-button' />";
                    modalString += "</div>";
                    modalString += "</form>";
                    modalString += "</div>";
                    modalString += "</div>";
                    modalString += "</div>";

                    // modalString += "<div id=" + "'drip-success-panel-104672'" + "class=" + "'drip-success drip-panel drip-clearfix'" + "style=" + "'display: none'" + ">";
                    // modalString += "<h3>Thank you for signing up!</h3>";
                    // modalString += "<p class=" + "'drip-description drip-post-submission'" + ">Please check your email and click the link provided to confirm your subscription.</p>";
                    // modalString += "</div>";

                    modalString += "</div>";
                    modalString += "</div>";
                    modalString += "</div>";
                    #endregion
                }

                return modalString;
            }
            catch (Exception ex)
            {
                // ErrorLogs.LogError(ex);
                return "Something gone wrong with design HTML";
            }

        }

        public static string GetModalPopUpHtmlOld(FormModalPopUP Model)
        {
            try
            {
                var modalString = "";

                if (Model.FormFields != null && Model.FormFields.Count != 0)
                {
                    if (Model.Orientation != Convert.ToInt32(Enums.OrientationType.LightBox))
                    {

                        #region 'All Tabs'
                        modalString += "<div class=" + "'drip-tab-container'" + ">";
                        modalString += "<div id='drip' class='drip-tab side-image no-image " + Model.SideTabClass + " " + Model.ImagePlaceClass + "'>";
                        modalString += "<div id ='drip-header' class='drip-header drip-hidden' style='" + Model.HeaderStyle + ";background-color:" + Model.TabColorCode + " !important;' onclick='return ManageTabs(this)'>";
                        modalString += "<a href = '#' id='drip-toggle' class='drip-toggle'>";
                        modalString += "<h2 id = 'drip-teaser'>" + Model.HeadLine + "</h2>";
                        modalString += "<span id='drip-tab-up' class='drip-arrow up'>";
                        modalString += "<svg width = '12px' height='8px' viewBox='1362 659 12 8' version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'>";
                        modalString += "<polygon id ='right_angle' stroke='none' fill='#FFFFFF' fill-rule='evenodd' transform='translate(1368.000000, 662.703125) rotate(-90.000000) translate(-1368.000000, -662.703125)' points = '1364.29688 667.296875 1368.89062 662.703125 1364.29688 658.109375 1365.70312 656.703125 1371.70312 662.703125 1365.70312 668.703125'></polygon>";
                        modalString += "</svg>";
                        modalString += "</span>";
                        modalString += "<span id ='drip-tab-down' class='drip-arrow down' style='display: none'></span>";
                        modalString += "</a>";
                        modalString += "</div>";

                        modalString += "<div id = " + "'drip-content'" + "class=" + "'drip-content drip-clearfix'" + "style='" + Model.WidgetStyle + "'>";
                        // modalString += "<a id='drip-close' class='drip-close' onclick='return ManageContainer(this)'>";
                        // modalString += "<span><b>X</b></span>";
                        // modalString += "</a>";
                        modalString += "<div id = " + "'drip-form-panel'" + "class=" + "'drip-panel drip-clearfix'" + "style=" + "'display: block;'" + ">";
                        // if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Left) && Model.ImagePath != null)
                        // {
                        //     modalString += "<div class='drip-form-aside left'>";
                        //     modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                        //     modalString += "</div>";
                        // }
                        // if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Right) && Model.ImagePath != null)
                        // {
                        //     modalString += "<div class='drip-form-aside right'>";
                        //     modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                        //     modalString += "</div>";
                        // }

                        modalString += "<div class=" + "'drip-form-main'" + ">";
                        modalString += "<h3 id = 'drip-content-header'>" + Model.HeadLine + "</h3>";
                        modalString += "<div id = " + "'drip-scroll'" + "class=" + "'drip-scroll'" + ">";
                        modalString += "<div class=" + "'drip-description'" + ">" + Model.Description + "</div>";
                        modalString += "<form id ='drip-form' action='" + Model.SubmitUrl + "' method='post' class='mainform'>";
                        modalString += "<div style=" + "'display: none'" + ">";
                        modalString += "<input type=" + "'hidden'" + "name =" + "'form_id'" + "value ='" + Model.FormId + "'>";
                        modalString += "</div >";
                        modalString += "<div class='form_max_height'>";
                        modalString += "<dl class=" + "style=" + "'font-size:" + Model.FontSize + "px" + "!important;'" + ">";
                        if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                        {
                            foreach (var itm in Model.FormFields)
                            {
                                modalString += "<dt class='" + itm.Identifier + "'>" + itm.Label;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                modalString += "<input type='text' id = '" + itm.Identifier + "' name = '" + itm.IdentifierName + "' value='' placeholder='' class='drip-text-field form-control'>";
                                modalString += "<span class='text-danger'>This field is required.</span>";
                                modalString += "</dd>";
                            }
                        }
                        else if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.InSideTheTextBoxes))
                        {
                            foreach (var itm in Model.FormFields)
                            {
                                if (itm.IsRequired)
                                {
                                    modalString += "<dt class='" + itm.Identifier + "'>";
                                    modalString += "<span class='required'></span>";
                                    modalString += "</dt>";
                                    modalString += "<dd>";
                                    modalString += "<input type='text' id = '" + itm.Identifier + "' name = '" + itm.IdentifierName + "' value='' placeholder='" + itm.Label + "*" + "' class='drip-text-field form-control'>";
                                    //modalString += "<span class='asterisk_input'></span>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                    modalString += "</dd>";
                                }
                                else
                                {
                                    modalString += "<dd>";
                                    modalString += "<input type='text' id = '" + itm.Identifier + "' name = '" + itm.IdentifierName + "' value='' placeholder='" + itm.Label + "' class='drip-text-field form-control'>";
                                    // modalString += "<span class='text-danger'>This field is required.</span>";
                                    modalString += "</dd>";
                                }
                            }
                        }
                        //else
                        //{
                        //    modalString += "<dt> Email Address </dt >";
                        //    modalString += "<dd>";
                        //    modalString += "<input type='text' name = 'field_value_Email' value=" + "''" + "placeholder=" + "''" + "class=" + "'drip-text-field form-control'" + ">";
                        //    modalString += "<div id=" + "'drip-errors-for-email-104672'" + "class=" + "'drip-errors'" + "></div>";
                        //    modalString += "</dd>";
                        //}
                        modalString += "</dl>";
                        modalString += "</div>";
                        modalString += "<div class=" + "'form-controls'" + ">";
                        modalString += "<input type ='button' value='" + Model.ButtonText + "' id='drip-submit' class='drip-submit-button' style='background-color:" + Model.ButtonColor + "!important;'" + " onclick='return SubmitWidget()'/>";
                        modalString += "</div>";
                        modalString += "</form>";
                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";

                        // modalString += "<div id=" + "'drip-success-panel'" + "class=" + "'drip-success drip-panel drip-clearfix'" + "style=" + "'display: none'" + ">";
                        // modalString += "<h3>Thank you for signing up!</h3>";
                        // modalString += "<p class=" + "'drip-description drip-post-submission'" + ">Please check your email and click the link provided to confirm your subscription.</p>";
                        // modalString += "</div>";

                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";

                        #endregion
                    }
                    else
                    {
                        #region 'Light Box'
                        modalString += "<div class=" + "'drip-lightbox-wrapper'" + ">";
                        modalString += "<div id = " + "'drip'" + "class=" + "'drip-lightbox side-image no-image bottom right drip-in " + Model.ImagePlaceClass + "'>";
                        modalString += "<div id = " + "'drip-content'" + "class='drip-content' style='" + Model.WidgetStyle + "'>";
                        // modalString += "<a id='drip-close' class='drip-close' onclick='return ManageContainer(this)'><span><b>X</b></span></a>";

                        modalString += "<div id = " + "'drip-form-panel'" + "class=" + "'drip-panel'" + "style=" + "'display: block;'" + ">";
                        if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Left) && Model.ImagePath != null)
                        {
                            modalString += "<div class='drip-form-aside left'>";
                            modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                            modalString += "</div>";
                        }
                        if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Right) && Model.ImagePath != null)
                        {
                            modalString += "<div class='drip-form-aside right'>";
                            modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                            modalString += "</div>";
                        }

                        modalString += "<div class=" + "'drip-form-main'" + ">";
                        modalString += "<h3 id = 'drip-content-header'>" + Model.HeadLine + "</h3>";
                        modalString += "<div id = " + "'drip-scroll'" + "class=" + "'drip-scroll'" + ">";
                        modalString += "<div class=" + "'drip-description'" + ">" + Model.Description + "</div>";

                        modalString += "<form id ='drip-form' action='" + Model.SubmitUrl + "' method='post' class='mainform'>";
                        modalString += "<div style=" + "'display: none'" + ">";
                        modalString += "<input type=" + "'hidden'" + "name =" + "'form_id'" + "value =" + Model.FormId + ">";
                        modalString += "</div >";
                        modalString += "<div class='form_max_height'>";
                        modalString += "<dl class=" + "style=" + "'font-size:" + Model.FontSize + "px" + "!important;;'" + ">";
                        if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                        {
                            foreach (var itm in Model.FormFields)
                            {
                                modalString += "<dt class='" + itm.Identifier + "'>" + itm.Label;
                                if (itm.IsRequired)
                                {
                                    modalString += "<span class='required'>*</span>";
                                }
                                modalString += "</dt>";
                                modalString += "<dd>";
                                modalString += "<input type='text' id = '" + itm.Identifier + "' name = '" + itm.IdentifierName + "' value='' placeholder='' class='drip-text-field form-control'>";
                                modalString += "<span class='text-danger'>This field is required.</span>";
                                modalString += "</dd>";
                            }
                        }
                        else if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.InSideTheTextBoxes))
                        {
                            foreach (var itm in Model.FormFields)
                            {
                                if (itm.IsRequired)
                                {
                                    modalString += "<dt class='" + itm.Identifier + "'>";
                                    modalString += "<span class='required'></span>";
                                    modalString += "</dt>";
                                    modalString += "<dd>";
                                    modalString += "<input type='text' id = '" + itm.Identifier + "' name = '" + itm.IdentifierName + "' value='' placeholder='" + itm.Label + "*" + "' class='drip-text-field form-control'>";
                                    //modalString += "<span class='asterisk_input'></span>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                    modalString += "</dd>";
                                }
                                else
                                {
                                    modalString += "<dd>";
                                    modalString += "<input type='text' id = '" + itm.Identifier + "' name = '" + itm.IdentifierName + "' value='' placeholder='" + itm.Label + "' class='drip-text-field form-control'>";
                                    modalString += "<span class='text-danger'>This field is required.</span>";
                                    modalString += "</dd>";
                                }
                            }
                        }
                        //else
                        //{
                        //    modalString += "<dt> Email Address </dt >";
                        //    modalString += "<dd>";
                        //    modalString += "<input type='text' name = '" + "field_value_Email" + "' value=" + "''" + "placeholder=" + "''" + "class=" + "'drip-text-field form-control'" + ">";
                        //    modalString += "<div id=" + "'drip-errors-for-email'" + "class=" + "'drip-errors'" + "></div>";
                        //    modalString += "</dd>";
                        //}
                        modalString += "</dl>";
                        modalString += "</div>";
                        modalString += "<div class=" + "'form-controls'" + ">";
                        modalString += "<input type ='button' value='" + Model.ButtonText + "' id='drip-submit' class='drip-submit-button' style='background-color:" + Model.ButtonColor + "!important;'" + " onclick='return SubmitWidget()'/>";
                        modalString += "</div>";
                        modalString += "</form>";
                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";

                        modalString += "<div id=" + "'drip-success-panel-104672'" + "class=" + "'drip-success drip-panel drip-clearfix'" + "style=" + "'display: none'" + ">";
                        modalString += "<h3>Thank you for signing up!</h3>";
                        modalString += "<p class=" + "'drip-description drip-post-submission'" + ">Please check your email and click the link provided to confirm your subscription.</p>";
                        modalString += "</div>";

                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";
                        #endregion
                    }
                }
                else
                {
                    if (Model.Orientation != Convert.ToInt32(Enums.OrientationType.LightBox))
                    {

                        #region 'All Tabs'
                        modalString += "<div class=" + "'drip-tab-container'" + ">";
                        modalString += "<div id='drip' class='drip-tab side-image no-image " + Model.SideTabClass + " " + Model.ImagePlaceClass + "'>";
                        modalString += "<div id ='drip-header' class='drip-header drip-hidden' style='" + Model.HeaderStyle + ";background-color:" + Model.TabColorCode + " !important;' onclick='return ManageTabs(this)'>";
                        modalString += "<a href = '#' id='drip-toggle' class='drip-toggle'>";
                        modalString += "<h2 id = 'drip-teaser'>" + Model.HeadLine + "</h2>";
                        modalString += "<span id='drip-tab-up' class='drip-arrow up'>";
                        modalString += "<svg width = '12px' height='8px' viewBox='1362 659 12 8' version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'>";
                        modalString += "<polygon id ='right_angle' stroke='none' fill='#FFFFFF' fill-rule='evenodd' transform='translate(1368.000000, 662.703125) rotate(-90.000000) translate(-1368.000000, -662.703125)' points = '1364.29688 667.296875 1368.89062 662.703125 1364.29688 658.109375 1365.70312 656.703125 1371.70312 662.703125 1365.70312 668.703125'></polygon>";
                        modalString += "</svg>";
                        modalString += "</span>";
                        modalString += "<span id ='drip-tab-down' class='drip-arrow down' style='display: none'></span>";
                        modalString += "</a>";
                        modalString += "</div>";

                        modalString += "<div id = " + "'drip-content'" + "class=" + "'drip-content drip-clearfix'" + "style='" + Model.WidgetStyle + "'>";
                        // modalString += "<a id='drip-close' class='drip-close' onclick='return ManageContainer(this)'>";
                        // modalString += "<span><b>X</b></span>";
                        // modalString += "</a>";
                        modalString += "<div id = " + "'drip-form-panel'" + "class=" + "'drip-panel drip-clearfix'" + "style=" + "'display: block;'" + ">";
                        if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Left) && Model.ImagePath != null)
                        {
                            modalString += "<div class='drip-form-aside left'>";
                            modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                            modalString += "</div>";
                        }
                        if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Right) && Model.ImagePath != null)
                        {
                            modalString += "<div class='drip-form-aside right'>";
                            modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                            modalString += "</div>";
                        }

                        modalString += "<div class=" + "'drip-form-main'" + ">";
                        modalString += "<h3 id = 'drip-content-header'>" + Model.HeadLine + "</h3>";
                        modalString += "<div id = " + "'drip-scroll'" + "class=" + "'drip-scroll'" + ">";
                        modalString += "<div class=" + "'drip-description'" + ">" + Model.Description + "</div>";

                        modalString += "<form id ='drip-form' action='" + Model.SubmitUrl + "' method='post' class='mainform'>";
                        modalString += "<div style=" + "'display: none'" + ">";
                        modalString += "<input type=" + "'hidden'" + "name =" + "'form_id'" + "value ='" + Model.FormId + "'>";
                        modalString += "</div >";
                        modalString += "<div class='form_max_height'>";
                        modalString += "<dl class=" + "style=" + "'font-size:" + Model.FontSize + "px" + "!important;'" + ">";

                        if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                        {
                            modalString += "<dt class='3'>Email Address";
                            modalString += "<span class='required'></span>";
                            modalString += "</dt>";
                            modalString += "<dd>";
                            modalString += "<input type='text' id='3' name = 'Email_Address' value='' placeholder=" + "''" + "class=" + "'drip-text-field form-control'" + ">";
                            modalString += "<span class='text-danger'>This field is required.</span>";
                            modalString += "</dd>";
                        }

                        if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.InSideTheTextBoxes))
                        {
                            modalString += "<dt id = 'field-lable-3' class='3'>";
                            modalString += "<span class='required'></span>";
                            modalString += "</dt>";
                            modalString += "<dd>";
                            modalString += "<input type='text' id='3' name = 'Email_Address' value='' placeholder='Email Address*' class='drip-text-field form-control'>";
                            //modalString += "<span class='asterisk_input'></span>";
                            modalString += "<span class='text-danger'>This field is required.</span>";
                            modalString += "</dd>";
                        }

                        modalString += "</dl>";
                        modalString += "</div>";
                        modalString += "<div class=" + "'form-controls'" + ">";
                        modalString += "<input type ='button' value='" + Model.ButtonText + "' id='drip-submit' class='drip-submit-button' style='background-color:" + Model.ButtonColor + "!important;'" + " onclick='return SubmitWidget()'/>";
                        modalString += "</div>";
                        modalString += "</form>";
                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";

                        modalString += "<div id=" + "'drip-success-panel-104672'" + "class=" + "'drip-success drip-panel drip-clearfix'" + "style=" + "'display: none'" + ">";
                        modalString += "<h3>Thank you for signing up!</h3>";
                        modalString += "<p class=" + "'drip-description drip-post-submission'" + ">Please check your email and click the link provided to confirm your subscription.</p>";
                        modalString += "</div>";

                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";

                        #endregion
                    }
                    else
                    {
                        #region 'Light Box'
                        modalString += "<div class=" + "'drip-lightbox-wrapper'" + ">";
                        modalString += "<div id = " + "'drip'" + "class=" + "'drip-lightbox side-image no-image bottom right drip-in " + Model.ImagePlaceClass + "'>";
                        modalString += "<div id = " + "'drip-content'" + "class=" + "'drip-content'" + "style=" + Model.WidgetStyle + ">";
                        // modalString += "<a id='drip-close' class='drip-close' onclick='return ManageContainer(this)'><span><b>X</b></span></a>";

                        modalString += "<div id = " + "'drip-form-panel'" + "class=" + "'drip-panel'" + "style=" + "'display: block;'" + ">";
                        if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Left) && Model.ImagePath != null)
                        {
                            modalString += "<div class='drip-form-aside left'>";
                            modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                            modalString += "</div>";
                        }
                        if (Model.ImagePlacement == Convert.ToInt32(Enums.ImagePlacementType.Right) && Model.ImagePath != null)
                        {
                            modalString += "<div class='drip-form-aside right'>";
                            modalString += "<img src=" + "'" + Model.ImagePath + "'" + "class=" + "'hosted-form-image drip-image'" + "/>";
                            modalString += "</div>";
                        }

                        modalString += "<div class=" + "'drip-form-main'" + ">";
                        modalString += "<h3 id = 'drip-content-header'>" + Model.HeadLine + "</h3>";
                        modalString += "<div id = " + "'drip-scroll'" + "class=" + "'drip-scroll'" + ">";
                        modalString += "<div class=" + "'drip-description'" + ">" + Model.Description + "</div>";

                        modalString += "<form id ='drip-form' action='" + Model.SubmitUrl + "' method='post' class='mainform'>";
                        modalString += "<div style=" + "'display: none'" + ">";
                        modalString += "<input type=" + "'hidden'" + "name =" + "'form_id'" + "value =" + Model.FormId + ">";
                        modalString += "</div >";
                        modalString += "<div class='form_max_height'>";
                        modalString += "<dl class=" + "style=" + "'font-size:" + Model.FontSize + "px" + "!important;;'" + ">";
                        if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.AboveTheTextBoxes))
                        {
                            modalString += "<dt class = '3'>Email Address";
                            modalString += "<span class='required'>*</span>";
                            modalString += "</dt>";
                            modalString += "<dd>";
                            modalString += "<input type='text' id='3' name = 'Email_Address' value='' placeholder=" + "''" + "class=" + "'drip-text-field form-control'" + ">";
                            modalString += "<div id=" + "'drip-errors-for-email'" + "class=" + "'drip-errors'" + "></div>";
                            modalString += "</dd>";
                        }

                        if (Model.FormLabelPlaceId == Convert.ToInt32(Enums.PlaceType.InSideTheTextBoxes))
                        {
                            modalString += "<dt class = '3'>Email Address";
                            modalString += "<span class='required'></span>";
                            modalString += "</dt>";
                            modalString += "<dd>";
                            modalString += "<input type='text' id='3' name = 'Email_Address' value='' placeholder='Email Address*' class='drip-text-field form-control'>";
                            //modalString += "<span class='asterisk_input'></span>";
                            modalString += "<span class='text-danger'>This field is required.</span>";
                            modalString += "</dd>";
                        }

                        modalString += "</dl>";
                        modalString += "</div>";
                        modalString += "<div class=" + "'form-controls'" + ">";
                        modalString += "<input type ='button' value='" + Model.ButtonText + "' id='drip-submit' class='drip-submit-button' style='background-color:" + Model.ButtonColor + "!important;'" + " onclick='return SubmitWidget()'/>";
                        modalString += "</div>";
                        modalString += "</form>";
                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";

                        modalString += "<div id=" + "'drip-success-panel'" + "class=" + "'drip-success drip-panel drip-clearfix'" + "style=" + "'display: none'" + ">";
                        modalString += "<h3>Thank you for signing up!</h3>";
                        modalString += "<p class=" + "'drip-description drip-post-submission'" + ">Please check your email and click the link provided to confirm your subscription.</p>";
                        modalString += "</div>";

                        modalString += "</div>";
                        modalString += "</div>";
                        modalString += "</div>";
                        #endregion
                    }
                }
                return modalString;
            }
            catch (Exception ex)
            {
                // ErrorLogs.LogError(ex);
                return "Something gone wrong with design HTML";
            }

        }

        public async Task<RecapchaDto> ValidateHCapchaResponse(RecapchaDto model)
        {
            model.secret = OneClappContext.HcaptchaSiteSecret;
            var client = new WebClient();
            var result = client.DownloadString(string.Format(OneClappContext.HcaptchaVerifyUrl, model.secret, model.response));
            model = JObject.Parse(result).ToObject<RecapchaDto>();
            // var data = JsonConvert.DeserializeObject<RecapchaDto>(result);
            return model;
        }

        public string GetMimeTypes(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }



}


