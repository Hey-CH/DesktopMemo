using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace DesktopMemo {
    /// <summary>
    /// Memo.xaml の相互作用ロジック
    /// </summary>
    public partial class Memo : Window {
        internal MemoViewModel mvm = new MemoViewModel();
        public Memo() {
            InitializeComponent();
            this.DataContext = mvm;
        }

        /// <summary>
        /// TextBoxのフォント変更
        /// </summary>
        internal void ChangeFont(System.Drawing.Font font, System.Windows.Controls.TextBox tage) {
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
        /// TextBoxのカラー変更
        /// </summary>
        internal void ChangeColor(System.Drawing.Color color, System.Windows.Controls.TextBox tage) {
            tage.Foreground
                = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }
        /// <summary>
        /// Windowのカラー変更
        /// </summary>
        internal void ChangeWindowColor(System.Drawing.Color color, Window tage1, System.Windows.Controls.TextBox tage2) {
            tage1.Background
                = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            tage2.Background
                = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            try {
                DragMove();
            } catch { }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            //コレクションの処理
            var vm = (ViewModel)this.Owner.DataContext;
            vm.Memos.Remove((MemoViewModel)this.DataContext);
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
            FontDialog fd = new FontDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                mvm.MemoFont = fd.Font;
                ChangeFont(mvm.MemoFont, textBox1);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                mvm.MemoColor = cd.Color;
                ChangeColor(mvm.MemoColor, textBox1);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                mvm.MemoWindowColor = cd.Color;
                ChangeWindowColor(mvm.MemoWindowColor, this, textBox1);
            }
        }
    }
    public class MemoViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        string _Text = "";
        public string Text {
            get {
                return _Text;
            }
            set {
                _Text = value;
                OnPropertyChanged("Text");
            }
        }
        double _MemoLeft = 0;
        public double MemoLeft {
            get {
                return _MemoLeft;
            }
            set {
                _MemoLeft = value;
                OnPropertyChanged("MemoLeft");
            }
        }
        double _MemoTop = 0;
        public double MemoTop {
            get {
                return _MemoTop;
            }
            set {
                _MemoTop = value;
                OnPropertyChanged("MemoTop");
            }
        }
        double _MemoHeight = 0;
        public double MemoHeight {
            get {
                return _MemoHeight;
            }
            set {
                _MemoHeight = value;
                OnPropertyChanged("MemoHeight");
            }
        }
        double _MemoWidth = 0;
        public double MemoWidth {
            get {
                return _MemoWidth;
            }
            set {
                _MemoWidth = value;
                OnPropertyChanged("MemoWidth");
            }
        }
        System.Drawing.Font _MemoFont = System.Drawing.SystemFonts.DefaultFont;
        [XmlIgnore]
        public System.Drawing.Font MemoFont {
            get {
                return _MemoFont;
            }
            set {
                _MemoFont = value;
                OnPropertyChanged("MemoFont");
            }
        }
        public string MemoFontAsString {
            get { return ConvertToString(MemoFont); }
            set { MemoFont = ConvertFromString<System.Drawing.Font>(value); }
        }

        System.Drawing.Color _MemoColor = System.Drawing.Color.Black;
        [XmlIgnore]
        public System.Drawing.Color MemoColor {
            get {
                return _MemoColor;
            }
            set {
                _MemoColor = value;
                OnPropertyChanged("MemoColor");
            }
        }
        public string MemoColorAsString {
            get { return ConvertToString(MemoColor); }
            set { MemoColor = ConvertFromString<System.Drawing.Color>(value); }
        }

        System.Drawing.Color _MemoWindowColor = System.Drawing.Color.White;
        [XmlIgnore]
        public System.Drawing.Color MemoWindowColor {
            get {
                return _MemoWindowColor;
            }
            set {
                _MemoWindowColor = value;
                OnPropertyChanged("MemoWindowColor");
            }
        }
        public string MemoWindowColorAsString {
            get { return ConvertToString(MemoWindowColor); }
            set { MemoWindowColor = ConvertFromString<System.Drawing.Color>(value); }
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
