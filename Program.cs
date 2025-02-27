using System;
using System.Threading;
using System.Windows.Forms;

namespace Instances
{
    public class Program : Form
    {
        private Label statusLabel;
        private Mutex mutex;
        private bool isMutexAcquired;

        public Program()
        {
            Text = "MultiInstances";
            Width = 250;
            Height = 200;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = true;
            MinimizeBox = false;
            MaximizeBox = false;

            Label mainLabel = new Label()
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

            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.Focus();

            mutex = new Mutex(true, "ROBLOX_singletonMutex", out isMutexAcquired);
            
            if (!isMutexAcquired)
            {
                MessageBox.Show("Roblox is already running, relaunch Roblox with the program started.");
            }
            
            UpdateStatus();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                ToggleMutex();
            }
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
            }
            else
            {
                statusLabel.Text = "Status: stopped";
                statusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new Program());
        }
    }
}