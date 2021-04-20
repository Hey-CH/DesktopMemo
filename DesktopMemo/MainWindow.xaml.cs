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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Xml;

namespace DesktopMemo {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        ViewModel vm = new ViewModel();
        public MainWindow() {
            InitializeComponent();
            this.DataContext = vm;

            //デシリアライズするとなぜかMainWIndowがnullになってしまう。
            //原因でデシリアライズの際空のコンストラクタが呼ばれ、MainWindow=nullになってしまうから。
            if (File.Exists(@"data.xml")) {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"data.xml");
                var MainLeft = doc.SelectSingleNode("//MainLeft");
                var MainTop = doc.SelectSingleNode("//MainTop");
                var MainFontAsString = doc.SelectSingleNode("//MainFontAsString");
                var MainColorAsString = doc.SelectSingleNode("//MainColorAsString");
                vm.MainLeft = double.Parse(MainLeft.InnerText);
                vm.MainTop = double.Parse(MainTop.InnerText);
                vm.MainFontAsString = MainFontAsString.InnerText;
                vm.MainColorAsString = MainColorAsString.InnerText;

                ChangeFont(vm.MainFont, textBlock1);
                ChangeColor(vm.MainColor, textBlock1);
            }

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e) {
            while (true) {
                vm.OnPropertyChanged("Time");
                System.Threading.Thread.Sleep(100);
            }
        }
        /// <summary>
        /// label1のフォント変更
        /// </summary>
        private void ChangeFont(System.Drawing.Font font, TextBlock tage) {
            tage.FontFamily = new FontFamily(font.Name);
            tage.FontSize = font.Size * 96.0 / 72.0;
            tage.FontWeight = font.Bold ? FontWeights.Bold : FontWeights.Regular;
            tage.FontStyle = font.Italic ? FontStyles.Italic : FontStyles.Normal;
            TextDecorationCollection tdc = new TextDecorationCollection();
            if (font.Underline) tdc.Add(TextDecorations.Underline);
            if (font.Strikeout) tdc.Add(TextDecorations.Strikethrough);
            tage.TextDecorations = tdc;
        }
        /// <summary>
        /// label1のカラー変更
        /// </summary>
        private void ChangeColor(System.Drawing.Color color, TextBlock tage) {
            tage.Foreground
                = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            try {
                DragMove();
            } catch { }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
            FontDialog fd = new FontDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                vm.MainFont = fd.Font;
                ChangeFont(vm.MainFont, textBlock1);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                vm.MainColor = cd.Color;
                ChangeColor(vm.MainColor, textBlock1);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) {
            Memo m = new Memo();
            m.Owner = this;
            m.Show();
            //コレクションの処理
            vm.Memos.Add((MemoViewModel)m.DataContext);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) {
            if (!this.Topmost)
                this.Topmost = true;
            else
                this.Topmost = false;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            if (File.Exists(@"data.xml")) {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"data.xml");
                //Memoの処理
                var Memos = doc.SelectNodes(".//MemoViewModel");
                foreach (XmlElement memo in Memos) {
                    var Text = memo.SelectSingleNode(".//Text");
                    var MemoLeft = memo.SelectSingleNode(".//MemoLeft");
                    var MemoTop = memo.SelectSingleNode(".//MemoTop");
                    var MemoHeight = memo.SelectSingleNode(".//MemoHeight");
                    var MemoWidth = memo.SelectSingleNode(".//MemoWidth");
                    var MemoFontAsString = memo.SelectSingleNode(".//MemoFontAsString");
                    var MemoColorAsString = memo.SelectSingleNode(".//MemoColorAsString");
                    var MemoWindowColorAsString = memo.SelectSingleNode(".//MemoWindowColorAsString");
                    var mvm = new MemoViewModel();
                    mvm.Text = Text.InnerText;
                    mvm.MemoLeft = double.Parse(MemoLeft.InnerText);
                    mvm.MemoTop = double.Parse(MemoTop.InnerText);
                    mvm.MemoHeight = double.Parse(MemoHeight.InnerText);
                    mvm.MemoWidth = double.Parse(MemoWidth.InnerText);
                    mvm.MemoFontAsString = MemoFontAsString.InnerText;
                    mvm.MemoColorAsString = MemoColorAsString.InnerText;
                    mvm.MemoWindowColorAsString = MemoWindowColorAsString.InnerText;
                    vm.Memos.Add(mvm);
                    var m = new Memo();
                    m.DataContext = mvm;
                    m.ChangeFont(mvm.MemoFont, m.textBox1);
                    m.ChangeColor(mvm.MemoColor, m.textBox1);
                    m.ChangeWindowColor(mvm.MemoWindowColor, m, m.textBox1);
                    m.Owner = this;
                    m.Show();
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            //終了する時にシリアライズして保存
            XmlSerializer xs = new XmlSerializer(typeof(ViewModel));
            using (var sw = new StreamWriter(@"data.xml", false, Encoding.UTF8)) {
                xs.Serialize(sw, vm);
            }
        }
    }

    public class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Time {
            get {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }
        double _MainLeft = 0;
        public double MainLeft {
            get {
                return _MainLeft;
            }
            set {
                _MainLeft = value;
                OnPropertyChanged("MainLeft");
            }
        }
        double _MainTop = 0;
        public double MainTop {
            get {
                return _MainTop;
            }
            set {
                _MainTop = value;
                OnPropertyChanged("MainTop");
            }
        }
        ObservableCollection<MemoViewModel> _Memos = new ObservableCollection<MemoViewModel>();
        public ObservableCollection<MemoViewModel> Memos {
            get {
                return _Memos;
            }
            set {
                _Memos = value;
                OnPropertyChanged("Memos");
            }
        }
        System.Drawing.Font _MainFont = System.Drawing.SystemFonts.DefaultFont;
        [XmlIgnore]
        public System.Drawing.Font MainFont {
            get {
                return _MainFont;
            }
            set {
                _MainFont = value;
                OnPropertyChanged("MainFont");
            }
        }
        public string MainFontAsString {
            get { return ConvertToString(MainFont); }
            set { MainFont = ConvertFromString<System.Drawing.Font>(value); }
        }

        System.Drawing.Color _MainColor = System.Drawing.Color.Black;
        [XmlIgnore]
        public System.Drawing.Color MainColor {
            get {
                return _MainColor;
            }
            set {
                _MainColor = value;
                OnPropertyChanged("MainColor");
            }
        }
        public string MainColorAsString {
            get { return ConvertToString(MainColor); }
            set { MainColor = ConvertFromString<System.Drawing.Color>(value); }
        }

        //FontやColorを文字列にしてくれる奴
        public static string ConvertToString<T>(T value) {
            return TypeDescriptor.GetConverter(typeof(T)).ConvertToString(value);
        }
        public static T ConvertFromString<T>(string value) {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }
    }
}
