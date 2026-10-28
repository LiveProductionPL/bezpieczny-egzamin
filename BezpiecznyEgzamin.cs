using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: AssemblyTitle("Bezpieczny Egzamin — Aplikacja stanowiskowa")]
[assembly: AssemblyDescription("Oficjalna aplikacja stanowiskowa dla ucznia platformy egzamin.info.pl (Wydawca: Live Production, Autor: Karol Kowalski)")]
[assembly: AssemblyConfiguration("Release x64/AnyCPU")]
[assembly: AssemblyCompany("Live Production (egzamin.info.pl)")]
[assembly: AssemblyProduct("Bezpieczny Egzamin (egzamin.info.pl)")]
[assembly: AssemblyCopyright("Copyright © 2023–2026 Live Production — Karol Kowalski (egzamin.info.pl). Wszelkie prawa zastrzeżone.")]
[assembly: AssemblyTrademark("Live Production • egzamin.info.pl")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("2.1.0.0")]
[assembly: AssemblyFileVersion("2.1.0.0")]
[assembly: AssemblyInformationalVersion("2.1.0 (Wydawca: Live Production • Autor: Karol Kowalski)")]

namespace BezpiecznyEgzaminApp
{
    internal static class Program
    {
        internal const string AppVersion = "2.1.0";
        internal const string AppBuildDate = "2026-10-28";
        internal const string AppAuthor = "Karol Kowalski";
        internal const string AppPublisher = "Live Production (egzamin.info.pl)";
        internal const string AppContact = "pomoc@egzamin.info.pl";
        private const string DefaultHost = "https://egzamin.info.pl";
        private const string NativeSecret = "EgzaminInfoPl_NativeGuard_2026_SecretKey_#9f8a7b6c5d4e3f2a1b0c";
        private const string CustomUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 BezpiecznyEgzamin/2.1 (EGZ-Native; Live Production)";

        private const string PublisherCertBase64 = "MIIDujCCAqKgAwIBAgIQIoV2vO1v6ZtO642iGuupqDANBgkqhkiG9w0BAQsFADB1MQswCQYDVQQGEwJQTDEPMA0GA1UEBwwGUG9sc2thMSkwJwYDVQQKDCBMaXZlIFByb2R1Y3Rpb24gLSBLYXJvbCBLb3dhbHNraTEqMCgGA1UEAwwhTGl2ZSBQcm9kdWN0aW9uIChlZ3phbWluLmluZm8ucGwpMB4XDTI2MTAyODIwNTAzMloXDTM2MTAyODIxMDAzM1owdTELMAkGA1UEBhMCUEwxDzANBgNVBAcMBlBvbHNrYTEpMCcGA1UECgwgTGl2ZSBQcm9kdWN0aW9uIC0gS2Fyb2wgS293YWxza2kxKjAoBgNVBAMMIUxpdmUgUHJvZHVjdGlvbiAoZWd6YW1pbi5pbmZvLnBsKTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMh+/R79C9tdSGBU0f5LObmcPwH3LKt0PhUaWv3IyJal/b+UnzaIJp424KTG7z1kgdfbtF5xGB7dOMF5Xe3Tb3oKdLmleE2WSnu2mj1/JA9BnBhIZYvcFARqUJWgtHplH2oaeny+VNCyhdjWCskqXwmfT4OBpObQA7TLgqvamOUpvIe8NgFrsDz3najghLawvz37R/tVz7IC0944+Nvsq6ZEV4Bw/7ZFGZpmqvZVPb0VC3seAo/mm6NykDMkZevZS5aiU/jWH4VXDVSWKNGQZONxxCBHHLN3ZuU9S4JLbzzm1eAmOSNbhsmNXAALsJuVPr/D64KdOPXCANaOFx010EUCAwEAAaNGMEQwDgYDVR0PAQH/BAQDAgeAMBMGA1UdJQQMMAoGCCsGAQUFBwMDMB0GA1UdDgQWBBQgtUy0112Ge3zgLcRZhA6VjP6nijANBgkqhkiG9w0BAQsFAAOCAQEAStimpiVLAJk1uR++GBZ0B6RnipZej5Khu7kczJBitmpwk63PqR8pY9uvvCm/yWiQ8AY9teDadURvxETObcVs02DYA25i9xFlkWG/LWQINuPooYK8qfx/toSZZaEruHZCm3Rw4u2qX5xkxbSoynkbEQYqfzYRkgHH17PddL9pMVmp1GvfJGG7wnLFMbXTqgQtYDoadnkMZJy+9CwutUMx8Z6PFBPV45lRcGeP6tT6CDCqjrmaeL1TmQxLMaSQ7xmeTe1+NKgjztqOf/HBwPyazLdncOMPndA+2yJp6Sm1MUFpzWUQhMLwefm7aE8E8QKCvYVf6d64d9v27RUEB4bnnw==";

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TryRegisterAssociations();
            TryInstallPublisherCert(false);

