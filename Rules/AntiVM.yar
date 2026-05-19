rule AntiVM_G04_Comprehensive
{
    meta:
        description = "Phat hien ky thuat Anti-VM qua file, process, registry va WMI (VMWare, VirtualBox, Hyper-V)"
        author = "Nhom G04"

    strings:
        // --- Dấu hiệu VMWare ---
        $vmware_str1 = "VMware Tools" ascii wide nocase
        $vmware_str2 = "vmtoolsd" ascii wide nocase
        $vmware_str3 = "vmmouse.sys" ascii wide nocase
        $vmware_mac = "005056" ascii wide nocase

        // --- Dấu hiệu VirtualBox ---
        $vbox_str1 = "VBoxMouse.sys" ascii wide nocase
        $vbox_str2 = "VBoxTray" ascii wide nocase
        $vbox_reg = "VBOX__" ascii wide nocase

        // --- Dấu hiệu Hyper-V ---
        $hyperv_str1 = "vmbus.sys" ascii wide nocase
        $hyperv_str2 = "vmwp" ascii wide nocase
        $hyperv_reg = "Virtual Machine\\Guest\\Parameters" ascii wide nocase
        
        // --- Dấu hiệu Behavior/WMI ---
        $wmi_query = "Select * from Win32_ComputerSystem" ascii wide nocase

    condition:
        // Kiem tra Header la file thuc thi (MZ)
        uint16(0) == 0x5A4D and 
        (
            // Phat hien neu co it nhat 2 dau hieu cua mot trong cac moi truong
            2 of ($vmware_*) or 
            2 of ($vbox_*) or 
            2 of ($hyperv_*) or
            
            // Hoac phat hien truy van WMI tim kiem phan cung
            $wmi_query
        )
}