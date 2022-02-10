.data
    onesArray DWORD 4 dup (1)
.code
CountCharFrequencyAsm proc

    mov rdi, 0
    mov r15, [RDX]
    add r15, 4

_loopMain:    
    mov rbx, 255
    pxor xmm0, xmm0
    movups xmm0, DWORD ptr [onesArray] ; pass { 1 1 1 1 }
    pxor xmm2, xmm2 
    mov rsi, 0

    movups xmm3, DWORD ptr [RCX + rdi * sizeof dword]
    add rdi, 4
    cmp r15, rdi
    jbe _end
    jmp _loop

_loop:
    movups xmm1, xmm3
    CMPEQPS xmm1, xmm2
    andps xmm1, xmm0

    call _seg0
    call _seg1
    call _seg2
    call _seg3

    ; Should process xmm1 data
    addps xmm2, xmm0
    inc rsi
    dec rbx
    jz _loopMain
    jmp _loop

_seg0:
    VPEXTRD eax, xmm1, 0
    cmp eax, 0
    jne _increment1
    ret 

_seg1:
    VPEXTRD eax, xmm1, 1
    cmp eax, 0
    jne _increment1
    ret 

_seg2:
    VPEXTRD eax, xmm1, 2
    cmp eax, 0
    jne _increment1
    ret 

_seg3:
    VPEXTRD eax, xmm1, 3
    cmp eax, 0
    jne _increment1
    ret 

_increment1:
    inc dword ptr [R8 + rsi * sizeof dword]
    ret

_end:
    ret 

CountCharFrequencyAsm endp

end