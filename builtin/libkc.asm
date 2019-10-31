GLOBAL func_putc
SECTION .text
; void putc(char ch)
func_putc:
push	rbp
mov	rbp, rsp

mov	rax, 1
mov	rdi, 1
lea	rsi, [rsp+16]
mov	rdx, 1
syscall

mov	rsp, rbp
pop	rbp
ret