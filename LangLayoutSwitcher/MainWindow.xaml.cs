using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using LangLayoutSwitcher.Classes;
using Forms = System.Windows.Forms;

namespace LangLayoutSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint KEY_Q = 0x51; //CTRL
        private HwndSource source;

        public MainWindow()
        {
            InitializeComponent();
        }

        private IntPtr _windowHandle;
        private HwndSource _source;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL, KEY_Q); //CTRL + Q
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == KEY_Q)
                            {
                                ChangeText();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private string LayoutSwitcher(string inp, string currLanguage)
        {
            string result = "";

            foreach (var item in inp)
            {
                if (currLanguage == "en-US")
                    result += CharConvert(item, DictionaryClass.DictEngToRus);
                else if (currLanguage == "ru-RU")
                    result += CharConvert(item, DictionaryClass.DictRusToEng);
                else
                {
                    MessageBox.Show("Неизвестная культура", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }

            }

            return result;
        }

        private string CharConvert(char item, Dictionary<string, string> dict)
        {
            bool IsUpper = false;
            if (Char.IsUpper(item))
                IsUpper = true;

            string key = item.ToString().ToLower();

            if (dict.ContainsKey(key))
                key = dict[key];

            if (IsUpper)
                return key.ToUpper();

            return key;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeText();
        }

        private void ChangeText()
        {
            if (Clipboard.ContainsText())
            {
                Clipboard.SetText(LayoutSwitcher(Clipboard.GetText(), Forms.InputLanguage.CurrentInputLanguage.Culture.Name));
                return;
            }

            if (!string.IsNullOrWhiteSpace(Tbx.Text))
            {
                Tbx.Text = LayoutSwitcher(Tbx.Text, Forms.InputLanguage.CurrentInputLanguage.Culture.Name);
                return;
            }
        }

        private void KeyBinding_Changed(object sender, EventArgs e)
        {

        }
    }
}
