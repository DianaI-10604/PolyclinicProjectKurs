using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.ComponentModel;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для MyProfile.xaml
    /// </summary>
    public partial class MyProfile : UserControl, INotifyPropertyChanged
    {
        private User _user;
        private DoctorAccount _account;

 

        public MyProfile()
        {
            InitializeComponent();
        }

        public MyProfile(User user, DoctorAccount account)
        {
            InitializeComponent();
            _user = user;
            _account = account;

            using (var db = new PolycCursContext())
            {
                if (_user != null)
                {
                    DataContext = _user;

                    doctorPanel.Visibility = Visibility.Collapsed;
                    statusPanel.Visibility = Visibility.Visible;
                }
                else if (_account != null)
                {
                    // Запрашиваем информацию о специальности врача
                    var doctor = db.Doctors.FirstOrDefault(d => d.DoctorId == _account.DoctorId);
                    if (doctor != null)
                    {
                        // Создаем объект, который будет использоваться для привязки данных
                        var doctorContext = new
                        {
                            Useremail = _account.Useremail,
                            Phone = _account.Phone,
                            Speciality = doctor.Speciality
                        };

                        DataContext = doctorContext;

                        doctorPanel.Visibility = Visibility.Visible;
                        statusPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
