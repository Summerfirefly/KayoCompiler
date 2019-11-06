GLOBAL putc
GLOBAL getc
SECTION .text
; void putc(char ch)
$putc:
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
$getc:
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
jmp	getc_rtn
eof:
mov	rax, -1

getc_rtn:
mov	rsp, rbp
pop	rbp
ret