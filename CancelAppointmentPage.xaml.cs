using PolyclinicProjectKurs.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Логика взаимодействия для CancelAppointmentPage.xaml
    /// </summary>
    public partial class CancelAppointmentPage : UserControl
    {
        public CancelAppointmentPage()
        {
            InitializeComponent();
        }

        private void FindAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastNameTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string phone = PhoneTextBox.Text;

            using (var context = new PolycCursContext())
            {
                var appointment = context.DoctorAppointmentsWithoutAuthorization
                    .FirstOrDefault(a => a.LastName == lastName && a.FirstName == firstName && a.Patronymic == patronymic && a.Phone == phone);

                if (appointment != null)
                {
                    var doctor = context.Doctors.FirstOrDefault(d => d.DoctorId == appointment.DoctorId);
                    if (doctor != null)
                    {
                        string appointmentInfo = $"Дата записи: {appointment.AppointmentDate.ToShortDateString()}\n" +
                                                 $"Имя врача: {doctor.Doctorname} \n" +
                                                 $"Время приема: {appointment.AppointmentTime}\n" +
                                                 "Отменить запись?";

                        MessageBoxResult result = MessageBox.Show(appointmentInfo, "Подтверждение отмены", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            context.DoctorAppointmentsWithoutAuthorization.Remove(appointment);
                            context.SaveChanges();
                            MessageBox.Show("Запись успешно отменена.");

                            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                            mainWindow.ContentControlPage.Content = new ServicesWithoutAuth();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Запись не найдена.");
                }
            }
        }
    }
}
