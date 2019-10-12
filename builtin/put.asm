GLOBAL puts

SECTION .data
str_true:	db "true"
str_false:	db "false"

SECTION .text
; void puts(int a)
; void puts(bool a)
puts:
push	rbp
mov	rbp, rsp
push	rax
push	rdx
push	rcx

mov	rax, qword [rsp+40]
cmp	rax, 0
je	put_bool

put_int:
mov	rax, qword [rsp+48]
put_int_loop:
cqo
mov	rbx, 10
idiv	rbx
add	rdx, 48
push	rdx
call	putc
cmp	rax, 0
jne	put_int_loop
jmp	exit

put_bool:
mov	rax, qword [rsp+48]
cmp	rax, 0
je	put_false
mov	rax, 1
mov	rdi, 1
mov	rsi, str_true
mov	rdx, 4
syscall
jmp	exit
put_false:
mov	rax, 1
mov	rdi, 1
mov	rsi, str_false
mov	rdx, 5
syscall

exit:
push	10
call	putc
pop	rcx
pop	rdx
pop	rax
mov	rsp, rbp
pop	rbp
ret

; void putc(char ch)
putc:
push	rbp
mov	rbp, rsp
push	rax
push	rdx
push	rcx

mov	rax, 1
mov	rdi, 1
mov	rsi, rsp
add	rsi, 40
mov	rdx, 1
syscall

pop	rcx
pop	rdx
pop	rax
mov	rsp, rbp
pop	rbp
ret