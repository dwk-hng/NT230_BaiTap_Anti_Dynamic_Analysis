rule Detect_AntiDebug_IsDebuggerPresent_G04
{
    meta:
        author = "Nhóm G04 - NT230"
        description = "Phát hiện hành vi Anti-Debugging sử dụng API IsDebuggerPresent trong file thực thi .NET"

    strings:
        // Chuỗi đặc trưng gọi hàm API
        $api_name = "IsDebuggerPresent" ascii wide
        $dll_name = "kernel32.dll" ascii wide nocase

        // Dấu hiệu nhận biết đây là file .NET (C#)
        $dotnet_magic = "_CorExeMain" ascii wide

    condition:
        // 1. Phải là file PE (Bắt đầu bằng MZ)
        uint16(0) == 0x5A4D 
        
        // 2. Phải là ứng dụng .NET
        and $dotnet_magic 
        
        // 3. Phải chứa khai báo gọi hàm IsDebuggerPresent từ kernel32.dll
        and $api_name and $dll_name
}


rule Detect_AntiDebug_ManualPEBCheck_G04
{
    meta:
        author = "Nhóm G04 - NT230"
        description = "Phát hiện hành vi Anti-Debugging bằng cách đọc thủ công PEB (NtQueryInformationProcess & ReadProcessMemory) trong .NET"

    strings:
        // Dấu hiệu nhận biết file .NET (C#)
        $dotnet_magic = "_CorExeMain" ascii wide

        // Các hàm API cấp thấp được sử dụng để trích xuất và đọc bộ nhớ PEB
        $api_query_process = "NtQueryInformationProcess" ascii wide
        $api_read_memory = "ReadProcessMemory" ascii wide
        
        // Thư viện lõi chứa các Native API này
        $dll_ntdll = "ntdll.dll" ascii wide nocase
        $dll_kernel32 = "kernel32.dll" ascii wide nocase

    condition:
        // 1. Kiểm tra header của file PE (Windows Executable)
        uint16(0) == 0x5A4D 
        
        // 2. Xác nhận đây là ứng dụng .NET
        and $dotnet_magic 
        
        // 3. Phải chứa đồng thời 2 hàm Native API cấp thấp chuyên dùng để chọc vào bộ nhớ
        and $api_query_process and $api_read_memory
        
        // 4. Phải gọi từ các thư viện lõi hệ thống
        and $dll_ntdll and $dll_kernel32
}