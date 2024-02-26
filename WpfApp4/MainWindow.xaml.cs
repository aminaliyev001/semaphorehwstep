using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Thread> _created = new ObservableCollection<Thread>();
        public static int counter = 1;
        public ObservableCollection<Thread> Created
        {
            get { return _created; }
            set
            {
                _created = value;
                OnPropertyChanged(nameof(Created));
            }
        }
        private ObservableCollection<Thread> _waiting = new ObservableCollection<Thread>();
        public ObservableCollection<Thread> Waiting
        {
            get { return _waiting; }
            set
            {
                _waiting = value;
                OnPropertyChanged(nameof(Waiting));
            }
        }
        private ObservableCollection<Thread> _working = new ObservableCollection<Thread>();
        public ObservableCollection<Thread> Working
        {
            get { return _working; }
            set
            {
                _working = value;
                OnPropertyChanged(nameof(Working));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Semaphore Semaphore = null; 

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Semaphore = new Semaphore(3, 3, "semafor");
        }
        void Task(object threadParam)
        {
            Thread currentThread = threadParam as Thread;
            Semaphore.WaitOne();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Waiting.Remove(currentThread);
                Working.Add(currentThread);
            });
            int a = 5;
            int b = 15;
            int c = 25;
            int result = a * b + c;
            Thread.Sleep(5000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Working.Remove(currentThread);
            });
            Semaphore.Release();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(Task);
            thread.Name = "Thread" + counter++;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Created.Add(thread);
            });
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var thread = listView.SelectedItem as Thread;
            if (thread != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Created.Remove(thread);
                    Waiting.Add(thread);
                });
                thread.Start(thread); 
            }
        }
    }
}