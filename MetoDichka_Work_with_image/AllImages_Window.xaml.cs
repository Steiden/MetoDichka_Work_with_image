using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Deployment.Internal;
using System.Web;

namespace MetoDichka_Work_with_image
{
    /// <summary>
    /// Логика взаимодействия для AllImages_Window.xaml
    /// </summary>
    public partial class AllImages_Window : Window
    {
        private List<Images> imageList;

        public AllImages_Window(List<Images> imageList)
        {
            InitializeComponent();
            
            this.imageList = imageList;

            // Глубокое копирование списка изображений
            List<Images> imageListCopied = copyImageList(imageList);

            // Преобразование путей всех изображени из списка на абсолютные
            foreach (Images image in imageListCopied)
                image.path = System.IO.Path.Combine(MainWindow.rootPath, "img", image.path);

            // Заполнение списка изображений
            imageList_ListView.ItemsSource = imageListCopied;
        }
        private void imageList_ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Получение выбранного изображения
                Images imageSelected = imageList[imageList_ListView.SelectedIndex];

                // Абсолютный путь к изображению
                string imagePath = System.IO.Path.Combine(MainWindow.rootPath, "img", imageSelected.path);

                // Преобразование пути выбранного изображения в BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath);
                bitmapImage.EndInit();
                
                // Установка выбранного изображения в главном окне
                MainWindow.image_Image.Source = bitmapImage;

                // Установка имени изображения в главном окне
                MainWindow.imageName = imageSelected.path;

                // Установка пути к изображению в главном окне
                MainWindow.imageSource = imagePath;

                // Закрытие этого окна
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка получения изображения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<Images> copyImageList(List<Images> imageList)
        {
            List<Images> newImageList = new List<Images>();
            foreach (Images image in imageList)
            {
                newImageList.Add(image.Clone() as Images);
            }
            return newImageList;
        }
    }
}
//...