using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Net;
using System.Net.Mail;
using System.Windows.Controls;
using System.ComponentModel;
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;

namespace PolyclinicProjectKurs
{
    public partial class ChooseHourAppointment : UserControl, INotifyPropertyChanged
    {
        private User _user;
        private Doctor _doctor;
        private DateOnly _selecteddate;
        private string doctorname;
        private List<DateTime> _availableHours;

        public List<DateTime> AvailableHours
        {
            get { return _availableHours; }
            set
            {
                _availableHours = value;
                OnPropertyChanged(nameof(AvailableHours));
            }
        }

        public string ChoosenDoctorName
        {
            get { return doctorname; }
            set
            {
                doctorname = value;
                OnPropertyChanged(nameof(ChoosenDoctorName));
            }
        }

        public DateOnly ChoosenDate
        {
            get { return _selecteddate; }
            set
            {
                _selecteddate = value;
                OnPropertyChanged(nameof(ChoosenDate));
            }
        }

        public ChooseHourAppointment()
        {
            InitializeComponent();
        }

        public ChooseHourAppointment(Doctor doctor, DateOnly selectedDate, User user)
        {
            InitializeComponent();
            _doctor = doctor;
            _selecteddate = selectedDate;
            doctorname = _doctor.Doctorname;
            _user = user;

            // Преобразование строк в DateTime
            TimeOnly startTimeOnly = TimeOnly.Parse(_doctor.Availabletimebefore);
            TimeOnly endTimeOnly = TimeOnly.Parse(_doctor.Availabletimeafter);
            int intervalMinutes = _doctor.Appointmentinterval ?? 30; // Используйте значение по умолчанию, если интервал не указан

            DateTime startTime = _selecteddate.ToDateTime(startTimeOnly);
            DateTime endTime = _selecteddate.ToDateTime(endTimeOnly);

            // Генерация доступного времени
            var allAvailableHours = GenerateAvailableHours(startTime, endTime, intervalMinutes);

            // Получение занятых слотов
            var occupiedSlots = GetOccupiedSlots(_selecteddate);

            // Отфильтровать занятые слоты
            AvailableHours = allAvailableHours.Except(occupiedSlots).ToList();

            DataContext = this;
        }

        // Генерация временных слотов для записи
        private List<DateTime> GenerateAvailableHours(DateTime startTime, DateTime endTime, int intervalMinutes)
        {
            var availableHours = new List<DateTime>();

            for (var time = startTime; time <= endTime; time = time.AddMinutes(intervalMinutes))
            {
                availableHours.Add(time);
            }

            return availableHours;
        }

        private List<DateTime> GetOccupiedSlots(DateOnly selectedDate)
        {
            using (var context = new PolycCursContext())
            {
                // Преобразуем дату в DateTime, устанавливая время на начало дня
                var date = selectedDate.ToDateTime(TimeOnly.MinValue);

                // Получаем все записи на выбранную дату
                var appointments = context.Appointments
                    .Where(a => a.DoctorId == _doctor.DoctorId && a.Appointmenttime == selectedDate)
                    .Select(a => a.AppointmentTime1)
                    .ToList(); // Выполняем запрос и загружаем данные в память

                // Преобразуем TimeOnly в DateTime и возвращаем список занятых слотов
                var occupiedSlots = appointments
                    .Where(timeOnly => timeOnly.HasValue) // Фильтруем только имеющие значение
                    .Select(timeOnly => date.Date.Add(new TimeSpan(timeOnly.Value.Hour, timeOnly.Value.Minute, 0)))
                    .ToList();

                return occupiedSlots;
            }
        }


        private void ChooseHour_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button selectedButton = (Button)sender;
            DateTime selectedTime = (DateTime)selectedButton.DataContext;

            // Формируем текст сообщения
            string message = $"Вы собираетесь записаться к врачу {_doctor.Doctorname} {_selecteddate:dd MMMM yyyy} на {selectedTime:HH:mm}. Подтвердить запись?";

            // Показываем окно подтверждения
            MessageBoxResult result = MessageBox.Show(message, "Подтверждение записи", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Запись в базу данных
                using (var context = new PolycCursContext())
                {
                    //если пользователь не авторизован то 
                    if (_user == null)
                    {
                        FillUserDataPage fillUserDataPage = new FillUserDataPage();
                        if (fillUserDataPage.ShowDialog() == true)
                        {
                            var appointmentWithoutAuth = new DoctorAppointmentWithoutAuthorization
                            {
                                LastName = fillUserDataPage.LastName,
                                FirstName = fillUserDataPage.FirstName,
                                Patronymic = fillUserDataPage.Patronymic,
                                BirthDate = fillUserDataPage.BirthDate,
                                Email = fillUserDataPage.Email,
                                Phone = fillUserDataPage.Phone,
                                DoctorId = _doctor.DoctorId,
                                AppointmentDate = _selecteddate,
                                AppointmentTime = TimeOnly.FromDateTime(selectedTime),
                            };

                            context.DoctorAppointmentsWithoutAuthorization.Add(appointmentWithoutAuth);
                            context.SaveChanges();

                            var emailService = new EmailService();
                            emailService.SendEmail(fillUserDataPage.Email, appointmentWithoutAuth);

                            MessageBox.Show("Запись успешно добавлена и отправлена на ваш email.");
                        }
                        //тут нужно открывать поверх окно с вводом данных: почта, куда отправлять талон, ФИО
                        //а потом уже сохранять данные в отдельную таблицу CallDoctorWithoutAuth

                    }
                    else
                    {
                        var appointment = new Appointment
                        {
                            DoctorId = _doctor.DoctorId,
                            UserId = _user.UserId,
                            Appointmenttime = _selecteddate,
                            AppointmentTime1 = TimeOnly.FromDateTime(selectedTime),
                            AppointmentStatus = "Предстоящие"
                        };

                        context.Appointments.Add(appointment);
                        context.SaveChanges();

                        var emailService = new EmailService();
                        emailService.SendEmail(_user.Useremail, appointment);
                    }
                }

                MessageBox.Show("Запись успешно добавлена.");

                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow.ContentControlPage.Content = new MainMenu(_user); // замените на Profile, если необходимо

                mainWindow.CloseSideMenu();
            }
            else
            {
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow.ContentControlPage.Content = new DoctorAppointment(_user); // замените на Profile, если необходимо

                mainWindow.CloseSideMenu();
                // Вернуться в окно выбора врача
                // Реализуйте логику возврата, если необходимо
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
