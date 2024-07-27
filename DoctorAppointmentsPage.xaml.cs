using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для DoctorAppointmentsPage.xaml
    /// </summary>
    public partial class DoctorAppointmentsPage : UserControl
    {
        public ObservableCollection<Appointment> DoctorAppointmentsCollection { get; set; }
        private Doctor _doctor;

        public DoctorAppointmentsPage()
        {
            InitializeComponent();
        }

        public DoctorAppointmentsPage(Doctor doctor)
        {
            InitializeComponent();
            _doctor = doctor;

            LoadAppointments();

            DataContext = this; // Установка контекста данных на текущий UserControl
        }

        private void LoadAppointments()
        {
            using (var context = new PolycCursContext())
            {
                // Загружаем все завершенные записи для указанного врача
                var appointmentList = context.Appointments
                    .Include(a => a.User)
                    .Where(a => a.DoctorId == _doctor.DoctorId && a.AppointmentStatus == "Завершенные")
                    .ToList();

                // Для каждой записи загружаем соответствующую медицинскую запись
                foreach (var appointment in appointmentList)
                {
                    var medicalRecord = context.Medicalrecords
                        .FirstOrDefault(m => m.AppointmentId == appointment.Id && m.DoctorId == _doctor.DoctorId);

                    if (medicalRecord != null)
                    {
                        appointment.Medicalrecords = new List<Medicalrecord> { medicalRecord };
                    }
                    else
                    {
                        appointment.Medicalrecords = new List<Medicalrecord>();
                    }
                }

                // Создаем коллекцию для отображения
                DoctorAppointmentsCollection = new ObservableCollection<Appointment>(appointmentList);
            }
        }

        private void ChangeDiagnosisButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент из коллекции
            var selectedAppointment = (sender as Button).DataContext as Appointment;

            // Если у выбранного приема есть медицинские записи, открываем окно для изменения диагноза
            if (selectedAppointment != null && selectedAppointment.Medicalrecords.Any())
            {
                var medicalRecord = selectedAppointment.Medicalrecords.First();
                ChangeDiagnosisPage changeDiagnosisPage = new ChangeDiagnosisPage(medicalRecord);
                if (changeDiagnosisPage.ShowDialog() == true)
                {
                    // Обновить диагноз в базе данных и коллекции после закрытия окна
                    using (var context = new PolycCursContext())
                    {
                        var record = context.Medicalrecords.FirstOrDefault(m => m.Id == medicalRecord.Id);
                        if (record != null)
                        {
                            record.Diagnosis = changeDiagnosisPage.UpdatedMedicalRecord.Diagnosis;
                            context.SaveChanges();

                            // Обновить коллекцию и UI
                            var updatedRecord = record;
                            UpdateMedicalRecordInCollection(selectedAppointment, updatedRecord);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти запись в базе данных.");
                        }
                    }
                }
            }
        }

        private void UpdateMedicalRecordInCollection(Appointment appointment, Medicalrecord updatedRecord)
        {
            var record = appointment.Medicalrecords.FirstOrDefault(m => m.Id == updatedRecord.Id);
            if (record != null)
            {
                record.Diagnosis = updatedRecord.Diagnosis;
                // Уведомление об изменениях
                var index = DoctorAppointmentsCollection.IndexOf(appointment);
                DoctorAppointmentsCollection[index] = null; // Чтобы DataGrid обновил привязку
                DoctorAppointmentsCollection[index] = appointment;
            }
        }
    }
}