            if (args != null && args.Length > 0)
            {
                string arg0 = args[0].Trim();
                if (string.Equals(arg0, "--register", StringComparison.OrdinalIgnoreCase))
                {
                    TryInstallPublisherCert(true);
                    MessageBox.Show(
                        "Program Bezpieczny Egzamin v" + AppVersion + " został zarejestrowany w systemie.\n" +
                        "Wydawca: " + AppPublisher + "\n" +
                        "Pliki .egz oraz linki egzamin:// są gotowe do użycia.",
                        "Bezpieczny Egzamin — Rejestracja",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                if (arg0.StartsWith("egzamin://", StringComparison.OrdinalIgnoreCase))
                {
                    HandleProtocolUri(arg0);
                    return;
                }

                if (File.Exists(arg0))
                {
                    HandleEgzFile(arg0);
                    return;
                }
            }

            Application.Run(new LauncherForm());
        }

        internal static bool TryInstallPublisherCert(bool includeRoot)
        {
            try
            {
                byte[] raw = Convert.FromBase64String(PublisherCertBase64);
                X509Certificate2 cert = new X509Certificate2(raw);
                using (X509Store pubStore = new X509Store(StoreName.TrustedPublisher, StoreLocation.CurrentUser))
                {
                    pubStore.Open(OpenFlags.ReadWrite);
                    if (!pubStore.Certificates.Contains(cert))
                    {
                        pubStore.Add(cert);
                    }
                }
                if (includeRoot)
                {
                    using (X509Store rootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
                    {
                        rootStore.Open(OpenFlags.ReadWrite);
                        if (!rootStore.Certificates.Contains(cert))
                        {
                            rootStore.Add(cert);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void TryRegisterAssociations()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                using (RegistryKey kExt = Registry.CurrentUser.CreateSubKey(@"Software\Classes\.egz"))
                {
                    if (kExt != null)
                    {
                        kExt.SetValue("", "BezpiecznyEgzamin.File");
                        kExt.SetValue("Content Type", "application/x-egzamin");
                    }
                }
                using (RegistryKey kProg = Registry.CurrentUser.CreateSubKey(@"Software\Classes\BezpiecznyEgzamin.File"))
                {
                    if (kProg != null)
                    {
                        kProg.SetValue("", "Plik startowy sprawdzianu (Bezpieczny Egzamin)");
                        using (RegistryKey kIcon = kProg.CreateSubKey("DefaultIcon"))
                        {
                            if (kIcon != null) kIcon.SetValue("", "\"" + exePath + "\",0");
                        }
                        using (RegistryKey kCmd = kProg.CreateSubKey(@"shell\open\command"))
                        {
                            if (kCmd != null) kCmd.SetValue("", "\"" + exePath + "\" \"%1\"");
                        }
                    }
                }
                using (RegistryKey kProto = Registry.CurrentUser.CreateSubKey(@"Software\Classes\egzamin"))
                {
                    if (kProto != null)
                    {
                        kProto.SetValue("", "URL:Bezpieczny Egzamin Protocol");
                        kProto.SetValue("URL Protocol", "");
                        using (RegistryKey kIcon = kProto.CreateSubKey("DefaultIcon"))
                        {
                            if (kIcon != null) kIcon.SetValue("", "\"" + exePath + "\",0");
                        }
                        using (RegistryKey kCmd = kProto.CreateSubKey(@"shell\open\command"))
                        {
                            if (kCmd != null) kCmd.SetValue("", "\"" + exePath + "\" \"%1\"");
                        }
                    }
                }
            }
            catch
            {
                // Ignoruj błędy rejestru na kontach z ograniczeniami
            }
        }

        internal static void HandleProtocolUri(string rawUri)
        {
            try
            {
                Uri uri = new Uri(rawUri);
                string query = uri.Query.TrimStart('?');
                string code = "";
                string ck = "GLOBAL";
                string host = DefaultHost;

                foreach (string part in query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int eq = part.IndexOf('=');
                    if (eq <= 0) continue;
                    string k = Uri.UnescapeDataString(part.Substring(0, eq)).ToLowerInvariant();
                    string v = Uri.UnescapeDataString(part.Substring(eq + 1));
                    if (k == "code") code = Regex.Replace(v.ToUpperInvariant(), "[^A-Z0-9]", "");
                    else if (k == "ck" || k == "config_key") ck = v;
                    else if (k == "host" && (v.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || v.StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
                        host = v.TrimEnd('/');
                }

                LaunchKioskSession(host, code, ck, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się otworzyć linku egzaminacyjnego: " + ex.Message, "Bezpieczny Egzamin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        internal static void HandleEgzFile(string path)
        {
            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                string code = ExtractJsonString(json, "code");
                string ck = ExtractJsonString(json, "config_key");
                string host = ExtractJsonString(json, "host");
                if (string.IsNullOrEmpty(host)) host = DefaultHost;
                if (string.IsNullOrEmpty(ck)) ck = "GLOBAL";
                code = Regex.Replace((code ?? "").ToUpperInvariant(), "[^A-Z0-9]", "");
                bool checkMon = !json.Contains("\"block_multi_display\":false");
                LaunchKioskSession(host.TrimEnd('/'), code, ck, checkMon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wybrany plik .egz jest nieprawidłowy: " + ex.Message, "Bezpieczny Egzamin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string ExtractJsonString(string json, string key)
        {
            Match m = Regex.Match(json, "\"" + Regex.Escape(key) + "\"\\s*:\\s*\"((?:\\\\.|[^\"\\\\])*)\"");
            if (!m.Success) return "";
            return Regex.Unescape(m.Groups[1].Value);
        }

        internal static void LaunchKioskSession(string host, string code, string configKey, bool enforceSingleMonitor)
        {
            while (enforceSingleMonitor && Screen.AllScreens.Length > 1)
            {
                DialogResult dr = MessageBox.Show(
                    "Wykryto podłączony drugi monitor lub projektor.\n\nPodczas sprawdzianu dozwolona jest praca wyłącznie na jednym ekranie.\nOdłącz dodatkowy monitor lub naciśnij Win + P i wybierz 'Tylko ekran komputera', a następnie kliknij Ponów próbę.",
                    "Bezpieczny Egzamin - Wykryto drugi monitor",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning
                );
                if (dr != DialogResult.Retry) return;
            }

            string browserExe = FindChromiumBrowser();
            string signedUrl = BuildSignedStartUrl(host, code, configKey);

            if (string.IsNullOrEmpty(browserExe))
            {
                Process.Start(signedUrl);
                return;
            }

            try
            {
                string tempProfile = Path.Combine(Path.GetTempPath(), "BezpiecznyEgzamin_Profile_" + DateTime.UtcNow.Ticks);
                Directory.CreateDirectory(tempProfile);
                File.WriteAllText(Path.Combine(tempProfile, "First Run"), "");
                string defDir = Path.Combine(tempProfile, "Default");
                Directory.CreateDirectory(defDir);
                File.WriteAllText(
                    Path.Combine(defDir, "Preferences"),
                    "{\"browser\":{\"has_seen_welcome_page\":true},\"devtools\":{\"preferences\":{\"currentDockState\":\"\\\"undocked\\\"\"}}}"
                );

                string args = string.Format(
                    "--user-data-dir=\"{0}\" --kiosk \"{1}\" --no-first-run --no-default-browser-check --disable-extensions --disable-plugins-discovery --disable-sync --disable-translate --disable-features=Translate,EdgeSidebar,msEdgeSidebarV2,msHubApps,msWelcomePage --user-agent=\"{2}\"",
                    tempProfile,
                    signedUrl,
                    CustomUserAgent
                );

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = browserExe,
                    Arguments = args,
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd uruchamiania stanowiska: " + ex.Message, "Bezpieczny Egzamin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string BuildSignedStartUrl(string host, string code, string ck)
        {
            if (string.IsNullOrEmpty(host)) host = DefaultHost;
            host = host.TrimEnd('/');
            code = Regex.Replace((code ?? "").ToUpperInvariant(), "[^A-Z0-9]", "");
            if (string.IsNullOrEmpty(ck)) ck = "GLOBAL";

            long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            string nonce = Guid.NewGuid().ToString("N").Substring(0, 16);
            string payload = ts + "|" + nonce + "|" + code + "|" + ck;
            string sig = ComputeHmacSha256(payload, NativeSecret);

            StringBuilder sb = new StringBuilder();
            sb.Append(host).Append("/?egz_client=1&kiosk_app=1");
            if (!string.IsNullOrEmpty(code))
            {
                sb.Append("&code=").Append(Uri.EscapeDataString(code));
            }
            sb.Append("&egz_ts=").Append(ts);
            sb.Append("&egz_nonce=").Append(nonce);
            sb.Append("&egz_ck=").Append(Uri.EscapeDataString(ck));
            sb.Append("&egz_sig=").Append(sig);
            return sb.ToString();
        }

        private static string ComputeHmacSha256(string data, string secret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            byte[] msgBytes = Encoding.UTF8.GetBytes(data);
            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(msgBytes);
                StringBuilder sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static string FindChromiumBrowser()
        {
            string[] roots = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            };
            string[] rels = new string[]
            {
                @"Microsoft\Edge\Application\msedge.exe",
                @"Google\Chrome\Application\chrome.exe"
            };
            foreach (string r in roots)
            {
                if (string.IsNullOrEmpty(r)) continue;
                foreach (string rel in rels)
                {
                    string p = Path.Combine(r, rel);
                    if (File.Exists(p)) return p;
                }
            }
            return "";
        }
    }

    internal sealed class LauncherForm : Form
    {
        private readonly TextBox txtPin;

        public LauncherForm()
        {
            Text = "Bezpieczny Egzamin v" + Program.AppVersion + " — egzamin.info.pl";
            Size = new Size(540, 445);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            try
            {
                Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (appIcon != null) Icon = appIcon;
            }
            catch { }

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = Color.FromArgb(15, 23, 42)
            };

            Label lblTitle = new Label
            {
                Text = "🛡️ Bezpieczny Egzamin",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(22, 14)
            };
            Label lblVerBadge = new Label
            {
                Text = "v" + Program.AppVersion,
                Font = new Font("Consolas", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(134, 239, 172),
                BackColor = Color.FromArgb(20, 83, 45),
                AutoSize = true,
                Padding = new Padding(6, 2, 6, 2),
                Location = new Point(258, 20)
            };
            Label lblSub = new Label
            {
                Text = "Wydawca: Live Production (egzamin.info.pl) • Autor: " + Program.AppAuthor,
                Font = new Font("Segoe UI", 8.8f, FontStyle.Regular),
                ForeColor = Color.FromArgb(191, 219, 254),
                AutoSize = true,
                Location = new Point(24, 52)
            };
            header.Controls.Add(lblTitle);
            header.Controls.Add(lblVerBadge);
            header.Controls.Add(lblSub);
            Controls.Add(header);

            int screens = Screen.AllScreens.Length;
            Label lblStatus = new Label
            {
                Text = "✓ Zarejestrowano obsługę plików .egz  |  Liczba ekranów: " + screens + (screens > 1 ? " (Odłącz 2. ekran!)" : " (OK)"),
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = screens > 1 ? Color.FromArgb(185, 28, 28) : Color.FromArgb(22, 101, 52),
                AutoSize = true,
                Location = new Point(24, 104)
            };
            Controls.Add(lblStatus);

            Label lblPin = new Label
            {
                Text = "Kod PIN sprawdzianu (opcjonalnie — możesz też wpisać go na ekranie):",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Location = new Point(24, 134)
            };
            Controls.Add(lblPin);

            txtPin = new TextBox
            {
                Location = new Point(24, 158),
                Size = new Size(476, 34),
                Font = new Font("Consolas", 14f, FontStyle.Bold),
                CharacterCasing = CharacterCasing.Upper
            };
            Controls.Add(txtPin);

            Button btnLaunch = new Button
            {
                Text = "🚀 Uruchom stanowisko egzaminacyjne (Pełny ekran)",
                Location = new Point(24, 206),
                Size = new Size(476, 46),
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLaunch.FlatAppearance.BorderSize = 0;
            btnLaunch.Click += (s, e) =>
            {
                string pin = txtPin.Text.Trim();
                Hide();
                Program.LaunchKioskSession("https://egzamin.info.pl", pin, "GLOBAL", true);
                Close();
            };
            Controls.Add(btnLaunch);

            Button btnOpenEgz = new Button
            {
                Text = "📂 Otwórz plik sprawdzianu (.egz) z dysku",
                Location = new Point(24, 262),
                Size = new Size(310, 40),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOpenEgz.FlatAppearance.BorderSize = 0;
            btnOpenEgz.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Pliki sprawdzianu (*.egz)|*.egz|Wszystkie pliki (*.*)|*.*";
                    ofd.Title = "Wybierz plik startowy sprawdzianu (.egz)";
                    if (ofd.ShowDialog(this) == DialogResult.OK)
                    {
                        Hide();
                        Program.HandleEgzFile(ofd.FileName);
                        Close();
                    }
                }
            };
            Controls.Add(btnOpenEgz);

            Button btnAbout = new Button
            {
                Text = "ℹ️ O programie / Wydawca",
                Location = new Point(344, 262),
                Size = new Size(156, 40),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAbout.FlatAppearance.BorderSize = 0;
            btnAbout.Click += (s, e) => ShowAboutDialog();
            Controls.Add(btnAbout);

            Panel footBox = new Panel
            {
                Location = new Point(24, 316),
                Size = new Size(476, 72),
                BackColor = Color.FromArgb(241, 245, 249),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblMeta1 = new Label
            {
                Text = "Wydawca: Live Production  •  Autor: " + Program.AppAuthor + "  •  Wersja: " + Program.AppVersion,
                Font = new Font("Segoe UI", 8.6f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Location = new Point(10, 8)
            };
            Label lblMeta2 = new Label
            {
                Text = "Wsparcie techniczne: " + Program.AppContact + "   •   Zamknięcie po teście: Alt + F4",
                Font = new Font("Segoe UI", 8.2f, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Location = new Point(10, 28)
            };
            LinkLabel lnkTrust = new LinkLabel
            {
                Text = "🔏 Zainstaluj certyfikat wydawcy w Windows (usuwa komunikat 'Nieznany wydawca')",
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                LinkColor = Color.FromArgb(29, 78, 216),
                AutoSize = true,
                Location = new Point(10, 48)
            };
            lnkTrust.LinkClicked += (s, e) =>
            {
                if (Program.TryInstallPublisherCert(true))
                {
                    MessageBox.Show(
                        "Certyfikat wydawcy (" + Program.AppPublisher + ") został zainstalowany w magazynie Zaufanych Wydawców systemu Windows na tym komputerze.",
                        "Certyfikat wydawcy zainstalowany",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            };
            footBox.Controls.Add(lblMeta1);
            footBox.Controls.Add(lblMeta2);
            footBox.Controls.Add(lnkTrust);
            Controls.Add(footBox);
        }

        private void ShowAboutDialog()
        {
            string info =
                "Bezpieczny Egzamin — Aplikacja stanowiskowa dla ucznia\n" +
                "------------------------------------------------------------------\n" +
                "Wersja programu:  " + Program.AppVersion + " (kompilacja " + Program.AppBuildDate + ")\n" +
                "Autor programu:   " + Program.AppAuthor + "\n" +
                "Wydawca:          " + Program.AppPublisher + "\n" +
                "Strona systemu:   https://egzamin.info.pl\n" +
                "Kontakt / pomoc:  " + Program.AppContact + "\n" +
                "Podpis cyfrowy:   SHA-256 Authenticode (CN=Live Production (egzamin.info.pl))\n\n" +
                "© 2023–2026 Live Production — Karol Kowalski (egzamin.info.pl). Wszelkie prawa zastrzeżone.";

            MessageBox.Show(info, "O programie — Bezpieczny Egzamin v" + Program.AppVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
