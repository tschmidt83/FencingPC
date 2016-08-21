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

namespace FencingPC
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cbFencerGender.Items.Add(GetResourceString("str_Gender_Male"));
            cbFencerGender.Items.Add(GetResourceString("str_Gender_Female"));
            cbFencerGender.SelectedIndex = 0;

            cbFencerMembership.Items.Add(GetResourceString("str_Member_Regular"));
            cbFencerMembership.Items.Add(GetResourceString("str_Member_Student"));
            cbFencerMembership.Items.Add(GetResourceString("str_Member_Guest"));
            cbFencerMembership.SelectedIndex = 0;
        }

        private string GetResourceString(string identifier)
        {
            string result = string.Empty;

            if (Application.Current.Resources[identifier] != null)
                result = Application.Current.Resources[identifier].ToString();

            return result;
        }

        private void btnEditFencer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewFencer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteFencer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerImage_Webcam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerImage_Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerOK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
