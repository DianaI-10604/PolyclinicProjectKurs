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
using PolyclinicProjectKurs.Models;
using PolyclinicProjectKurs.Context;
using System.Collections.ObjectModel;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для DoctorAppointment.xaml
    /// </summary>
    public partial class DoctorAppointment : UserControl
    {
        public ObservableCollection<Doctor> DoctorsList { get; set; }
        private User _user;
        public DoctorAppointment()
        {
            InitializeComponent();
        }

        public DoctorAppointment(User user)
        {
            InitializeComponent();
            _user = user;

            using (var context = new PolycCursContext())
            {
                var doctorslist = context.Doctors.ToList(); // Получаем всех врачей из базы данных

                DoctorsList = new ObservableCollection<Doctor>(doctorslist);
            }
            DataContext = this;
        }
        
        private void ChooseDoctorButtonClick(object sender, RoutedEventArgs e)
        {
            Button doctor = (Button)sender;
            Doctor selectedDoctor = (Doctor)doctor.DataContext;

            if (selectedDoctor != null)
            {
                using (var dbContext = new PolycCursContext())
                {
                    Doctor showdoctor = dbContext.Doctors.FirstOrDefault(d => d.DoctorId == selectedDoctor.DoctorId);

                    if (showdoctor != null)
                    {
                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow.ContentControlPage.Content = new ChooseAppointmentDate(showdoctor, _user);

                        mainWindow.CloseSideMenu();
                    }
                }
            }
        }
    }
}
