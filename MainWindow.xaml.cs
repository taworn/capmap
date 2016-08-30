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

        private string title = null;

        private Map mapObject = null;
        private bool mapChanged = false;
        private string mapFileName = null;

        private Image imageSource = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            title = Title;
            radioBlock.IsChecked = true;
            imageSource = imageBlock;
            EnableMenu(false);
        }

        private void Window_Closed(object sender, EventArgs e) {
            //
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var image = (Image)sender;
            if (imageSource != null) {
                int imageTag = int.Parse(image.Tag.ToString());
                int imageSourceTag = int.Parse(imageSource.Tag.ToString());
                mapObject.Set(imageTag, imageSourceTag);
                image.Source = imageSource.Source;

                if (!mapChanged) {
                    mapChanged = true;
                    Title = (mapChanged ? "* " : "") + (mapFileName != null ? mapFileName : "new map") + " - " + title;
                }
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) {
            var radio = (RadioButton)sender;
            if (mapObject != null) {
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
        }

        private void menuFileNew_Click(object sender, RoutedEventArgs e) {
            // if maps opened and its changed
            if (mapObject != null && mapChanged) {
                var r = MessageBox.Show(this, "Map data does not save.  Are you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (r == MessageBoxResult.No)
                    return;
            }

            // starts new map
            mapObject = new Map(16, 16);
            mapChanged = false;
            mapFileName = null;
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

            Title = (mapChanged ? "* " : "") + (mapFileName != null ? mapFileName : "new map") + " - " + title;
            EnableMenu(true);
        }

        private void menuFileOpen_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".map";
            dialog.FileName = "";
            dialog.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result == true) {
                // if maps opened and its changed
                if (mapObject != null && mapChanged) {
                    var r = MessageBox.Show(this, "Map data does not save.  Are you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (r == MessageBoxResult.No)
                        return;
                }

                // starts new map
                mapObject = new Map(16, 16);
                mapObject.Open(dialog.FileName);
                mapChanged = false;
                mapFileName = dialog.FileName;
                var children = mapGrid.Children;
                children.Clear();
                for (int i = 0; i < 256; i++) {
                    var image = new Image();
                    image.Source = MapDataToImage(mapObject.Get(i)).Source;
                    image.Margin = new Thickness(1, 1, 1, 1);
                    image.MouseDown += Image_MouseDown;
                    image.Tag = i;
                    children.Add(image);
                }

                Title = (mapChanged ? "* " : "") + (mapFileName != null ? mapFileName : "new map") + " - " + title;
                EnableMenu(true);
            }
        }

        private void menuFileSave_Click(object sender, RoutedEventArgs e) {
            if (mapFileName != null) {
                mapObject.Save(mapFileName);
                mapChanged = false;
            }
            else {
                menuFileSaveAs_Click(sender, e);
            }
        }

        private void menuFileSaveAs_Click(object sender, RoutedEventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".map";
            dialog.FileName = "";
            dialog.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result == true) {
                mapFileName = dialog.FileName;
                mapObject.Save(mapFileName);
                mapChanged = false;
            }
        }

        private void menuFileExportToJavaScript_Click(object sender, RoutedEventArgs e) {
            //
        }

        private void menuFileClose_Click(object sender, RoutedEventArgs e) {
            // if maps opened and its changed
            if (mapObject != null && mapChanged) {
                var r = MessageBox.Show(this, "Map data does not save.  Are you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (r == MessageBoxResult.No)
                    return;
            }

            mapObject = null;
            mapChanged = false;
            mapFileName = null;
            var children = mapGrid.Children;
            children.Clear();
        }

        private void menuFileExit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void EnableMenu(bool enabled) {
            menuFileSave.IsEnabled = enabled;
            menuFileSaveAs.IsEnabled = enabled;
            menuFileExportToJavaScript.IsEnabled = enabled;
            menuFileClose.IsEnabled = enabled;
        }

        private Image MapDataToImage(int data) {
            switch (data) {
                default:
                case 1:
                    return imageBlock;
                case 2:
                    return imageMovable;
                case 3:
                    return imageCoke;
                case 4:
                    return imageBread;
                case 5:
                    return imageDivo;
                case 6:
                    return imagePacman;
            }
        }
    }
}
