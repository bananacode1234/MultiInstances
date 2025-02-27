using System;
using System.Threading;
using System.Windows.Forms;

namespace Instances
{
    public class Program : Form
    {
        private Label mainLabel;
        private Label statusLabel;
        private Button toggleButton;
        private Mutex mutex;
        private bool isMutexAcquired;

        public Program()
        {
            Text = "MultiInstances";
            Width = 250;
            Height = 225;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = true;
            MinimizeBox = false;
            MaximizeBox = false;

            mainLabel = new Label()
            {
                Text = "MultiInstances\nby Matt\n\nPress space to toggle",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(25, 25)
            };
            Controls.Add(mainLabel);

            statusLabel = new Label()
            {
                Text = "Status: running",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Green,
                AutoSize = true,
                Location = new System.Drawing.Point(25, 100)
            };
            Controls.Add(statusLabel);

            toggleButton = new Button()
            {
                Text = "Toggle",
                Location = new System.Drawing.Point(25, 130)
            };
            toggleButton.Click += new EventHandler(ToggleButton_Click);
            Controls.Add(toggleButton);
            toggleButton.Focus();

            mutex = new Mutex(true, "ROBLOX_singletonMutex", out isMutexAcquired);
            
            if (!isMutexAcquired)
            {
                MessageBox.Show("Roblox is already running, relaunch Roblox with the program started.");
            }
            
            UpdateStatus();
        }

        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            ToggleMutex();
        }

        private void ToggleMutex()
        {
            if (isMutexAcquired)
            {
                mutex.ReleaseMutex();
                isMutexAcquired = false;
            }
            else
            {
                try
                {
                    isMutexAcquired = mutex.WaitOne(0, true);
                }
                catch (AbandonedMutexException)
                {
                    isMutexAcquired = true;
                }

                if (!isMutexAcquired)
                {
                    MessageBox.Show("Roblox is already running, relaunch Roblox with the program started.");
                }
            }
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (isMutexAcquired)
            {
                statusLabel.Text = "Status: running";
                statusLabel.ForeColor = System.Drawing.Color.Green;
                toggleButton.Text = "Stop";
            }
            else
            {
                statusLabel.Text = "Status: stopped";
                statusLabel.ForeColor = System.Drawing.Color.Red;
                toggleButton.Text = "Start";
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }
    }
}