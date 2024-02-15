using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MetoDichka_Work_with_image
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml йцукен
    /// </summary>
    public partial class MainWindow : Window
    {
        private MegaImageListEntities db = new MegaImageListEntities(); // База данных

        public static string rootPath = AppDomain.CurrentDomain.BaseDirectory;  // Путь к папке Debug
        public static string imageName = "";    // Имя файла текущего изображения
        public static string imageSource = "";  // Путь к файлу текущего изображения

        public static Image image_Image;

        public MainWindow()
        {
            InitializeComponent();

            image_Image = image;
        }

        private void showAll_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение всех изображений из БД
                List<Images> imageList = db.Images.ToList();

                // Открытие окна со списком всех изображений
                AllImages_Window allImages_Window = new AllImages_Window(imageList);
                allImages_Window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка получения всех изображения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loadImage_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Настройка диалогового окна
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "png (*.png)|*.png|All files (*.*)|*.*";
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                
                // Открытие диалогового окна
                if(openFileDialog.ShowDialog() == true)
                {
                    // Установка изображения в элемент Image
                    image.Source = new BitmapImage(new Uri(openFileDialog.FileName));

                    // Сохранение данных изображения
                    imageName = openFileDialog.SafeFileName;
                    imageSource = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка загрузки изображения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveImage_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка, существует ли такое изображение в БД
                if (db.Images.Where(i => i.path == imageName).FirstOrDefault() != null)
                {
                    MessageBox.Show("Такое изображение уже есть", "Давай другое!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
    
                // Получение изображения из элемента Image
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imageSource);
                bitmapImage.EndInit();

                // Настройка энкодера для png файлов
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                // Путь для сохранения изображения
                string imagePath = System.IO.Path.Combine(rootPath, "img", imageName);

                // Сохранение изображения
                using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                // Сохранение изображения в БД
                saveImage();

                // Оповещение пользователя об успешном сохранении
                MessageBox.Show("Изображение успешно сохранено!", "Мои поздравления!!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка сохранения изображения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void saveImage()
        {
            try
            {
                // Добавление нового изображения в таблицу Images
                db.Images.Add(new Images()
                {
                    path = imageName,
                });

                // Сохранение изменений
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка сохранения в БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
