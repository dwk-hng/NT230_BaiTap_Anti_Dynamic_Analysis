using System;
using System.Windows.Forms;

namespace BaiTap_G04
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. CHẠY CÁC LỚP BẢO VỆ (ANTI-ANALYSIS)
            bool isDebugged = AntiDebug.CheckAll();
            bool isVM = AntiVM.CheckAll();
            bool isSandbox = AntiSandbox.CheckAll();

            // 2. KIỂM TRA MÔI TRƯỜNG VÀ THỰC THI
            if (isDebugged || isVM || isSandbox)
            {
                // Bị phát hiện -> Thoát ngay lập tức
                Environment.Exit(0);
            }
            else
            {
                // Môi trường sạch -> Bung Payload
                MessageBox.Show(
                    "NT230.Q21.ANTT | G04 Group | Payload Executed",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                Environment.Exit(0);
            }
        }
    }
}