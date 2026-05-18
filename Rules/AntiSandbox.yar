rule Detect_AntiSandbox_MouseMovement { 

    meta: 
        author = "Nhom G04" 
        description = "Phat hien ma doc kiem tra tuong tac chuot de ne tranh Sandbox" 
        reference = "NT230 - Anti-Dynamic Analysis" 
        date = "2026-05-18" 

    strings: 
        // Các API 
        $api_mouse = "GetCursorPos" ascii wide 
        $api_sleep = "Sleep" ascii wide 

    condition: 
        // Kiểm tra chữ ký MZ để xác nhận đây là file PE 
        uint16(0) == 0x5A4D  
        // Điều kiện kích hoạt là file PE phải chứa cả hai hàm API trên 
        and all of them 
} 