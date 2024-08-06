using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;

namespace PolyclinicProjectKurs
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, object appointment, User user, Doctor doctor)
        {
            using var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "dlya.urokov9kl@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Ваш талон на запись к врачу";

            // Генерация PDF-файла
            string pdfFilePath;
            try
            {
                var pdfContent = GeneratePdfContent(appointment, user, doctor);
                pdfFilePath = CreatePdf(pdfContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации PDF: {ex.Message}");
                return;
            }

            // Создание MimePart для вложения
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(pdfFilePath)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "AppointmentDetails.pdf"
            };

            // Создание тела письма
            var body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<p>Пожалуйста, найдите ваше талон на запись к врачу во вложении.</p>"
            };

            // Создание multipart-части для сообщения с вложением
            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);

            emailMessage.Body = multipart;

            using (var client = new SmtpClient())
            {
                var cts = new CancellationTokenSource();
                var timeout = 10000; // Устанавливаем тайм-аут в 10 секунд
                var timeoutTask = Task.Delay(timeout, cts.Token);

                try
                {
                    // Подключение
                    var connectTask = client.ConnectAsync("smtp.gmail.com", 465, true);
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        MessageBox.Show("Время выполнения истекло при подключении!");
                        client.Disconnect(true);
                        return;
                    }

                    await connectTask; // Завершаем подключение

                    // Аутентификация
                    var authenticateTask = client.AuthenticateAsync("dlya.urokov9kl@gmail.com", "ecvvboajaesekylo");
                    completedTask = await Task.WhenAny(authenticateTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        MessageBox.Show("Время выполнения истекло при аутентификации!");
                        client.Disconnect(true);
                        return;
                    }

                    await authenticateTask; // Завершаем аутентификацию

                    // Отправка письма
                    var sendTask = client.SendAsync(emailMessage);
                    completedTask = await Task.WhenAny(sendTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        MessageBox.Show("Время выполнения истекло при отправке!");
                        client.Disconnect(true);
                        return;
                    }

                    await sendTask; // Завершаем отправку

                    MessageBox.Show("Письмо успешно отправлено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка отправки письма: {ex.Message}");
                }
                finally
                {
                    // Обеспечиваем отключение клиента
                    await client.DisconnectAsync(true);
                }
            }
        }

        private string GeneratePdfContent(object appointment, User user, Doctor doctor)
        {
            if (appointment is DoctorAppointmentWithoutAuthorization appt)
            {
                var doctorName = appt.Doctor?.Doctorname;
                var appointmentDate = appt.AppointmentDate.ToString("yyyy-MM-dd");
                var appointmentTime = appt.AppointmentTime.ToString("HH:mm");
                var firstName = appt.FirstName;
                var lastName = appt.LastName;

                return $"Уважаемый(ая) {firstName} {lastName},\n" +
                       $"Вы записаны к врачу {doctorName}\nДата:{appointmentDate}\n Время:{appointmentTime}.\n" +
                       "Спасибо за использование нашего сервиса!";
            }

            else if (appointment is Appointment appt2)
            {
                var userName = user?.Username;
                var userSurname = user?.Usersurname;
                var doctorName = doctor?.Doctorname;
                var appointmentDate = appt2.Appointmenttime;
                var appointmentTime = appt2.AppointmentTime1;

                return $"Уважаемый(ая) {userName} {userSurname},\n" +
                        $"Вы записаны к врачу {doctorName}\nДата:{appointmentDate}\n Время:{appointmentTime}.\n" +
                       "Спасибо за использование нашего сервиса!";
            }
            return string.Empty;
        }

        private string CreatePdf(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Содержимое для PDF пустое.");
                return string.Empty;
            }

            var filePath = Path.Combine(Path.GetTempPath(), "AppointmentDetails.pdf");

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Rectangle pageSize = new Rectangle(300f, 400f); // Задаем размеры страницы (например, 300x400 пикселей)
                    Document doc = new Document(pageSize, 10, 10, 10, 10);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                    // Путь к шрифту TimesNewRomanRegular
                    var fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fonts", "TimesNewRomanRegular.ttf");

                    // Создаем BaseFont с использованием TimesNewRomanRegular
                    BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    // Создаем шрифт с использованием BaseFont
                    var font = new Font(baseFont, 13, Font.NORMAL);

                    doc.Open();
                    doc.Add(new Paragraph(content, font));
                    doc.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании PDF: {ex.Message}");
                return string.Empty;
            }

            return filePath;
        }


    }
}
