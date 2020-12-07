using System;
using System.Diagnostics;

namespace sms
{
    public class Bash
    {
        private string script;
        private string args;
        public Bash(string script, string args)
        {
            this.script = script;
            this.args = args;

        }

        public string Execute()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = script,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(error)) { return output; }
            else { return error; }
        }
    }
}