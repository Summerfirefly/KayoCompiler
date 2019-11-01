GLOBAL func_putc
GLOBAL func_getc
SECTION .text
; void putc(char ch)
func_putc:
push	rbp
mov	rbp, rsp

mov	rax, 1
mov	rdi, 1
lea	rsi, [rbp+16]
mov	rdx, 1
syscall

mov	rsp, rbp
pop	rbp
ret

; int getc(void)
func_getc:
push	rbp
mov	rbp, rsp
sub	rsp, 1

mov	rax, 0
mov	rdi, 0
lea	rsi, [rbp-1]
mov	rdx, 1
syscall

cmp	rax, 0
je	eof
movzx	rax, byte [rbp-1]
jmp	getc_return
eof:
mov	rax, -1

getc_return:
mov	rsp, rbp
pop	rbp
ret