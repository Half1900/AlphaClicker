﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace AlphaClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        private bool notifyiconHasInitialzed = false;//此变量用于控制避免重复执行后面的初始化函数
        Process[] myprocess = Process.GetProcessesByName("AlphaClicker.exe");
        public MainWindow()
        {
            bool isAppRunning = false;
            Mutex mutex = new Mutex(true,Process.GetCurrentProcess().ProcessName,out isAppRunning);
            if (!isAppRunning)
            {
                Environment.Exit(1);
                return;
            }
            InitializeComponent();
            this.ShowInTaskbar = false;
        }
        
        public void LoadKeybind()
        {
            startBtn.Content = $"开始 ({Keybinds.keyBinding})";
            stopBtn.Content = $"停止 ({Keybinds.keyBinding})";
        }

        private void Cerror(string errormessage)
        {
            ToggleClick();
            System.Windows.MessageBox.Show(errormessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        private int ToInt(string number)
        {
            // Return int | Replace Empty With 0
            return Int32.Parse((number == "") ? "0" : number);
        }

        public void InitialTray()
        {
            //隐藏主窗体
            Visibility = Visibility.Hidden;
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "鼠标点击器By Forty";
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

            var exit = new MenuItem("退出(Eixt)");
            exit.Click += Exit_Click;

            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { exit });
            notifyIcon.Visible = true;

            notifyIcon.MouseClick += notifyIcon_MouseDoubleClick;
            //StateChanged += MainWindow_StateChanged; //窗体状态改变时候触发

            notifyiconHasInitialzed = true;
        }
        void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            this.Close();//关闭系统
        }
        void notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    Visibility = Visibility.Hidden;
                    this.Hide();
                    this.WindowState = WindowState.Minimized;
                }
                else if (this.WindowState == WindowState.Minimized)
                {
                    Visibility = Visibility.Visible;
                    this.Show();
                    this.WindowState = WindowState.Normal;
                }
            }
        }
        private void FadeButtonColor(System.Windows.Controls.Button btn, string hex)
        {
            ColorAnimation animation =
               new ColorAnimation(
                   (Color)ColorConverter.ConvertFromString(hex),
                   new Duration(TimeSpan.FromSeconds(0.2))
               );
            btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void ToggleClick()
        {
            string startEnabled = "#1494e3";
            string startDisabled = "#084466";

            string stopEnabled = "#FF605C";
            string stopDisabled = "#c43c35";


            if (startBtn.IsEnabled)
            {
                FadeButtonColor(startBtn, startDisabled);
                FadeButtonColor(stopBtn, stopEnabled);
                startBtn.IsEnabled = false;
                stopBtn.IsEnabled = true;

                Thread clickhandler = new Thread(ClickHandler);
                clickhandler.Start();
            }
            else
            {
                FadeButtonColor(startBtn, startEnabled);
                FadeButtonColor(stopBtn, stopDisabled);

                startBtn.IsEnabled = true;
                stopBtn.IsEnabled = false;
            }
        }

        public bool keyEnabled = true;
        void KeyHandler()
        {
            while (true)
            {
                if (keyEnabled)
                {
                    if (Keybinds.key1 == -1)
                    {
                        if (WinApi.GetAsyncKeyState(Keybinds.key2) > 0)
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                ToggleClick();
                            }));
                        }
                    }
                    else
                    {
                        if ((WinApi.GetAsyncKeyState(Keybinds.key1) & 0x8000) == 0x8000 &&
                            WinApi.GetAsyncKeyState(Keybinds.key2) > 0)
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                ToggleClick();
                            }));
                        }
                    }
                }
                Thread.Sleep(200);
            }

        }

        void ClickHandler()
        {
            int sleep = 0;

            bool useRandomSleep = false;
            int randnum1 = 0;
            int randnum2 = 0;

            string mouseBtn = "";
            string clickType = "";

            bool repeatTimesChecked = false;
            int repeatTimes = 0;

            bool customCoordsChecked = false;
            bool bgModel = false;
            int customCoordsX = 0, customCoordsY = 0; 

            Dispatcher.Invoke((Action)(() =>
            {
                /* Grab Click Interval */
                try
                {
                    useRandomSleep = (bool)randomIntervalMode.IsChecked;
                    if (useRandomSleep)
                    {
                        randnum1 = (int)(float.Parse(randomSecs1Box.Text,
                                                CultureInfo.InvariantCulture.NumberFormat) * 1000);
                        randnum2 = (int)(float.Parse(randomSecs2Box.Text,
                                               CultureInfo.InvariantCulture.NumberFormat) * 1000);
                        randnum1 = (randnum1 == 0) ? 1 : randnum1;
                        randnum2 = (randnum2 == 0) ? 1 : randnum2;


                    }
                    else
                    {
                        sleep = ToInt(millisecsBox.Text)
                        + ToInt(secondsBox.Text) * 1000
                        + ToInt(minsBox.Text) * 60000
                        + ToInt(hoursBox.Text) * 3600000;
                        sleep = (sleep == 0) ? 1 : sleep;
                    }
                }

                catch (FormatException ex)
                {
                    Cerror(ex.ToString());
                    return;
                }

                /* Grab Mousebutton And Clicktype */
                mouseBtn = mouseBtnCBOX.Text;
                clickType = clickTypeCBOX.Text;

                /* Grab Repeat Stuff */
                repeatTimesChecked = (bool)repeatTimesRBtn.IsChecked;
                if (repeatTimesChecked)
                {
                    try
                    {
                        repeatTimes = Int32.Parse(repeatTimesBox.Text);
                    }
                    catch (FormatException)
                    {
                        Cerror("Invalid Repeat Times Number");
                        return;
                    }
                }


                /* Grab Coords Stuff */
                customCoordsChecked = (bool)coordsCBtn.IsChecked;
                bgModel = (bool)BGModel.IsChecked; 
                if (customCoordsChecked)
                {
                    try
                    {
                        customCoordsX = Int32.Parse(xBox.Text);
                        customCoordsY = Int32.Parse(yBox.Text);
                    }
                    catch (FormatException)
                    {
                        Cerror("Invalid Repeat Times Number");
                        return;
                    }
                }
            }));

            int repeatCount = 0;
            Random rnd = new Random();

            while (true)
            {
                bool doClick = false;
                Dispatcher.Invoke((Action)(() =>
                {
                    doClick = stopBtn.IsEnabled;
                }));

                if (doClick)
                {
                    if (repeatTimesChecked)
                    {
                        if (repeatCount >= repeatTimes)
                        {
                            Dispatcher.Invoke((Action)(() =>
                            {
                                ToggleClick();
                            }));
                            break;
                        }
                        repeatCount += 1;
                    }

                    if (clickType == "单击")
                    {
                        WinApi.DoClick(mouseBtn, customCoordsChecked, customCoordsX, customCoordsY,bgModel);
                    }
                    else
                    {               
                        WinApi.DoClick(mouseBtn, customCoordsChecked, customCoordsX, customCoordsY, bgModel);
                        Thread.Sleep(300);
                        WinApi.DoClick(mouseBtn, customCoordsChecked, customCoordsX, customCoordsY, bgModel);
                    }

                    Dispatcher.Invoke((Action)(() =>
                    {
                        // Random sleep
                        sleep = (!useRandomSleep) ? sleep : rnd.Next((randnum1 < randnum2) ? randnum1 : randnum2
                                , (randnum1 > randnum2) ? randnum1 : randnum2);
                        
                    }));

                    Thread.Sleep(sleep);
                }
                else
                {
                    break;
                }
            }
        }


        bool bgModel1 = true;
        private void ChangeState(object sender, RoutedEventArgs e)
        {
           
            bgModel1 = false;
            BGModel.IsChecked = false;
        
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AlphaRegistry.GetTheme() == "Dark")
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            this.Topmost = AlphaRegistry.GetTopmost();

            Thread keyhandler = new Thread(KeyHandler);
            keyhandler.Start();

            // Get Keybind Values From SOFTWARE\AlphaClicker
            AlphaRegistry.GetKeybindValues();
            LoadKeybind();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // So all threads close and not become background process
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /* Top Bar */
        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void minimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Visibility = Visibility.Visible;
            this.ShowInTaskbar = false;
            if (!notifyiconHasInitialzed)
            {
                InitialTray();
            }
        }

        private void getCoordsBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            GetCursorPos win = new GetCursorPos();
            win.Owner = this;
            win.Show();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleClick();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleClick();
        }

        private void changeHotkeyBtn_Click(object sender, RoutedEventArgs e)
        {
            keyEnabled = false;
            ChangeHotkey win = new ChangeHotkey();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
        }

        private void windowSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowSettings win = new WindowSettings();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
        }
    }
}