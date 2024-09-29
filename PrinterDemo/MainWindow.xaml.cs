using System;
using System.IO.Ports;
using System.Windows;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Printers;
using ESCPOS_NET.Utilities;

namespace POSPrinterExample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadComPorts();
        }

        private void LoadComPorts()
        {
            cmbComPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbComPort.Items.Add(port);
            }
            if (cmbComPort.Items.Count > 0)
            {
                cmbComPort.SelectedIndex = 0; // Select the first port by default
            }
            else
            {
                MessageBox.Show("No COM ports found.");
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (cmbComPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM port.");
                return;
            }

            string printerPort = cmbComPort.SelectedItem.ToString();
            if (!int.TryParse(txtBaudRate.Text, out int baudRate))
            {
                MessageBox.Show("Please enter a valid baud rate.");
                return;
            }

            try
            {
                // Initialize the printer
                var printer = new SerialPrinter(portName: printerPort, baudRate: baudRate);

                // Create an instance of the EPSON emitter
                var en = new EPSON();

                // Combine commands to send to the printer
                var commands = ByteSplicer.Combine(
                    en.Initialize(),
                    en.PrintLine("Hello, World!"),
                    en.PrintLine("This is a test print."),
                    en.PrintLine("Thank you for using our service."),
                    en.FullCut()
                );

                // Send the commands to the printer
                printer.Write(commands);

                // Close the printer connection
                printer.Dispose();

                MessageBox.Show("Print job completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
