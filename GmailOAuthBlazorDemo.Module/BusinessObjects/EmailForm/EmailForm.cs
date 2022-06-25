using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace GmailOAuthBlazorDemo.Module.BusinessObjects.EmailForm
{
    [DomainComponent]
    public class EmailForm : NonPersistentBaseObject
    {
        public EmailForm()
        {

        }

        string from;
        [RuleRequiredField("EmailFormValidateFromNotNullDestination", DefaultContexts.Save, CustomMessageTemplate = "The from field shouldn't be empty", TargetContextIDs = "DialogOK")]
        public string From
        {
            get => from;
            set => SetPropertyValue(ref from, value);
        }
        string to;
        [RuleRequiredField("EmailFormValidateToNotNullDestination", DefaultContexts.Save, CustomMessageTemplate = "The destination field shouldn't be empty", TargetContextIDs = "DialogOK")]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string To
        {
            get => to;
            set => SetPropertyValue(ref to, value);
        }

        string subject;
        [RuleRequiredField("EmailFormValidateSubjectNotNullDestination", DefaultContexts.Save, CustomMessageTemplate = "The subject field shouldn't be empty", TargetContextIDs = "DialogOK")]
        public string Subject
        {
            get => subject;
            set => SetPropertyValue(ref subject, value);
        }
      
        string messageBody;

        public string MessageBody
        {
            get => messageBody;
            set => SetPropertyValue(ref messageBody, value);
        }

    }
}