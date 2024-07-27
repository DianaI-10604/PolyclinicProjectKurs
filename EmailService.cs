using PolyclinicProjectKurs.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;

namespace PolyclinicProjectKurs
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.yandex.ru";
        private readonly int _smtpPort = 465; // Используйте порт 465 для SMTPS
        private readonly string _smtpUser = "avaloniaapplication@yandex.ru";
        private readonly string _smtpPassword = "zvaeicplnvxhtbxj"; // Ваш новый пароль приложения

        public void SendEmail(string recipientEmail, object appointment)
        {
            string subject = "Ваш талон на запись к врачу";
            string body = "<html><body> Test Body </body></html>";

            try
            {
                MailAddress from = new MailAddress(_smtpUser, "Tom");
                MailAddress to = new MailAddress("delirius.cody@gmail.com");
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Тест";
                m.Body = "Письмо-тест 2 работы smtp-клиента";
                SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 465);
                smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtp.EnableSsl = true;
                smtp.Send(m);

                //using (SmtpClient smtp = new SmtpClient(_smtpServer, _smtpPort))
                //{
                //    smtp.EnableSsl = true; // Включение SSL
                //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //    smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);

                //    MailMessage mail = new MailMessage();
                //    mail.From = new MailAddress(_smtpUser);
                //    mail.To.Add(recipientEmail);
                //    mail.Subject = subject;
                //    mail.Body = body;
                //    mail.IsBodyHtml = true;

                //    // Асинхронная отправка письма
                //    await smtp.SendMailAsync(mail);
                //}

                MessageBox.Show("Письмо успешно отправлено.");
            }
            catch (SmtpException smtpEx)
            {
                // Логирование информации об ошибке
                MessageBox.Show($"Ошибка отправки письма: {smtpEx.Message}\nInner Exception: {smtpEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // Логирование информации об ошибке
                MessageBox.Show($"Произошла ошибка: {ex.Message}\nInner Exception: {ex.InnerException?.Message}");
            }
        }
    }
}
