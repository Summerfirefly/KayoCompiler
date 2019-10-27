GLOBAL read

SECTION .data
temp: db 0x00

SECTION .text
read:
push	rbp
mov	rbp, rsp

mov	rax, qword [rsp+16]
cmp	rax, 0
je	read_bool

read_num:
find_num_loop:
mov	rbx, 0
call	getc
cmp	rax, '-'
jne	test_num
sete	bl
call	getc
test_num:
cmp	rax, 0x0a
je	read_return
cmp	rax, '0'
jl	find_num_loop
cmp	rax, '9'
jg	find_num_loop
stop_find:

xor	rcx, rcx
parse_num:
sub	rax, '0'
imul	rcx, 10
add	rcx, rax
push	rcx
call	getc
pop	rcx
cmp	rax, '0'
jl	end_parse
cmp	rax, '9'
jg	end_parse
jmp	parse_num
end_parse:
cmp	rbx, 0
je	read_return
not	rcx
add	rcx, 1
jmp	read_return

read_bool:
call	getc
cmp	rax, 't'
je	return_true
cmp	rax, 'T'
je	return_true
mov	rcx, 0
jmp	read_return
return_true:
mov	rcx, 1
jmp	read_return

read_return:
read_loop:
cmp	rax, -1
je	end_loop
cmp	rax, 0x0a
je	end_loop
push	rcx
call	getc
pop	rcx
jmp	read_loop
end_loop:

mov	rax, rcx
mov	rsp, rbp
pop	rbp
ret


getc:
push	rbp
mov	rbp, rsp

mov	rax, 0
mov	rdi, 0
mov	rsi, temp
mov	rdx, 1
syscall

cmp	rax, 0
je	eof
movzx	rax, byte [temp]
jmp	getc_return
eof:
mov	rax, -1

getc_return:
mov	rsp, rbp
pop	rbp
ret