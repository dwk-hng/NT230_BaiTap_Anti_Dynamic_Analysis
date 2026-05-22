using System;
using System.Runtime.InteropServices;

namespace BaiTap_G04
{
    public static class AntiDebug
    {
        // PHƯƠNG THỨC 1: SỬ DỤNG WINDOWS API 
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool IsDebuggerPresent();


        // PHƯƠNG THỨC 2: MANUAL PEB CHECK (LOW-LEVEL)
        // Import các API cấp thấp để can thiệp bộ nhớ
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, out int returnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        // Định nghĩa cấu trúc bộ nhớ để hứng dữ liệu chứa con trỏ PEB
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress; // Con trỏ trỏ tới cấu trúc PEB
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr Reserved3;
        }

        // Hàm đọc PEB thủ công
        private static bool ManualPEBCheck()
        {
            try
            {
                PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                int returnLength;

                // Lấy thông tin tiến trình hiện tại (ProcessInformationClass = 0)
                int status = NtQueryInformationProcess(GetCurrentProcess(), 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);

                if (status == 0 && pbi.PebBaseAddress != IntPtr.Zero)
                {
                    // Cờ BeingDebugged nằm ở vị trí offset 0x02 trong cấu trúc PEB (áp dụng cả x86 và x64)
                    IntPtr beingDebuggedAddress = new IntPtr(pbi.PebBaseAddress.ToInt64() + 2);
                    byte[] buffer = new byte[1];
                    IntPtr bytesRead;

                    // Đọc trực tiếp 1 byte tại vùng nhớ đó
                    if (ReadProcessMemory(GetCurrentProcess(), beingDebuggedAddress, buffer, 1, out bytesRead))
                    {
                        return buffer[0] == 1; // Nếu byte có giá trị = 1 -> Đang bị Debug
                    }
                }
            }
            catch
            {
                // Bắt lỗi im lặng để tránh làm crash chương trình khi không có quyền đọc bộ nhớ
            }

            return false;
        }

        // HÀM TỔNG HỢP KIỂM TRA
        /// <summary>
        /// Gọi hàm này để chạy toàn bộ các phương thức kiểm tra Debugger.
        /// Trả về true nếu phát hiện bất kỳ dấu hiệu nào.
        /// </summary>
        public static bool CheckAll()
        {
            bool isDetectedByAPI = IsDebuggerPresent();
            bool isDetectedByPEB = ManualPEBCheck();

            // Do đây là phần test 2 phương pháp có hoạt động đúng hay không,
            // nên cần đảm bảo cả 2 phương pháp đều có thể phát hiện được Debugger.
            // Nên phần này được chỉnh lại là
            // nếu cả 2 phương pháp đều phát hiện ra Debugger thì mới kết luận là môi trường debugger, 
            // nếu chỉ 1 trong 2 phương pháp phát hiện ra Debugger thì sẽ không hoàn thành bài test.
            // Do đó chỉnh lại như sau:
            return isDetectedByAPI && isDetectedByPEB;

            // Ở môi trường bình thường, chỉ cần 1 trong 2 phương pháp phát hiện ra Debugger là đủ,
            // lúc đó chỉnh lại là:
            // return isDetectedByAPI || isDetectedByPEB;
        }
    }
}