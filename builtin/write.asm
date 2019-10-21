GLOBAL write

SECTION .data
str_true:	db "true"
str_false:	db "false"

SECTION .text
; void write(int a)
; void write(bool a)
write:
push	rbp
mov	rbp, rsp
push	rax
push	rdx
push	rcx

mov	rax, qword [rsp+40]
cmp	rax, 0
je	put_bool

convert_int:
mov	rdi, qword [rsp+48]
mov	rax, rdi
cmp	rax, 0
jnl	abs_end
; abs
not	rax
add	rax, 1
abs_end:
mov	rcx, 0
convert_int_loop:
cqo
mov	rbx, 10
idiv	rbx
add	rdx, 48
push	rdx
inc	rcx
cmp	rax, 0
jne	convert_int_loop

cmp	rdi, 0
jnl	put_int_loop
push	'-'
inc	rcx

put_int_loop:
call	putc
add	rsp, 8
dec	rcx
cmp	rcx, 0
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