﻿using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace capmap {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private string title = null;
        private Image imageSource = null;

        private Map mapObject = null;
        private bool mapChanged = false;
        private string mapFileName = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            title = Title;
            radioBlock.IsChecked = true;
            imageSource = imageBlock;
            EnableMenu(false);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!CheckFileSave()) {
                e.Cancel = true;
            }
        }

        private void menuFileNew_Click(object sender, RoutedEventArgs e) {
            if (!CheckFileSave())
                return;

            var dialog = new SetSizeWindow();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true) {
                // starts new map
                mapObject = new Map(dialog.GetResultWidth(), dialog.GetResultHeight());
                mapChanged = false;
                mapFileName = null;
                mapGrid.Columns = mapObject.GetWidth();
                mapGrid.Rows = mapObject.GetHeight();
                mapGrid.Width = mapGrid.Columns * 48;
                mapGrid.Height = mapGrid.Rows * 48;
                var children = mapGrid.Children;
                children.Clear();
                var size = mapObject.GetWidth() * mapObject.GetHeight();
                for (var i = 0; i < size; i++) {
                    var image = new Image();
                    image.Source = imageBlock.Source;
                    image.Margin = new Thickness(1, 1, 1, 1);
                    image.MouseDown += Image_MouseDown;
                    image.Tag = i;
                    children.Add(image);
                }

                textSize.Text = mapObject.GetWidth() + " x " + mapObject.GetHeight();
                UpdateTitle();
                EnableMenu(true);
            }
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
                if (!CheckFileSave())
                    return;

                // opens old map
                mapObject = new Map(1, 1);
                try {
                    mapObject.Open(dialog.FileName);
                    mapChanged = false;
                    mapFileName = dialog.FileName;
                    mapGrid.Columns = mapObject.GetWidth();
                    mapGrid.Rows = mapObject.GetHeight();
                    mapGrid.Width = mapGrid.Columns * 48;
                    mapGrid.Height = mapGrid.Rows * 48;
                    var children = mapGrid.Children;
                    children.Clear();
                    var size = mapObject.GetWidth() * mapObject.GetHeight();
                    for (var i = 0; i < size; i++) {
                        var image = new Image();
                        image.Source = MapDataToImage(mapObject.Get(i)).Source;
                        image.Margin = new Thickness(1, 1, 1, 1);
                        image.MouseDown += Image_MouseDown;
                        image.Tag = i;
                        children.Add(image);
                    }

                    textSize.Text = mapObject.GetWidth() + " x " + mapObject.GetHeight();
                    UpdateTitle();
                    EnableMenu(true);
                }
                catch (IOException ex) {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                    mapObject = null;
                    mapChanged = false;
                    mapFileName = null;
                    var children = mapGrid.Children;
                    children.Clear();
                    textSize.Text = "";
                    Title = title;
                    EnableMenu(false);
                }
            }
        }

        private void menuFileSave_Click(object sender, RoutedEventArgs e) {
            if (mapFileName != null) {
                mapObject.Save(mapFileName);
                mapChanged = false;
                UpdateTitle();
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
                UpdateTitle();
            }
        }

        private void menuFileExportToJavaScript_Click(object sender, RoutedEventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".js";
            dialog.FileName = mapFileName != null ? Path.GetFileNameWithoutExtension(mapFileName) + ".js" : "untitle.js";
            dialog.Filter = "JavaScript Files (*.js)|*.js";
            var result = dialog.ShowDialog();
            if (result == true) {
                mapObject.ExportJavaScript(dialog.FileName);
            }
        }

        private void menuFileClose_Click(object sender, RoutedEventArgs e) {
            if (!CheckFileSave())
                return;

            mapObject = null;
            mapChanged = false;
            mapFileName = null;
            var children = mapGrid.Children;
            children.Clear();

            textSize.Text = "";
            Title = title;
            EnableMenu(false);
        }

        private void menuFileExit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void menuEditFillCoke_Click(object sender, RoutedEventArgs e) {
            var children = mapGrid.Children;
            var size = mapObject.GetWidth() * mapObject.GetHeight();
            for (var i = 0; i < size; i++) {
                if (mapObject.Get(i) == 1) { // is a movable
                    mapObject.Set(i, 2); // replace with coke
                    ((Image)children[i]).Source = imageCoke.Source;
                }
            }
        }

        private void menuEditFillBread_Click(object sender, RoutedEventArgs e) {
            var children = mapGrid.Children;
            var size = mapObject.GetWidth() * mapObject.GetHeight();
            for (var i = 0; i < size; i++) {
                if (mapObject.Get(i) == 1) { // is a movable
                    mapObject.Set(i, 3); // replace with bread
                    ((Image)children[i]).Source = imageBread.Source;
                }
            }
        }

        private void menuEditRemoveAllItems_Click(object sender, RoutedEventArgs e) {
            var children = mapGrid.Children;
            var size = mapObject.GetWidth() * mapObject.GetHeight();
            for (var i = 0; i < size; i++) {
                if (mapObject.Get(i) == 2 || mapObject.Get(i) == 3) { // is a coke or a bread
                    mapObject.Set(i, 1); // replace with movable
                    ((Image)children[i]).Source = imageMovable.Source;
                }
            }
        }

        private void menuHelpAbout_Click(object sender, RoutedEventArgs e) {
            var dialog = new AboutWindow();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var image = (Image)sender;
            if (imageSource != null) {
                int imageTag = int.Parse(image.Tag.ToString());
                int imageSourceTag = int.Parse(imageSource.Tag.ToString());

                if (mapObject.Get(imageTag) != imageSourceTag) {
                    if (mapObject.Get(imageTag) == 4) {
                        var x = -1;
                        var y = -1;
                        mapObject.SetDivoStart(x, y);
                    }
                    if (mapObject.Get(imageTag) == 5) {
                        var x = -1;
                        var y = -1;
                        mapObject.SetPacmanStart(x, y);
                    }

                    mapObject.Set(imageTag, imageSourceTag);
                    image.Source = imageSource.Source;

                    if (imageSourceTag == 4) {
                        // only one Divo start point
                        var x = mapObject.GetDivoStartX();
                        var y = mapObject.GetDivoStartY();
                        if (x >= 0 && y >= 0) {
                            mapObject.Set(y * mapObject.GetWidth() + x, 1);
                            ((Image)mapGrid.Children[y * mapObject.GetWidth() + x]).Source = imageMovable.Source;
                        }
                        x = imageTag % mapObject.GetWidth();
                        y = imageTag / mapObject.GetWidth();
                        mapObject.SetDivoStart(x, y);
                    }
                    if (imageSourceTag == 5) {
                        // only one Pacman start point
                        var x = mapObject.GetPacmanStartX();
                        var y = mapObject.GetPacmanStartY();
                        if (x >= 0 && y >= 0) {
                            mapObject.Set(y * mapObject.GetWidth() + x, 1);
                            ((Image)mapGrid.Children[y * mapObject.GetWidth() + x]).Source = imageMovable.Source;
                        }
                        x = imageTag % mapObject.GetWidth();
                        y = imageTag / mapObject.GetWidth();
                        mapObject.SetPacmanStart(x, y);
                    }
                }

                if (!mapChanged) {
                    mapChanged = true;
                    UpdateTitle();
                }
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) {
            var radio = (RadioButton)sender;
            if (mapObject != null) {
                var tag = radio.Tag.ToString();
                if (tag == "0") {
                    imageSource = imageBlock;
                }
                else if (tag == "1") {
                    imageSource = imageMovable;
                }
                else if (tag == "2") {
                    imageSource = imageCoke;
                }
                else if (tag == "3") {
                    imageSource = imageBread;
                }
                else if (tag == "4") {
                    imageSource = imageDivo;
                }
                else if (tag == "5") {
                    imageSource = imagePacman;
                }
                else {
                    imageSource = imageBlock;
                }
            }
        }

        private Image MapDataToImage(int data) {
            switch (data) {
                default:
                case 0:
                    return imageBlock;
                case 1:
                    return imageMovable;
                case 2:
                    return imageCoke;
                case 3:
                    return imageBread;
                case 4:
                    return imageDivo;
                case 5:
                    return imagePacman;
            }
        }

        private bool CheckFileSave() {
            if (mapObject != null && mapChanged) {
                if (MessageBox.Show(this, "Map data does not save.  Are you sure you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
                    return false;
            }
            return true;
        }

        private void EnableMenu(bool enabled) {
            menuFileSave.IsEnabled = enabled;
            menuFileSaveAs.IsEnabled = enabled;
            menuFileExportToJavaScript.IsEnabled = enabled;
            menuFileClose.IsEnabled = enabled;
            menuEditFillCoke.IsEnabled = enabled;
            menuEditFillBread.IsEnabled = enabled;
            menuEditRemoveAllItems.IsEnabled = enabled;
        }

        private void UpdateTitle() {
            Title = (mapChanged ? "* " : "") + (mapFileName != null ? mapFileName : "new map") + " - " + title;
        }

    }
}
