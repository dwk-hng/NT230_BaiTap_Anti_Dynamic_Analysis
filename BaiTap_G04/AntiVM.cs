using System;

namespace BaiTap_G04
{
    public static class AntiVM
    {
        // VD: Phương thức 1 - Artifact-based (Kiểm tra file/thư mục đặc trưng của VMWare) (Có thể đổi)
        private static bool CheckVMWareFiles()
        {
            // Code kiểm tra file sẽ viết ở đây
            return false;
        }

        // VD: Phương thức 2 - Timing-based (Đo thời gian thực thi) (Có thể đổi)
        private static bool CheckExecutionTiming()
        {
            // Code đo timing sẽ viết ở đây
            return false;
        }

        // Hàm tổng hợp
        public static bool CheckAll()
        {
            return CheckVMWareFiles() || CheckExecutionTiming();
        }
    }
}