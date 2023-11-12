using System.Windows;
using System.Windows.Input;

namespace AlphaClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChangeHotkey: Window
    {
        public ChangeHotkey()
        {
            InitializeComponent();
        }


        private void hotkeyWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private string CodeToSpecialKey(int key)
        {
            switch (key)
            {
                case 27:
                    return "ESC";
                case 35:
                    return "END";
                case 36:
                    return "HOME";
                case 112:
                    return "F1";
                case 113:
                    return "F2";
                case 114:
                    return "F3";
                case 115:
                    return "F4";
                case 116:
                    return "F5";
                case 117:
                    return "F6";
                case 118:
                    return "F7";
                case 119:
                    return "F8";
                case 120:
                    return "F9";
                case 121:
                    return "F10";
                case 122:
                    return "F11";
                case 123:
                    return "F12";
                case 145:
                    return "SCRLK";
            }
            return "";
        }

        private bool hasSpecKey = false;
        private int key1 = -1;
        private int key2 = -1;
        private void hotkeyWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Not good way of doing it, but it works.
            if (keyBox.Text == "请按键")
            {
                keyBox.Clear();
            }

            if (!startBtn.IsEnabled)
            {
                for (int i = 8; i < 123; i++)
                {
                    if (WinApi.GetAsyncKeyState(i) > 0)
                    {
                        /* If Char Or Number */
                        if (i >= 48 && i <= 90)
                        {
                            keyBox.AppendText(((char)i).ToString());
                            startBtn.IsEnabled = true;
                            key2 = i;
                            okBtn.IsEnabled = true;
                            break;
                        }
                        /* If function key */
                        else if (i >= 112 && i <= 145)
                        {
                            keyBox.AppendText(CodeToSpecialKey(i));
                            startBtn.IsEnabled = true;
                            key2 = i;

                            okBtn.IsEnabled = true;
                            break;
                        }
                        else if (i == 27 || (i >= 35 && i <= 36))
                        {
                            keyBox.AppendText(CodeToSpecialKey(i));
                            startBtn.IsEnabled = true;
                            key2 = i;

                            okBtn.IsEnabled = true;
                            break;
                        }
                        /* If any extra keys (shift, ctrl, alt) */
                        else if (i >= 16 && i <= 18)
                        {
                            if (hasSpecKey == false)
                            {
                                keyBox.AppendText(e.Key.ToString().Replace("左键", "")
                                    .Replace("右键", "") + " + ");
                                key1 = i;
                                hasSpecKey = true;
                            }
                        }
                    }


                }
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            okBtn.IsEnabled = false;
            hasSpecKey = false;
            startBtn.IsEnabled = false;
            keyBox.Text = "请按键";
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Keybinds.key1 = key1;
            Keybinds.key2 = key2;
            Keybinds.keyBinding = keyBox.Text;
            AlphaRegistry.SetKeybindValues();

            ((MainWindow)this.Owner).LoadKeybind();
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void hotkeyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            keyBox.Text = Keybinds.keyBinding;
        }

        private void hotkeyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindow)this.Owner).keyEnabled = true;
        }
    }

    public class Keybinds
    {
        // Default keybinds if reg fail
        public static int key1 = -1;
        public static int key2 = (int)VK.VK_ESCAPE;
        public static string keyBinding = "ESC"; 
    }
}
