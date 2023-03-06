using Microsoft.Win32;
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
using Ookii.Dialogs.Wpf;
using System.Diagnostics;

namespace GamepassUnlocker
{
    public partial class MainWindow : Window
    {
        private string pastaSelecionada;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Ao clicar no botão de selecionar, o usuário poderá escolher a pasta do jogo para modificar 

            VistaFolderBrowserDialog SelecionarPasta = new VistaFolderBrowserDialog();
            SelecionarPasta.Description = "Selecione a pasta do jogo que está no gamepass";
            SelecionarPasta.UseDescriptionForTitle = true;     
            if ((bool)SelecionarPasta.ShowDialog(this))
            {
                pastaSelecionada = SelecionarPasta.SelectedPath;
                TextBox.Text = SelecionarPasta.SelectedPath;
                StartButton.Visibility = Visibility.Visible;
            }
        }

        // Esse botão vai iniciar o processo de trocar as permissões das pastas e arquivos
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Caso o usuário não tenha selecionado uma pasta / cancelou a ação
            if (pastaSelecionada == null)
            {
                Progress.Text = "Você não selecionou uma pasta, por favor selecione uma.";
                Progress.Visibility = Visibility.Visible;
            }
            // Caso selecionado a pasta, começara a modificação
            else
            {
                Progress.Text = "Por favor aguarde, isso pode demorar um pouco...";
                Progress.Visibility = Visibility.Visible;

                await Task.Run(() =>
                {
                    Process process = new Process();
                    process.StartInfo.Verb = "runas";
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = $"/c takeown /a /r /d Y /f \"{pastaSelecionada}\" && icacls \"{pastaSelecionada}\" /reset /t /c && icacls \"{pastaSelecionada}\" /grant:r Users:(OI)(CI)F /t /c /q";
                    //process.StartInfo.Arguments = $"/c icacls \"{pastaSelecionada}\" /grant Administrators:F /t";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.OutputDataReceived += new DataReceivedEventHandler((s, args) =>
                    {

                        if (args.Data != null)
                        {
                            Debug.WriteLine(args.Data);
                            Dispatcher.Invoke(() =>
                            {
                                OutputText.Text += $"{args.Data}\n";
                            });
                        }
                    });
                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                });
                Progress.Text = "Pasta modificada com sucesso";
            }
        }
    }
}
