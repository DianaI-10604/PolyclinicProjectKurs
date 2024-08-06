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
using PolyclinicProjectKurs.Context;
using PolyclinicProjectKurs.Models;

namespace PolyclinicProjectKurs
{
    /// <summary>
    /// Логика взаимодействия для Contacts.xaml
    /// </summary>
    public partial class Contacts : UserControl
    {
        public string Address { get; set; } = "197183, Санкт-Петербург, улица Оскаленко, дом 18 лит. А";
        public string RegNumber { get; set; } = "+7 (812) 241-33-64";
        public string Type { get; set; } = "Поликлиника для взрослых";
        public string Zaved { get; set; } = "Нестерова Елена Николаевна\nТел.: 241-33-70\nEmail: p49_op33@zdrav.spb.ru";
        public string ZavedTerap { get; set; } = "Ежова Ольга Валентиновна\nТел.: 241-33-67";
        public string Proezd { get; set; } = "До поликлиники №33 можно доехать от станции метро " +
            "«Черная речка» или «Старая деревня» автобусом №211 или маршрутным такси N132, 133," +
            " 405 (выход на остановке «Улица Оскаленко»).";

        private User _user;
        public Contacts(User user)
        {
            InitializeComponent();
            DataContext = this;
            _user = user;
        }
    }

}
