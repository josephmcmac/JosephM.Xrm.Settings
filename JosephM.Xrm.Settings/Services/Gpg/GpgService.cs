using System;
using System.Diagnostics;
using System.IO;
using JosephM.Xrm.Settings.Core;
using JosephM.Xrm.Settings.Services.Encryption;

namespace JosephM.Xrm.Settings.Services.Gpg
{
    public class GpgService
    {
        public string Decrypt(byte[] encryptedFile, string passphrase)
        {
            var workFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GpgTemp");
            FileUtility.CheckCreateFolder(workFolder);

            var tempFileName = "tempencrypted_" + DateTime.Now.ToFileTime() + ".txt.gpg";
            var tempFile = Path.Combine(workFolder, tempFileName);
            File.WriteAllBytes(tempFile, encryptedFile);

            var info = new FileInfo(tempFile);

            string decryptedFileName = Path.Combine(workFolder, tempFileName.Substring(0, tempFileName.Length - 4));
            string encryptedFileName = info.FullName;
            string password = passphrase;

            var psi = new ProcessStartInfo("cmd.exe");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WorkingDirectory = workFolder;

            Process process = Process.Start(psi);

            //string sCommandLine = @"echo " + password + "|gpg.exe --passphrase-fd 0 --batch --verbose --yes --output " + decryptedFileName + @" --decrypt """ + encryptedFileName;
            string sCommandLine = @"echo " + password + @"|gpg.exe --passphrase-fd 0 --batch --verbose --yes --output """ + decryptedFileName + @""" --decrypt """ + encryptedFileName + @"""";
            process.StandardInput.WriteLine(sCommandLine);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            process.Close();
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (File.Exists(decryptedFileName))
            {
                var decryptedContent = File.ReadAllText(decryptedFileName);
                File.Delete(decryptedFileName);
                return decryptedContent;
            }
            throw new Exception(string.Format("There Was An Unexpected Error Decrypting Bpay File " + sCommandLine.Replace(password, "[PASSPHRASE]")));
        }
    }
}
