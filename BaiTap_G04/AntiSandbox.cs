using System;

namespace BaiTap_G04
{
    public static class AntiSandbox
    {
        // VD: Kiểm tra tương tác chuột (Sandbox thường không có người thật di chuột) (có thể đổi)
        private static bool CheckMouseMovement()
        {
            // Code kiểm tra chuột sẽ viết ở đây
            return false;
        }

        // Hàm tổng hợp
        public static bool CheckAll()
        {
            return CheckMouseMovement();
        }
    }
}