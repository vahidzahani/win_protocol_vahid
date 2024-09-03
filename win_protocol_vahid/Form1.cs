using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace win_protocol_vahid
{
    public partial class Form1 : Form
    {
        private string[] commandLineArgs;

        public Form1(string[] args)
        {
            InitializeComponent();
            commandLineArgs = args;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // نمایش آرگومان‌های ورودی
            string argsMessage = "آرگومان‌های ورودی:\n";
            foreach (string arg in commandLineArgs)
            {
                argsMessage += arg + "\n";
            }
            //MessageBox.Show(argsMessage, "آرگومان‌ها", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // بررسی و باز کردن مسیر
            if (commandLineArgs.Length > 0)
            {
                string arg = commandLineArgs[0];
                string path = ExtractPathFromArgument(arg);
                //MessageBox.Show(path);

                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        // بررسی اینکه آیا مسیر یک فایل یا پوشه معتبر است
                        if (File.Exists(path) || Directory.Exists(path))
                        {
                            Process.Start("explorer.exe", path);
                        }
                        else
                        {
                            MessageBox.Show("مسیر مشخص شده وجود ندارد یا معتبر نیست.", "مسیر نامعتبر", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطا در باز کردن مسیر: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Application.Exit();
            }
        }

        private string ExtractPathFromArgument(string arg)
        {
            // بررسی و استخراج مسیر از آرگومان
            const string protocol = "ftvahidpath:";
            if (arg.StartsWith(protocol, StringComparison.OrdinalIgnoreCase))
            {
                string encodedPath = arg.Substring(protocol.Length);
                // رمزگشایی URL
                string decodedPath = Uri.UnescapeDataString(encodedPath);
                return decodedPath;
            }
            return string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // مسیر فایل اجرایی برنامه
                string exePath = Application.ExecutablePath;

                // کلید پروتکل در رجیستری
                string protocolKey = @"HKEY_CLASSES_ROOT\ftvahidpath";

                // ایجاد کلید پروتکل
                Microsoft.Win32.Registry.SetValue(protocolKey, "", "URL:ftvahidpath Protocol");
                Microsoft.Win32.Registry.SetValue(protocolKey, "URL Protocol", "");

                // ایجاد کلید SubKey برای دستورات
                string commandKey = @"HKEY_CLASSES_ROOT\ftvahidpath\shell\open\command";
                Microsoft.Win32.Registry.SetValue(commandKey, "", "\"" + exePath + "\" \"%1\"");

                // نمایش پیام موفقیت
                MessageBox.Show("پروتکل 'ftvahidpath' با موفقیت ثبت شد.", "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // نمایش پیام خطا
                MessageBox.Show("خطا در ثبت پروتکل: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
