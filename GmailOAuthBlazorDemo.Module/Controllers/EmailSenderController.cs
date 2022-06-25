using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using GmailOAuthBlazorDemo.Module.BusinessObjects;
using GmailOAuthBlazorDemo.Module.BusinessObjects.EmailForm;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GmailOAuthBlazorDemo.Module.Controllers
{
    public class EmailSenderController : ObjectViewController<DetailView, Client>
    {
        PopupWindowShowAction showSendEmailForm;
        public EmailSenderController()
        {
            showSendEmailForm = new PopupWindowShowAction(this, "acShowSenEmailForm", DevExpress.Persistent.Base.PredefinedCategory.Tools);
            showSendEmailForm.Caption = "Send Email";
            showSendEmailForm.CustomizePopupWindowParams += ShowSendEmailForm_CustomizePopupWindowParams;
        }
        private void ShowSendEmailForm_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {



            var os = Application.CreateObjectSpace(typeof(EmailForm));
            var emailForm = os.CreateObject<EmailForm>();
            var detailview = Application.CreateDetailView(os, emailForm);
            detailview.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            e.View = detailview;
            e.DialogController.AcceptAction.Caption = "Send";
            e.DialogController.SaveOnAccept = true;

            e.DialogController.Accepting += (sender, args) =>
            {
                try
                {
                    //https://www.emailarchitect.net/easendmail/sdk/html/object_oauth.htm
                    //https://developers.google.com/gmail/api/quickstart/dotnet
                    //http://www.mimekit.net/docs/html/Creating-Messages.htm#CreateMessageWithAttachments
                    //redirect authorize  URL http://127.0.0.1/authorize/
                    UserCredential credential;
                    using (var stream =
                           new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                    {
                        /* The file token.json stores the user's access and refresh tokens, and is created
                         automatically when the authorization flow completes for the first time. */
                        string credPath = "token.json";
                        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.FromStream(stream).Secrets,
                            new[] { "email", "profile", "https://mail.google.com/" },
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)).Result;

                    }

                    var jwtPayload = GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken, null, true).Result;
                    var username = jwtPayload.Email;
                    var accessToken = credential.Token.AccessToken;

                    var oauth2 = new SaslMechanismOAuth2(username, credential.Token.AccessToken);

                    MimeMessage msg = new MimeMessage();
                    msg.From.Add(new MailboxAddress("nombre del usuario", "correo del remitente"));

                    msg.To.Add(new MailboxAddress("nombre del receptor", "correo del receptor"));

                    msg.Subject = emailForm.Subject;
                    var bodyBuilder = new BodyBuilder();
                    //Nota para agregar attachments
                    //bodyBuilder.Attachments.Add($"{DateTime.Today.ToShortDateString()}-Predispatch.xlsx", memorystream);
                    bodyBuilder.TextBody = emailForm.MessageBody;
                    msg.Body = bodyBuilder.ToMessageBody();
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        client.Authenticate(oauth2);
                        client.Send(msg);
                        client.Disconnect(true);
                    }

                    MessageOptions success = new MessageOptions();
                    success.Duration = 3000;
                    success.Type = InformationType.Success;
                    success.Message = "Email Send Successfully";
                    Application.ShowViewStrategy.ShowMessage(success);
                }
                catch (Exception ex)
                {

                    throw new UserFriendlyException(ex.Message);
                }


            };



        }
    }
}
