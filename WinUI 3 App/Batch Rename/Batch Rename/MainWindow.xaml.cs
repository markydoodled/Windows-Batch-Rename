using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Microsoft.UI;           
using Microsoft.UI.Windowing; 
using WinRT.Interop;
using Windows.UI.WindowManagement;
using Windows.ApplicationModel;

namespace Batch_Rename
{
    public sealed partial class MainWindow : Window
    {
        private Microsoft.UI.Windowing.AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();
            m_AppWindow = GetAppWindowForCurrentWindow();
            ExtendsContentIntoTitleBar = true;
            var titleBar = m_AppWindow.TitleBar;
            SetTitleBar(AppTitleBar);
        }

        private Microsoft.UI.Windowing.AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(wndId);
        }

        private async void folderButton_Click(object sender, RoutedEventArgs e)
        {
            string script = $@"
                    Add-Type -AssemblyName System.Windows.Forms;
                    $dialog = New-Object System.Windows.Forms.FolderBrowserDialog;
                    if ($dialog.ShowDialog() -eq 'OK') {{
                        $folderPath = $dialog.SelectedPath;
                        Get-ChildItem -Path $folderPath -File | ForEach-Object {{
                            $newName = $_.Directory.Name + ' ' + $_.BaseName + $_.Extension;
                            Rename-Item -Path $_.FullName -NewName $newName;
                        }};
                    }}";
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Unrestricted -Command \"{script}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                /*var dialog = new ContentDialog
                {
                    Title = "PowerShell Script Output",
                    Content = new TextBlock { Text = output },
                    CloseButtonText = "OK",
                    XamlRoot = ((FrameworkElement)sender).XamlRoot
                };

                await dialog.ShowAsync();*/
            }
        }

        private async void nameButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Enter A New Name",
                Content = new TextBox { PlaceholderText = "New Folder Name" },
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = ((FrameworkElement)sender).XamlRoot
            };
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string newName = ((TextBox)dialog.Content).Text;
                string script = $@"
                    Add-Type -AssemblyName System.Windows.Forms;
                    $dialog = New-Object System.Windows.Forms.FolderBrowserDialog;
                    if ($dialog.ShowDialog() -eq 'OK') {{
                        $folderPath = $dialog.SelectedPath;
                        Get-ChildItem -Path $folderPath -File | ForEach-Object {{
                            $newName = '{newName}' + ' ' + $_.BaseName + $_.Extension;
                            Rename-Item -Path $_.FullName -NewName $newName;
                        }};
                    }}";
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Unrestricted -Command \"{script}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();

                    /*dialog = new ContentDialog
                    {
                        Title = "PowerShell Script Output",
                        Content = new TextBlock { Text = output },
                        CloseButtonText = "OK",
                        XamlRoot = ((FrameworkElement)sender).XamlRoot
                    };

                    await dialog.ShowAsync();*/
                }
            }
        }
    }
}
