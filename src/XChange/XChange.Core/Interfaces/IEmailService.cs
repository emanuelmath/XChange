using System;
using System.Collections.Generic;
using System.Text;

namespace XChange.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toName, string toEmail, string subject, string body, string htmlBody);
    }
}
