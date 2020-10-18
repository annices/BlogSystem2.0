using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

/*
Copyright (c) 2020 Annice Strömberg – Annice.se

This script is MIT (Massachusetts Institute of Technology) licensed, which means that
permission is granted, free of charge, to any person obtaining a copy of this software
and associated documentation files to deal in the software without restriction. This
includes, without limitation, the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the software, and to permit persons to whom the software
is furnished to do so subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the software.
*/
namespace BlogSystem.Services
{
    /// <summary>
    /// This is a service class to send password recovery emails to the admin user.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Inject dependencies.
        /// </summary>
        /// <param name="config"></param>
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Method to send an email via SMTP settings.
        /// </summary>
        /// <param name="to">Receiver email.</param>
        /// <param name="from">Sender email.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="message">Email message.</param>
        public void SendEmail(string to, string from, string subject, string message)
        {   
            // The email server account that emails will be sent from:
            string sender = _config["EmailSettings:From"];

            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress(sender, "Blog System - Password Recovery", System.Text.Encoding.UTF8);
            mail.Subject = subject;
            // Encode to UTF8 to make sure that letters other than English ones are read properly:
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = message;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = false;
            mail.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(sender, _config["EmailSettings:Password"]),
                Port = Convert.ToInt32(_config["EmailSettings:Port"]),
                Host = _config["EmailSettings:Host"],
                EnableSsl = true
            };

            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

    } // End class.
} // End namespace.
