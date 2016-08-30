using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace capmap {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Map map = null;
        private bool changed = false;
        private string fileName = null;

        private Image imageSource = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            radioBlock.IsChecked = true;

            var children = mapGrid.Children;
            children.Clear();
            for (int i = 0; i < 256; i++) {
                var image = new Image();
                image.Source = imageBlock.Source;
                image.Margin = new Thickness(1, 1, 1, 1);
                image.MouseDown += Image_MouseDown;
                image.Tag = i;
                children.Add(image);
            }
        }

        private void Window_Closed(object sender, EventArgs e) {
            //
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var image = (Image)sender;
            if (imageSource != null) {
                image.Source = imageSource.Source;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) {
            var radio = (RadioButton)sender;
            var tag = radio.Tag.ToString();

            if (tag == "1") {
                imageSource = imageBlock;
            }
            else if (tag == "2") {
                imageSource = imageMovable;
            }
            else if (tag == "3") {
                imageSource = imageCoke;
            }
            else if (tag == "4") {
                imageSource = imageBread;
            }
            else if (tag == "5") {
                imageSource = imageDivo;
            }
            else if (tag == "6") {
                imageSource = imagePacman;
            }
            else {
                imageSource = null;
            }

        }

        private void menuFileNew(object sender, RoutedEventArgs e) {
            /*
            var children = mapGrid.Children;
            children.Clear();
            for (int i = 0; i < 256; i++) {
                var image = new Image();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/capmap;component/res/block.png");
                bitmap.EndInit();
                image.Source = bitmap;
                children.Add(image);
            }
            */
        }

        private void menuFileOpen(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".map";
            dialog.FileName = "";
            dialog.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result == true) {
                //
            }
        }

        private void menuFileSave(object sender, RoutedEventArgs e) {
            //
        }

        private void menuFileSaveAs(object sender, RoutedEventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".map";
            dialog.FileName = "";
            dialog.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result == true) {
                //
            }
        }

        private void menuFileExportToJavaScript(object sender, RoutedEventArgs e) {
            //
        }

        private void menuFileClose(object sender, RoutedEventArgs e) {
            /*
            var children = mapGrid.Children;
            children.Clear();
            */
        }

        private void menuFileExit(object sender, RoutedEventArgs e) {
            Close();
        }

    }
}
