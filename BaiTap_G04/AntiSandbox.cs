using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BaiTap_G04
{
    public static class AntiSandbox
    {
        // Struct để lưu tọa độ X, Y của con trỏ chuột
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        // Import hàm GetCursorPos từ API của Windows
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        // Kỹ thuật: Kiểm tra tương tác chuột
        private static bool CheckMouseMovement()
        {
            POINT firstPos;
            POINT secondPos;

            // Lấy vị trí chuột lần 1
            if (GetCursorPos(out firstPos))
            {
                // Ngủ 2 giây. Trong lúc này nếu là người dùng thật thì chuột thường sẽ nhúc nhích.
                Thread.Sleep(2000);

                // Lấy vị trí chuột lần 2
                if (GetCursorPos(out secondPos))
                {
                    // Nếu tọa độ X và Y đứng im sau 2 giây -> Sandbox
                    if (firstPos.X == secondPos.X && firstPos.Y == secondPos.Y)
                    {
                        return true; // Phát hiện Sandbox
                    }
                }
            }

            // Nếu chuột có di chuyển hoặc hệ thống lỗi không lấy được tọa độ
            return false;
        }

        // Hàm tổng hợp để gọi từ hàm Main trong file Program.cs
        public static bool CheckAll()
        {
            Console.WriteLine("- Dang kiem tra moi truong Sandbox...");
            bool isSandbox = CheckMouseMovement();

            if (isSandbox)
            {
                Console.WriteLine("- Phat hien Sandbox");
            }
            else
            {
                Console.WriteLine("- An toan, Khong phat hien Sandbox.");
            }

            return isSandbox;
        }
    }
}